#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["ComputingProject/Dashboard/Server/Dashboard.Server.csproj", "Dashboard/Server/"]
COPY ["ComputingProject/Dashboard/Client/Dashboard.Client.csproj", "Dashboard/Client/"]
COPY ["ComputingProject/Dashboard/Shared/Dashboard.Shared.csproj", "Dashboard/Shared/"]
RUN dotnet restore "Dashboard/Server/Dashboard.Server.csproj"
COPY ComputingProject/. .
WORKDIR "/src/Dashboard/Server"
RUN dotnet build "Dashboard.Server.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Dashboard.Server.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Dashboard.Server.dll"]