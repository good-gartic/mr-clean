# Mr. Clean bot

An alternative to the CleanChat, that will be shutting down soon.

## How to build and run this bot

<!-- TODO: Instructions for installing dotnet / docker version -->

- Obtain a valid bot token at [https://discord.com/developers/applications](https://discord.com/developers/application)
- Configure the bot token with `dotnet user-secrets set Discord.Token <token> --project MrClean`
- Configure the bot connection string with `dotnet user-secrets set ConnectionStrings.Default <connection string> --project MrClean`
- Run the application with `dotnet run --project MrClean`

![Mr. Clean logo](./logo.png)