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

## Sesiones — Redis

### Desarrollo local (opcional)

En local no es necesario Redis. Si la variable `Redis__ConnectionString` no está definida y el entorno es `Development`, la app usa `DistributedMemoryCache` automáticamente.

Si quieres probar Redis en local, levanta una instancia con Docker:

```bash
docker run -d -p 6379:6379 redis:alpine
```

Y define la variable de entorno antes de arrancar la app:

```bash
Redis__ConnectionString=localhost:6379
```

### Producción (obligatorio)

En producción la app requiere Redis para las sesiones. Si `Redis__ConnectionString` no está definida, la app lanza un error al iniciar.

Configura la variable de entorno en Render (u otro proveedor):

```
Redis__ConnectionString=<host>:<port>
```

Si Render entrega la URL en formato `redis://host:port`, la app la normaliza automáticamente — no necesitas hacer nada extra.

#### Render — Redis interno

En Render puedes crear un servicio Redis gratuito y usar la variable de entorno `REDIS_URL` que genera automáticamente. Mapéala así en el panel de Render:

| Variable en Render | Valor |
|---|---|
| `Redis__ConnectionString` | (pega el valor de `REDIS_URL` de tu servicio Redis de Render) |
