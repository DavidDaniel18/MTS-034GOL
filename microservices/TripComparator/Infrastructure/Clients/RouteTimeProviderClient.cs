using Application.Interfaces;
using Application.Interfaces.Policies;
using Newtonsoft.Json;
using ServiceMeshHelper;
using ServiceMeshHelper.BusinessObjects;
using ServiceMeshHelper.BusinessObjects.InterServiceRequests;
using ServiceMeshHelper.Controllers;

namespace Infrastructure.Clients;

public class RouteTimeProviderClient : IRouteTimeProvider
{
    private readonly IInfiniteRetryPolicy<RouteTimeProviderClient> _infiniteRetry;

    private const string FilePath = "/app/data/carTimeFile.txt";

    private int? _savedTravelTimeInSeconds;

    public RouteTimeProviderClient(IInfiniteRetryPolicy<RouteTimeProviderClient> infiniteRetry)
    {
        _infiniteRetry = infiniteRetry;
    }
    
    public Task<int> GetTravelTimeInSeconds(string startingCoordinates, string destinationCoordinates)
    {
        return _infiniteRetry.ExecuteAsync(async () =>
        {
            var res = await RestController.Get(new GetRoutingRequest()
            {
                TargetService = "RouteTimeProvider",
                Endpoint = $"RouteTime/Get",
                Params = new List<NameValue>()
                {
                    new()
                    {
                        Name = "startingCoordinates",
                        Value = startingCoordinates
                    },
                    new()
                    {
                        Name = "destinationCoordinates",
                        Value = destinationCoordinates
                    },
                },
                Mode = LoadBalancingMode.RoundRobin
            });

            var times = new List<int>();

            await foreach (var result in res!.ReadAllAsync())
            {
                times.Add(JsonConvert.DeserializeObject<int>(result.Content));
            }

            var average = (int)times.Average();

            WriteIntegerToFile(average);

            _savedTravelTimeInSeconds = average;

            return average;
        });
    }

    public int GetSavedTravelTimeInSeconds()
    {
        return _savedTravelTimeInSeconds ??= ReadIntegerFromFile();
    }

    static void WriteIntegerToFile(int number)
    {
        File.WriteAllText(FilePath, number.ToString());
    }

    static int ReadIntegerFromFile()
    {
        var content = File.ReadAllText(FilePath);

        if (int.TryParse(content, out var result))
        {
            return result;
        }

        throw new Exception($"Could not parse {content} to an integer");
    }
}

//Exemple of how to use Restsharp for a simple request to a service (without the pros (and cons) of using the NodeController)

//var restClient = new RestClient("http://RouteTimeProvider");
//var restRequest = new RestRequest("RouteTime/Get");

//restRequest.AddQueryParameter("startingCoordinates", startingCoordinates);
//restRequest.AddQueryParameter("destinationCoordinates", destinationCoordinates);
            
//return (await restClient.ExecuteGetAsync<int>(restRequest)).Data;