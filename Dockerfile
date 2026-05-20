# Stage 1: Build Quasar Frontend
FROM node:20 AS frontend-build
WORKDIR /app/frontend
# Copy frontend source
COPY CaseStudy01/ ./
RUN npm install
# Build Quasar app (produces output in dist/spa)
RUN npx quasar build

# Stage 2: Build ASP.NET Core Backend
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS backend-build
WORKDIR /app/backend
# Copy csproj and restore dependencies
COPY Casestudy/*.csproj ./Casestudy/
WORKDIR /app/backend/Casestudy
RUN dotnet restore
# Copy everything else and build
COPY Casestudy/. ./
RUN dotnet publish -c Release -o /app/backend/publish

# Stage 3: Serve the application
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

# Copy backend publish output
COPY --from=backend-build /app/backend/publish .
# Copy frontend build output to wwwroot
COPY --from=frontend-build /app/frontend/dist/spa ./wwwroot

ENTRYPOINT ["dotnet", "Casestudy.dll"]
