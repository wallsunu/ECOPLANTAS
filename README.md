# EcoPlantas

Aplicación web ASP.NET Core MVC para catálogo de plantas ecológicas, con formulario de contacto y panel de administración.

## Requisitos

- .NET 8 SDK
- PostgreSQL 14 o superior

## Base de datos — PostgreSQL

SQLite fue reemplazado por PostgreSQL como base de datos principal.

### Configuración local

En `appsettings.json` está la cadena de conexión de ejemplo para desarrollo local:

```json
"ConnectionStrings": {
  "DefaultConnection": "Host=localhost;Port=5432;Database=ecoplantas;Username=postgres;Password=postgres"
}
```

Crea la base de datos en PostgreSQL antes de aplicar migraciones:

```sql
CREATE DATABASE ecoplantas;
```

### Aplicar migraciones

```bash
dotnet ef database update
```

Esto crea las tablas `Productos`, `ContactoMensajes` y `Reciclajes`.

### Variable de entorno para producción (Render)

En producción la conexión se configura mediante la variable de entorno:

```
ConnectionStrings__DefaultConnection=Host=<host>;Port=5432;Database=<db>;Username=<user>;Password=<password>
```

Esta variable sobreescribe el valor de `appsettings.json` en tiempo de ejecución. No pongas credenciales reales en el archivo de configuración.
