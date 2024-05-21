FROM mcr.microsoft.com/dotnet/runtime:8.0 AS base
USER $APP_UID
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Chat/Client/Client.csproj", "Chat/Client/"]
COPY ["SocketCommunication/SocketCommunication.csproj", "SocketCommunication/"]
COPY ["Domain/Domain.csproj", "Domain/"]
RUN dotnet restore "Chat/Client/Client.csproj"
COPY . .
WORKDIR "/src/Chat/Client"
RUN dotnet build "Client.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Client.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Client.dll"]
