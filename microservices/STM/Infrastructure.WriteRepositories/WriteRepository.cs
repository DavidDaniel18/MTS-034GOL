﻿using Application.CommandServices;
using Application.Common.Extensions;
using Domain.Common.Seedwork.Abstract;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Infrastructure.WriteRepositories;

public abstract class WriteRepository<TAggregate> : IWriteRepository<TAggregate>
    where TAggregate : Aggregate<TAggregate>, IEquatable<Entity<TAggregate>>
{
    private protected readonly ILogger Logger;

    //use only for bulk insert not saving
    private protected readonly AppWriteDbContext WriteDbContext;

    private protected bool DatabaseIsInMemory => WriteDbContext.IsInMemory();

    protected WriteRepository(AppWriteDbContext writeDbContext, ILogger logger)
    {
        WriteDbContext = writeDbContext;
        Logger = logger;

        Aggregates = writeDbContext.Set<TAggregate>();
    }

    protected DbSet<TAggregate> Aggregates { get; set; }

    public virtual async Task<IEnumerable<TAggregate>> GetAllAsync(params string[] ids)
    {
        return await (ids.IsEmpty()
            ? Aggregates.ToListAsync()
            : Aggregates.Where(a => ids.Contains(a.Id)).ToListAsync());
    }

    public virtual async Task<TAggregate> GetAsync(string id)
    {
        return await Aggregates.FindAsync(id) ??
               throw new KeyNotFoundException(
                   $"Aggregate of type {typeof(TAggregate)} could not be found using id: {id}");
    }

    public async Task<bool> AnyAsync()
    {
        return await Aggregates.AnyAsync();
    }

    public virtual async Task AddOrUpdateAsync(TAggregate aggregate)
    {
        var persistedAggregate = await Aggregates.FindAsync(aggregate.Id);

        if (persistedAggregate == null)
        {
            Aggregates.Add(aggregate);
        }
        else
        {
            var entry = Aggregates.Entry(persistedAggregate);

            entry.State = EntityState.Detached;

            Aggregates.Entry(aggregate).State = EntityState.Modified;

            Aggregates.Update(aggregate);
        }
    }

    public virtual async Task AddAllAsync(IEnumerable<TAggregate> aggregates)
    {
        if (WriteDbContext.Database.ProviderName != "Microsoft.EntityFrameworkCore.InMemory")
            await WriteDbContext.BulkInsertAsync(aggregates);
        else
            await Aggregates.AddRangeAsync(aggregates);
    }

    public virtual async Task UpdateAllAsync(IEnumerable<TAggregate> aggregates)
    {
        if (WriteDbContext.Database.ProviderName != "Microsoft.EntityFrameworkCore.InMemory")
            await WriteDbContext.BulkUpdateAsync(aggregates);
        else
            Aggregates.UpdateRange(aggregates);
    }

    public void Remove(TAggregate aggregate)
    {
        Aggregates.Remove(aggregate);
    }
}