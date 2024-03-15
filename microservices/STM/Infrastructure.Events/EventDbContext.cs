using Application.EventHandlers;
using Application.EventHandlers.Interfaces;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace Infrastructure.Events;

public sealed class EventDbContext : DbContext, IEventContext
{
    public EventDbContext(DbContextOptions<EventDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Configure the table name here
        modelBuilder.Entity<EventDto>(b =>
        {
           b.HasKey(e => e.EventType);
           b.Property(e => e.Event);
        });
    }

    public async Task<T?> TryGetAsync<T>()
    {
        var @event = await Set<EventDto>().Where(e => e.EventType == typeof(T).FullName)
            .Select(e => e.Event)
            .AsNoTracking()
            .FirstOrDefaultAsync();

        return @event == null ? default : JsonConvert.DeserializeObject<T>(@event);
    }

    public async Task AddOrUpdateAsync<T>(T @event)
    {
        if (await Set<EventDto>().FindAsync(typeof(T).FullName) is {} eventDto)
        {
            eventDto.Event = JsonConvert.SerializeObject(@event);
        }
        else
        {
            await Set<EventDto>().AddAsync(new EventDto(typeof(T).FullName!, JsonConvert.SerializeObject(@event)));
        }

        await SaveChangesAsync();
    }
}