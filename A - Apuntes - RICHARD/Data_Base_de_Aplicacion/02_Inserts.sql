use GestionOfPolicial
--________________________________ INSERTAR ROLES ________________________________
insert into rol(descripcion,esActivo) values
('Administrador',1),
('Empleado',1),
('Supervisor',1)


--________________________________ INSERTAR USUARIOS ________________________________
SELECT * FROM PersonalPolicial
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




-- PARA PRUEBAS
INSERT INTO PersonalPolicial
(
    Legajo, idUsuario, ApellidoYNombre, Grado, Chapa, Sexo, Funcion, Horario, SituacionRevista,
    FechaNacimiento, Telefono, TelefonoEmergencia, DNI, SubsidioSalud, EstudiosCurs, EstadoCivil,
    Especialidad, AltaEnDivision, AltaEnPolicia, DestinoAnterior, Email, Trasladado, Detalles,
    urlImagen, nombreImagen, FechaEliminacion, FechaRegistro
)
VALUES
('LP-001', 1, 'Martínez Juan Carlos', 'Sargento', 'CH-1023', 'Masculino', 'Custodia VIP', '08:00-16:00', 'Activo',
 '1985-03-15', '1123456789', '1134567890', '30123456', 'OSPEPOL', 'Secundario Completo', 'Casado',
 'Seguridad Personal', '2015-02-10', '2005-04-15', 'Comisaría 1°', 'juan.martinez@policia.gob', 0,
 'Asignado a Casa de Gobierno', NULL, NULL,
 DATEADD(DAY, ABS(CHECKSUM(NEWID())) % 365, '2025-01-01'),
 DATEADD(DAY, ABS(CHECKSUM(NEWID())) % 365, '2025-01-01')
),
('LP-002', 2, 'Gómez María Laura', 'Oficial Principal', 'CH-1078', 'Femenino', 'Investigaciones', '09:00-17:00', 'Activo',
 '1988-07-09', '1145566778', '1177889900', '32122345', 'IOSFA', 'Universitario Completo', 'Soltera',
 'Criminalística', '2017-06-22', '2010-09-01', 'Comisaría 3°', 'maria.gomez@policia.gob', 0,
 'Jefa de división de investigaciones', NULL, NULL,
 DATEADD(DAY, ABS(CHECKSUM(NEWID())) % 365, '2025-01-01'),
 DATEADD(DAY, ABS(CHECKSUM(NEWID())) % 365, '2025-01-01')
),
('LP-003', 2, 'Rodríguez Pablo Andrés', 'Suboficial Mayor', 'CH-1102', 'Masculino', 'Patrullaje', '14:00-22:00', 'Activo',
 '1979-12-01', '1165543210', '1122334455', '27123456', 'OSPEPOL', 'Secundario Completo', 'Casado',
 'Seguridad Urbana', '2010-04-12', '1999-02-10', 'Comisaría 7°', 'pablo.rodriguez@policia.gob', 1,
 'Trasladado a División Motorizada', NULL, NULL,
 DATEADD(DAY, ABS(CHECKSUM(NEWID())) % 365, '2025-01-01'),
 DATEADD(DAY, ABS(CHECKSUM(NEWID())) % 365, '2025-01-01')
),
('LP-004', 1, 'Fernández Lucía', 'Cabo Primero', 'CH-1189', 'Femenino', 'Administración', '07:00-15:00', 'Activo',
 '1992-02-18', '1158765432', '1145671234', '34199876', 'IOSFA', 'Terciario Completo', 'Casada',
 'Gestión Documental', '2020-08-05', '2014-03-11', 'Comisaría 5°', 'lucia.fernandez@policia.gob', 0,
 'Encargada de archivo de personal', NULL, NULL,
 DATEADD(DAY, ABS(CHECKSUM(NEWID())) % 365, '2025-01-01'),
 DATEADD(DAY, ABS(CHECKSUM(NEWID())) % 365, '2025-01-01')
),
('LP-005', 1, 'López Daniel', 'Comisario Inspector', 'CH-1001', 'Masculino', 'Dirección General', '08:00-16:00', 'Activo',
 '1970-06-02', '1199988776', '1143345566', '23123123', 'OSPEPOL', 'Universitario Completo', 'Casado',
 'Gestión Policial', '2000-01-20', '1990-03-10', 'Comisaría Central', 'daniel.lopez@policia.gob', 0,
 'Jefe de Recursos Humanos', NULL, NULL,
 DATEADD(DAY, ABS(CHECKSUM(NEWID())) % 365, '2025-01-01'),
 DATEADD(DAY, ABS(CHECKSUM(NEWID())) % 365, '2025-01-01')
);
GO
INSERT INTO PersonalPolicial
(
    Legajo, idUsuario, ApellidoYNombre, Grado, Chapa, Sexo, Funcion, Horario, SituacionRevista,
    FechaNacimiento, Telefono, TelefonoEmergencia, DNI, SubsidioSalud, EstudiosCurs, EstadoCivil,
    Especialidad, AltaEnDivision, AltaEnPolicia, DestinoAnterior, Email, Trasladado, Detalles,
    urlImagen, nombreImagen, FechaEliminacion, FechaRegistro
)
VALUES
('LP-006', 2, 'Pérez Natalia Soledad', 'Oficial', 'CH-1205', 'Femenino', 'Atención Ciudadana', '10:00-18:00', 'Activo',
 '1990-05-14', '1133344556', '1199988777', '35222333', 'IOSFA', 'Secundario Completo', 'Casada',
 'Mediación Comunitaria', '2018-09-03', '2012-11-22', 'Comisaría 2°', 'natalia.perez@policia.gob', 0,
 'Encargada de recepción de denuncias', NULL, NULL,
 DATEADD(DAY, ABS(CHECKSUM(NEWID())) % 365, '2025-01-01'),
 DATEADD(DAY, ABS(CHECKSUM(NEWID())) % 365, '2025-01-01')
),
('LP-007', 1, 'Ramírez Hugo Alberto', 'Suboficial Principal', 'CH-1220', 'Masculino', 'Seguridad Bancaria', '06:00-14:00', 'Activo',
 '1982-09-28', '1177723311', '1156654321', '28233445', 'OSPEPOL', 'Secundario Completo', 'Casado',
 'Custodia de Valores', '2012-03-17', '2001-05-05', 'Comisaría 8°', 'hugo.ramirez@policia.gob', 1,
 'Asignado a Banco Nación Sucursal Centro', NULL, NULL,
 DATEADD(DAY, ABS(CHECKSUM(NEWID())) % 365, '2025-01-01'),
 DATEADD(DAY, ABS(CHECKSUM(NEWID())) % 365, '2025-01-01')
),
('LP-008', 1, 'Sosa Mariana Belén', 'Sargento Ayudante', 'CH-1248', 'Femenino', 'Tránsito', '07:00-15:00', 'Activo',
 '1987-11-02', '1188877665', '1122233344', '30111222', 'IOSFA', 'Terciario Completo', 'Soltera',
 'Seguridad Vial', '2016-05-30', '2008-09-14', 'Comisaría 4°', 'mariana.sosa@policia.gob', 0,
 'Supervisora de control vehicular', NULL, NULL,
 DATEADD(DAY, ABS(CHECKSUM(NEWID())) % 365, '2025-01-01'),
 DATEADD(DAY, ABS(CHECKSUM(NEWID())) % 365, '2025-01-01')
),
('LP-009', 2, 'Domínguez Carlos Eduardo', 'Cabo', 'CH-1275', 'Masculino', 'Vigilancia', '22:00-06:00', 'Activo',
 '1995-04-21', '1177788899', '1144456677', '38991234', 'OSPEPOL', 'Secundario Completo', 'Soltero',
 'Seguridad Nocturna', '2021-10-12', '2018-01-05', 'Comisaría 6°', 'carlos.dominguez@policia.gob', 0,
 'Guardia de turno nocturno', NULL, NULL,
 DATEADD(DAY, ABS(CHECKSUM(NEWID())) % 365, '2025-01-01'),
 DATEADD(DAY, ABS(CHECKSUM(NEWID())) % 365, '2025-01-01')
),
('LP-010', 1, 'Ruiz Andrea Paola', 'Comisario', 'CH-1301', 'Femenino', 'Dirección Técnica', '09:00-17:00', 'Activo',
 '1975-03-19', '1167788990', '1133344455', '23111999', 'IOSFA', 'Universitario Completo', 'Casada',
 'Gestión Estratégica', '2004-07-15', '1993-02-10', 'Comisaría Central', 'andrea.ruiz@policia.gob', 0,
 'Subdirectora de Planeamiento Policial', NULL, NULL,
 DATEADD(DAY, ABS(CHECKSUM(NEWID())) % 365, '2025-01-01'),
 DATEADD(DAY, ABS(CHECKSUM(NEWID())) % 365, '2025-01-01')
),
('LP-011', 2, 'Castro Miguel Ángel', 'Sargento Primero', 'CH-1322', 'Masculino', 'Investigaciones', '13:00-21:00', 'Activo',
 '1983-10-05', '1198877665', '1166655544', '29222333', 'OSPEPOL', 'Secundario Completo', 'Casado',
 'Investigación Criminal', '2011-11-09', '2002-03-22', 'Comisaría 9°', 'miguel.castro@policia.gob', 1,
 'Trasladado a Brigada de Homicidios', NULL, NULL,
 DATEADD(DAY, ABS(CHECKSUM(NEWID())) % 365, '2025-01-01'),
 DATEADD(DAY, ABS(CHECKSUM(NEWID())) % 365, '2025-01-01')
),
('LP-012', 1, 'Flores Carla Jimena', 'Cabo Primero', 'CH-1337', 'Femenino', 'Atención Telefónica', '08:00-16:00', 'Activo',
 '1993-02-07', '1146677889', '1188800099', '36999888', 'IOSFA', 'Terciario Incompleto', 'Soltera',
 'Comunicaciones', '2020-02-01', '2016-03-10', 'Comisaría 10°', 'carla.flores@policia.gob', 0,
 'Operadora de mesa de llamadas', NULL, NULL,
 DATEADD(DAY, ABS(CHECKSUM(NEWID())) % 365, '2025-01-01'),
 DATEADD(DAY, ABS(CHECKSUM(NEWID())) % 365, '2025-01-01')
),
('LP-013', 1, 'Silva Ricardo Javier', 'Cabo', 'CH-1349', 'Masculino', 'Patrullaje', '14:00-22:00', 'Activo',
 '1991-08-30', '1177700011', '1155544433', '35995555', 'OSPEPOL', 'Secundario Completo', 'Casado',
 'Seguridad Urbana', '2019-05-22', '2014-04-15', 'Comisaría 11°', 'ricardo.silva@policia.gob', 1,
 'Patrulla Zona Norte', NULL, NULL,
 DATEADD(DAY, ABS(CHECKSUM(NEWID())) % 365, '2025-01-01'),
 DATEADD(DAY, ABS(CHECKSUM(NEWID())) % 365, '2025-01-01')
),
('LP-014', 2, 'Luna Mónica Verónica', 'Oficial Ayudante', 'CH-1365', 'Femenino', 'Asistencia Legal', '09:00-17:00', 'Activo',
 '1989-01-25', '1166600998', '1133388877', '33110022', 'IOSFA', 'Universitario Completo', 'Casada',
 'Derecho Penal', '2018-10-01', '2011-05-17', 'Comisaría Central', 'monica.luna@policia.gob', 0,
 'Asesora legal de la división administrativa', NULL, NULL,
 DATEADD(DAY, ABS(CHECKSUM(NEWID())) % 365, '2025-01-01'),
 DATEADD(DAY, ABS(CHECKSUM(NEWID())) % 365, '2025-01-01')
),
('LP-015', 1, 'Torres Javier Adrián', 'Suboficial Principal', 'CH-1380', 'Masculino', 'Custodia', '06:00-14:00', 'Activo',
 '1980-12-09', '1155567788', '1177788899', '26123344', 'OSPEPOL', 'Secundario Completo', 'Casado',
 'Seguridad Institucional', '2013-03-25', '2000-06-11', 'Comisaría 12°', 'javier.torres@policia.gob', 1,
 'Custodia en edificio gubernamental', NULL, NULL,
 DATEADD(DAY, ABS(CHECKSUM(NEWID())) % 365, '2025-01-01'),
 DATEADD(DAY, ABS(CHECKSUM(NEWID())) % 365, '2025-01-01')
);
GO
INSERT INTO PersonalPolicial
(
    Legajo, idUsuario, ApellidoYNombre, Grado, Chapa, Sexo, Funcion, Horario, SituacionRevista,
    FechaNacimiento, Telefono, TelefonoEmergencia, DNI, SubsidioSalud, EstudiosCurs, EstadoCivil,
    Especialidad, AltaEnDivision, AltaEnPolicia, DestinoAnterior, Email, Trasladado, Detalles,
    urlImagen, nombreImagen, FechaEliminacion, FechaRegistro
)
VALUES
('LP-016', 2, 'Acosta Guillermo Hernán', 'Sargento', 'CH-1402', 'Masculino', 'Patrullaje', '06:00-14:00', 'Activo',
 '1984-04-11', '1132214567', '1156677788', '28765432', 'OSPEPOL', 'Secundario Completo', 'Casado',
 'Seguridad Urbana', '2014-07-08', '2003-09-15', 'Comisaría 2°', 'guillermo.acosta@policia.gob', 0,
 'Patrulla de zona céntrica', NULL, NULL,
 DATEADD(DAY, ABS(CHECKSUM(NEWID())) % 365, '2025-01-01'),
 DATEADD(DAY, ABS(CHECKSUM(NEWID())) % 365, '2025-01-01')
),
('LP-017', 1, 'Benítez Laura Eugenia', 'Oficial Auxiliar', 'CH-1415', 'Femenino', 'Atención al público', '08:00-16:00', 'Activo',
 '1994-11-20', '1198876543', '1145567890', '38999888', 'IOSFA', 'Terciario Completo', 'Soltera',
 'Relaciones con la Comunidad', '2021-04-18', '2017-02-10', 'Comisaría 4°', 'laura.benitez@policia.gob', 0,
 'Responsable de coordinación vecinal', NULL, NULL,
 DATEADD(DAY, ABS(CHECKSUM(NEWID())) % 365, '2025-01-01'),
 DATEADD(DAY, ABS(CHECKSUM(NEWID())) % 365, '2025-01-01')
),
('LP-018', 2, 'Cabrera Diego Alejandro', 'Suboficial Mayor', 'CH-1428', 'Masculino', 'Investigaciones', '14:00-22:00', 'Activo',
 '1976-01-09', '1176654433', '1133347755', '25888444', 'OSPEPOL', 'Secundario Completo', 'Casado',
 'Investigación Criminal', '2008-09-25', '1995-08-10', 'Comisaría 9°', 'diego.cabrera@policia.gob', 1,
 'Trasladado a División Narcóticos', NULL, NULL,
 DATEADD(DAY, ABS(CHECKSUM(NEWID())) % 365, '2025-01-01'),
 DATEADD(DAY, ABS(CHECKSUM(NEWID())) % 365, '2025-01-01')
),
('LP-019', 1, 'Chávez Mariana Lorena', 'Sargento Ayudante', 'CH-1440', 'Femenino', 'Transito', '07:00-15:00', 'Activo',
 '1988-03-30', '1165532211', '1188811777', '31888444', 'IOSFA', 'Secundario Completo', 'Soltera',
 'Seguridad Vial', '2015-11-12', '2010-03-05', 'Comisaría 1°', 'mariana.chavez@policia.gob', 0,
 'Supervisora de control de tránsito', NULL, NULL,
 DATEADD(DAY, ABS(CHECKSUM(NEWID())) % 365, '2025-01-01'),
 DATEADD(DAY, ABS(CHECKSUM(NEWID())) % 365, '2025-01-01')
),
('LP-020', 1, 'Díaz Federico Manuel', 'Oficial Principal', 'CH-1452', 'Masculino', 'Custodia', '22:00-06:00', 'Activo',
 '1985-12-05', '1154421100', '1177711223', '29111222', 'OSPEPOL', 'Universitario Incompleto', 'Casado',
 'Seguridad Institucional', '2013-01-20', '2004-06-15', 'Comisaría 3°', 'federico.diaz@policia.gob', 1,
 'Custodia en sede ministerial', NULL, NULL,
 DATEADD(DAY, ABS(CHECKSUM(NEWID())) % 365, '2025-01-01'),
 DATEADD(DAY, ABS(CHECKSUM(NEWID())) % 365, '2025-01-01')
),
('LP-021', 2, 'Escobar Andrea Micaela', 'Cabo Primero', 'CH-1467', 'Femenino', 'Mesa de entrada', '09:00-17:00', 'Activo',
 '1992-05-17', '1188822211', '1133367788', '30997755', 'IOSFA', 'Terciario Completo', 'Soltera',
 'Administración Policial', '2019-06-21', '2015-04-10', 'Comisaría 5°', 'andrea.escobar@policia.gob', 0,
 'Responsable de gestión documental', NULL, NULL,
 DATEADD(DAY, ABS(CHECKSUM(NEWID())) % 365, '2025-01-01'),
 DATEADD(DAY, ABS(CHECKSUM(NEWID())) % 365, '2025-01-01')
),
('LP-022', 1, 'Fernández Tomás Eduardo', 'Cabo', 'CH-1479', 'Masculino', 'Vigilancia', '18:00-02:00', 'Activo',
 '1996-09-14', '1199981122', '1155549900', '40222111', 'OSPEPOL', 'Secundario Completo', 'Soltero',
 'Seguridad Nocturna', '2022-02-03', '2019-05-12', 'Comisaría 6°', 'tomas.fernandez@policia.gob', 0,
 'Ronda nocturna sector oeste', NULL, NULL,
 DATEADD(DAY, ABS(CHECKSUM(NEWID())) % 365, '2025-01-01'),
 DATEADD(DAY, ABS(CHECKSUM(NEWID())) % 365, '2025-01-01')
),
('LP-023', 2, 'García Paula Noemí', 'Oficial Ayudante', 'CH-1490', 'Femenino', 'Atención Telefónica', '07:00-15:00', 'Activo',
 '1991-07-23', '1187716612', '1177700332', '33114455', 'IOSFA', 'Terciario Completo', 'Soltera',
 'Comunicaciones', '2020-11-30', '2014-03-21', 'Comisaría 10°', 'paula.garcia@policia.gob', 0,
 'Operadora de emergencias', NULL, NULL,
 DATEADD(DAY, ABS(CHECKSUM(NEWID())) % 365, '2025-01-01'),
 DATEADD(DAY, ABS(CHECKSUM(NEWID())) % 365, '2025-01-01')
),
('LP-024', 1, 'Herrera Nicolás Sebastián', 'Sargento Primero', 'CH-1501', 'Masculino', 'Investigaciones', '14:00-22:00', 'Activo',
 '1986-08-01', '1176654400', '1198833001', '27775555', 'OSPEPOL', 'Secundario Completo', 'Casado',
 'Investigación Criminal', '2010-03-14', '2001-11-10', 'Comisaría 7°', 'nicolas.herrera@policia.gob', 1,
 'Trasladado a unidad antirrobos', NULL, NULL,
 DATEADD(DAY, ABS(CHECKSUM(NEWID())) % 365, '2025-01-01'),
 DATEADD(DAY, ABS(CHECKSUM(NEWID())) % 365, '2025-01-01')
),
('LP-025', 2, 'Ibarra Sofía Milagros', 'Cabo Ayudante', 'CH-1513', 'Femenino', 'Transito', '06:00-14:00', 'Activo',
 '1997-01-12', '1144412233', '1188855667', '41122334', 'IOSFA', 'Secundario Completo', 'Soltera',
 'Seguridad Vial', '2023-07-10', '2020-09-15', 'Comisaría 1°', 'sofia.ibarra@policia.gob', 0,
 'Control vehicular en accesos principales', NULL, NULL,
 DATEADD(DAY, ABS(CHECKSUM(NEWID())) % 365, '2025-01-01'),
 DATEADD(DAY, ABS(CHECKSUM(NEWID())) % 365, '2025-01-01')
);
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
INSERT INTO Domicilio (IdUsuario, CalleBarrio, Localidad, ComisariaJuris, IdPersonal)
VALUES
(1, 'Calle Belgrano 1320', 'Lomas de Zamora', 'Comisaría 7°', 16),
(2, 'B° San José, Calle 14 N°233', 'Florencio Varela', 'Comisaría 3°', 17),
(1, 'Av. Rivadavia 4500', 'Morón', 'Comisaría 1°', 18),
(2, 'Calle Laprida 560', 'Hurlingham', 'Comisaría 2°', 19),
(1, 'B° Las Flores, Manzana 5 Casa 12', 'Claypole', 'Comisaría 5°', 20),
(2, 'Av. Calchaquí 7200', 'Quilmes Oeste', 'Comisaría 9°', 21),
(1, 'Calle French 1880', 'San Isidro', 'Comisaría 2°', 22),
(1, 'B° Primavera, Calle 7 N°345', 'José C. Paz', 'Comisaría 4°', 23),
(2, 'Calle Italia 2301', 'Pilar', 'Comisaría 1°', 24),
(1, 'Av. Senzabello 4100', 'La Matanza', 'Comisaría 3°', 25);
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
INSERT INTO Arma (IdUsuario, NumeroSerie, Marca, IdPersonal)
VALUES
(1, 'AR-16789', 'Bersa Thunder 9', 16),
(2, 'AR-17890', 'Glock 19 Gen5', 17),
(1, 'AR-18901', 'Smith & Wesson SD9 VE', 18),
(2, 'AR-19012', 'CZ P-10 C', 19),
(1, 'AR-20123', 'Sig Sauer P320', 20),
(2, 'AR-21234', 'Taurus PT92', 21),
(1, 'AR-22345', 'Browning Hi-Power MKIII', 22),
(1, 'AR-23456', 'Beretta APX', 23),
(2, 'AR-24567', 'Glock 17 Gen4', 24),
(1, 'AR-25678', 'Bersa TPR9C', 25);
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
GO
INSERT INTO Chaleco (SerieChaleco, IdUsuario, MarcaYmodelo, Talle, AnoFabricacion, AnoVencimiento, EstadoChaleco, Observaciones, IdPersonal)
VALUES
('CHL016', 1, 'MORON DOLPHIN BEYON DOLPHIN', 'L', '2020-07-14', '2025-07-14', 'Nuevo', 'Chaleco disponible', NULL),
('CHL017', 2, 'MORON DOLPHIN BEYON DOLPHIN', 'M', '2019-03-20', '2024-03-20', 'Bueno', 'Chaleco disponible', NULL),
('CHL018', 1, 'MORON DOLPHIN BEYON DOLPHIN', 'XL', '2021-09-08', '2026-09-08', 'Regular', 'Chaleco disponible', NULL),
('CHL019', 2, 'MORON DOLPHIN BEYON DOLPHIN', 'S', '2018-06-11', '2023-06-11', 'Usado', 'Chaleco disponible', NULL),
('CHL020', 1, 'MORON DOLPHIN BEYON DOLPHIN', 'M', '2022-10-30', '2027-10-30', 'Nuevo', 'Chaleco disponible', NULL),
('CHL021', 2, 'MORON DOLPHIN BEYON DOLPHIN', 'L', '2019-11-19', '2024-11-19', 'Bueno', 'Chaleco disponible', NULL),
('CHL022', 1, 'MORON DOLPHIN BEYON DOLPHIN', 'S', '2021-01-22', '2026-01-22', 'Regular', 'Chaleco disponible', NULL),
('CHL023', 2, 'MORON DOLPHIN BEYON DOLPHIN', 'XL', '2018-12-03', '2023-12-03', 'Usado', 'Chaleco disponible', NULL),
('CHL024', 1, 'MORON DOLPHIN BEYON DOLPHIN', 'XXL', '2020-04-27', '2025-04-27', 'Bueno', 'Chaleco disponible', NULL),
('CHL025', 2, 'MORON DOLPHIN BEYON DOLPHIN', 'M', '2022-02-16', '2027-02-16', 'Nuevo', 'Chaleco disponible', NULL);
GO
INSERT INTO Chaleco (SerieChaleco, IdUsuario, MarcaYmodelo, Talle, AnoFabricacion, AnoVencimiento, EstadoChaleco, Observaciones, IdPersonal)
VALUES
('CHL026', 1, 'MORON DOLPHIN BEYON DOLPHIN', 'L',  '2020-08-19', '2025-08-19', 'Nuevo',   'Chaleco disponible', NULL),
('CHL027', 2, 'MORON DOLPHIN BEYON DOLPHIN', 'M',  '2019-07-04', '2024-07-04', 'Bueno',   'Chaleco disponible', NULL),
('CHL028', 1, 'MORON DOLPHIN BEYON DOLPHIN', 'S',  '2021-03-18', '2026-03-18', 'Regular', 'Chaleco disponible', NULL),
('CHL029', 2, 'MORON DOLPHIN BEYON DOLPHIN', 'XL', '2018-05-26', '2023-05-26', 'Usado',   'Chaleco disponible', NULL),
('CHL030', 1, 'MORON DOLPHIN BEYON DOLPHIN', 'XXL','2022-09-09', '2027-09-09', 'Nuevo',   'Chaleco disponible', NULL),
('CHL031', 2, 'MORON DOLPHIN BEYON DOLPHIN', 'L',  '2020-11-22', '2025-11-22', 'Bueno',   'Chaleco disponible', NULL),
('CHL032', 1, 'MORON DOLPHIN BEYON DOLPHIN', 'M',  '2021-06-30', '2026-06-30', 'Regular', 'Chaleco disponible', NULL),
('CHL033', 2, 'MORON DOLPHIN BEYON DOLPHIN', 'S',  '2018-09-13', '2023-09-13', 'Usado',   'Chaleco disponible', NULL),
('CHL034', 1, 'MORON DOLPHIN BEYON DOLPHIN', 'XL', '2020-02-17', '2025-02-17', 'Bueno',   'Chaleco disponible', NULL),
('CHL035', 2, 'MORON DOLPHIN BEYON DOLPHIN', 'L',  '2022-12-01', '2027-12-01', 'Nuevo',   'Chaleco disponible', NULL),
('CHL036', 1, 'MORON DOLPHIN BEYON DOLPHIN', 'M',  '2019-10-05', '2024-10-05', 'Bueno',   'Chaleco disponible', NULL),
('CHL037', 2, 'MORON DOLPHIN BEYON DOLPHIN', 'XXL','2021-04-11', '2026-04-11', 'Regular', 'Chaleco disponible', NULL),
('CHL038', 1, 'MORON DOLPHIN BEYON DOLPHIN', 'S',  '2018-01-29', '2023-01-29', 'Usado',   'Chaleco disponible', NULL),
('CHL039', 2, 'MORON DOLPHIN BEYON DOLPHIN', 'XL', '2020-07-07', '2025-07-07', 'Bueno',   'Chaleco disponible', NULL),
('CHL040', 1, 'MORON DOLPHIN BEYON DOLPHIN', 'L',  '2022-03-14', '2027-03-14', 'Nuevo',   'Chaleco disponible', NULL);
GO
INSERT INTO Escopeta (SerieEscopeta, IdUsuario, MarcayModelo, EstadoEscopeta, Observaciones, Eliminado, FechaRegistro, FechaEliminacion)
VALUES
('ESC-1001', 1, 'Mossberg 500', 'Operativa', '', 0, '2021-03-15', NULL),
('ESC-1002', 2, 'Remington 870', 'Operativa', '', 1, '2019-07-22', '2023-02-10'),
('ESC-1003', 1, 'Benelli M3', 'En Reparación', '', 1, '2020-11-05', '2022-08-19'),
('ESC-1004', 2, 'Ithaca 37', 'Operativa', '', 0, '2023-01-09', NULL),
('ESC-1005', 1, 'Mossberg Maverick 88', 'Fuera de Servicio', '', 1, '2018-05-30', '2021-12-04'),
('ESC-1006', 2, 'Stoeger P3000', 'Operativa', '', 0, '2024-06-12', NULL),
('ESC-1007', 1, 'Benelli SuperNova', 'En Reparación', '', 1, '2022-08-21', '2024-01-28'),
('ESC-1008', 2, 'Remington 870 Tactical', 'Operativa', '', 0, '2020-09-14', NULL),
('ESC-1009', 1, 'Winchester SXP Defender', 'Operativa', '', 1, '2021-04-19', '2023-11-06'),
('ESC-1010', 2, 'Benelli M4', 'Operativa', '', 0, '2019-12-25', NULL),
('ESC-1011', 1, 'Mossberg 590A1', 'En Reparación', '', 1, '2018-10-03', '2022-05-17'),
('ESC-1012', 2, 'Stoeger Double Defense', 'Operativa', '', 0, '2023-09-08', NULL),
('ESC-1013', 1, 'Winchester Model 1300', 'Fuera de Servicio', '', 1, '2020-02-11', '2021-10-29'),
('ESC-1014', 2, 'Benelli Nova', 'Operativa', '', 0, '2025-01-20', NULL),
('ESC-1015', 1, 'Mossberg 535', 'En Reparación', '', 1, '2019-03-27', '2024-04-12');
GO
INSERT INTO Radio (SerieRadio, IdUsuario, MarcayModelo, EstadoRadio, Tipo, Observaciones, Eliminado, FechaRegistro, FechaEliminacion)
VALUES
('RAD-2001', 1, 'Motorola XiR P3688', 'Operativa', 'Portátil', '', 0, '2022-04-10', NULL),
('RAD-2002', 2, 'Hytera PD506', 'Operativa', 'Portátil', '', 1, '2020-11-22', '2023-03-15'),
('RAD-2003', 1, 'Motorola GP340', 'En Reparación', 'Portátil', '', 1, '2019-08-17', '2021-09-30'),
('RAD-2004', 2, 'Kenwood TK-3000', 'Operativa', 'Base', '', 0, '2023-07-05', NULL),
('RAD-2005', 1, 'Icom IC-F3003', 'Fuera de Servicio', 'Portátil', '', 1, '2018-02-14', '2020-12-01'),
('RAD-2006', 2, 'Motorola XiR P6600i', 'Operativa', 'Portátil', '', 0, '2024-01-19', NULL),
('RAD-2007', 1, 'Hytera PD786', 'En Reparación', 'Portátil', '', 1, '2021-05-23', '2024-02-11'),
('RAD-2008', 2, 'Motorola GP380', 'Operativa', 'Móvil', '', 0, '2020-09-09', NULL),
('RAD-2009', 1, 'Kenwood NX-1200', 'Operativa', 'Portátil', '', 1, '2021-03-28', '2023-10-18'),
('RAD-2010', 2, 'Hytera TM-610', 'Operativa', 'Base', '', 0, '2019-12-20', NULL),
('RAD-2011', 1, 'Motorola XiR P8668i', 'En Reparación', 'Portátil', '', 1, '2018-07-03', '2022-06-14'),
('RAD-2012', 2, 'Icom IC-F5023', 'Operativa', 'Móvil', '', 0, '2023-10-08', NULL),
('RAD-2013', 1, 'Kenwood TK-2360', 'Fuera de Servicio', 'Portátil', '', 1, '2020-01-11', '2021-11-25'),
('RAD-2014', 2, 'Motorola XiR E8600', 'Operativa', 'Portátil', '', 0, '2025-02-02', NULL),
('RAD-2015', 1, 'Hytera PD416', 'En Reparación', 'Portátil', '', 1, '2019-05-29', '2024-05-02');
GO
INSERT INTO Vehiculo (TUC, IdUsuario, Tipo, Dominio, MarcayModelo, MotorNumero, ChasisNumero, AñoFabricacion, EstadoVehiculo, LugarDeReparacion, Observaciones, KmActual, UltimoService, Eliminado, FechaRegistro, FechaEliminacion)
VALUES
('TUC-3001', 1, 'Camioneta', 'AB123CD', 'Toyota Hilux 4x4', '1KD-FTV3001', 'JT121UZ3001001', '2019-01-01', 'Operativo', '', '', '120000', '2024-03-10', 0, '2020-05-15', NULL),
('TUC-3002', 2, 'Auto', 'AC456EF', 'Chevrolet Cruze', 'LXV45002', '3G1BE5SM3HS3002', '2017-01-01', 'Fuera de Servicio', 'Taller Central', '', '185000', '2022-07-25', 1, '2019-11-22', '2023-08-19'),
('TUC-3003', 1, 'Moto', 'A123BCD', 'Honda Tornado 250', 'MD25E33003', 'ME4MD2530H33003', '2018-01-01', 'En Reparación', 'Mecánica Policial', '', '45000', '2023-12-05', 1, '2021-03-08', '2024-02-15'),
('TUC-3004', 2, 'Camioneta', 'AD789GH', 'Ford Ranger', 'P5AT3004', 'AFMPXXMJ2L3004', '2020-01-01', 'Operativo', '', '', '98000', '2024-04-20', 0, '2022-01-19', NULL),
('TUC-3005', 1, 'Auto', 'AE321IJ', 'Volkswagen Vento', 'CAX30005', 'WVWSR13B7ME3005', '2016-01-01', 'Fuera de Servicio', 'Taller Local', '', '210000', '2021-09-14', 1, '2018-07-30', '2022-12-02'),
('TUC-3006', 2, 'Moto', 'B234EFG', 'Yamaha FZ 250', 'G3Y3006', 'ME1RG1033006', '2021-01-01', 'Operativo', '', '', '22000', '2024-01-07', 0, '2023-04-21', NULL),
('TUC-3007', 1, 'Camioneta', 'AF654KL', 'Nissan Frontier', 'YS232007', 'MNTCPND23Z3007', '2019-01-01', 'En Reparación', 'Taller Central', '', '134000', '2023-06-29', 1, '2020-10-12', '2024-05-03'),
('TUC-3008', 2, 'Auto', 'AG987MN', 'Peugeot 408', 'EP6803008', '8ADBH2HX2H3008', '2017-01-01', 'Operativo', '', '', '165000', '2023-11-10', 0, '2021-08-26', NULL),
('TUC-3009', 1, 'Moto', 'C345HIJ', 'Honda CB500X', 'PC50093009', 'MLHPC4693K3009', '2020-01-01', 'Fuera de Servicio', 'Mecánica Policial', '', '52000', '2022-05-18', 1, '2020-02-14', '2023-07-27'),
('TUC-3010', 2, 'Camioneta', 'AH112OP', 'Toyota Corolla', '2ZRFE3010', 'JTDBU4EE9B3010', '2018-01-01', 'Operativo', '', '', '140000', '2024-02-12', 0, '2019-04-03', NULL),
('TUC-3011', 1, 'Auto', 'AI223QR', 'Renault Fluence', 'H4M30011', 'VF14R1A3H30011', '2016-01-01', 'En Reparación', 'Taller Local', '', '198000', '2023-03-20', 1, '2018-03-29', '2024-01-11'),
('TUC-3012', 2, 'Moto', 'D456JKL', 'Zanella RX150', 'ZNA30012', 'L6JXKRL30N3012', '2022-01-01', 'Operativo', '', '', '12000', '2024-05-01', 0, '2023-09-15', NULL),
('TUC-3013', 1, 'Camioneta', 'AJ334ST', 'Chevrolet S10', '2.8CTDI3013', '8AG1450H3L3013', '2021-01-01', 'Fuera de Servicio', 'Taller Central', '', '88000', '2022-11-11', 1, '2021-04-18', '2023-09-20'),
('TUC-3014', 2, 'Auto', 'AK556UV', 'Fiat Cronos', 'GSE3014', '9BD358265L3014', '2020-01-01', 'Operativo', '', '', '76000', '2024-03-04', 0, '2022-06-09', NULL),
('TUC-3015', 1, 'Moto', 'E567LMN', 'Corven Triax 250', 'ZONG3015', 'LT8A2A1A5K3015', '2019-01-01', 'En Reparación', 'Mecánica Policial', '', '38000', '2023-10-16', 1, '2019-11-27', '2024-02-28');



SELECT * FROM PersonalPolicial;

DELETE FROM PersonalPolicial;
DBCC CHECKIDENT ('PersonalPolicial', RESEED, 0);

DELETE FROM Chaleco;
DBCC CHECKIDENT ('Chaleco', RESEED, 0);

DELETE FROM Domicilio;
DBCC CHECKIDENT ('Domicilio', RESEED, 0);

DELETE FROM Arma;
DBCC CHECKIDENT ('Arma', RESEED, 0);

DELETE FROM Escopeta;
DBCC CHECKIDENT ('Escopeta', RESEED, 0);

DELETE FROM Radio;
DBCC CHECKIDENT ('Radio', RESEED, 0);

DELETE FROM Vehiculo;
DBCC CHECKIDENT ('Vehiculo', RESEED, 0);