# EcoPlantas

Aplicación web desarrollada en ASP.NET Core MVC (.NET 8) para el distrito de Cieneguilla, Perú.  
Permite gestionar un catálogo de plantas ecológicas, registrar reciclaje, enviar mensajes de contacto, autenticar usuarios y exponer APIs REST con modelos de inteligencia artificial usando ML.NET.

---

## Funcionalidades principales

| Módulo | Descripción |
|--------|-------------|
| Home landing | Página principal con información del programa |
| Catálogo de plantas | Lista de plantas ecológicas disponibles para canje |
| Contacto | Formulario de contacto persistido en base de datos |
| Login | Autenticación con usuarios en PostgreSQL y BCrypt |
| Registro de usuarios | Registro de nuevos vecinos con validación de email duplicado |
| Sesiones | Sesiones distribuidas con Redis en producción |
| Reciclaje | CRUD de registros de reciclaje, protegido con sesión |
| Base de datos | PostgreSQL con Entity Framework Core y migraciones |
| APIs REST | Endpoints REST documentados con Swagger |
| Swagger | Documentación interactiva en `/swagger` |
| Render | Despliegue en la nube con Docker y variables de entorno |
| Redis | Caché distribuido para sesiones en producción |
| ML.NET Clasificación | Modelo multiclase que clasifica el impacto de un reciclaje |
| ML.NET Recomendación | Modelo multiclase que recomienda una planta según el reciclaje |

---

## Tecnologías utilizadas

- **ASP.NET Core MVC** — .NET 8
- **Entity Framework Core** — ORM y migraciones
- **PostgreSQL** — Base de datos relacional
- **Redis** — Caché distribuido para sesiones
- **Render** — Plataforma de despliegue en la nube
- **Docker** — Contenedorización con imagen multi-stage
- **Swagger / Swashbuckle** — Documentación interactiva de APIs
- **ML.NET 3.0.1** — Modelos de clasificación y recomendación
- **BCrypt.Net-Next** — Hash seguro de contraseñas
- **Git / GitHub** — Control de versiones por ramas de feature

---

## Base de datos — PostgreSQL

El proyecto usa PostgreSQL como base de datos principal, conectado mediante Entity Framework Core con migraciones automáticas.

| Tabla | Propósito |
|-------|-----------|
| `Productos` | Catálogo de plantas ecológicas |
| `ContactoMensajes` | Mensajes enviados desde el formulario de contacto |
| `Reciclajes` | Registros de reciclaje por tipo de material |
| `Usuarios` | Cuentas de usuario con contraseña hasheada |
| `__EFMigrationsHistory` | Historial de migraciones de EF Core |

---

## Credenciales demo

> Estas credenciales son exclusivamente para entorno académico y de demostración.  
> En producción deben cambiarse antes del despliegue.

| Campo | Valor |
|-------|-------|
| Email | `admin@ecoplantas.com` |
| Contraseña | `Admin123!` |
| Rol | `Admin` |

El usuario administrador se crea automáticamente mediante el seed de datos al iniciar la aplicación.

---

## Despliegue en Render

El proyecto incluye `Dockerfile` y `.dockerignore` listos para Render.

### Variables de entorno requeridas

| Variable | Valor |
|----------|-------|
| `ASPNETCORE_ENVIRONMENT` | `Production` |
| `ConnectionStrings__DefaultConnection` | `Host=<host_interno>;Port=5432;Database=<db>;Username=<user>;Password=<password>` |
| `Redis__ConnectionString` | `redis://<host_interno>:6379` |

### Consideraciones importantes

- No usar `localhost` en Render — los servicios se conectan mediante hostname interno.
- Para PostgreSQL: usar el **Internal Database URL** del servicio de base de datos en Render.
- Para Redis: usar la **Internal Redis URL** o **Internal Key Value URL** del servicio Redis en Render.
- El puerto lo asigna Render mediante la variable `PORT`; el `Dockerfile` ya lo gestiona automáticamente.

---

## API REST

La documentación interactiva está disponible en `/swagger`.  
Swagger muestra únicamente los endpoints que comienzan con `/api`.

### Endpoints disponibles

| Método | Ruta | Descripción |
|--------|------|-------------|
| `GET` | `/api/health` | Estado del servicio |
| `GET` | `/api/productos` | Listar productos disponibles |
| `GET` | `/api/productos/{id}` | Detalle de un producto por ID |
| `GET` | `/api/reciclajes` | Listar todos los reciclajes |
| `POST` | `/api/reciclajes` | Registrar un nuevo reciclaje |
| `POST` | `/api/contacto` | Enviar un mensaje de contacto |
| `GET` | `/api/usuarios/me` | Datos del usuario con sesión activa |
| `POST` | `/api/ml/clasificar-reciclaje` | Clasificar impacto ambiental con ML.NET |
| `POST` | `/api/ml/recomendar-planta` | Recomendar una planta con ML.NET |

