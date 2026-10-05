# Evaluación funcional y calidad de una plataforma web de interacción universitaria

**Autora:** Victoria Isabel Lavarello Vidaurre  
**Código:** 2023077281  
**Proyecto:** CampusConecta  
**Fecha:** 4 de octubre de 2026

## Resumen

Este artículo evalúa CampusConecta, una plataforma web de interacción universitaria para estudiantes, docentes y personal. La evaluación convierte los requisitos del sistema en criterios verificables y combina pruebas automáticas de integración, análisis estático, compilación del frontend, pruebas HTTP contra PostgreSQL real y revisión visual de las pantallas principales. Las seis pruebas automáticas de backend aprobaron sin fallos ni omisiones. El frontend pasó el análisis estático y la compilación de producción. El flujo integral confirmó registro, inicio de sesión, publicación, comentario, reacción, búsqueda y actualización del panel. Se documentaron ocho capturas reales de la interfaz. El análisis se organiza en torno a adecuación funcional, fiabilidad, usabilidad, seguridad y mantenibilidad, con referencia al modelo ISO/IEC 25010:2023. Los resultados apoyan el uso académico del sistema dentro del alcance probado, pero no sustituyen pruebas con usuarios externos, accesibilidad asistida, concurrencia ni una auditoría especializada de seguridad.

**Palabras clave:** calidad de software, pruebas funcionales, experiencia de usuario, validación, ISO 25010, red social universitaria.

## Introducción

La calidad de una aplicación no depende sólo de que las pantallas sean atractivas. En una plataforma social, cada flujo debe permitir completar una tarea y conservar sus resultados. Los perfiles, el contenido creado por usuarios y las interacciones son elementos habituales de una red social [1]. CampusConecta adapta estos elementos al entorno universitario con perfiles por rol, publicaciones, comentarios, reacciones y comunidades.

La evaluación se concentró en hechos observables. No se aceptó una función porque apareciera como un botón. Para considerarla verificada, debía enviar una solicitud válida, obtener una respuesta coherente, persistir en PostgreSQL cuando correspondiera y reflejarse en una vista posterior. Este criterio evita confundir una maqueta con una implementación funcional.

ISO/IEC 25010:2023 proporciona un modelo de calidad para productos de software [2]. El presente trabajo lo usa como marco de análisis, no como certificación. Se seleccionaron características que podían contrastarse con las evidencias disponibles: adecuación funcional, fiabilidad, usabilidad, seguridad y mantenibilidad.

## Objetivo de evaluación

Determinar si CampusConecta satisface las funciones solicitadas y si los recorridos principales mantienen coherencia entre frontend, API y base de datos en un entorno local controlado.

Los criterios de aceptación fueron los siguientes:

- Una cuenta válida se registra e inicia sesión; una contraseña débil se rechaza.
- Un usuario autenticado consulta y actualiza su perfil.
- El feed recupera publicaciones persistidas y permite crear contenido.
- Los comentarios y las reacciones cambian la publicación y se recuperan en consultas posteriores.
- Las comunidades se crean, permiten unirse o salir y no duplican membresías.
- La búsqueda devuelve usuarios, publicaciones y comunidades.
- El panel personal calcula indicadores desde los datos persistidos.
- Las rutas protegidas rechazan solicitudes sin JWT.
- Los errores se presentan sin exponer detalles internos del servidor.

## Marco de calidad

| Característica | Pregunta de evaluación | Evidencia utilizada |
| --- | --- | --- |
| Adecuación funcional | ¿Cada requisito tiene un flujo ejecutable? | Casos de prueba, solicitudes HTTP y capturas. |
| Fiabilidad | ¿La API responde de manera consistente y conserva datos? | Pruebas de integración y PostgreSQL real. |
| Usabilidad | ¿El usuario reconoce el estado y puede completar tareas? | Revisión de ocho pantallas y formularios. |
| Seguridad | ¿Las rutas y operaciones respetan la identidad autenticada? | JWT, respuesta 401 y controles de propiedad. |
| Mantenibilidad | ¿La solución puede verificarse y modificarse? | Capas separadas, Swagger, migraciones y scripts. |

