using Application.Interfaces;
using Application.Interfaces.Policies;
using Application.Usecases;
using Configuration.Policies;
using Contracts;
using Controllers.Controllers;
using Infrastructure.Clients;
using MassTransit;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.OpenApi.Models;
using MqContracts;
using RabbitMQ.Client;
using ServiceMeshHelper;
using ServiceMeshHelper.Controllers;

namespace Configuration
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            ConfigureServices(builder.Services);

            var app = builder.Build();

            app.UseSwagger();

            app.UseSwaggerUI();

            app.UseHttpsRedirection();

            app.UseCors(
                options =>
                {
                    options.AllowAnyOrigin();
                    options.AllowAnyHeader();
                    options.AllowAnyMethod();
                }
            );

            app.UseAuthorization();

            app.MapControllers();

            await app.RunAsync();
        }

        private static void ConfigureServices(IServiceCollection services)
        {
            ConfigureMassTransit(services);

            services.AddControllers().PartManager.ApplicationParts.Add(new AssemblyPart(typeof(TripComparatorMqController).Assembly));

            services.AddEndpointsApiExplorer();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "TripComparator", Version = "v1" });
                c.EnableAnnotations();
            });

            services.AddSingleton<IHostInfo, HostInfo>();

            services.AddScoped(typeof(IInfiniteRetryPolicy<>), typeof(InfiniteRetryPolicy<>));

            services.AddScoped(typeof(IBackOffRetryPolicy<>), typeof(BackOffRetryPolicy<>));

            services.AddScoped<CompareTimes>();

            services.AddScoped<IRouteTimeProvider, RouteTimeProviderClient>();

            services.AddScoped<IDataStreamWriteModel, MassTransitRabbitMqClient>();

            services.AddScoped<IBusInfoProvider, StmClient>();
        }

        private static void ConfigureMassTransit(IServiceCollection services)
        {
            var hostInfo = new HostInfo();
            
            var routingData = RestController.GetAddress(hostInfo.GetMQServiceName(), LoadBalancingMode.RoundRobin).Result.First();

            var uniqueQueueName = "TripComparator";

            services.AddMassTransit(x =>
            {
                x.AddConsumer<TripComparatorMqController>();
                x.AddConsumer<RideTrackingUpdatedMqController>();

                x.UsingRabbitMq((context, cfg) =>
                {
                    cfg.Host($"rabbitmq://{ routingData.Host }:{routingData.Port}", c =>
                    {
                        c.RequestedConnectionTimeout(100);
                        c.Heartbeat(TimeSpan.FromMilliseconds(50));
                    });

                    cfg.Message<CoordinateMessage>(topologyConfigurator => topologyConfigurator.SetEntityName("coordinate_message"));
                    cfg.Message<ApplicationRideTrackingUpdated>(topologyConfigurator => topologyConfigurator.SetEntityName("ride_tracking_updated"));

                    cfg.Message<BusPositionUpdated>(topologyConfigurator => topologyConfigurator.SetEntityName("bus_position_updated"));
                    cfg.Message<BusPositionsUpdateCompleted>(topologyConfigurator => topologyConfigurator.SetEntityName("bus_position_update_completed"));

                    cfg.ReceiveEndpoint(uniqueQueueName + "Coordinate_Message", endpoint =>
                    {
                        endpoint.ConfigureConsumeTopology = false;

                        endpoint.Bind<CoordinateMessage>(binding =>
                        {
                            binding.ExchangeType = ExchangeType.Topic;
                            binding.RoutingKey = "trip_comparison.query";
                        });

                        endpoint.ConfigureConsumer<TripComparatorMqController>(context);

                        endpoint.SingleActiveConsumer = true;
                        endpoint.PrefetchCount = 1;
                    });

                    cfg.ReceiveEndpoint(uniqueQueueName + "Ride_Update", endpoint =>
                    {
                        endpoint.ConfigureConsumeTopology = false;

                        endpoint.Bind<ApplicationRideTrackingUpdated>(binding =>
                        {
                            binding.ExchangeType = ExchangeType.Topic;
                            binding.RoutingKey = "Stm.RideTrackingUpdated";
                        });

                        endpoint.ConfigureConsumer<RideTrackingUpdatedMqController>(context);

                        endpoint.SingleActiveConsumer = true;
                        endpoint.PrefetchCount = 1;
                    });

                    cfg.Publish<BusPositionUpdated>(p => p.ExchangeType = ExchangeType.Topic);
                    cfg.Publish<BusPositionsUpdateCompleted>(p => p.ExchangeType = ExchangeType.Topic);
                });
            });
        }
    }
}