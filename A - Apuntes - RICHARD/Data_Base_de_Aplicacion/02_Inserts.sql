use GestionOfPolicial
--________________________________ INSERTAR ROLES ________________________________
insert into rol(descripcion,esActivo) values
('Administrador',1),
('Empleado',1),
('Supervisor',1)


--________________________________ INSERTAR USUARIOS ________________________________
SELECT * FROM reportes
--clave : 123
insert into Usuario(nombre,correo,telefono,idRol,urlFoto,nombreFoto,clave,esActivo) values
('911','codigo@example.com','909090',1,'','','a665a45920422f9d417e4867efdc4fb8a04a1f3fff1fa07e998e86f7f7a27ae3',1)

INSERT INTO Usuario (nombre, correo, telefono, idRol, urlFoto, nombreFoto, clave, esActivo)
VALUES
('Admin X', 'admin@example.com', '909090', 1, '', '', 'a665a45920422f9d417e4867efdc4fb8a04a1f3fff1fa07e998e86f7f7a27ae3', 1),
('Empleado X', 'empleado@example.com', '808080', 2, '', '', 'a665a45920422f9d417e4867efdc4fb8a04a1f3fff1fa07e998e86f7f7a27ae3', 1),
('Supervisor X', 'supervisor@example.com', '707070', 3, '', '', 'a665a45920422f9d417e4867efdc4fb8a04a1f3fff1fa07e998e86f7f7a27ae3', 1);


--________________________________ RECURSOS DE FIREBASE_STORAGE Y CORREO ________________________________
--(AQUI DEBES INCLUIR TUS PROPIAS CLAVES Y CRENDENCIALES)

insert into Configuracion(recurso,propiedad,valor) values
('FireBase_Storage','email','richard2025@gmail.com'),
('FireBase_Storage','clave','richard2025'),
('FireBase_Storage','ruta','gestionpolicial-daa24.firebasestorage.app'),
('FireBase_Storage','api_key','AIzaSyB63Q2T7EWdt2eI30XrQJtKrssXeEAfHbo'),
('FireBase_Storage','carpeta_usuario','IMAGENES_USUARIO'),
('FireBase_Storage','carpeta_personal','IMAGENES_PERSONAL'),
('FireBase_Storage','carpeta_logo','IMAGENES_LOGO')

select * from Configuracion
insert into Configuracion(recurso,propiedad,valor) values
('Servicio_Correo','correo','gestionpolicial2025@gmail.com'),
('Servicio_Correo','clave','axkezbeyptvhiafo'),
('Servicio_Correo','alias','GestionPolicial.com'),
('Servicio_Correo','host','smtp.gmail.com'),
('Servicio_Correo','puerto','587')


--________________________________ INSERTAR LA DEPENDENCIA POLICIAL _______________________________
select * from Dependencia

INSERT INTO Dependencia (IdDependencia, IdUsuario, UrlLogo, NombreLogo, Nombre, Correo, Direccion, Telefono)
VALUES (1, NULL, '', '', '', '', '', '');

--________________________________ INSERTAR MENUS ________________________________
select * from RolMenu

-- Menú padre
INSERT INTO Menu(descripcion, icono, controlador, paginaAccion, esActivo) VALUES
('DashBoard','fas fa-fw fa-tachometer-alt','DashBoard','Index',1);

INSERT INTO Menu (descripcion, icono, esActivo) VALUES
('Administración', 'fas fa-fw fa-cog', 1),
('Personal', 'fas fa-fw fa-user-shield', 1),
('Chalecos', 'fas fa-fw fa-vest', 1),
('Escopetas', 'fas fa-fw fa-crosshairs', 1),
('Radios', 'fas fa-fw fa-broadcast-tower', 1),
('Vehiculos', 'fas fa-fw fa-car', 1),
('Reportes', 'fas fa-fw fa-chart-area', 1);

-- Hijos - Administración
INSERT INTO Menu(descripcion,idMenuPadre, controlador,paginaAccion,esActivo) VALUES
('Usuarios',2,'Usuario','Index',1),
('Dependencia',2,'Division','Index',1);

-- Hijos - Personal
INSERT INTO Menu(descripcion,idMenuPadre, controlador,paginaAccion,esActivo) VALUES
('Personal',3,'Personal','Personal',1),
('Trasladados',3,'PersonalTrasladado','Index',1);

-- Hijos - Chaleco
INSERT INTO Menu(descripcion,idMenuPadre, controlador,paginaAccion,esActivo) VALUES
('Chalecos',4,'Chaleco','Chaleco',1);