---

## ML.NET — Clasificación de impacto de reciclaje

El endpoint clasifica el impacto ambiental de un reciclaje en tres niveles usando un modelo de clasificación multiclase entrenado con ML.NET (SDCA Maximum Entropy).

El modelo se entrena en memoria al primer uso con datos sintéticos. No requiere archivos externos ni configuración adicional.

**Endpoint:** `POST /api/ml/clasificar-reciclaje`

**Request:**
```json
{
  "tipoMaterial": "Plástico",
  "cantidadKg": 8
}
```

**Response:**
```json
{
  "tipoMaterial": "Plástico",
  "cantidadKg": 8,
  "nivelImpacto": "Medio",
  "mensaje": "Reciclaje de 8 kg de Plástico tiene un impacto ambiental MEDIO. ¡Sigue sumando!"
}
```

**Niveles de impacto:**

| Nivel | Criterio general |
|-------|-----------------|
| `Bajo` | Cantidades pequeñas según el material |
| `Medio` | Cantidades intermedias |
| `Alto` | Grandes volúmenes de reciclaje |

**Materiales soportados:** Plástico, Vidrio, Metal, Papel, Orgánico

---

## ML.NET — Recomendación de plantas

El endpoint recomienda una planta del catálogo según el tipo de material reciclado, la cantidad en kg y la preferencia del usuario. Usa ML.NET para recomendar una planta según el reciclaje y preferencia del usuario mediante un modelo de clasificación multiclase (SDCA Maximum Entropy).

El modelo se entrena en memoria al primer uso con datos sintéticos que asocian combinaciones de material, cantidad y preferencia con plantas reales del catálogo.

**Endpoint:** `POST /api/ml/recomendar-planta`

**Request:**
```json
{
  "tipoMaterial": "Plástico",
  "cantidadKg": 8,
  "preferencia": "Fácil cuidado"
}
```

**Response:**
```json
{
  "tipoMaterial": "Plástico",
  "cantidadKg": 8,
  "preferencia": "Fácil cuidado",
  "plantaRecomendada": "Pothos Dorado",
  "motivo": "El Pothos Dorado es ideal para quienes reciclan plástico: crece en cualquier espacio y purifica el aire de toxinas comunes en materiales sintéticos."
}
```

**Preferencias soportadas:** Interior, Fácil cuidado, Decorativa, Resistente

**Plantas del catálogo:**

| Planta | Perfil |
|--------|--------|
| Pothos Dorado | Fácil cuidado, interior, plástico |
| Suculenta Echeveria | Decorativa, vidrio, plástico |
| Helecho de Boston | Interior, orgánico, papel |
| Cactus San Pedro | Resistente, metal |
| Lavanda Ecológica | Decorativa, vidrio, papel |
| Aloe Vera Orgánico | Interior, fácil cuidado, orgánico |

---

## Agente IA con Semantic Kernel + Ollama

EcoPlantas incluye un agente inteligente que combina **Microsoft Semantic Kernel** como capa de orquestación con **Ollama** como LLM local.

### Flujo de la arquitectura

```
Usuario → AiController → AgentService → Semantic Kernel → OllamaService → Respuesta
```

### Componentes

| Componente | Archivo | Propósito |
|------------|---------|-----------|
| `EcoPlantasSemanticKernelService` | `Services/SemanticKernel/EcoPlantasSemanticKernelService.cs` | Orquestador con `Kernel.CreateBuilder()` y plugin nativo |
| `EcoPlantasPlugin` | (mismo archivo) | Plugin con funciones que el Kernel puede invocar |
| `AgentService` | `Services/AgentService.cs` | Delega al Kernel para enriquecer el contexto antes de llamar a Ollama |
| `OllamaService` | `Services/OllamaService.cs` | LLM local vía API REST de Ollama (modelo `qwen3:4b`) |
| `AiController` | `Controllers/AiController.cs` | Endpoints HTTP del agente |

### Funciones del plugin `EcoPlantas`

| Función | Descripción |
|---------|-------------|
| `ObtenerTotalKgReciclados` | Total de kg reciclados desde la base de datos |
| `ObtenerTotalRegistros` | Número de registros de reciclaje |
| `ObtenerProductosDisponibles` | Lista de plantas disponibles en el catálogo |
| `ObtenerContextoEcologico` | Contexto y personalidad del agente EcoBot |

### Endpoints del agente

| Método | Ruta | Descripción |
|--------|------|-------------|
| `POST` | `/api/ai/chat` | Chat con EcoBot (texto libre) |
| `GET` | `/api/ai/test` | Prueba rápida: consulta de reciclaje |
| `GET` | `/api/ai/semantic-kernel-test` | Estado del Kernel: versión, plugins y datos en tiempo real |

