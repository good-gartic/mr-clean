using Microsoft.EntityFrameworkCore;
using MrClean.Data;
using MrClean.Exceptions;
using MrClean.Models;

namespace MrClean.Services.Filters;

public class MessageFiltersService : IMessageFiltersService
{
    private readonly IDbContextFactory<MrCleanDbContext> _factory;

    public MessageFiltersService(IDbContextFactory<MrCleanDbContext> factory)
    {
        _factory = factory;
    }

    public async Task<MessageFilter> GetMessageFilterAsync(int filterId)
    {
        await using var context = await _factory.CreateDbContextAsync();

        return await context.MessageFilters.FirstOrDefaultAsync(f => f.Id == filterId)
               ?? throw new MessageFilterNotFoundException();
    }

    public async Task<List<MessageFilter>> ListMessageFiltersAsync()
    {
        await using var context = await _factory.CreateDbContextAsync();
        
        return await context.MessageFilters.ToListAsync();
    }

    public async Task<MessageFilter> CreateMessageFilterAsync(string pattern, int delay = 0, ulong? repostChannelId = null)
    {
        await using var context = await _factory.CreateDbContextAsync();

        if (delay is < 0 or > 120)
        {
            throw new ArgumentOutOfRangeException(nameof(delay)); 
        }

        var filter = new MessageFilter
        {
            Delay = delay,
            Pattern = pattern,
            RepostChannelId = repostChannelId
        };

        await context.MessageFilters.AddAsync(filter);
        await context.SaveChangesAsync();

        return filter;
    }

    public Task<MessageFilter> EnableMessageFilterAsync(int filterId)
    {
        throw new NotImplementedException();
    }

    public Task<MessageFilter> DisableMessageFilterAsync(int filterId)
    {
        throw new NotImplementedException();
    }

    public Task<MessageFilter> DeleteMessageFilterAsync(int filterId)
    {
        throw new NotImplementedException();
    }

    public Task<MessageFilter> AddAllowedEntityAsync(int filterId, MessageFilterSpecificationType type, ulong id)
    {
        throw new NotImplementedException();
    }

    public Task<MessageFilter> AddDeniedEntityAsync(int filterId, MessageFilterSpecificationType type, ulong id)
    {
        throw new NotImplementedException();
    }

    public Task<MessageFilter> ResetFilterSpecificationAsync(int filterId)
    {
        throw new NotImplementedException();
    }
}