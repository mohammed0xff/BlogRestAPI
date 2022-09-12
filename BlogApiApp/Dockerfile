FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base

ENV ASPNETCORE_ENVIRONMENT=Development 
ENV ASPNETCORE_URLS=http://+:7086
EXPOSE 7086

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src

COPY ["BlogApi/BlogApi.csproj", "./BlogApi/"]
COPY ["DataAccess/DataAccess.csproj", "./DataAccess/"]
COPY ["Models/Models.csproj", "./Data/"]
COPY ["Services/Services.csproj", "./Services/"]

RUN dotnet restore "BlogApi/BlogApi.csproj"
COPY . .

WORKDIR "/src/BlogApi"
RUN dotnet build "BlogApi.csproj" -c Release -o /app/build


FROM build AS publish
RUN dotnet publish "BlogApi.csproj" -c Release -o /app/publish


FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .


ENTRYPOINT ["dotnet", "BlogApi.dll"]