FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["src/backend/services/VoiceRecognition/AttendancePlatform.VoiceRecognition.Api/AttendancePlatform.VoiceRecognition.Api.csproj", "src/backend/services/VoiceRecognition/AttendancePlatform.VoiceRecognition.Api/"]
COPY ["src/backend/shared/Infrastructure/AttendancePlatform.Shared.Infrastructure/AttendancePlatform.Shared.Infrastructure.csproj", "src/backend/shared/Infrastructure/AttendancePlatform.Shared.Infrastructure/"]
COPY ["src/backend/shared/Domain/AttendancePlatform.Shared.Domain/AttendancePlatform.Shared.Domain.csproj", "src/backend/shared/Domain/AttendancePlatform.Shared.Domain/"]
RUN dotnet restore "src/backend/services/VoiceRecognition/AttendancePlatform.VoiceRecognition.Api/AttendancePlatform.VoiceRecognition.Api.csproj"
COPY . .
WORKDIR "/src/src/backend/services/VoiceRecognition/AttendancePlatform.VoiceRecognition.Api"
RUN dotnet build "AttendancePlatform.VoiceRecognition.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "AttendancePlatform.VoiceRecognition.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AttendancePlatform.VoiceRecognition.Api.dll"]
