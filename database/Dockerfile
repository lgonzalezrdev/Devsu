FROM mcr.microsoft.com/mssql/server:2022-latest

USER root
COPY database/BaseDatos.sql /scripts/BaseDatos.sql
COPY database/iniciar-sqlserver.sh /usr/local/bin/iniciar-sqlserver.sh
RUN chmod +x /usr/local/bin/iniciar-sqlserver.sh

USER mssql
ENTRYPOINT ["/usr/local/bin/iniciar-sqlserver.sh"]
