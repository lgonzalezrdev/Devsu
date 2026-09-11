#!/usr/bin/env bash
set -euo pipefail

/opt/mssql/bin/sqlservr &
proceso_sqlserver=$!

for intento in $(seq 1 60); do
    if /opt/mssql-tools18/bin/sqlcmd -C -S localhost -U sa -P "$MSSQL_SA_PASSWORD" -Q "SELECT 1" > /dev/null 2>&1; then
        /opt/mssql-tools18/bin/sqlcmd -C -S localhost -U sa -P "$MSSQL_SA_PASSWORD" -i /scripts/BaseDatos.sql
        touch /tmp/base-datos-lista
        break
    fi

    if [ "$intento" -eq 60 ]; then
        echo "SQL Server no estuvo disponible para inicializar la base de datos."
        exit 1
    fi

    sleep 2
done

trap 'kill "$proceso_sqlserver"; wait "$proceso_sqlserver"' TERM INT
wait "$proceso_sqlserver"
