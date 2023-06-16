using DAL.Entity;
using Microsoft.EntityFrameworkCore;
using Services.Interfaces;

namespace DAL.Repository;

public class LoadHistoryRepository : ILoadHistoryRepository
{
    private readonly IDbContextFactory<AppDbContext> _contextFactory;

    public LoadHistoryRepository(IDbContextFactory<AppDbContext> contextFactory)
    {
        _contextFactory = contextFactory;
    }

    public async Task<DateTime> GetLastEntryTime(string source)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        return await context.LoadHistory
            .OrderByDescending(x => x.CreatedAt)
            .Select(x => x.CreatedAt)
            .FirstOrDefaultAsync();
    }

    public async Task SaveLoadInfo(DateTime createdAt, string source, string hash)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        var newEntity = new LoadHistory
        {
            CreatedAt = createdAt,
            Source = source,
            FileHash = hash
        };
        context.LoadHistory.Add(newEntity);
        await context.SaveChangesAsync();
    }

    public async Task<bool> HashAlreadyExists(string hash)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        return await context.LoadHistory
            // проверяем данные только за последний час, что бы сильно не нагружать БД 
            .Where(x => x.CreatedAt > DateTime.Now.AddHours(-1))
            .AnyAsync(x => x.FileHash == hash);
    }
}