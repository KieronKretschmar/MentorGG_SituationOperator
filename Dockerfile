# ===============
# BUILD IMAGE
# ===============
FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build
WORKDIR /app

# Copy csproj and restore as distinct layers

WORKDIR /app/SituationOperator
COPY ./SituationOperator/*.csproj ./
RUN dotnet restore

# Copy everything else and build
WORKDIR /app
COPY ./SituationOperator/ ./SituationOperator

RUN dotnet publish SituationOperator/ -c Release -o out

# ===============
# RUNTIME IMAGE
# ===============
FROM mcr.microsoft.com/dotnet/core/aspnet:3.1 AS runtime
WORKDIR /app

COPY --from=build /app/out .
ENTRYPOINT ["dotnet", "SituationOperator.dll"]

