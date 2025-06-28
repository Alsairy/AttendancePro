FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["src/backend/AttendancePlatform.Api/AttendancePlatform.Api.csproj", "AttendancePlatform.Api/"]
COPY ["src/backend/AttendancePlatform.Application/AttendancePlatform.Application.csproj", "AttendancePlatform.Application/"]
COPY ["src/backend/shared/Domain/AttendancePlatform.Shared.Domain/AttendancePlatform.Shared.Domain.csproj", "shared/Domain/AttendancePlatform.Shared.Domain/"]
COPY ["src/backend/shared/Infrastructure/AttendancePlatform.Shared.Infrastructure/AttendancePlatform.Shared.Infrastructure.csproj", "shared/Infrastructure/AttendancePlatform.Shared.Infrastructure/"]

RUN dotnet restore "AttendancePlatform.Api/AttendancePlatform.Api.csproj"
COPY src/backend/ .
WORKDIR "/src/AttendancePlatform.Api"
RUN dotnet build "AttendancePlatform.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "AttendancePlatform.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AttendancePlatform.Api.dll"]
