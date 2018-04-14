FROM microsoft/dotnet:2.0-sdk AS build
WORKDIR /app

# copy csproj and restore as distinct layers
COPY BackEnd.csproj ./BackEnd.csproj
RUN dotnet restore

# copy everything else and build app
COPY ./ ./
WORKDIR /app
RUN dotnet publish -o out -c Release /p:PublishWithAspNetCoreTargetManifest="false"


FROM microsoft/dotnet:2.0-runtime AS runtime
ENV ASPNETCORE_URLS http://+:80
WORKDIR /app
COPY --from=build /app/out ./
ENTRYPOINT ["dotnet", "BackEnd.dll"]