### Ejemplo: Semantic Kernel test

```
GET /api/ai/semantic-kernel-test
```

```json
{
  "semanticKernelVersion": "1.29.0.0",
  "kernelActivo": true,
  "pluginsRegistrados": [
    {
      "nombre": "EcoPlantas",
      "funciones": ["ObtenerTotalKgReciclados", "ObtenerTotalRegistros", "ObtenerProductosDisponibles", "ObtenerContextoEcologico"]
    }
  ],
  "datosEnTiempoReal": {
    "totalKgReciclados": "42 kg",
    "totalRegistros": "7",
    "plantasDisponibles": "Pothos Dorado, Suculenta Echeveria, ..."
  }
}
```

> Requiere Ollama corriendo localmente con el modelo `qwen3:4b` para los endpoints de chat. El endpoint `semantic-kernel-test` funciona sin Ollama.

---

## Ramas del repositorio

| Rama | Propósito |
|------|-----------|
| `main` | Rama principal con todo integrado |
| `feature/postgres-db` | Migración a PostgreSQL con EF Core |
| `feature/login-db-usuarios` | Login con usuarios en base de datos y BCrypt |
| `feature/reciclaje-crud` | CRUD de reciclajes protegido con sesión |
| `feature/home-landing` | Rediseño de la página principal |
| `feature/register-users` | Registro de nuevos usuarios |
| `feature/api-rest` | APIs REST + Swagger |
| `feature/mlnet-clasificacion` | Modelo ML.NET de clasificación de impacto |
| `feature/mlnet-recomendacion` | Modelo ML.NET de recomendación de plantas |
| `chore/render-docker-main` | Dockerfile y configuración para Render |
| `docs/practica-3-final` | Documentación final de la 3ra práctica |

---

## Cómo ejecutar en local

### Requisitos previos

- .NET 8 SDK
- PostgreSQL corriendo localmente
- (Opcional) Redis para sesiones distribuidas

### Pasos

```bash
# 1. Restaurar dependencias
dotnet restore

# 2. Compilar
dotnet build

# 3. Aplicar migraciones (requiere dotnet-ef instalado)
dotnet ef database update

# 4. Ejecutar
dotnet run
```

### URLs locales

| URL | Descripción |
|-----|-------------|
| `http://localhost:5055` | Aplicación web |
| `http://localhost:5055/swagger` | Documentación Swagger |

> Si no hay Redis disponible en local, la aplicación usa caché en memoria automáticamente (modo Development).

---

## Evidencias sugeridas para la presentación

Capturas recomendadas para el informe o sustentación:

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
| `Ollama__BaseUrl` | (Opcional) URL pública del servidor Ollama (ver sección IA · Ollama) |
| `Ollama__Model` | (Opcional) Modelo de Ollama, por defecto `llama3.2` |

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

### IA · Ollama (configurable)

El servicio de IA (Ollama) es configurable por entorno; ya **no** usa una URL fija en código:

```
Ollama__BaseUrl=http://localhost:11434   (por defecto)
Ollama__Model=llama3.2                   (por defecto)
```

En **local** no necesitas configurar nada: si las variables no están definidas, la app usa
`http://localhost:11434` y el modelo `llama3.2` automáticamente.

En **Render** (u otro proveedor) define:

```
Ollama__BaseUrl=https://URL_PUBLICA_DE_OLLAMA
Ollama__Model=llama3.2
```

> ⚠️ **Importante:** en Render, `localhost` **no** apunta a tu computadora, sino al propio
> servidor/contenedor de Render. Por eso `http://localhost:11434` solo funciona en tu máquina.
> Para usar IA en producción necesitas exponer Ollama en una URL pública accesible
> (host propio, túnel, o un servicio Ollama remoto) y ponerla en `Ollama__BaseUrl`.

Diagnóstico rápido: `GET /api/ai/ollama-health` devuelve `baseUrl`, `model`, `disponible` y un
`mensaje` (sin exponer secretos). Si Ollama no responde, el chat (`POST /api/ai/chat`) devuelve
un mensaje amigable en lugar de un error 500.

### Notas de infraestructura

- El contenedor escucha en HTTP plano en el puerto `PORT` (Render lo inyecta como variable). La terminación TLS la hace el proxy de Render.
- `UseHttpsRedirection` está desactivado en producción para evitar redirect loops.
- Las migraciones se aplican automáticamente al arrancar vía `EnsureCreated`. Para migraciones incrementales futuras usa `dotnet ef database update` antes del deploy o agrega un paso de startup.
- **No subas passwords reales a GitHub.** Todas las credenciales van como variables de entorno en el panel de Render.
