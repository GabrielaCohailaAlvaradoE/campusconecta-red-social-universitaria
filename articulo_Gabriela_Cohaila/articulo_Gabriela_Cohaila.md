# Diseño e implementación de una arquitectura web para una red social universitaria

**Autora:** Gabriela Estefania Cohaila Alvarado  
**Código:** 2022075746  
**Proyecto:** CampusConecta  
**Fecha:** 4 de octubre de 2026

## Resumen

Este artículo describe el diseño y la implementación de CampusConecta, una plataforma web para la interacción entre estudiantes, docentes y personal universitario. La solución se construyó con React y TypeScript en el cliente, ASP.NET Core Web API en el servidor y PostgreSQL como base de datos. El trabajo se enfoca en las decisiones de arquitectura, el modelo relacional, la API REST, la autenticación mediante JSON Web Token, la validación de datos y la persistencia. La verificación incluyó seis pruebas automáticas de integración, compilación del frontend y un flujo HTTP ejecutado contra PostgreSQL real. En ese flujo se creó una publicación, un comentario y una reacción; posteriormente, la búsqueda y el panel personal recuperaron los cambios persistidos. Las seis pruebas automáticas aprobaron. Una muestra exploratoria de veinte consultas locales al feed obtuvo una media de 5.40 ms y un percentil 95 de 9.58 ms. Estas cifras corresponden al entorno de desarrollo y no constituyen una prueba de carga. El resultado es una plataforma funcional, reproducible y preparada para evolucionar hacia un escenario institucional.

**Palabras clave:** arquitectura web, ASP.NET Core, API REST, JWT, PostgreSQL, React, red social universitaria.

## Introducción

Un sitio de red social permite construir perfiles, relacionar participantes e intercambiar contenido dentro de un sistema delimitado. Boyd y Ellison definen estos elementos como rasgos centrales de las redes sociales en línea [1]. En una universidad, la misma idea requiere adaptar la interacción a una comunidad con estudiantes, docentes y personal, así como proteger las operaciones que cambian contenido o identidad.

CampusConecta se desarrolló como una aplicación web completa y ejecutable. No se planteó como una interfaz estática. Cada interacción visible se conecta con una ruta de la API y con una operación persistente. El sistema cubre registro, inicio de sesión, perfiles, publicaciones, comentarios, reacciones, comunidades, membresías, búsqueda y un panel personal. También incorpora respuestas de error y documentación Swagger para inspeccionar los contratos HTTP.

El reto principal consistió en integrar estas funciones sin concentrar la lógica en la interfaz ni permitir que el cliente decidiera permisos sensibles. Por ello, el proyecto separa presentación, servicios y persistencia. Esta organización facilita probar el backend de forma aislada, usar otro cliente en el futuro y mantener las reglas de autorización cerca de los datos que protegen.

## Objetivo

Diseñar e implementar una arquitectura web para una red social universitaria que incluya API REST, autenticación JWT, persistencia PostgreSQL, interfaz React y mecanismos básicos de validación y manejo de errores.

El alcance implementado incluye:

- Registro e inicio de sesión.
- Consulta y edición del perfil propio.
- Feed, creación y eliminación autorizada de publicaciones.
- Comentarios y reacciones.
- Creación, consulta y administración básica de comunidades.
- Unión y salida de comunidades.
- Búsqueda de usuarios, publicaciones y comunidades.
- Panel personal con métricas y actividad reciente.
- Swagger/OpenAPI, endpoint de salud, migraciones y pruebas.

No se implementaron mensajería privada, recuperación de contraseñas, carga de archivos, notificaciones en tiempo real ni autenticación federada. Declarar estos límites evita atribuir al sistema capacidades que no fueron desarrolladas.

## Arquitectura de la solución

CampusConecta sigue una arquitectura de tres capas desplegables.

| Capa | Tecnología | Responsabilidad |
| --- | --- | --- |
| Presentación | React 19, TypeScript y Vite | Formularios, navegación, estados de carga, visualización del feed y mensajes al usuario. |
| Servicios | ASP.NET Core 8 Web API | Endpoints REST, contratos, validaciones, autenticación, autorización y manejo de errores. |
| Persistencia | Entity Framework Core 8, Npgsql y PostgreSQL | Modelo relacional, migraciones, consultas e integridad de los datos. |
| Operación | Swagger/OpenAPI y Docker Compose | Exploración de la API, ejecución local y base de datos reproducible. |

La aplicación React consume respuestas JSON a través de una capa de acceso centralizada. Esa capa adjunta el token Bearer a las solicitudes autenticadas y traduce los errores de la API en mensajes que el usuario puede comprender. La interfaz contiene vistas para acceso, feed, comunidades, búsqueda, panel y perfil.

