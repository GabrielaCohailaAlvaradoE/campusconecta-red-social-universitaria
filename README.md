# CampusConecta

Proyecto académico de una plataforma web de red social universitaria para estudiantes, docentes y personal universitario.

## Características

- Registro, inicio de sesión y autenticación JWT.
- Perfiles editables.
- Publicaciones, comentarios y reacciones.
- Comunidades, membresías y administración básica.
- Búsqueda de usuarios, publicaciones y comunidades.
- Feed y panel personal.
- API REST documentada con Swagger/OpenAPI.
- PostgreSQL, migraciones, pruebas automatizadas y evidencias reales.

## Inicio rápido

El código ejecutable se encuentra en [`codigo_fuente`](codigo_fuente). Desde esa carpeta:

```powershell
.\instalar.ps1
.\iniciar.ps1
```

Aplicación: `http://localhost:5173`  
Swagger: `http://localhost:5050/swagger`

## Enlaces públicos

- Aplicación: [campusconecta-upt-20261004.azurewebsites.net](https://campusconecta-upt-20261004.azurewebsites.net/)
- API documentada: [Swagger/OpenAPI](https://campusconecta-upt-20261004.azurewebsites.net/swagger/index.html)
- Automatización: [GitHub Actions](https://github.com/GabrielaCohailaAlvaradoE/campusconecta-red-social-universitaria/actions)

Para ejecutar pruebas:

```powershell
.\probar.ps1
```

## Entrega académica

- [Resolución técnica](RESOLUCION_COMPLETA.md)
- [Artículo de Gabriela](articulo_Gabriela_Cohaila/articulo_Gabriela_Cohaila.md)
- [Artículo de Victoria](articulo_Victoria_Lavarello/articulo_Victoria_Lavarello.md)
- [Guion de video](guion-video.md)
- [Evidencias](evidencias)

## Integrantes

- Gabriela Estefania Cohaila Alvarado — 2022075746
- Victoria Isabel Lavarello Vidaurre — 2023077281

Las claves y contraseñas incluidas en la configuración corresponden exclusivamente al entorno local de demostración. Para un despliegue real deben sustituirse por variables de entorno y un gestor de secretos.