-- Hijos - Escopeta
INSERT INTO Menu(descripcion,idMenuPadre, controlador,paginaAccion,esActivo) VALUES
('Escopetas',5,'Escopeta','Escopeta',1);

-- Hijos - Radio
INSERT INTO Menu(descripcion,idMenuPadre, controlador,paginaAccion,esActivo) VALUES
('Radios',6,'Radio','Radio',1);

-- Hijos - Vehiculo
INSERT INTO Menu(descripcion,idMenuPadre, controlador,paginaAccion,esActivo) VALUES
('Vehiculos',7,'Vehiculo','Vehiculo',1);

-- Hijos - Reportes
INSERT INTO Menu(descripcion,idMenuPadre, controlador,paginaAccion,esActivo) VALUES
('Reportes',8,'Reportes','Reportes',1);

-- Asegurar jerarquía padre-hijo
UPDATE Menu SET idMenuPadre = idMenu WHERE idMenuPadre IS NULL;

--________________________________ INSERTAR ROL MENU ________________________________

-- Limpiar registros previos
DELETE FROM RolMenu WHERE idRol IN (1,2,3);

-- 1. Administrador → acceso total
INSERT INTO RolMenu(idRol, idMenu, esActivo)
SELECT 1, idMenu, 1 FROM Menu;

-- 2. Empleado → todo excepto 'Reportes' y 'Administración'
INSERT INTO RolMenu(idRol, idMenu, esActivo)
SELECT 2, idMenu, 1
FROM Menu
WHERE idMenu NOT IN (
    -- Administración y sus hijos
    SELECT idMenu FROM Menu WHERE descripcion = 'Administración'
    UNION
    SELECT idMenu FROM Menu WHERE idMenuPadre IN (
        SELECT idMenu FROM Menu WHERE descripcion = 'Administración'
    )
    UNION
    -- Reportes y sus hijos
    SELECT idMenu FROM Menu WHERE descripcion = 'Reportes'
    UNION
    SELECT idMenu FROM Menu WHERE idMenuPadre IN (
        SELECT idMenu FROM Menu WHERE descripcion = 'Reportes'
    )
);

-- 3. Supervisor → solo DashBoard y Reportes
INSERT INTO RolMenu(idRol, idMenu, esActivo)
SELECT 3, idMenu, 1 
FROM Menu 
WHERE descripcion IN ('DashBoard', 'Reportes')
   OR idMenuPadre IN (SELECT idMenu FROM Menu WHERE descripcion = 'Reportes');




-- PARA PRUEBAS:
INSERT INTO PersonalPolicial
(
    Legajo, idUsuario, ApellidoYNombre, Grado, Chapa, Sexo, Funcion, Horario, SituacionRevista,
    FechaNacimiento, Telefono, TelefonoEmergencia, DNI, SubsidioSalud, EstudiosCurs, EstadoCivil,
    Especialidad, AltaEnDivision, AltaEnPolicia, DestinoAnterior, Email, Trasladado, Detalles,
    urlImagen, nombreImagen
)
VALUES
('LP-001', 1, 'Martínez Juan Carlos', 'Sargento', 'CH-1023', 'Masculino', 'Custodia VIP', '08:00-16:00', 'Activo',
 '1985-03-15', '1123456789', '1134567890', '30123456', 'OSPEPOL', 'Secundario Completo', 'Casado',
 'Seguridad Personal', '2015-02-10', '2005-04-15', 'Comisaría 1°', 'juan.martinez@policia.gob', 0,
 'Asignado a Casa de Gobierno', NULL, NULL),
('LP-002', 2, 'Gómez María Laura', 'Oficial Principal', 'CH-1078', 'Femenino', 'Investigaciones', '09:00-17:00', 'Activo',
 '1988-07-09', '1145566778', '1177889900', '32122345', 'IOSFA', 'Universitario Completo', 'Soltera',
 'Criminalística', '2017-06-22', '2010-09-01', 'Comisaría 3°', 'maria.gomez@policia.gob', 0,
 'Jefa de división de investigaciones', NULL, NULL),
('LP-003', 2, 'Rodríguez Pablo Andrés', 'Suboficial Mayor', 'CH-1102', 'Masculino', 'Patrullaje', '14:00-22:00', 'Activo',
 '1979-12-01', '1165543210', '1122334455', '27123456', 'OSPEPOL', 'Secundario Completo', 'Casado',
 'Seguridad Urbana', '2010-04-12', '1999-02-10', 'Comisaría 7°', 'pablo.rodriguez@policia.gob', 1,
 'Trasladado a División Motorizada', NULL, NULL),
