FROM mcr.microsoft.com/dotnet/sdk:10.0 AS compilacion
WORKDIR /origen

COPY ["src/Clientes/Clientes.Api/Clientes.Api.csproj", "src/Clientes/Clientes.Api/"]
COPY ["src/Clientes/Clientes.Aplicacion/Clientes.Aplicacion.csproj", "src/Clientes/Clientes.Aplicacion/"]
COPY ["src/Clientes/Clientes.Dominio/Clientes.Dominio.csproj", "src/Clientes/Clientes.Dominio/"]
COPY ["src/Clientes/Clientes.Infraestructura/Clientes.Infraestructura.csproj", "src/Clientes/Clientes.Infraestructura/"]
COPY ["src/Construccion/Contratos.Compartidos/Contratos.Compartidos.csproj", "src/Construccion/Contratos.Compartidos/"]
RUN dotnet restore "src/Clientes/Clientes.Api/Clientes.Api.csproj"

COPY . .
RUN dotnet publish "src/Clientes/Clientes.Api/Clientes.Api.csproj" --configuration Release --output /aplicacion/publicada --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /aplicacion
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
COPY --from=compilacion /aplicacion/publicada .
ENTRYPOINT ["dotnet", "Clientes.Api.dll"]
