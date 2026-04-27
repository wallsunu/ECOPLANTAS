# EcoPlantas

Aplicación web desarrollada en ASP.NET Core MVC para promover el reciclaje y el canje de plantas ecológicas.  
El sistema permite visualizar un catálogo de plantas, registrar reciclaje, enviar mensajes de contacto y administrar sesiones de usuario.

## Tecnologías utilizadas

- ASP.NET Core MVC (.NET 8)
- Entity Framework Core
- PostgreSQL
- Redis para sesiones en producción
- Bootstrap
- Docker
- Render

## Funcionalidades principales

### Funcionalidades sin iniciar sesión

Los usuarios pueden acceder libremente a:

- Página principal tipo landing.
- Catálogo de plantas ecológicas.
- Página de contacto.
- Registro de nuevos usuarios.
- Inicio de sesión.

### Funcionalidades con sesión iniciada

Los usuarios registrados pueden:

- Iniciar sesión.
- Registrar reciclaje.
- Ver el listado de reciclajes.
- Consultar el detalle de un registro de reciclaje.
- Cerrar sesión.

### Funcionalidades de autenticación

El sistema cuenta con:

- Login con usuarios guardados en base de datos.
- Registro de nuevos usuarios.
- Contraseñas protegidas con BCrypt.
- Validación de email duplicado.
- Manejo de sesión con `UsuarioId`, `UsuarioEmail` y `UsuarioRol`.

## Usuario administrador de prueba

El sistema crea automáticamente un usuario administrador inicial mediante el seed de datos:

| Campo      |          Valor         |
|------------|------------------------|
| Email      | `admin@ecoplantas.com` |
| Contraseña | `Admin123!`            |
| Rol        | `Admin`                |

> Nota: Esta credencial es solo para pruebas académicas.

## Base de datos — PostgreSQL

El proyecto usa PostgreSQL como base de datos principal.

Tablas principales:

- `Productos`
- `ContactoMensajes`
- `Reciclajes`
- `Usuarios`

