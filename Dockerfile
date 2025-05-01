# Etapa 1: build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copia os csproj e restaura os pacotes
COPY ["src/Vrumm.Api/Vrumm.Api.csproj", "src/Vrumm.Api/"]
COPY ["src/Vrumm.Application/Vrumm.Application.csproj", "src/Vrumm.Application/"]
COPY ["src/Vrumm.Infrastructure/Vrumm.Infrastructure.csproj", "src/Vrumm.Infrastructure/"]
COPY ["src/Vrumm.Domain/Vrumm.Domain.csproj", "src/Vrumm.Domain/"]

# Copia o restante dos arquivos
COPY . .

# Restaura dependências
RUN dotnet restore "src/Vrumm.Api/Vrumm.Api.csproj"

# Build
RUN dotnet publish "src/Vrumm.Api/Vrumm.Api.csproj" -c Release -o /app/publish

# Etapa 2: runtime
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Vrumm.Api.dll"]
