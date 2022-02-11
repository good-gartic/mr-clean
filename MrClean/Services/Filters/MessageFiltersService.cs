using System.Diagnostics.CodeAnalysis;
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

    public async Task<MessageFilter> CreateMessageFilterAsync(string pattern, int delay = 0,
        ulong? repostChannelId = null)
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
        return await ApplyChangesToFilter(filterId, f =>
        {
            f.Enabled = true;
            return f;
        });
    }

    public async Task<MessageFilter> DisableMessageFilterAsync(int filterId)
    {
        return await ApplyChangesToFilter(filterId, f =>
        {
            f.Enabled = false;
            return f;
        });
    }

    public async Task<MessageFilter> DeleteMessageFilterAsync(int filterId)
    {
        await using var context = await _factory.CreateDbContextAsync();

        var filter = await GetMessageFilterAsync(filterId);

        context.MessageFilters.Remove(filter);

        await context.SaveChangesAsync();

        return filter;
    }

    public async Task<MessageFilter> AddAllowedEntityAsync(int filterId, SpecificationEntityType entityType, ulong id)
    {
        return await ApplyChangesToFilter(filterId, f =>
        {
            switch (entityType)
            {
                case SpecificationEntityType.User:
                    f.Users = f.Users.AddAllowedEntity(id);
                    return f;

                case SpecificationEntityType.Role:
                    f.Roles = f.Roles.AddAllowedEntity(id);
                    return f;

                case SpecificationEntityType.Channel:
                    f.Channels = f.Channels.AddAllowedEntity(id);
                    return f;

                default:
                    throw new ArgumentOutOfRangeException(nameof(entityType), entityType, null);
            }
        });
    }

    public async Task<MessageFilter> AddDeniedEntityAsync(int filterId, SpecificationEntityType entityType, ulong id)
    {
        return await ApplyChangesToFilter(filterId, f =>
        {
            switch (entityType)
            {
                case SpecificationEntityType.User:
                    f.Users = f.Users.AddDeniedEntity(id);
                    return f;

                case SpecificationEntityType.Role:
                    f.Roles = f.Roles.AddDeniedEntity(id);
                    return f;

                case SpecificationEntityType.Channel:
                    f.Channels = f.Channels.AddDeniedEntity(id);
                    return f;

                default:
                    throw new ArgumentOutOfRangeException(nameof(entityType), entityType, null);
            }
        });
    }

    public async Task<MessageFilter> ResetFilterSpecificationAsync(int filterId)
    {
        return await ApplyChangesToFilter(filterId, f =>
        {
            f.UsersSpecification = null;
            f.RolesSpecification = null;
            f.ChannelsSpecification = null;

            return f;
        });
    }

    private async Task<MessageFilter> ApplyChangesToFilter(int id, Func<MessageFilter, MessageFilter> transform)
    {
        await using var context = await _factory.CreateDbContextAsync();

        var filter = await GetMessageFilterAsync(id);
        var updated = transform(filter);

        context.MessageFilters.Update(updated);

        await context.SaveChangesAsync();

        return updated;
    }
}