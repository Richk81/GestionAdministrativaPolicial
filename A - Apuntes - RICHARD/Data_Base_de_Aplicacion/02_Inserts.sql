use GestionOfPolicial
--________________________________ INSERTAR ROLES ________________________________
insert into rol(descripcion,esActivo) values
('Administrador',1),
('Empleado',1),
('Supervisor',1)


--________________________________ INSERTAR USUARIOS ________________________________
SELECT * FROM usuario
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

-- INSERTS de prueba para la tabla Chaleco (todos disponibles)
INSERT INTO Chaleco (SerieChaleco, IdUsuario, MarcaYmodelo, Talle, AnoFabricacion, AnoVencimiento, EstadoChaleco, Observaciones, IdPersonal)
VALUES 
('CHL001', 12, 'MORON DOLPHIN BEYON DOLPHIN', 'M', '2020-01-15', '2025-01-15', 'Nuevo', 'Chaleco disponible', NULL),
('CHL002', 13, 'MORON DOLPHIN BEYON DOLPHIN', 'L', '2019-06-10', '2024-06-10', 'Bueno', 'Chaleco disponible', NULL),
('CHL003', 12, 'MORON DOLPHIN BEYON DOLPHIN', 'S', '2021-03-20', '2026-03-20', 'Regular', 'Chaleco disponible', NULL),
('CHL004', 13, 'MORON DOLPHIN BEYON DOLPHIN', 'M', '2018-11-05', '2023-11-05', 'Usado', 'Chaleco disponible', NULL),
('CHL005', 12, 'MORON DOLPHIN BEYON DOLPHIN', 'XXL', '2022-07-12', '2027-07-12', 'Nuevo', 'Chaleco disponible', NULL);

INSERT INTO Chaleco (SerieChaleco, IdUsuario, MarcaYmodelo, Talle, AnoFabricacion, AnoVencimiento, EstadoChaleco, Observaciones, IdPersonal)
VALUES 
('CHL006', 12, 'MORON DOLPHIN BEYON DOLPHIN', 'L', '2020-09-01', '2025-09-01', 'Nuevo', 'Chaleco disponible', NULL),
('CHL007', 13, 'MORON DOLPHIN BEYON DOLPHIN', 'M', '2019-04-22', '2024-04-22', 'Bueno', 'Chaleco disponible', NULL),
('CHL008', 12, 'MORON DOLPHIN BEYON DOLPHIN', 'XL', '2021-02-10', '2026-02-10', 'Regular', 'Chaleco disponible', NULL),
('CHL009', 13, 'MORON DOLPHIN BEYON DOLPHIN', 'S', '2018-08-15', '2023-08-15', 'Usado', 'Chaleco disponible', NULL),
('CHL010', 12, 'MORON DOLPHIN BEYON DOLPHIN', 'M', '2022-11-25', '2027-11-25', 'Nuevo', 'Chaleco disponible', NULL),
('CHL011', 13, 'MORON DOLPHIN BEYON DOLPHIN', 'L', '2019-12-30', '2024-12-30', 'Bueno', 'Chaleco disponible', NULL),
('CHL012', 12, 'MORON DOLPHIN BEYON DOLPHIN', 'S', '2021-05-05', '2026-05-05', 'Regular', 'Chaleco disponible', NULL),
('CHL013', 13, 'MORON DOLPHIN BEYON DOLPHIN', 'XL', '2018-10-18', '2023-10-18', 'Usado', 'Chaleco disponible', NULL),
('CHL014', 12, 'MORON DOLPHIN BEYON DOLPHIN', 'XXL', '2020-03-11', '2025-03-11', 'Bueno', 'Chaleco disponible', NULL),
('CHL015', 13, 'MORON DOLPHIN BEYON DOLPHIN', 'M', '2022-01-09', '2027-01-09', 'Nuevo', 'Chaleco disponible', NULL);


-- Verificar los datos
SELECT * FROM Chaleco;