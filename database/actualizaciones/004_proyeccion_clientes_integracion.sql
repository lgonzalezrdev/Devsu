USE DevsuCuentas;
GO

IF OBJECT_ID(N'cuentas.ClientesIntegracion', N'U') IS NULL
BEGIN
    CREATE TABLE cuentas.ClientesIntegracion
    (
        ClienteId UNIQUEIDENTIFIER NOT NULL,
        Nombre NVARCHAR(150) NOT NULL,
        Estado BIT NOT NULL,
        ActualizadoEn DATETIME2(0) NOT NULL,
        CONSTRAINT PK_ClientesIntegracion PRIMARY KEY (ClienteId)
    );
END;
GO
