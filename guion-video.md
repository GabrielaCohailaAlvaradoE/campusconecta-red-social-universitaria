# Guion del video conjunto de CampusConecta

**Duración objetivo: 4 minutos 40 segundos. Límite: 5 minutos.**

**Participantes:** Gabriela Estefania Cohaila Alvarado (2022075746) y Victoria Isabel Lavarello Vidaurre (2023077281).

El video muestra una ejecución real de CampusConecta. La columna **Pantalla y acción** indica qué mostrar y no debe leerse. Ensayar una vez con cronómetro y mantener un margen de al menos veinte segundos. Si se muestra una prueba ya terminada, decirlo explícitamente; no simular que se está ejecutando en ese momento.

## Preparación antes de grabar

1. Ejecutar `instalar.ps1` y `iniciar.ps1` desde `codigo_fuente`. Verificar la web en `http://localhost:5173` y Swagger en `http://localhost:5050/swagger`.
2. Iniciar sesión con `gabriela@campus.test` y la contraseña local indicada en `codigo_fuente/README.md`.
3. Abrir cuatro ventanas o pestañas: CampusConecta, Swagger, `evidencias/10_resultados_pruebas.txt` y `evidencias/09_base_datos_postgresql.txt`.
4. Dejar listo un texto breve para publicar: `Demostración de CampusConecta para validar publicación, comentario, reacción, búsqueda y panel.`
5. Cerrar notificaciones y aumentar el zoom si los textos no se leen. Grabar en 1920 × 1080 si está disponible.

## Diálogo y secuencia de pantalla

| Tiempo | Persona | Pantalla y acción | Diálogo exacto |
| --- | --- | --- | --- |
| 0:00–0:25 | Gabriela | Mostrar la página de acceso y ambos nombres en una portada sencilla. | «Hola. Somos Gabriela Estefania Cohaila Alvarado y Victoria Isabel Lavarello Vidaurre. Presentamos CampusConecta, una red social universitaria funcional para estudiantes, docentes y personal. La solución usa React, ASP.NET Core y PostgreSQL.» |
| 0:25–0:55 | Gabriela | Iniciar sesión y abrir el feed. | «La plataforma permite registrarse e iniciar sesión. Después del acceso, el usuario visualiza su feed y puede crear publicaciones, comentar y reaccionar. La API protege estas acciones mediante un token JWT.» |
| 0:55–1:30 | Gabriela | Crear la publicación preparada, añadir un comentario breve y seleccionar una reacción. | «Esta publicación se crea desde la interfaz y se envía a la API. Ahora añadimos un comentario y una reacción. Estas acciones no quedan sólo en pantalla: se guardan en PostgreSQL y luego aparecen en las demás consultas del sistema.» |
| 1:30–2:00 | Gabriela | Abrir Swagger; señalar Auth, Posts, Communities, Search y Dashboard. | «El backend es una API REST documentada con Swagger. Aquí se pueden revisar y probar los endpoints de autenticación, perfiles, publicaciones, comunidades, búsqueda y panel. La API valida datos y devuelve respuestas como 400, 401, 403 o 404 según corresponda.» |
| 2:00–2:35 | Victoria | Abrir Comunidades; mostrar una comunidad y el estado de membresía. | «Las comunidades permiten organizar intereses académicos. Un usuario puede crear una comunidad, unirse o salir. El backend controla que una membresía no se duplique y que las operaciones se realicen con una identidad autenticada.» |
| 2:35–3:00 | Victoria | Usar Búsqueda con una palabra de la publicación creada. | «La búsqueda consulta usuarios, publicaciones y comunidades en un mismo flujo. Buscamos una palabra de la publicación recién creada y comprobamos que el resultado fue recuperado desde la base de datos.» |
| 3:00–3:25 | Victoria | Abrir Mi perfil y luego Mi panel. Señalar los indicadores. | «El perfil permite editar nombre, biografía y carrera o área. El panel personal resume publicaciones, comentarios, reacciones recibidas, comunidades y actividad reciente. Sus valores se calculan desde datos persistidos.» |
| 3:25–4:05 | Victoria | Mostrar `10_resultados_pruebas.txt` y `09_base_datos_postgresql.txt`. | «La verificación automática ejecutó seis pruebas de integración: las seis aprobaron y no hubo fallos ni omisiones. También se validó el frontend sin advertencias y se ejecutó el flujo contra PostgreSQL real. La evidencia registra usuarios, publicaciones, comentarios, reacciones, comunidades y membresías persistidas.» |
| 4:05–4:25 | Gabriela | Mostrar Swagger o el feed actualizado. | «Como referencia local, veinte consultas consecutivas al feed tuvieron una media de 5.40 milisegundos y un percentil 95 de 9.58 milisegundos. Esta medición no es una prueba de carga; sólo documenta el comportamiento del entorno de desarrollo.» |
| 4:25–4:40 | Ambas | Volver al feed y mostrar una pantalla final con ambos nombres. | Gabriela: «CampusConecta integra interfaz, API, seguridad y persistencia.» Victoria: «La entrega incluye código, pruebas, evidencias y dos artículos individuales. Gracias.» |

## Material de respaldo que se puede mostrar

- Web: `http://localhost:5173`
- Swagger: `http://localhost:5050/swagger`
- Pruebas: `evidencias/10_resultados_pruebas.txt`
- PostgreSQL: `evidencias/09_base_datos_postgresql.txt`
- Artículo de Gabriela: `articulo_Gabriela_Cohaila/articulo_Gabriela_Cohaila.md`
- Artículo de Victoria: `articulo_Victoria_Lavarello/articulo_Victoria_Lavarello.md`

## Texto para publicar el video

**Título:** Demostración de CampusConecta, red social universitaria con ASP.NET Core, React y PostgreSQL

**Descripción:**

Trabajo desarrollado por Gabriela Estefania Cohaila Alvarado (2022075746) y Victoria Isabel Lavarello Vidaurre (2023077281).

El video demuestra CampusConecta, una plataforma web de red social universitaria con registro, inicio de sesión, perfiles, publicaciones, comentarios, reacciones, comunidades, búsqueda, panel personal, API REST documentada con Swagger, JWT y PostgreSQL.

La entrega incluye dos artículos individuales, código fuente, evidencias y resultados de pruebas. Publicar con visibilidad pública o con la visibilidad que indique el curso; verificar que la duración final sea menor de 5:00 antes de compartir el enlace.