La API agrupa responsabilidades por recurso. `AuthController` registra y autentica cuentas. `UsersController` consulta y actualiza perfiles. `PostsController` resuelve feed, publicaciones, comentarios y reacciones. `CommunitiesController` administra comunidades y membresías. `SearchController` combina resultados de diferentes recursos, y `DashboardController` calcula los indicadores personales. Un middleware global transforma excepciones conocidas en respuestas HTTP consistentes y evita devolver trazas internas.

## Modelo de datos y persistencia

El esquema relacional contiene seis entidades principales.

| Entidad | Propósito y restricciones relevantes |
| --- | --- |
| `Users` | Conserva identidad, rol, perfil y hash de contraseña. El correo y el nombre de usuario son únicos. |
| `Communities` | Representa cada comunidad, su descripción, creador y estado. |
| `CommunityMembers` | Materializa la relación entre usuarios y comunidades; evita duplicar la membresía. |
| `Posts` | Guarda contenido, fecha, autor y comunidad opcional. |
| `Comments` | Relaciona un comentario con una publicación y su autor. |
| `Reactions` | Registra una reacción por combinación de usuario y publicación. |

Las claves foráneas garantizan que una publicación no exista sin autor y que un comentario no exista sin publicación. Las restricciones únicas protegen correos, nombres de usuario, nombres de comunidad, membresías y reacciones repetidas. PostgreSQL documenta las restricciones como reglas que rechazan escrituras incompatibles con el esquema [2]. En CampusConecta complementan las validaciones de la API, no las sustituyen.

Entity Framework Core genera y aplica la migración `InitialCreate`. Durante la validación se usó una instancia PostgreSQL real. La migración se aplicó correctamente y, tras el flujo de aceptación, se verificaron 3 usuarios, 3 publicaciones, 2 comentarios, 3 reacciones, 1 comunidad y 3 membresías. La configuración de entrega incorpora Docker Compose con PostgreSQL 16 para que el entorno pueda reconstruirse sin depender de la instancia usada durante las pruebas.

## Diseño de la API REST

Las rutas describen recursos y utilizan verbos HTTP de acuerdo con la operación. `POST` registra recursos; `GET` consulta; `PUT` actualiza perfil o reacción; y `DELETE` elimina contenido propio o abandona una comunidad. El contrato se expone en Swagger, que permite examinar esquemas y probar endpoints durante el desarrollo. La especificación OpenAPI define un formato estándar para describir este tipo de interfaces [3].

| Área | Operaciones representativas | Protección |
| --- | --- | --- |
| Autenticación | `POST /api/auth/register`, `POST /api/auth/login` | Pública |
| Perfil | `GET /api/users/me`, `PUT /api/users/me` | JWT |
| Publicaciones | `GET/POST /api/posts`, comentarios y reacciones | JWT; propiedad para eliminar |
| Comunidades | `GET/POST /api/communities`, `join`, `leave` | JWT; reglas de membresía |
| Consulta | `GET /api/search`, `GET /api/dashboard` | JWT |
| Operación | `GET /health`, `/swagger` | Pública en desarrollo |

La API usa códigos de estado para distinguir condiciones. Una entrada inválida devuelve 400; una solicitud sin sesión devuelve 401; una acción sobre un recurso ajeno devuelve 403; y un recurso inexistente devuelve 404. Este comportamiento permite que el frontend informe el problema sin interpretar mensajes técnicos del servidor.

## Autenticación y autorización

El registro valida el formato del correo, la longitud de los campos y la fortaleza mínima de la contraseña. La contraseña no se persiste como texto legible. ASP.NET Core `PasswordHasher<AppUser>` crea un hash con salt y parámetros de trabajo. OWASP recomienda almacenar contraseñas con funciones lentas y adaptativas, y nunca en texto plano [4].

Después de un inicio de sesión válido, la API genera un JWT firmado con identificador, correo, nombre de usuario y rol. RFC 7519 describe JWT como una representación compacta de claims intercambiable entre partes [5]. El middleware valida la firma, el emisor, la audiencia y el vencimiento antes de crear la identidad de la solicitud. Los controladores obtienen el identificador desde los claims y verifican propiedad o membresía antes de modificar un recurso.

La prueba automática de rutas protegidas solicitó el panel personal sin token y recibió 401. Las operaciones de eliminación también se restringen en servidor. Por ejemplo, ocultar un botón en React no basta: la API revisa que el autor de la solicitud sea el propietario del contenido.

La clave JWT y las credenciales de base de datos incluidas en la configuración son únicamente valores de desarrollo local. Antes de desplegar se deben reemplazar mediante variables de entorno o un gestor de secretos, habilitar HTTPS y reducir la exposición de Swagger. Para una universidad, la siguiente mejora prioritaria es incorporar OpenID Connect con la identidad institucional. Microsoft indica que los tokens propios son apropiados para sistemas cerrados y pruebas, pero no reemplazan una autoridad de identidad administrada [6].

