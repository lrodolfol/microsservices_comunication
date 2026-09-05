# Config/Dockerfile

# Build configuration default
ARG BUILD_CONFIGURATION=Release

# Runtime image
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

# Build image
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copia a solução e os csproj para aproveitar cache
COPY ../NameBankSln.sln .
COPY ../NamesBank/NamesBank.csproj NamesBank/
COPY ../NamesBankTests/NamesBank.Tests.csproj NamesBankTests/

# Restaura dependências
RUN dotnet restore NameBankSln.sln

# Copia o restante do código
COPY . .

# Build
WORKDIR /src/NamesBank
RUN dotnet build "NamesBank.csproj" -c Release -o /app/build

# (Opcional) roda os testes (COLOCAR DENTRO DO STAGE DE publish PARA FALHAR O BUILD SE OS TESTES FALHAREM)
#FROM build AS test
#WORKDIR /src
#RUN dotnet test NamesBankTests/NamesBank.Tests.csproj --no-build --logger:"console;verbosity=normal"

# Build e teste do projeto de testes (sem --no-build)
WORKDIR /src/NamesBankTests
RUN dotnet build "NamesBank.Tests.csproj" -c Release
RUN dotnet test "NamesBank.Tests.csproj" --logger "console;verbosity=normal"

# Publica a aplicação
FROM build AS publish
WORKDIR /src
RUN dotnet publish NamesBank/"NamesBank.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Imagem final (runtime)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "NamesBank.dll"]
