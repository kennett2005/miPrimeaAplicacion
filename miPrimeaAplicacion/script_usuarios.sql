-- ============================================================
-- Script SQL para crear la tabla usuarios_usss090925
-- Base de datos: db_academica.mdf (LocalDB SQL Server)
-- ============================================================

-- Crear la tabla de usuarios
CREATE TABLE usuarios_usss090925 (
    idUsuario   INT IDENTITY(1,1) PRIMARY KEY,
    usuario     VARCHAR(35) NOT NULL,
    clave       VARCHAR(35) NOT NULL,
    nombre      VARCHAR(65) NOT NULL,
    direccion   VARCHAR(85),
    telefono    VARCHAR(9)
);
GO

-- Insertar 3 registros de prueba con nuevos usuarios y contraseñas
INSERT INTO usuarios_usss090925 (usuario, clave, nombre, direccion, telefono)
VALUES ('admin2025', 'Admin#2025!', 'Administrador del Sistema', 'Tegucigalpa, Honduras', '99887766');

INSERT INTO usuarios_usss090925 (usuario, clave, nombre, direccion, telefono)
VALUES ('crodriguez', 'Carlos@123', 'Carlos Roberto Rodríguez', 'San Pedro Sula, Cortés', '98765432');

INSERT INTO usuarios_usss090925 (usuario, clave, nombre, direccion, telefono)
VALUES ('lhernandez', 'Laura$456', 'Laura Hernández Gómez', 'La Ceiba, Atlántida', '33445566');
GO