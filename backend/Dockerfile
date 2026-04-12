# --- Build Stage ---
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy csproj and restore dependencies
COPY ["Backend.csproj", "./"]
RUN dotnet restore "Backend.csproj"

# Copy everything else and build
COPY . .
RUN dotnet publish "Backend.csproj" -c Release -o /app/out

# --- Runtime Stage ---
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

# Set Render default port
ENV ASPNETCORE_URLS=http://+:10000

# Copy build artifacts from the build stage
COPY --from=build /app/out .

ENTRYPOINT ["dotnet", "Backend.dll"]
