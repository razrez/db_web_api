FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["DB.csproj", "./"]
RUN dotnet restore "DB.csproj"
COPY . .
WORKDIR "/src/"
RUN dotnet build "DB.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "DB.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DB.dll"]
