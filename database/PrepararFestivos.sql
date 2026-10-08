-- Preparación opcional. Ejecutar en SSMS conectado a GARZON, base Festivos.
-- Conserva tablas y registros existentes; no usa DROP, DELETE ni UPDATE.
-- La API no ejecuta este archivo ni modifica el esquema al iniciar.
USE [Festivos];
GO
SET XACT_ABORT ON;
BEGIN TRANSACTION;

IF OBJECT_ID(N'dbo.Pais', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Pais (
        Id int IDENTITY(1,1) NOT NULL CONSTRAINT PK_Pais PRIMARY KEY,
        Nombre nvarchar(100) NOT NULL CONSTRAINT UQ_Pais_Nombre UNIQUE
    );
END;
IF OBJECT_ID(N'dbo.TipoFestivo', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.TipoFestivo (
        Id int IDENTITY(1,1) NOT NULL CONSTRAINT PK_TipoFestivo PRIMARY KEY,
        Tipo nvarchar(100) NOT NULL CONSTRAINT UQ_TipoFestivo_Tipo UNIQUE
    );
END;
IF OBJECT_ID(N'dbo.Festivo', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Festivo (
        Id int IDENTITY(1,1) NOT NULL CONSTRAINT PK_Festivo PRIMARY KEY,
        IdPais int NOT NULL,
        Nombre nvarchar(100) NOT NULL,
        Dia int NOT NULL,
        Mes int NOT NULL,
        DiasPascua int NOT NULL,
        IdTipo int NOT NULL,
        CONSTRAINT FK_Festivo_Pais FOREIGN KEY (IdPais) REFERENCES dbo.Pais(Id),
        CONSTRAINT FK_Festivo_TipoFestivo FOREIGN KEY (IdTipo) REFERENCES dbo.TipoFestivo(Id)
    );
END;

IF EXISTS (SELECT 1 FROM dbo.Pais WHERE Id = 1 AND Nombre <> N'Colombia')
    THROW 50001, 'El país 1 ya pertenece a otro país. No se modificó ningún registro.', 1;
IF EXISTS (SELECT 1 FROM dbo.Pais WHERE Nombre = N'Colombia' AND Id <> 1)
    THROW 50002, 'Colombia ya existe con otro Id. Revise el catálogo antes de cargar los ejemplos.', 1;

IF COLUMNPROPERTY(OBJECT_ID(N'dbo.Pais'), 'Id', 'IsIdentity') = 1
    SET IDENTITY_INSERT dbo.Pais ON;
IF NOT EXISTS (SELECT 1 FROM dbo.Pais WHERE Id = 1)
    INSERT INTO dbo.Pais (Id, Nombre) VALUES (1, N'Colombia');
IF COLUMNPROPERTY(OBJECT_ID(N'dbo.Pais'), 'Id', 'IsIdentity') = 1
    SET IDENTITY_INSERT dbo.Pais OFF;

IF COLUMNPROPERTY(OBJECT_ID(N'dbo.TipoFestivo'), 'Id', 'IsIdentity') = 1
    SET IDENTITY_INSERT dbo.TipoFestivo ON;
INSERT INTO dbo.TipoFestivo (Id, Tipo)
SELECT datos.Id, datos.Tipo FROM (VALUES
    (1, N'Fijo'),
    (2, N'Ley de Puente festivo'),
    (3, N'Basado en el domingo de pascua'),
    (4, N'Basado en el domingo de pascua y Ley de Puente festivo')
) AS datos(Id, Tipo)
WHERE NOT EXISTS (SELECT 1 FROM dbo.TipoFestivo existente WHERE existente.Id = datos.Id);
IF COLUMNPROPERTY(OBJECT_ID(N'dbo.TipoFestivo'), 'Id', 'IsIdentity') = 1
    SET IDENTITY_INSERT dbo.TipoFestivo OFF;

INSERT INTO dbo.Festivo (IdPais, Nombre, Dia, Mes, DiasPascua, IdTipo)
SELECT datos.IdPais, datos.Nombre, datos.Dia, datos.Mes, datos.DiasPascua, datos.IdTipo
FROM (VALUES
    (1, N'Año nuevo', 1, 1, 0, 1),
    (1, N'Santos Reyes', 6, 1, 0, 2),
    (1, N'San José', 19, 3, 0, 2),
    (1, N'Jueves Santo', 0, 0, -3, 3),
    (1, N'Viernes Santo', 0, 0, -2, 3),
    (1, N'Domingo de Pascua', 0, 0, 0, 3),
    (1, N'Día del Trabajo', 1, 5, 0, 1),
    (1, N'Ascensión del Señor', 0, 0, 40, 4),
    (1, N'Corpus Christi', 0, 0, 61, 4),
    (1, N'Sagrado Corazón de Jesús', 0, 0, 68, 4),
    (1, N'San Pedro y San Pablo', 29, 6, 0, 2),
    (1, N'Independencia Colombia', 20, 7, 0, 1),
    (1, N'Batalla de Boyacá', 7, 8, 0, 1),
    (1, N'Asunción de la Virgen', 15, 8, 0, 2),
    (1, N'Día de la Raza', 12, 10, 0, 2),
    (1, N'Todos los santos', 1, 11, 0, 2),
    (1, N'Independencia de Cartagena', 11, 11, 0, 2),
    (1, N'Inmaculada Concepción', 8, 12, 0, 1),
    (1, N'Navidad', 25, 12, 0, 1)
) AS datos(IdPais, Nombre, Dia, Mes, DiasPascua, IdTipo)
WHERE NOT EXISTS (
    SELECT 1 FROM dbo.Festivo existente
    WHERE existente.IdPais = datos.IdPais AND existente.Nombre = datos.Nombre
);
COMMIT TRANSACTION;
GO
