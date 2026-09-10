USE DevsuCuentas;
GO

IF NOT EXISTS (SELECT 1 FROM sys.default_constraints WHERE name = N'DF_Cuentas_Estado')
    ALTER TABLE cuentas.Cuentas ADD CONSTRAINT DF_Cuentas_Estado DEFAULT 1 FOR Estado;
GO

IF NOT EXISTS (SELECT 1 FROM sys.check_constraints WHERE name = N'CK_Cuentas_NumeroCuenta')
    ALTER TABLE cuentas.Cuentas WITH CHECK ADD CONSTRAINT CK_Cuentas_NumeroCuenta CHECK (LEN(NumeroCuenta) = 6 AND NumeroCuenta NOT LIKE '%[^0-9]%');
GO

IF NOT EXISTS (SELECT 1 FROM sys.check_constraints WHERE name = N'CK_Cuentas_TipoCuenta')
    ALTER TABLE cuentas.Cuentas WITH CHECK ADD CONSTRAINT CK_Cuentas_TipoCuenta CHECK (TipoCuenta IN (N'Ahorros', N'Corriente'));
GO

IF NOT EXISTS (SELECT 1 FROM sys.check_constraints WHERE name = N'CK_Cuentas_Saldos')
    ALTER TABLE cuentas.Cuentas WITH CHECK ADD CONSTRAINT CK_Cuentas_Saldos CHECK (SaldoInicial >= 0 AND SaldoDisponible >= 0);
GO

IF NOT EXISTS (SELECT 1 FROM sys.check_constraints WHERE name = N'CK_Movimientos_Valor')
    ALTER TABLE cuentas.Movimientos WITH CHECK ADD CONSTRAINT CK_Movimientos_Valor CHECK ((TipoMovimiento = N'Deposito' AND Valor > 0) OR (TipoMovimiento = N'Retiro' AND Valor < 0));
GO
