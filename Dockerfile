# Stage 1: Build Tailwind CSS
FROM node:20-alpine AS tailwind
WORKDIR /src
COPY src/WebApp/WebApp/Styles/input.css src/WebApp/WebApp/Styles/input.css
COPY src/WebApp/WebApp/Components/ src/WebApp/WebApp/Components/
COPY src/WebApp/WebApp.Client/ src/WebApp/WebApp.Client/
RUN npx tailwindcss -i src/WebApp/WebApp/Styles/input.css -o src/WebApp/WebApp/wwwroot/tailwind.css --minify

# Stage 2: Build .NET app
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY src/WebApp/WebApp/WebApp.csproj src/WebApp/WebApp/
COPY src/WebApp/WebApp.Client/WebApp.Client.csproj src/WebApp/WebApp.Client/
RUN dotnet restore src/WebApp/WebApp/WebApp.csproj
COPY src/WebApp/ src/WebApp/
COPY --from=tailwind /src/src/WebApp/WebApp/wwwroot/tailwind.css src/WebApp/WebApp/wwwroot/tailwind.css
RUN dotnet publish src/WebApp/WebApp/WebApp.csproj -c Release -o /app/publish

# Stage 3: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
EXPOSE 8080
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "WebApp.dll"]
