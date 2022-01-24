FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["MrClean/MrClean.csproj", "MrClean/"]
RUN dotnet restore "MrClean/MrClean.csproj"
COPY . .
WORKDIR "/src/MrClean"
RUN dotnet build "MrClean.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "MrClean.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MrClean.dll"]
