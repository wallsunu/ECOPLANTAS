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

## API REST

La documentación interactiva está disponible en `/swagger`.

### Endpoints disponibles

| Método | Ruta | Descripción |
|--------|------|-------------|
| GET | `/api/health` | Estado del servicio |
| GET | `/api/productos` | Listar productos disponibles |
| GET | `/api/productos/{id}` | Detalle de un producto |
| GET | `/api/reciclajes` | Listar reciclajes |
| POST | `/api/reciclajes` | Registrar un reciclaje |
| POST | `/api/contacto` | Enviar mensaje de contacto |
| GET | `/api/usuarios/me` | Datos del usuario en sesión |
| POST | `/api/ml/clasificar-reciclaje` | Clasificar impacto de un reciclaje con ML.NET |
| POST | `/api/ml/recomendar-planta` | Recomendar una planta según reciclaje y preferencia con ML.NET |

## ML.NET — Clasificación de impacto de reciclaje

El endpoint `POST /api/ml/clasificar-reciclaje` usa un modelo de clasificación multiclase entrenado con ML.NET.

El modelo toma como entrada el tipo de material y la cantidad en kg, y predice el nivel de impacto ambiental: **Bajo**, **Medio** o **Alto**.

El modelo se entrena en memoria al primer uso con datos sintéticos. No requiere archivos externos ni configuración adicional.

### Ejemplo de request

```json
POST /api/ml/clasificar-reciclaje
Content-Type: application/json

{
  "tipoMaterial": "Plástico",
  "cantidadKg": 8
}
```

### Ejemplo de response

```json
{
  "tipoMaterial": "Plástico",
  "cantidadKg": 8,
  "nivelImpacto": "Medio",
  "mensaje": "Reciclaje de 8 kg de Plástico tiene un impacto ambiental MEDIO. ¡Sigue sumando!"
}
```

### Materiales soportados

- Plástico
- Vidrio
- Metal
- Papel
- Orgánico

## ML.NET — Recomendación de plantas

El endpoint `POST /api/ml/recomendar-planta` usa ML.NET para recomendar una planta del catálogo según el tipo de material reciclado, la cantidad en kg y la preferencia del usuario.

El modelo se entrena en memoria al primer uso con datos sintéticos que asocian combinaciones de material, cantidad y preferencia con plantas reales del catálogo. No requiere archivos externos ni configuración adicional.

### Ejemplo de request

```json
POST /api/ml/recomendar-planta
Content-Type: application/json

{
  "tipoMaterial": "Plástico",
  "cantidadKg": 8,
  "preferencia": "Fácil cuidado"
}
```

### Ejemplo de response

```json
{
  "tipoMaterial": "Plástico",
  "cantidadKg": 8,
  "preferencia": "Fácil cuidado",
  "plantaRecomendada": "Pothos Dorado",
  "motivo": "El Pothos Dorado es ideal para quienes reciclan plástico: crece en cualquier espacio y purifica el aire de toxinas comunes en materiales sintéticos."
}
```

### Preferencias soportadas

- Interior
- Fácil cuidado
- Decorativa
- Resistente

### Plantas del catálogo

- Pothos Dorado
- Suculenta Echeveria
- Helecho de Boston
- Cactus San Pedro
- Lavanda Ecológica
- Aloe Vera Orgánico

