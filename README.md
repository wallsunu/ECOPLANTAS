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

## Despliegue en Render

La app incluye un `Dockerfile` multi-stage listo para Render.

### Pasos

1. Crear un servicio **Web Service** en Render apuntando al repositorio.
2. Render detecta el `Dockerfile` automáticamente.
3. Configurar las variables de entorno en el panel de Render (Environment).

### Variables de entorno obligatorias

| Variable | Descripción |
|---|---|
| `ASPNETCORE_ENVIRONMENT` | Debe ser `Production` |
| `ConnectionStrings__DefaultConnection` | Cadena de conexión PostgreSQL (ver abajo) |
| `Redis__ConnectionString` | Cadena de conexión Redis (ver abajo) |

### ConnectionStrings__DefaultConnection

Usa el **Internal Database URL** de tu PostgreSQL de Render. El formato requerido es:

```
Host=HOST_INTERNO;Port=5432;Database=NOMBRE_DB;Username=USUARIO;Password=PASSWORD
```

> Render muestra estos datos en el panel del servicio PostgreSQL → Internal Connection String. Copia los campos por separado y arma la cadena en el formato anterior. **No uses la External URL** — es más lenta y tiene cuota limitada.

### Redis__ConnectionString

Usa el **Internal Key Value URL** de tu Redis de Render. Render lo entrega en formato:

```
redis://red-xxxxxxxxxxxx:6379
```

La app normaliza este formato automáticamente. Pégalo tal cual como valor de `Redis__ConnectionString`.

### Notas de infraestructura

- El contenedor escucha en HTTP plano en el puerto `PORT` (Render lo inyecta como variable). La terminación TLS la hace el proxy de Render.
- `UseHttpsRedirection` está desactivado en producción para evitar redirect loops.
- Las migraciones se aplican automáticamente al arrancar vía `EnsureCreated`. Para migraciones incrementales futuras usa `dotnet ef database update` antes del deploy o agrega un paso de startup.
- **No subas passwords reales a GitHub.** Todas las credenciales van como variables de entorno en el panel de Render.
