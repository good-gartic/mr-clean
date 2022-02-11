using MrClean.Models;

namespace MrClean.Services.Filters;

public interface IMessageFiltersService
{
    Task<MessageFilter> GetMessageFilterAsync(int filterId);

    Task<List<MessageFilter>> ListMessageFiltersAsync();

    Task<MessageFilter> CreateMessageFilterAsync(string pattern, int delay = 0, ulong? repostChannelId = null);

    Task<MessageFilter> EnableMessageFilterAsync(int filterId);

    Task<MessageFilter> DisableMessageFilterAsync(int filterId);

    Task<MessageFilter> DeleteMessageFilterAsync(int filterId);

    Task<MessageFilter> AddAllowedEntityAsync(int filterId, SpecificationEntityType entityType, ulong id);

    Task<MessageFilter> AddDeniedEntityAsync(int filterId, SpecificationEntityType entityType, ulong id);

    Task<MessageFilter> ResetFilterSpecificationAsync(int filterId);
}