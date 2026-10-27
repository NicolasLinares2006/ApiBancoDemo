# Etapa de compilación
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY ["ApiBancoDemo.csproj", "./"]
RUN dotnet restore "ApiBancoDemo.csproj"
COPY . .
RUN dotnet publish "ApiBancoDemo.csproj" -c Release -o /app/publish

# Etapa final de ejecución
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080
# Render inyecta la variable PORT (por defecto 10000). 
# La app debe escuchar en 0.0.0.0 en ese puerto.
ENV ASPNETCORE_URLS=http://0.0.0.0:${PORT:-8080}
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "ApiBancoDemo.dll"]