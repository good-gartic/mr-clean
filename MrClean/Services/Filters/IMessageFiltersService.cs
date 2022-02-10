using MrClean.Models;

namespace MrClean.Services.Filters;

public interface IMessageFiltersService
{
    Task<MessageFilter> GetMessageFilterAsync(int filterId);
    
    Task<IEnumerable<MessageFilter>> ListMessageFiltersAsync();
    
    Task<MessageFilter> CreateMessageFilterAsync(string pattern, long delay = 0, ulong repostChannelId = 0);

    Task<MessageFilter> EnableMessageFilterAsync(int filterId);
    
    Task<MessageFilter> DisableMessageFilterAsync(int filterId);
    
    Task<MessageFilter> DeleteMessageFilterAsync(int filterId);

    Task<MessageFilter> AddAllowedEntityAsync(int filterId, MessageFilterSpecificationType type, ulong id);
    
    Task<MessageFilter> AddDeniedEntityAsync(int filterId, MessageFilterSpecificationType type, ulong id);

    Task<MessageFilter> ResetFilterSpecificationAsync(int filterId);
}