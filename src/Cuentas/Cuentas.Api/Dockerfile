FROM mcr.microsoft.com/dotnet/sdk:10.0 AS compilacion
WORKDIR /origen

COPY ["src/Cuentas/Cuentas.Api/Cuentas.Api.csproj", "src/Cuentas/Cuentas.Api/"]
COPY ["src/Cuentas/Cuentas.Aplicacion/Cuentas.Aplicacion.csproj", "src/Cuentas/Cuentas.Aplicacion/"]
COPY ["src/Cuentas/Cuentas.Dominio/Cuentas.Dominio.csproj", "src/Cuentas/Cuentas.Dominio/"]
COPY ["src/Cuentas/Cuentas.Infraestructura/Cuentas.Infraestructura.csproj", "src/Cuentas/Cuentas.Infraestructura/"]
COPY ["src/Construccion/Contratos.Compartidos/Contratos.Compartidos.csproj", "src/Construccion/Contratos.Compartidos/"]
COPY ["src/Construccion/Observabilidad.Compartida/Observabilidad.Compartida.csproj", "src/Construccion/Observabilidad.Compartida/"]
RUN dotnet restore "src/Cuentas/Cuentas.Api/Cuentas.Api.csproj"

COPY . .
RUN dotnet publish "src/Cuentas/Cuentas.Api/Cuentas.Api.csproj" --configuration Release --output /aplicacion/publicada --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /aplicacion
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
COPY --from=compilacion /aplicacion/publicada .
ENTRYPOINT ["dotnet", "Cuentas.Api.dll"]