('LP-004', 1, 'Fernández Lucía', 'Cabo Primero', 'CH-1189', 'Femenino', 'Administración', '07:00-15:00', 'Activo',
 '1992-02-18', '1158765432', '1145671234', '34199876', 'IOSFA', 'Terciario Completo', 'Casada',
 'Gestión Documental', '2020-08-05', '2014-03-11', 'Comisaría 5°', 'lucia.fernandez@policia.gob', 0,
 'Encargada de archivo de personal', NULL, NULL),
('LP-005', 1, 'López Daniel', 'Comisario Inspector', 'CH-1001', 'Masculino', 'Dirección General', '08:00-16:00', 'Activo',
 '1970-06-02', '1199988776', '1143345566', '23123123', 'OSPEPOL', 'Universitario Completo', 'Casado',
 'Gestión Policial', '2000-01-20', '1990-03-10', 'Comisaría Central', 'daniel.lopez@policia.gob', 0,
 'Jefe de Recursos Humanos', NULL, NULL);
GO
INSERT INTO PersonalPolicial
(
    Legajo, idUsuario, ApellidoYNombre, Grado, Chapa, Sexo, Funcion, Horario, SituacionRevista,
    FechaNacimiento, Telefono, TelefonoEmergencia, DNI, SubsidioSalud, EstudiosCurs, EstadoCivil,
    Especialidad, AltaEnDivision, AltaEnPolicia, DestinoAnterior, Email, Trasladado, Detalles,
    urlImagen, nombreImagen
)
VALUES
('LP-006', 2, 'Pérez Natalia Soledad', 'Oficial', 'CH-1205', 'Femenino', 'Atención Ciudadana', '10:00-18:00', 'Activo',
 '1990-05-14', '1133344556', '1199988777', '35222333', 'IOSFA', 'Secundario Completo', 'Casada',
 'Mediación Comunitaria', '2018-09-03', '2012-11-22', 'Comisaría 2°', 'natalia.perez@policia.gob', 0,
 'Encargada de recepción de denuncias', NULL, NULL),
('LP-007', 1, 'Ramírez Hugo Alberto', 'Suboficial Principal', 'CH-1220', 'Masculino', 'Seguridad Bancaria', '06:00-14:00', 'Activo',
 '1982-09-28', '1177723311', '1156654321', '28233445', 'OSPEPOL', 'Secundario Completo', 'Casado',
 'Custodia de Valores', '2012-03-17', '2001-05-05', 'Comisaría 8°', 'hugo.ramirez@policia.gob', 1,
 'Asignado a Banco Nación Sucursal Centro', NULL, NULL),
('LP-008', 1, 'Sosa Mariana Belén', 'Sargento Ayudante', 'CH-1248', 'Femenino', 'Tránsito', '07:00-15:00', 'Activo',
 '1987-11-02', '1188877665', '1122233344', '30111222', 'IOSFA', 'Terciario Completo', 'Soltera',
 'Seguridad Vial', '2016-05-30', '2008-09-14', 'Comisaría 4°', 'mariana.sosa@policia.gob', 0,
 'Supervisora de control vehicular', NULL, NULL),
('LP-009', 2, 'Domínguez Carlos Eduardo', 'Cabo', 'CH-1275', 'Masculino', 'Vigilancia', '22:00-06:00', 'Activo',
 '1995-04-21', '1177788899', '1144456677', '38991234', 'OSPEPOL', 'Secundario Completo', 'Soltero',
 'Seguridad Nocturna', '2021-10-12', '2018-01-05', 'Comisaría 6°', 'carlos.dominguez@policia.gob', 0,
 'Guardia de turno nocturno', NULL, NULL),
('LP-010', 1, 'Ruiz Andrea Paola', 'Comisario', 'CH-1301', 'Femenino', 'Dirección Técnica', '09:00-17:00', 'Activo',
 '1975-03-19', '1167788990', '1133344455', '23111999', 'IOSFA', 'Universitario Completo', 'Casada',
 'Gestión Estratégica', '2004-07-15', '1993-02-10', 'Comisaría Central', 'andrea.ruiz@policia.gob', 0,
 'Subdirectora de Planeamiento Policial', NULL, NULL),
