use GestionOfPolicial
--________________________________ INSERTAR ROLES ________________________________
insert into rol(descripcion,esActivo) values
('Administrador',1),
('Empleado',1),
('Supervisor',1)


--________________________________ INSERTAR USUARIOS ________________________________
SELECT * FROM Usuario
--clave : 123
insert into Usuario(nombre,correo,telefono,idRol,urlFoto,nombreFoto,clave,esActivo) values
('911','codigo@example.com','909090',1,'','','a665a45920422f9d417e4867efdc4fb8a04a1f3fff1fa07e998e86f7f7a27ae3',1)

INSERT INTO Usuario (nombre, correo, telefono, idRol, urlFoto, nombreFoto, clave, esActivo)
VALUES
('Admin X', 'empleado@example.com', '909090', 1, '', '', 'a665a45920422f9d417e4867efdc4fb8a04a1f3fff1fa07e998e86f7f7a27ae3', 1),
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

insert into Dependencia(idDependencia,urlLogo,nombreLogo,nombre,correo,direccion,telefono)
values(1,'','','','','','')

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