La adecuación funcional se evaluó contra las tareas exigidas. La fiabilidad se verificó ejecutando la API con datos iniciales y operaciones repetibles. La usabilidad se revisó a partir de jerarquía visual, etiquetas, mensajes y adaptación a una ventana estrecha. La seguridad se examinó en las decisiones implementadas, sin afirmar una auditoría exhaustiva. La mantenibilidad se relacionó con la organización por capas, las pruebas y las instrucciones de ejecución.

## Estrategia de pruebas

La estrategia combinó cuatro niveles complementarios.

| Nivel | Técnica | Propósito |
| --- | --- | --- |
| Integración de API | xUnit y WebApplicationFactory | Validar contratos, reglas y autorización de manera automática. |
| Calidad del frontend | Oxlint, TypeScript y Vite | Detectar errores estáticos y generar el build de producción. |
| Extremo a extremo | Solicitudes HTTP contra PostgreSQL real | Confirmar persistencia y lectura desde distintos recorridos. |
| Revisión visual | Navegador y capturas | Comprobar composición, mensajes, navegación y presentación de datos. |

Las pruebas de integración se ejecutaron con una fábrica de aplicación y una base en memoria para repetir reglas de la API sin depender del navegador. La prueba integral se realizó después con PostgreSQL real. Esta diferencia es intencional: las primeras pruebas aíslan reglas y contratos; la segunda confirma que la integración con el motor relacional funciona.

La revisión visual registró inicio del sistema, inicio de sesión, feed, comunidades, panel personal, edición de perfil, búsqueda y Swagger. Las capturas no sustituyen las pruebas automáticas, pero ayudan a identificar una pantalla correcta en código que sea confusa o se descomponga al mostrar datos reales.

## Resultados de las pruebas automáticas

La suite del backend ejecutó seis casos y terminó con seis pruebas aprobadas, cero fallidas y cero omitidas.

| Caso | Resultado observado | Estado |
| --- | --- | --- |
| Salud | `GET /health` respondió `healthy`. | Aprobado |
| Contraseña | El hash de la cuenta inicial se verificó sin almacenar texto plano. | Aprobado |
| Registro | Se rechazó una contraseña débil y se aceptó un usuario válido con JWT. | Aprobado |
| Contenido | Feed, publicación, comentario, reacción y panel funcionaron. | Aprobado |
| Comunidad y perfil | Creación, búsqueda, membresía y edición respondieron correctamente. | Aprobado |
| Autorización | El panel sin token devolvió 401. | Aprobado |

El caso de registro validó dos rutas opuestas: una solicitud con contraseña débil recibió 400 y una solicitud válida recibió 201 con token. El caso de contenido inició sesión, consultó el feed, creó una publicación, añadió un comentario, estableció una reacción y verificó los indicadores del panel. El caso de comunidades creó un grupo, realizó una búsqueda y actualizó el perfil. Estas pruebas cubren los recorridos con mayor número de dependencias.

## Resultados del frontend y de persistencia

El análisis estático del frontend terminó sin advertencias. La compilación de producción fue correcta y generó 238.04 kB de JavaScript y 13.87 kB de CSS antes de compresión; el tamaño gzip fue de 73.35 kB y 3.83 kB, respectivamente. Este dato describe el paquete generado, no una medición de experiencia de usuario en red móvil.

El flujo integral comenzó con inicio de sesión en la API conectada a PostgreSQL real. Luego se creó una publicación de aceptación, un comentario y una reacción `Insightful`. Una búsqueda posterior recuperó la publicación y el panel mostró 2 publicaciones propias, 1 comentario, 2 reacciones recibidas y 1 comunidad. La base quedó con 3 usuarios, 3 publicaciones, 2 comentarios, 3 reacciones, 1 comunidad y 3 membresías. La migración de Entity Framework Core se había aplicado antes de las operaciones.

