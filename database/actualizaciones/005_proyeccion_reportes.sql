USE DevsuClientes;
GO

IF OBJECT_ID(N'clientes.CuentasReporte', N'U') IS NULL
BEGIN
    CREATE TABLE clientes.CuentasReporte
    (
        CuentaId UNIQUEIDENTIFIER NOT NULL,
        ClienteId UNIQUEIDENTIFIER NOT NULL,
        NumeroCuenta NVARCHAR(20) NOT NULL,
        TipoCuenta NVARCHAR(20) NOT NULL,
        SaldoInicial DECIMAL(18, 2) NOT NULL,
        SaldoDisponible DECIMAL(18, 2) NOT NULL,
        Estado BIT NOT NULL,
        ActualizadoEn DATETIME2(0) NOT NULL,
        CONSTRAINT PK_CuentasReporte PRIMARY KEY (CuentaId),
        CONSTRAINT IX_CuentasReporte_ClienteId UNIQUE (ClienteId, CuentaId)
    );
END;
GO

IF OBJECT_ID(N'clientes.MovimientosReporte', N'U') IS NULL
BEGIN
    CREATE TABLE clientes.MovimientosReporte
    (
        MovimientoId UNIQUEIDENTIFIER NOT NULL,
        CuentaId UNIQUEIDENTIFIER NOT NULL,
        Fecha DATETIME2(0) NOT NULL,
        TipoMovimiento NVARCHAR(20) NOT NULL,
        Valor DECIMAL(18, 2) NOT NULL,
        Saldo DECIMAL(18, 2) NOT NULL,
        CONSTRAINT PK_MovimientosReporte PRIMARY KEY (MovimientoId),
        CONSTRAINT FK_MovimientosReporte_CuentasReporte FOREIGN KEY (CuentaId)
            REFERENCES clientes.CuentasReporte(CuentaId) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS
(
    SELECT 1
    FROM sys.indexes
    WHERE name = N'IX_MovimientosReporte_CuentaId_Fecha'
      AND object_id = OBJECT_ID(N'clientes.MovimientosReporte')
)
BEGIN
    CREATE INDEX IX_MovimientosReporte_CuentaId_Fecha
        ON clientes.MovimientosReporte(CuentaId, Fecha);
END;
GO
