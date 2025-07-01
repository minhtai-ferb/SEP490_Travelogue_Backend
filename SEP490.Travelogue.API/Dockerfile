# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["Travelogue.API/Travelogue.API.csproj", "Travelogue.API/"]
COPY ["Travelogue.Repository/Travelogue.Repository.csproj", "Travelogue.Repository/"]
COPY ["Travelogue.Service/Travelogue.Service.csproj", "Travelogue.Service/"]
COPY *.sln .
RUN dotnet restore
COPY . .
WORKDIR "/src/Travelogue.API"
RUN dotnet publish -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 5565
ENV ASPNETCORE_URLS=http://*:5565
ENV ASPNETCORE_ENVIRONMENT=Production
ENTRYPOINT ["dotnet", "Travelogue.API.dll"]