La secuencia demuestra que los cambios no quedaron sólo en el estado de React. La misma publicación se observó en feed, búsqueda y panel después de viajar por la API y persistir en PostgreSQL.

## Evaluación de funcionalidades

### Autenticación y perfil

La interfaz separa registro e inicio de sesión. Los formularios identifican los campos necesarios y bloquean el envío cuando faltan datos básicos. La API aplica la validación definitiva y devuelve mensajes cuando el dato no es válido. Tras el inicio de sesión, la cabecera identifica la cuenta activa.

El perfil permite cambiar nombre visible, biografía, carrera o área y URL de avatar. La acción de guardar se diferencia de la navegación. El servidor no acepta que una cuenta modifique el perfil de otra porque el identificador se obtiene desde el token, no desde un campo enviado por la interfaz.

### Feed e interacciones

El feed organiza publicaciones en tarjetas con autor, fecha, contenido, comunidad cuando corresponde, reacciones y comentarios. La creación de una publicación actualiza la lista sin recargar la página completa. Los comentarios incluyen su autor y las reacciones usan un conjunto controlado de tipos. La gestión básica permite eliminar contenido propio; el backend vuelve a comprobar esa propiedad antes de aceptar la operación.

### Comunidades, búsqueda y panel

Las comunidades muestran nombre, descripción, creador, número de miembros y estado de membresía. Al crear una comunidad, el creador se incorpora como miembro. Las operaciones de unión y salida actualizan el estado visible y evitan duplicar una relación existente.

La búsqueda agrupa usuarios, publicaciones y comunidades en una sola respuesta. Esta decisión reduce pasos para encontrar una persona, un tema o un grupo. El panel personal presenta indicadores y actividad reciente obtenidos desde consultas persistentes; funciona como resumen de la participación de la cuenta autenticada.

## Usabilidad y accesibilidad

La revisión visual mostró consistencia de paleta, espaciado, cabecera y navegación en las ocho pantallas documentadas. Los formularios incluyen etiquetas y los botones usan acciones concretas. Durante la inspección en una ventana estrecha, no se observaron solapamientos ni contenido oculto. Los mensajes de error se originan en una capa común de la API, por lo que la interfaz puede presentarlos sin exponer excepciones internas.

La aplicación se revisó teniendo como referencia principios de WCAG 2.2 [3], especialmente identificación de campos, estructura visual y contraste. Esta revisión es parcial. No se ejecutó una auditoría con lector de pantalla, navegación exclusiva por teclado, pruebas automatizadas de contraste ni sesiones con personas usuarias. Por ello, no se declara conformidad formal con WCAG.

## Validación y manejo de errores

La validación se realiza en tres momentos. El cliente evita envíos evidentemente incompletos. Los contratos de entrada establecen formato y longitud. Los controladores aplican reglas que dependen de los datos existentes, como duplicados, recursos inexistentes, pertenencia y propiedad. El middleware global devuelve una estructura de error en vez de una página técnica.

| Condición | Respuesta esperada | Efecto para el usuario |
| --- | --- | --- |
| Contraseña débil | 400 Bad Request | No se crea la cuenta y se informa la regla. |
| Credenciales incorrectas | 401 Unauthorized | La sesión permanece cerrada. |
| JWT ausente | 401 Unauthorized | La ruta protegida no expone datos. |
| Recurso inexistente | 404 Not Found | Se indica que el recurso no fue encontrado. |
| Acción sobre contenido ajeno | 403 Forbidden | No se modifica el recurso. |
| Dato duplicado | 400 Bad Request | No se crea una segunda identidad o membresía. |

OWASP recomienda que las contraseñas se almacenen con algoritmos adaptativos y no en texto plano [4]. CampusConecta usa el componente de hash de ASP.NET Core. La autenticación JWT valida firma, emisor, audiencia y vencimiento antes de autorizar operaciones. Estas decisiones reducen riesgos comunes, aunque no reemplazan una revisión de seguridad especializada.

## Rendimiento exploratorio