('LP-011', 2, 'Castro Miguel Ángel', 'Sargento Primero', 'CH-1322', 'Masculino', 'Investigaciones', '13:00-21:00', 'Activo',
 '1983-10-05', '1198877665', '1166655544', '29222333', 'OSPEPOL', 'Secundario Completo', 'Casado',
 'Investigación Criminal', '2011-11-09', '2002-03-22', 'Comisaría 9°', 'miguel.castro@policia.gob', 1,
 'Trasladado a Brigada de Homicidios', NULL, NULL),
('LP-012', 1, 'Flores Carla Jimena', 'Cabo Primero', 'CH-1337', 'Femenino', 'Atención Telefónica', '08:00-16:00', 'Activo',
 '1993-02-07', '1146677889', '1188800099', '36999888', 'IOSFA', 'Terciario Incompleto', 'Soltera',
 'Comunicaciones', '2020-02-01', '2016-03-10', 'Comisaría 10°', 'carla.flores@policia.gob', 0,
 'Operadora de mesa de llamadas', NULL, NULL),
('LP-013', 1, 'Silva Ricardo Javier', 'Cabo', 'CH-1349', 'Masculino', 'Patrullaje', '14:00-22:00', 'Activo',
 '1991-08-30', '1177700011', '1155544433', '35995555', 'OSPEPOL', 'Secundario Completo', 'Casado',
 'Seguridad Urbana', '2019-05-22', '2014-04-15', 'Comisaría 11°', 'ricardo.silva@policia.gob', 1,
 'Patrulla Zona Norte', NULL, NULL),
('LP-014', 2, 'Luna Mónica Verónica', 'Oficial Ayudante', 'CH-1365', 'Femenino', 'Asistencia Legal', '09:00-17:00', 'Activo',
 '1989-01-25', '1166600998', '1133388877', '33110022', 'IOSFA', 'Universitario Completo', 'Casada',
 'Derecho Penal', '2018-10-01', '2011-05-17', 'Comisaría Central', 'monica.luna@policia.gob', 0,
 'Asesora legal de la división administrativa', NULL, NULL),
('LP-015', 1, 'Torres Javier Adrián', 'Suboficial Principal', 'CH-1380', 'Masculino', 'Custodia', '06:00-14:00', 'Activo',
 '1980-12-09', '1155567788', '1177788899', '26123344', 'OSPEPOL', 'Secundario Completo', 'Casado',
 'Seguridad Institucional', '2013-03-25', '2000-06-11', 'Comisaría 12°', 'javier.torres@policia.gob', 1,
 'Custodia en edificio gubernamental', NULL, NULL);