## Implementación del frontend

La interfaz se construyó con React, TypeScript y Vite. React organiza la experiencia mediante componentes que combinan estado y representación [7]. En CampusConecta, la vista principal recupera el feed y permite crear publicaciones; cada tarjeta muestra autor, fecha, comunidad, reacciones y comentarios. Las vistas de comunidad indican miembros y estado de unión. La búsqueda muestra resultados por tipo y el panel personal resume actividad reciente.

Los formularios controlan datos incompletos antes de enviar la solicitud, pero la validación definitiva está en la API. Las respuestas de error se muestran como mensajes de interfaz. Esta combinación evita solicitudes triviales innecesarias sin confiar en el navegador para decisiones de seguridad.

## Verificación técnica

La verificación combinó pruebas automáticas, revisión del frontend y ejecución integral.

| Verificación | Resultado |
| --- | --- |
| `dotnet test CampusConecta.sln --configuration Release` | 6 pruebas aprobadas, 0 fallidas, 0 omitidas. |
| `npm run lint` | Aprobado sin advertencias. |
| `npm run build` | Build de producción correcto. |
| PostgreSQL real | Migración aplicada y flujo de escritura/lectura confirmado. |
| Interfaz | Ocho capturas reales de los recorridos principales. |

Las pruebas de API cubrieron salud, hash de una cuenta inicial, rechazo de contraseña débil, registro con JWT, feed, publicaciones, comentarios, reacciones, panel, comunidades, búsqueda, perfil y rechazo de acceso sin JWT. La prueba integral creó una publicación, un comentario y una reacción mediante HTTP; la búsqueda recuperó la publicación y el panel actualizó sus indicadores.

Se realizaron veinte solicitudes consecutivas al feed en el mismo equipo que alojaba la API. La media fue 5.40 ms, la mediana 4.73 ms, el percentil 95 9.58 ms, el mínimo 3.27 ms y el máximo 11.21 ms. La muestra permite detectar regresiones locales; no mide concurrencia, red pública ni capacidad productiva.

## Discusión

La arquitectura logró que cada requisito tuviera una representación visible, un endpoint y una regla de persistencia. Separar React de ASP.NET Core permitió validar el backend sin navegador y después confirmar la integración completa. PostgreSQL preservó las relaciones entre usuarios, contenido y comunidades mediante restricciones explícitas.

Persisten límites propios del alcance académico. El feed no usa caché ni cursores; la búsqueda no incorpora índices especializados; las imágenes se manejan como URL y no existe almacenamiento administrado. Tampoco se realizaron pruebas de carga, auditoría externa de seguridad o evaluación con usuarios. Estas limitaciones son puntos de evolución, no resultados omitidos.

## Conclusiones

CampusConecta demuestra una implementación funcional de red social universitaria con arquitectura separada, API REST, JWT, PostgreSQL, migraciones y Swagger. Las acciones principales modificaron una base real y sus resultados aparecieron en feed, búsqueda y panel. Las seis pruebas aprobadas y la compilación del frontend aportan evidencia de corrección en el alcance desarrollado.

La siguiente etapa debe incorporar autenticación institucional, secretos externos, HTTPS obligatorio, auditoría de acciones, reportes de contenido, pruebas de carga y accesibilidad asistida. Estas mejoras pueden añadirse sobre la estructura actual sin reescribir las capas principales.

## Referencias

1. boyd, d. m., y Ellison, N. B. (2007). *Social Network Sites: Definition, History, and Scholarship*. Journal of Computer-Mediated Communication, 13(1), 210–230. https://doi.org/10.1111/j.1083-6101.2007.00393.x
2. PostgreSQL Global Development Group. (2026). *PostgreSQL documentation: Constraints*. https://www.postgresql.org/docs/current/ddl-constraints.html
3. OpenAPI Initiative. (2026). *OpenAPI Specification*. https://spec.openapis.org/oas/
4. OWASP Foundation. (2026). *Password Storage Cheat Sheet*. https://cheatsheetseries.owasp.org/cheatsheets/Password_Storage_Cheat_Sheet.html
5. Jones, M., Bradley, J., y Sakimura, N. (2015). *JSON Web Token (JWT), RFC 7519*. Internet Engineering Task Force. https://www.rfc-editor.org/rfc/rfc7519
6. Microsoft. (2026). *Configure JWT bearer authentication in ASP.NET Core*. https://learn.microsoft.com/aspnet/core/security/authentication/configure-jwt-bearer-authentication
7. React Team. (2026). *React documentation*. https://react.dev/learn