Se enviaron veinte consultas consecutivas al feed desde el mismo equipo que alojaba la API. La media fue 5.40 ms, la mediana 4.73 ms, el percentil 95 9.58 ms, el mínimo 3.27 ms y el máximo 11.21 ms. No se observaron errores durante la muestra.

La medición sirve como referencia local para detectar regresiones. No incluye usuarios concurrentes, conexión de Internet, dispositivos móviles, volúmenes grandes ni tiempo de pintura del navegador. Por tanto, no permite afirmar cuántas personas soportaría el sistema. Una evaluación productiva debería definir escenarios, datos representativos y umbrales antes de ejecutar pruebas de carga.

## Hallazgos y mejoras

La evidencia cubre todas las funcionalidades mínimas solicitadas y muestra coherencia entre interfaz, API y persistencia. Durante el desarrollo se corrigieron advertencias de consultas complejas mediante división explícita de consultas y se ordenaron los resultados antes de limitar las búsquedas. También se separó una demora de resolución local IPv6 de la medición de servicio, por lo que los resultados finales se tomaron directamente sobre `127.0.0.1`.

| Prioridad | Mejora propuesta | Motivo |
| --- | --- | --- |
| Alta | Integrar OpenID Connect y secretos administrados. | Fortalece identidad y gestión de claves. |
| Alta | Ampliar pruebas negativas de autorización por recurso. | Aumenta la cobertura de permisos. |
| Media | Auditar WCAG 2.2 con teclado y lector de pantalla. | Convierte observaciones visuales en evidencia accesible. |
| Media | Ejecutar pruebas de carga con datos volumétricos. | Permite estimar capacidad y cuellos de botella. |
| Media | Agregar reportes, roles de moderación y auditoría. | Amplía la gestión básica de contenido. |
| Baja | Añadir notificaciones y recuperación de cuenta. | Mejora la experiencia de uso prolongado. |

## Limitaciones

La evaluación se realizó en un entorno local con datos de demostración y por el equipo desarrollador. No participaron usuarios externos ni se comparó el producto con otra plataforma. Las pruebas automáticas se enfocaron en recorridos críticos, por lo que no cubren todas las combinaciones de límites. Las capturas demuestran estados correctos, pero no prueban por sí solas accesibilidad completa ni ausencia de defectos en todos los dispositivos.

## Conclusiones

CampusConecta satisface los requisitos funcionales definidos para la entrega. Cada acción principal se vinculó con una respuesta HTTP, un cambio persistido y una representación visible. Las seis pruebas aprobadas, el build del frontend y el flujo contra PostgreSQL real reducen el riesgo de presentar una solución sólo visual.

La calidad observada es adecuada para una demostración académica y una evolución controlada. Antes de un uso institucional se deben completar pruebas con usuarios, accesibilidad, concurrencia y seguridad. El resultado más valioso del proceso fue convertir requisitos generales en criterios verificables y dejar evidencia, scripts y documentación que permiten repetir la evaluación.

## Referencias

1. boyd, d. m., y Ellison, N. B. (2007). *Social Network Sites: Definition, History, and Scholarship*. Journal of Computer-Mediated Communication, 13(1), 210–230. https://doi.org/10.1111/j.1083-6101.2007.00393.x
2. International Organization for Standardization. (2023). *ISO/IEC 25010:2023 Systems and software Quality Requirements and Evaluation, product quality model*. https://www.iso.org/standard/78176.html
3. World Wide Web Consortium. (2024). *Web Content Accessibility Guidelines 2.2*. https://www.w3.org/TR/WCAG22/
4. OWASP Foundation. (2026). *Password Storage Cheat Sheet*. https://cheatsheetseries.owasp.org/cheatsheets/Password_Storage_Cheat_Sheet.html
5. Microsoft. (2026). *ASP.NET Core web API documentation with Swagger and OpenAPI*. https://learn.microsoft.com/aspnet/core/tutorials/web-api-help-pages-using-swagger
6. React Team. (2026). *React documentation*. https://react.dev/learn
