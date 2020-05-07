# ===============
# BUILD IMAGE
# ===============
FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build
WORKDIR /app

# Copy csproj and restore as distinct layers
WORKDIR /app/RabbitCommunicationLib
COPY ./RabbitCommunicationLib/*.csproj ./
RUN dotnet restore

WORKDIR /app/matchdb/MatchEntities/MatchEntities
COPY ./matchdb/MatchEntities/MatchEntities/*.csproj ./
RUN dotnet restore

WORKDIR /app/matchdb/Database
COPY ./matchdb/Database/*.csproj ./
RUN dotnet restore

WORKDIR /app/EquipmentLib/EquipmentLib
COPY ./EquipmentLib/EquipmentLib/*.csproj ./
RUN dotnet restore

WORKDIR /app/zonereader/ZoneReader
COPY ./zonereader/ZoneReader/*.csproj ./
RUN dotnet restore

WORKDIR /app/SituationDatabase
COPY ./SituationDatabase/*.csproj ./
RUN dotnet restore

WORKDIR /app/SituationOperator
COPY ./SituationOperator/*.csproj ./
RUN dotnet restore

# Copy everything else and build
WORKDIR /app
COPY ./RabbitCommunicationLib ./RabbitCommunicationLib
COPY ./matchdb/Database ./matchdb/Database
COPY ./matchdb/MatchEntities ./matchdb/MatchEntities
COPY ./SituationDatabase ./SituationDatabase
COPY ./SituationOperator/ ./SituationOperator
COPY ./EquipmentLib/EquipmentLib ./EquipmentLib/EquipmentLib
COPY ./zonereader/ZoneReader ./zonereader/ZoneReader

RUN dotnet publish SituationOperator/ -c Release -o out

# ===============
# RUNTIME IMAGE
# ===============
FROM mcr.microsoft.com/dotnet/core/aspnet:3.1 AS runtime
WORKDIR /app

# Runtime resources
WORKDIR /app/data
COPY ./EquipmentLib/EquipmentLib/EquipmentData ./EquipmentData
COPY ./zonereader/ZoneReader/resources ./ZoneData

COPY --from=build /app/out .
ENTRYPOINT ["dotnet", "SituationOperator.dll"]

# ===============
# SET ENVIRONMENT VARIABLES
# ===============
ENV ZONEREADER_RESOURCE_PATH /app/data/ZoneData
ENV EQUIPMENT_CSV_DIRECTORY /app/data/EquipmentData
