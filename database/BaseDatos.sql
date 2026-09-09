USE master;
GO

IF DB_ID(N'DevsuClientes') IS NULL
BEGIN
    CREATE DATABASE DevsuClientes;
END;
GO

IF DB_ID(N'DevsuCuentas') IS NULL
BEGIN
    CREATE DATABASE DevsuCuentas;
END;
GO

USE DevsuClientes;
GO

IF SCHEMA_ID(N'clientes') IS NULL
BEGIN
    EXEC(N'CREATE SCHEMA clientes');
END;
GO

IF OBJECT_ID(N'clientes.Personas', N'U') IS NULL
BEGIN
    CREATE TABLE clientes.Personas
    (
        PersonaId UNIQUEIDENTIFIER NOT NULL,
        Nombre NVARCHAR(150) NOT NULL,
        Genero NVARCHAR(30) NOT NULL CONSTRAINT DF_Personas_Genero DEFAULT N'',
        Edad INT NOT NULL,
        Identificacion NVARCHAR(30) NOT NULL,
        Direccion NVARCHAR(250) NOT NULL CONSTRAINT DF_Personas_Direccion DEFAULT N'',
        Telefono NVARCHAR(30) NOT NULL CONSTRAINT DF_Personas_Telefono DEFAULT N'',
        CONSTRAINT PK_Personas PRIMARY KEY (PersonaId),
        CONSTRAINT UQ_Personas_Identificacion UNIQUE (Identificacion)
    );
END;
GO

IF OBJECT_ID(N'clientes.Clientes', N'U') IS NULL
BEGIN
    CREATE TABLE clientes.Clientes
    (
        PersonaId UNIQUEIDENTIFIER NOT NULL,
        ContrasenaHash NVARCHAR(500) NOT NULL,
        Estado BIT NOT NULL,
        CONSTRAINT PK_Clientes PRIMARY KEY (PersonaId),
        CONSTRAINT FK_Clientes_Personas FOREIGN KEY (PersonaId)
            REFERENCES clientes.Personas(PersonaId) ON DELETE CASCADE
    );
END;
GO

USE DevsuCuentas;
GO

IF SCHEMA_ID(N'cuentas') IS NULL
BEGIN
    EXEC(N'CREATE SCHEMA cuentas');
END;
GO

IF OBJECT_ID(N'cuentas.Cuentas', N'U') IS NULL
BEGIN
    CREATE TABLE cuentas.Cuentas
    (
        CuentaId UNIQUEIDENTIFIER NOT NULL,
        ClienteId UNIQUEIDENTIFIER NOT NULL,
        NumeroCuenta NVARCHAR(20) NOT NULL,
        TipoCuenta NVARCHAR(20) NOT NULL,
        SaldoInicial DECIMAL(18, 2) NOT NULL,
        SaldoDisponible DECIMAL(18, 2) NOT NULL,
        Estado BIT NOT NULL,
        CONSTRAINT PK_Cuentas PRIMARY KEY (CuentaId),
        CONSTRAINT UQ_Cuentas_NumeroCuenta UNIQUE (NumeroCuenta)
    );
END;
GO

IF OBJECT_ID(N'cuentas.Movimientos', N'U') IS NULL
BEGIN
    CREATE TABLE cuentas.Movimientos
    (
        MovimientoId UNIQUEIDENTIFIER NOT NULL,
        CuentaId UNIQUEIDENTIFIER NOT NULL,
        Fecha DATETIMEOFFSET NOT NULL,
        TipoMovimiento NVARCHAR(20) NOT NULL,
        Valor DECIMAL(18, 2) NOT NULL,
        Saldo DECIMAL(18, 2) NOT NULL,
        CONSTRAINT PK_Movimientos PRIMARY KEY (MovimientoId),
        CONSTRAINT FK_Movimientos_Cuentas FOREIGN KEY (CuentaId)
            REFERENCES cuentas.Cuentas(CuentaId)
    );
END;
GO

IF NOT EXISTS
(
    SELECT 1
    FROM sys.indexes
    WHERE name = N'IX_Movimientos_CuentaId'
      AND object_id = OBJECT_ID(N'cuentas.Movimientos')
)
BEGIN
    CREATE INDEX IX_Movimientos_CuentaId ON cuentas.Movimientos(CuentaId);
END;
GO
