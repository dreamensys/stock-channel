FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build-env
WORKDIR /
EXPOSE 80

# Copy csproj and restore as distinct layers
COPY *.csproj ./

#COPY "StockChannel.Domain/StockChannel.Domain.csproj" "StockChannel.Domain/StockChannel.Domain.csproj"
#COPY "StockChannel.Infrastructure/StockChannel.Infrastructure.csproj" "StockChannel.Infrastructure/StockChannel.Infrastructure.csproj"
#COPY "StockChannel.StockBot.API/StockChannel.StockBot.API.csproj" "StockChannel.StockBot.API/StockChannel.StockBot.API.csproj"
RUN dotnet restore

# Copy everything else and build
COPY . ./
RUN dotnet publish -c Release -o out

# Build runtime image
FROM mcr.microsoft.com/dotnet/core/aspnet:3.1
WORKDIR /app
COPY --from=build-env /app/out .
ENTRYPOINT ["dotnet", "aspnetapp.dll"]