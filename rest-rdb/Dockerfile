
FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build-env
WORKDIR /app

# Copy csproj and restore as distinct layers
COPY *.csproj ./
RUN dotnet restore

# Copy everything else and build
COPY . ./
RUN dotnet publish -c Release -o out

# Build runtime image
FROM mcr.microsoft.com/dotnet/core/aspnet:3.1
# COPY clidriver /app/
# ENV LD_LIBRARY_PATH="/app/clidriver/lib/" 
# ENV PATH=$PATH:"/app/clidriver/bin:/app/clidriver/lib" 
# RUN apt-get update; \ 
#     apt-get install -y libxml2-dev;
WORKDIR /app
COPY --from=build-env /app/out .
ENTRYPOINT ["dotnet", "webapi.dll"]