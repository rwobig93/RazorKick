FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["TestBlazorServerApp/TestBlazorServerApp.csproj", "TestBlazorServerApp/"]
RUN dotnet restore "TestBlazorServerApp/TestBlazorServerApp.csproj"
COPY . .
WORKDIR "/src/TestBlazorServerApp"
RUN dotnet build "TestBlazorServerApp.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "TestBlazorServerApp.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TestBlazorServerApp.dll"]
