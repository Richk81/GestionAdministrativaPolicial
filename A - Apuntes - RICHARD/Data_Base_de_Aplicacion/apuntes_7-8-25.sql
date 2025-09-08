use GestionOfPolicial

select * from rol

select * from usuario

SELECT * FROM Configuracion

--________________________________ INSERTAR ROLES ________________________________
insert into rol(descripcion,esActivo) values
('Administrador',1),
('Empleado',1),
('Supervisor',1)


--________________________________ INSERTAR USUARIO ________________________________
--clave : 123
insert into Usuario(nombre,correo,telefono,idRol,urlFoto,nombreFoto,clave,esActivo) values
('911','codigo@example.com','909090',1,'','','a665a45920422f9d417e4867efdc4fb8a04a1f3fff1fa07e998e86f7f7a27ae3',1)


--________________________________ INSERTAR LA DEPENDENCIA POLICIAL _______________________________
select * from Dependencia

insert into Dependencia(idDependencia,urlLogo,nombreLogo,nombre,correo,direccion,telefono)
values(1,'','','','','','')