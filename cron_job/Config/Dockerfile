# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS builder

WORKDIR /src

COPY ["cron_job/cron_job.csproj", "cron_job/"]
RUN dotnet restore "cron_job/cron_job.csproj"

COPY . .
RUN dotnet build "cron_job/cron_job.csproj" -c Release -o /app/build

RUN dotnet publish "cron_job/cron_job.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/runtime:9.0

WORKDIR /app

COPY --from=builder /app/publish .

ENV ASPNETCORE_ENVIRONMENT=Production
ENV DOTNET_ENVIRONMENT=Production

ENTRYPOINT ["dotnet", "cron_job.dll"]
