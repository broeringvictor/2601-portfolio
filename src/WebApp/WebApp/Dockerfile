FROM node:20-alpine AS tailwind
WORKDIR /src

# Copia os arquivos necessários
COPY src/WebApp/WebApp/Styles/ src/WebApp/WebApp/Styles/
COPY src/WebApp/WebApp/Components/ src/WebApp/WebApp/Components/
COPY src/WebApp/WebApp.Client/ src/WebApp/WebApp.Client/

RUN mkdir -p src/WebApp/WebApp/wwwroot
RUN npm init -y
RUN npm install tailwindcss @tailwindcss/cli
RUN npx @tailwindcss/cli -i src/WebApp/WebApp/Styles/input.css -o src/WebApp/WebApp/wwwroot/tailwind.css --minify



FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/WebApp/WebApp/WebApp.csproj", "src/WebApp/WebApp/"]
COPY ["src/WebApp/WebApp.Client/WebApp.Client.csproj", "src/WebApp/WebApp.Client/"]
RUN dotnet restore "src/WebApp/WebApp/WebApp.csproj"
COPY . .
COPY --from=tailwind /src/src/WebApp/WebApp/wwwroot/tailwind.css src/WebApp/WebApp/wwwroot/tailwind.css
WORKDIR "/src/src/WebApp/WebApp"
RUN dotnet build "./WebApp.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./WebApp.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "WebApp.dll"]
