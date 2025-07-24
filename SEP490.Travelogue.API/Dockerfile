# ----- Build stage -----
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy project files
COPY ["Travelogue.API/Travelogue.API.csproj", "Travelogue.API/"]
COPY ["Travelogue.Service/Travelogue.Service.csproj", "Travelogue.Service/"]
COPY ["Travelogue.Repository/Travelogue.Repository.csproj", "Travelogue.Repository/"]
COPY *.sln ./

# Restore packages
RUN dotnet restore

# Copy all source code
COPY . .

# Publish
WORKDIR "/src/Travelogue.API"
RUN dotnet publish -c Release -o /app/publish

# ----- Runtime stage -----
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

COPY --from=build /app/publish .

# Expose backend port
EXPOSE 5565
ENV ASPNETCORE_URLS=http://*:5565
ENV ASPNETCORE_ENVIRONMENT=Production

ENTRYPOINT ["dotnet", "Travelogue.API.dll"]
