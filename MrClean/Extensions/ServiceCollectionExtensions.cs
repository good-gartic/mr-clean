using MrClean.Commands;

namespace MrClean.Extensions;

public static class ServiceCollectionExtensions
{
    public class CommandsCollection
    {
        private readonly IServiceCollection _collection;

        public CommandsCollection(IServiceCollection collection)
        {
            _collection = collection;
        }

        public CommandsCollection AddCommand<TCommand>() where TCommand : class, ISlashCommandProvider
        {
            _collection.AddTransient<ISlashCommandProvider, TCommand>();
            return this;
        }
    }

    public static IServiceCollection AddCommands(
        this IServiceCollection services,
        Func<CommandsCollection, CommandsCollection> configuration
    )
    {
        services.AddTransient<SlashCommandDispatcher>();
        configuration.Invoke(new CommandsCollection(services));

        return services;
    }
}