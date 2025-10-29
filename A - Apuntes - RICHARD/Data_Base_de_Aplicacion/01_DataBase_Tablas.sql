create database GestionOfPolicial

go

use GestionOfPolicial

go

create table Menu(
idMenu int primary key identity(1,1),
descripcion varchar(30),
idMenuPadre int references Menu(idMenu),
icono varchar(30),
controlador varchar(30),
paginaAccion varchar(30),
esActivo bit,
fechaRegistro datetime default getdate()
)

go

create table Rol(
idRol int primary key identity(1,1),
descripcion varchar(30),
esActivo bit,
fechaRegistro datetime default getdate()
)

go
 
 create table RolMenu(
 idRolMenu int primary key identity(1,1),
 idRol int references Rol(idRol),
 idMenu int references Menu(idMenu),
 esActivo bit,
 fechaRegistro datetime default getdate()
 )

go

create table Usuario(
idUsuario int primary key identity(1,1),
nombre varchar(50),
correo varchar(50),
telefono varchar(50),
idRol int references Rol(idRol),
urlFoto varchar(500),
nombreFoto varchar(100),
clave varchar(100),
esActivo bit,
fechaRegistro datetime default getdate()
)

go

CREATE TABLE Dependencia (
    IdDependencia INT PRIMARY KEY IDENTITY(1,1),
    IdUsuario INT NULL REFERENCES Usuario(IdUsuario), -- Usuario que puede modificar los datos de la dependencia
    UrlLogo VARCHAR(500),
    NombreLogo VARCHAR(100),
    Nombre VARCHAR(50),
    Correo VARCHAR(50),
    Direccion VARCHAR(100),
    Telefono VARCHAR(50),
    FechaRegistro DATETIME DEFAULT GETDATE()
);

go

create table Configuracion(
recurso varchar(50),
propiedad varchar(50),
valor varchar(60)
);

go

create table PersonalPolicial (
IdPersonal INT IDENTITY(1,1) PRIMARY KEY,
Legajo NVARCHAR(50),              
idUsuario int references Usuario(idUsuario),
ApellidoYNombre NVARCHAR(255),
Grado NVARCHAR(50),
Chapa NVARCHAR(50),
Sexo NVARCHAR(21),
Funcion NVARCHAR(100),
Horario NVARCHAR(50),
SituacionRevista NVARCHAR(50),
FechaNacimiento DATE,
Telefono NVARCHAR(55),
TelefonoEmergencia NVARCHAR(55),
DNI NVARCHAR(50),
SubsidioSalud NVARCHAR(50),
EstudiosCurs NVARCHAR(255),
EstadoCivil NVARCHAR(50),
Especialidad NVARCHAR(50),
AltaEnDivision DATE,
AltaEnPolicia DATE,
DestinoAnterior NVARCHAR(100),
Email NVARCHAR(50),
Trasladado BIT DEFAULT 0,
Detalles NVARCHAR(MAX) DEFAULT '',
urlImagen varchar(500),
nombreImagen varchar(100),
FechaRegistro DATETIME DEFAULT GETDATE(), -- Valor por defecto
FechaEliminacion DATETIME NULL, -- Nueva columna
);

GO

CREATE TABLE Domicilio (
IdDomicilio INT PRIMARY KEY IDENTITY(1,1),
IdUsuario INT REFERENCES Usuario(IdUsuario),
CalleBarrio NVARCHAR(159),
Localidad NVARCHAR(159),
ComisariaJuris NVARCHAR(75),
IdPersonal INT REFERENCES PersonalPolicial(IdPersonal) ON DELETE CASCADE,
FechaRegistro DATETIME DEFAULT GETDATE(), -- Valor por defecto
);

GO

CREATE TABLE Arma (
IdArma INT PRIMARY KEY IDENTITY(1,1),
IdUsuario INT REFERENCES Usuario(IdUsuario),
NumeroSerie NVARCHAR(50),
Marca NVARCHAR(50),
IdPersonal INT REFERENCES PersonalPolicial(IdPersonal) ON DELETE CASCADE,
FechaRegistro DATETIME DEFAULT GETDATE(), -- Valor por defecto
);

GO

CREATE TABLE Chaleco (
IdChaleco INT IDENTITY(1,1) PRIMARY KEY,
SerieChaleco VARCHAR(255),
IdUsuario INT REFERENCES Usuario(IdUsuario),
MarcaYModelo NVARCHAR(255),
Talle NVARCHAR(50),
AnoFabricacion DATE,
AnoVencimiento DATE,
EstadoChaleco NVARCHAR(50),
Observaciones NVARCHAR(MAX),
IdPersonal INT REFERENCES PersonalPolicial(IdPersonal) NULL,
Eliminado BIT DEFAULT 0,
FechaRegistro DATETIME DEFAULT GETDATE(), -- Valor por defecto
FechaEliminacion DATETIME NULL,
);

GO

CREATE TABLE Escopeta (
IdEscopeta INT IDENTITY(1,1) PRIMARY KEY,
SerieEscopeta VARCHAR(255),
IdUsuario int references Usuario(IdUsuario),
MarcayModelo NVARCHAR(50),
EstadoEscopeta NVARCHAR(50),
Observaciones NVARCHAR(MAX),
Eliminado BIT DEFAULT 0,
FechaRegistro DATETIME DEFAULT GETDATE(), -- Valor por defecto
FechaEliminacion DATETIME NULL, -- Nueva columna
);

GO

CREATE TABLE Radio (
IdRadio INT IDENTITY(1,1) PRIMARY KEY,
SerieRadio VARCHAR(255),
IdUsuario int references Usuario(IdUsuario),
MarcayModelo NVARCHAR(50),
EstadoRadio NVARCHAR(50),
Tipo NVARCHAR(50),
Observaciones NVARCHAR(MAX),
Eliminado BIT DEFAULT 0,
FechaRegistro DATETIME DEFAULT GETDATE(), -- Valor por defecto
FechaEliminacion DATETIME NULL, -- Nueva columna
);

GO

CREATE TABLE Vehiculo (
IdVehiculo INT IDENTITY(1,1) PRIMARY KEY,
TUC VARCHAR(255),
IdUsuario INT REFERENCES Usuario(IdUsuario),
Tipo NVARCHAR(50),
Dominio NVARCHAR(50),
MarcayModelo NVARCHAR(50),
MotorNumero NVARCHAR(50),
ChasisNumero NVARCHAR(50),
AñoFabricacion DATE,
EstadoVehiculo NVARCHAR(50),
LugarDeReparacion NVARCHAR(255),
Observaciones NVARCHAR(MAX),
KmActual NVARCHAR(50),
UltimoService NVARCHAR(50),
Eliminado BIT DEFAULT 0,
FechaRegistro DATETIME DEFAULT GETDATE(), -- Valor por defecto
FechaEliminacion DATETIME NULL, -- Nueva columna
);

GO

CREATE TABLE Reportes (
    IdReporte INT IDENTITY(1,1) PRIMARY KEY,
    TipoRecurso NVARCHAR(50),             -- 'Personal', 'Chaleco', 'Escopeta', etc.
    IdRecurso NVARCHAR(100),              -- Puede ser Legajo, Serie, TUC, etc.
    Accion NVARCHAR(10),                  -- 'Alta' o 'Baja'
    FechaAccion DATETIME DEFAULT GETDATE(),
    IdUsuario INT REFERENCES Usuario(IdUsuario), -- Usuario que ejecutó la acción
    Observaciones NVARCHAR(MAX) NULL
);
