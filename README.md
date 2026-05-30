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

| # | Pantalla | Qué mostrar |
|---|----------|-------------|
| 1 | Home | Landing completo con secciones |
| 2 | Login | Formulario y acceso con credencial demo |
| 3 | Registro | Formulario de nuevo usuario |
| 4 | Catálogo | Lista de plantas ecológicas |
| 5 | Reciclaje | Lista y formulario de registro |
| 6 | Swagger | Lista de endpoints `/api` |
| 7 | GET /api/health | Respuesta `{ "status": "ok" }` |
| 8 | POST /api/ml/clasificar-reciclaje | Request y response con nivel de impacto |
| 9 | POST /api/ml/recomendar-planta | Request y response con planta recomendada |
| 10 | Render | Dashboard con servicio activo y URL pública |
