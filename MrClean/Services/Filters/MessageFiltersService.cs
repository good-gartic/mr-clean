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

    public async Task<MessageFilter> EnableMessageFilterAsync(int filterId)
    {
        return await ApplyChangesToFilter(filterId, f => f.Enabled = true);
    }

    public async Task<MessageFilter> DisableMessageFilterAsync(int filterId)
    {
        return await ApplyChangesToFilter(filterId, f => f.Enabled = false);
    }

    public async Task<MessageFilter> DeleteMessageFilterAsync(int filterId)
    {
        await using var context = await _factory.CreateDbContextAsync();

        var filter = await GetMessageFilterAsync(filterId);

        context.MessageFilters.Remove(filter);
        
        await context.SaveChangesAsync();

        return filter;
    }

    public async Task<MessageFilter> AddAllowedEntityAsync(int filterId, MessageFilterSpecificationType type, ulong id)
    {
        return await ApplyChangesToFilter(filterId, f =>
        {
            var _ = type switch
            {
                MessageFilterSpecificationType.User => f.Users.AddAllowedEntity(id),
                MessageFilterSpecificationType.Role => f.Roles.AddAllowedEntity(id),
                MessageFilterSpecificationType.Channel => f.Channels.AddAllowedEntity(id),
                _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
            };

            f.UsersSpecification = f.Users.SpecificationString;
            f.RolesSpecification = f.Roles.SpecificationString;
            f.ChannelsSpecification = f.Channels.SpecificationString;
        });
    }

    public async Task<MessageFilter> AddDeniedEntityAsync(int filterId, MessageFilterSpecificationType type, ulong id)
    {
        return await ApplyChangesToFilter(filterId, f =>
        {
            var _ = type switch
            {
                MessageFilterSpecificationType.User => f.Users.AddDeniedEntity(id),
                MessageFilterSpecificationType.Role => f.Roles.AddDeniedEntity(id),
                MessageFilterSpecificationType.Channel => f.Channels.AddDeniedEntity(id),
                _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
            };

            f.UsersSpecification = f.Users.SpecificationString;
            f.RolesSpecification = f.Roles.SpecificationString;
            f.ChannelsSpecification = f.Channels.SpecificationString;
        });
    }

    public async Task<MessageFilter> ResetFilterSpecificationAsync(int filterId)
    {
        return await ApplyChangesToFilter(filterId, f =>
        {
            f.UsersSpecification = null;
            f.RolesSpecification = null;
            f.ChannelsSpecification = null;
        });
    }

    private async Task<MessageFilter> ApplyChangesToFilter(int id, Action<MessageFilter> transform)
    {
        await using var context = await _factory.CreateDbContextAsync();

        var filter = await GetMessageFilterAsync(id);
        
        transform(filter);
        context.MessageFilters.Update(filter);

        await context.SaveChangesAsync();

        return filter;
    }
}