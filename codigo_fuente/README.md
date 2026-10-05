# CampusConecta

CampusConecta es una red social universitaria funcional para estudiantes, docentes y personal administrativo. Incluye autenticación JWT, perfiles, publicaciones, comentarios, reacciones, comunidades, búsqueda, panel personal, moderación básica y persistencia en PostgreSQL.

## Tecnologías

- Backend: ASP.NET Core 8 Web API, Entity Framework Core 8 y Npgsql.
- Frontend: React 19, TypeScript y Vite.
- Base de datos: PostgreSQL 16 en Docker Compose.
- Seguridad: contraseñas con `PasswordHasher` de ASP.NET Core y tokens JWT Bearer.
- Documentación: Swagger/OpenAPI.
- Pruebas: xUnit, WebApplicationFactory y EF Core InMemory.

## Requisitos

- .NET SDK 8
- Node.js 20 o superior
- Docker Desktop con Docker Compose
- PowerShell 7 o Windows PowerShell 5.1

## Instalación y ejecución

Desde esta carpeta:

```powershell
.\instalar.ps1
.\iniciar.ps1
```

La aplicación web queda disponible en `http://localhost:5173` y Swagger en `http://localhost:5050/swagger`. El backend aplica las migraciones y carga datos de demostración al arrancar.

Para detener la aplicación:

```powershell
.\detener.ps1
```

## Pruebas

```powershell
.\probar.ps1
```

El script ejecuta las pruebas del backend, el análisis estático del frontend y la compilación de producción.

## Usuarios de demostración

La contraseña de las tres cuentas locales es `Demo123!`.

| Rol | Correo |
| --- | --- |
| Estudiante | `gabriela@campus.test` |
| Estudiante | `victoria@campus.test` |
| Docente | `docente@campus.test` |

Estas credenciales se crean únicamente para demostrar el proyecto en un entorno local. Antes de desplegar, reemplace la clave JWT y las credenciales de PostgreSQL mediante variables de entorno, active HTTPS y quite los datos iniciales de demostración.

## Despliegue público

La publicación integra el frontend compilado dentro de ASP.NET Core y utiliza la misma URL para la interfaz y la API. La instancia pública usa una base SQLite persistente configurada por variables de entorno de Azure; el entorno local y Docker continúan usando PostgreSQL. El flujo de GitHub Actions ejecuta pruebas, análisis estático, compilación y despliegue en cada cambio a `main`.

## Configuración

`.env.example` documenta las variables necesarias. ASP.NET Core acepta las claves `ConnectionStrings__Postgres`, `ConnectionStrings__Sqlite`, `Jwt__Issuer`, `Jwt__Audience`, `Jwt__Key` y `AllowedHosts`. El frontend acepta `VITE_API_URL`.

## Endpoints principales

- `POST /api/auth/register` y `POST /api/auth/login`
- `GET/PUT /api/users/me` y `GET /api/users/{id}`
- `GET/POST /api/posts`
- `POST /api/posts/{id}/comments`
- `PUT /api/posts/{id}/reaction`
- `GET/POST /api/communities`
- `POST /api/communities/{id}/join` y `DELETE /api/communities/{id}/leave`
- `GET /api/search?q=...`
- `GET /api/dashboard`
- `GET /health`

## Estructura

- `backend/CampusConecta.Api`: API, modelo de datos, migraciones y carga inicial.
- `backend/CampusConecta.Tests`: pruebas automáticas de integración.
- `frontend`: aplicación React.
- `docker-compose.yml`: servicio PostgreSQL y volumen persistente.

