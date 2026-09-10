USE DevsuClientes;
GO

IF NOT EXISTS (SELECT 1 FROM sys.default_constraints WHERE name = N'DF_Clientes_Estado')
BEGIN
    ALTER TABLE clientes.Clientes
        ADD CONSTRAINT DF_Clientes_Estado DEFAULT 1 FOR Estado;
END;
GO

IF NOT EXISTS (SELECT 1 FROM sys.check_constraints WHERE name = N'CK_Personas_Identificacion')
BEGIN
    ALTER TABLE clientes.Personas WITH CHECK
        ADD CONSTRAINT CK_Personas_Identificacion
        CHECK (LEN(Identificacion) = 10 AND Identificacion NOT LIKE '%[^0-9]%');
END;
GO

IF NOT EXISTS (SELECT 1 FROM sys.check_constraints WHERE name = N'CK_Personas_Genero')
BEGIN
    ALTER TABLE clientes.Personas WITH CHECK
        ADD CONSTRAINT CK_Personas_Genero
        CHECK (Genero IN (N'Masculino', N'Femenino', N'Otro', N'NoEspecificado'));
END;
GO

IF NOT EXISTS (SELECT 1 FROM sys.check_constraints WHERE name = N'CK_Personas_Direccion')
BEGIN
    ALTER TABLE clientes.Personas WITH CHECK
        ADD CONSTRAINT CK_Personas_Direccion
        CHECK (LEN(Direccion) BETWEEN 5 AND 250);
END;
GO

IF NOT EXISTS (SELECT 1 FROM sys.check_constraints WHERE name = N'CK_Personas_Telefono')
BEGIN
    ALTER TABLE clientes.Personas WITH CHECK
        ADD CONSTRAINT CK_Personas_Telefono
        CHECK (LEN(Telefono) BETWEEN 7 AND 15 AND Telefono NOT LIKE '%[^0-9]%');
END;
GO
