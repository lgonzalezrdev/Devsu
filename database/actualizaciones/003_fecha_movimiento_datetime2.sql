USE DevsuCuentas;
GO

ALTER TABLE cuentas.Movimientos
    ALTER COLUMN Fecha DATETIME2(0) NOT NULL;
GO