GO
INSERT INTO Domicilio (IdUsuario, CalleBarrio, Localidad, ComisariaJuris, IdPersonal)
VALUES
(1, 'Av. Corrientes 1500', 'CABA', 'Comisaría 1A', 1),
(2, 'Calle San Martín 245', 'La Plata', 'Comisaría 3°', 2),
(2, 'Av. Rivadavia 9800', 'Morón', 'Comisaría 7°', 3),
(1, 'B° San José, Calle 12 N°230', 'Lomas de Zamora', 'Comisaría 5°', 4),
(1, 'Calle Belgrano 600', 'San Isidro', 'Comisaría Central', 5);
GO
INSERT INTO Domicilio (IdUsuario, CalleBarrio, Localidad, ComisariaJuris, IdPersonal)
VALUES
(2, 'Calle Mitre 2450', 'Quilmes', 'Comisaría 2°', 6),
(1, 'Av. Eva Perón 3400', 'Avellaneda', 'Comisaría 4°', 7),
(1, 'B° Santa Rita, Pasaje 9 N°122', 'Lanús', 'Comisaría 8°', 8),
(2, 'Calle Tucumán 980', 'San Miguel', 'Comisaría 6°', 9),
(1, 'Av. Libertador 5550', 'Vicente López', 'Comisaría Central', 10),
(2, 'B° Los Pinos, Calle 3 N°456', 'Berazategui', 'Comisaría 5°', 11),
(1, 'Calle Mendoza 750', 'Moreno', 'Comisaría 2°', 12),
(1, 'Calle Sarmiento 1600', 'Merlo', 'Comisaría 3°', 13),
(2, 'Av. San Martín 8800', 'Tigre', 'Comisaría 1°', 14),
(1, 'B° El Progreso, Calle 18 N°89', 'Escobar', 'Comisaría 2°', 15);
GO
INSERT INTO Arma (IdUsuario, NumeroSerie, Marca, IdPersonal)
VALUES
(1, 'AR-10023', 'Bersa Thunder 9mm', 1),
(2, 'AR-20078', 'Glock 17', 2),
(2, 'AR-31102', 'Taurus PT92', 3),
(1, 'AR-41189', 'Bersa .380', 4),
(1, 'AR-50001', 'Glock 19', 5);
GO
INSERT INTO Arma (IdUsuario, NumeroSerie, Marca, IdPersonal)
VALUES
(2, 'AR-60045', 'Bersa Thunder Pro', 6),
(1, 'AR-71009', 'Glock 26', 7),
(1, 'AR-81234', 'Taurus G2C', 8),
(2, 'AR-92356', 'Browning Hi-Power', 9),
(1, 'AR-10345', 'Beretta 92FS', 10),
(2, 'AR-11278', 'Smith & Wesson M&P9', 11),
(1, 'AR-12389', 'Sig Sauer P226', 12),
(1, 'AR-13456', 'Bersa TPR9', 13),
(2, 'AR-14567', 'CZ 75 SP-01', 14),
(1, 'AR-15678', 'Glock 43X', 15);
GO
INSERT INTO Chaleco (SerieChaleco, IdUsuario, MarcaYmodelo, Talle, AnoFabricacion, AnoVencimiento, EstadoChaleco, Observaciones, IdPersonal)
VALUES 
('CHL001', 1, 'MORON DOLPHIN BEYON DOLPHIN', 'M', '2020-01-15', '2025-01-15', 'Nuevo', 'Chaleco disponible', NULL),
('CHL002', 2, 'MORON DOLPHIN BEYON DOLPHIN', 'L', '2019-06-10', '2024-06-10', 'Bueno', 'Chaleco disponible', NULL),
('CHL003', 2, 'MORON DOLPHIN BEYON DOLPHIN', 'S', '2021-03-20', '2026-03-20', 'Regular', 'Chaleco disponible', NULL),
('CHL004', 1, 'MORON DOLPHIN BEYON DOLPHIN', 'M', '2018-11-05', '2023-11-05', 'Usado', 'Chaleco disponible', NULL),
('CHL005', 1, 'MORON DOLPHIN BEYON DOLPHIN', 'XXL', '2022-07-12', '2027-07-12', 'Nuevo', 'Chaleco disponible', NULL);
GO
INSERT INTO Chaleco (SerieChaleco, IdUsuario, MarcaYmodelo, Talle, AnoFabricacion, AnoVencimiento, EstadoChaleco, Observaciones, IdPersonal)
VALUES 
('CHL006', 1, 'MORON DOLPHIN BEYON DOLPHIN', 'L', '2020-09-01', '2025-09-01', 'Nuevo', 'Chaleco disponible', NULL),
('CHL007', 2, 'MORON DOLPHIN BEYON DOLPHIN', 'M', '2019-04-22', '2024-04-22', 'Bueno', 'Chaleco disponible', NULL),
('CHL008', 2, 'MORON DOLPHIN BEYON DOLPHIN', 'XL', '2021-02-10', '2026-02-10', 'Regular', 'Chaleco disponible', NULL),
('CHL009', 1, 'MORON DOLPHIN BEYON DOLPHIN', 'S', '2018-08-15', '2023-08-15', 'Usado', 'Chaleco disponible', NULL),
('CHL010', 2, 'MORON DOLPHIN BEYON DOLPHIN', 'M', '2022-11-25', '2027-11-25', 'Nuevo', 'Chaleco disponible', NULL),
('CHL011', 1, 'MORON DOLPHIN BEYON DOLPHIN', 'L', '2019-12-30', '2024-12-30', 'Bueno', 'Chaleco disponible', NULL),
('CHL012', 2, 'MORON DOLPHIN BEYON DOLPHIN', 'S', '2021-05-05', '2026-05-05', 'Regular', 'Chaleco disponible', NULL),
('CHL013', 2, 'MORON DOLPHIN BEYON DOLPHIN', 'XL', '2018-10-18', '2023-10-18', 'Usado', 'Chaleco disponible', NULL),
('CHL014', 1, 'MORON DOLPHIN BEYON DOLPHIN', 'XXL', '2020-03-11', '2025-03-11', 'Bueno', 'Chaleco disponible', NULL),
('CHL015', 2, 'MORON DOLPHIN BEYON DOLPHIN', 'M', '2022-01-09', '2027-01-09', 'Nuevo', 'Chaleco disponible', NULL);


SELECT * FROM PersonalPolicial;

DELETE FROM PersonalPolicial;
DBCC CHECKIDENT ('PersonalPolicial', RESEED, 0);

DELETE FROM Chaleco;
DBCC CHECKIDENT ('Chaleco', RESEED, 0);