FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Discovery.csproj", "./"]
RUN dotnet restore "Discovery.csproj"
COPY . .
WORKDIR "/src/"
RUN dotnet build "Discovery.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Discovery.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
COPY ./Resources/configuration.json /app/resources/
ENTRYPOINT ["dotnet", "Discovery.dll"]
