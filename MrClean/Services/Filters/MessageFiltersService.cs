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

    public async Task<IEnumerable<MessageFilter>> ListMessageFiltersAsync()
    {
        await using var context = await _factory.CreateDbContextAsync();
        
        return await context.MessageFilters.ToListAsync();
    }

    public Task<MessageFilter> CreateMessageFilterAsync(string pattern, long delay = 0, ulong repostChannelId = 0)
    {
        throw new NotImplementedException();
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