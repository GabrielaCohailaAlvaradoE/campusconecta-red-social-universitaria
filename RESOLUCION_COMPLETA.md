# Resolución completa del proyecto CampusConecta

## Problema resuelto

Se desarrolló una red social universitaria web que permite registrar y autenticar personas, administrar perfiles, publicar contenido, comentar, reaccionar, crear comunidades, administrar membresías, buscar información y revisar un panel personal. Las acciones se persisten en PostgreSQL y se exponen mediante una API REST documentada con Swagger.

## Arquitectura

La solución separa tres capas:

1. React 19 y TypeScript presentan la interfaz de usuario.
2. ASP.NET Core 8 implementa los endpoints, validaciones, autenticación y reglas de autorización.
3. Entity Framework Core y Npgsql gestionan un esquema PostgreSQL reproducible mediante migraciones.

Docker Compose crea PostgreSQL 16 y un volumen persistente. La API aplica la migración inicial y carga datos de demostración al comenzar.

## Funcionalidades implementadas

| Requisito | Implementación | Evidencia |
| --- | --- | --- |
| Registro e inicio de sesión | Endpoints públicos, hash de contraseña y JWT | Pruebas 2, 3 y 4; capturas 01 y 02 |
| Perfiles y edición | Consulta pública por identificador y edición del perfil propio | Prueba 5; captura 06 |
| Publicaciones | Feed, detalle, creación y eliminación autorizada | Prueba 4; captura 03 |
| Comentarios | Creación y eliminación por propietario | Prueba 4; captura 03 |
| Reacciones | Tipos controlados y una reacción por usuario y publicación | Prueba 4; captura 03 |
| Comunidades | Consulta, creación, administración básica y eliminación | Prueba 5; captura 04 |
| Unión y salida | Endpoints join y leave con control de duplicados | Prueba 5; captura 04 |
| Búsqueda | Usuarios, publicaciones y comunidades en una respuesta | Prueba 5; captura 07 |
| Panel personal | Indicadores y actividad reciente | Prueba 4; captura 05 |
| Gestión de contenido | Eliminación restringida a autor o propietario | Controladores y respuestas 403 |
| Persistencia | Seis tablas y migración EF Core | Evidencia 09 |
| Errores y validación | Data annotations, reglas de dominio y middleware global | Pruebas 3 y 6 |

## Modelo de datos

- `Users`: identidad, rol, perfil y hash de contraseña.
- `Communities`: comunidad y creador.
- `CommunityMembers`: membresía entre usuario y comunidad.
- `Posts`: publicación, autor y comunidad opcional.
- `Comments`: comentario, autor y publicación.
- `Reactions`: tipo de reacción por usuario y publicación.

El esquema aplica claves primarias, foráneas e índices únicos para identidades y relaciones que no pueden duplicarse.

## Seguridad

- Hash de contraseñas mediante `PasswordHasher<AppUser>` de ASP.NET Core.
- JWT Bearer con validación de firma, emisor, audiencia y vencimiento.
- Identificación del propietario desde claims del token.
- Respuestas 401 para solicitudes sin autenticación y 403 para acciones no autorizadas.
- Clave y credenciales locales marcadas como valores de desarrollo que deben reemplazarse mediante variables de entorno antes de desplegar.

## Pruebas ejecutadas

`dotnet test CampusConecta.sln --configuration Release` completó seis pruebas sin fallos. Se cubrieron salud, hash, validación de contraseña, registro, JWT, feed, publicaciones, comentarios, reacciones, panel, comunidades, búsqueda, perfil y rechazo de acceso anónimo.

`npm run lint` terminó sin advertencias y `npm run build` generó el paquete de producción. El flujo integral usó PostgreSQL real y comprobó que una publicación, un comentario y una reacción se conservaron y aparecieron en búsqueda y panel.

Una medición exploratoria de veinte consultas locales al feed obtuvo una media de 5.40 ms y un percentil 95 de 9.58 ms. No fue una prueba de carga y no se usa para estimar capacidad productiva.

## Limitaciones declaradas

La versión entregada no incluye mensajería privada, notificaciones en tiempo real, recuperación de contraseña, archivos adjuntos administrados, autenticación federada ni moderación institucional con auditoría. Tampoco se realizaron pruebas con usuarios externos, carga concurrente o auditoría formal de accesibilidad y seguridad.

## Mejoras recomendadas

1. Integrar OpenID Connect con la identidad de la universidad.
2. Almacenar secretos en un servicio administrado y activar HTTPS obligatorio.
3. Incorporar reportes, roles de moderación y bitácora de acciones.
4. Ejecutar pruebas de carga con datos representativos.
5. Auditar WCAG 2.2 con teclado y tecnologías de asistencia.
6. Añadir almacenamiento de archivos y notificaciones.

