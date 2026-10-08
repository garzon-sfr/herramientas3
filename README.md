# Herramientas 3 — Ingeniería de Software

Este repositorio contiene las actividades, prácticas y proyectos desarrollados en la asignatura **Herramientas 3**, correspondiente al programa de **Ingeniería de Software** de la **Institución Universitaria Pascual Bravo**.

## 📌 Información General

* **Estudiante** Andres Felipe Ramirez Gazron
* **Institución:** Institución Universitaria Pascual Bravo
* **Programa:** Ingeniería de Software
* **Asignatura:** Herramientas 3
* **Docente:** Frain León Osorio Rivera

## 📂 Contenido del Repositorio

Aquí encontrarás los talleres, entregables y código fuente organizados según el avance del plan formativo del curso.

## ApiFestivos - entrega de aplicación y presentación

API REST en .NET 10. La solución permanece en `ApiFestivos/ApiFestivos.sln`.

### Ejecución local

Requisitos: SDK de .NET 10 y acceso a SQL Server desde la cuenta de Windows.
Desde la raíz `Evaluacion1`, ejecutar en PowerShell:

```powershell
dotnet restore .\ApiFestivos\ApiFestivos.sln
dotnet build .\ApiFestivos\ApiFestivos.sln --no-restore
dotnet test .\ApiFestivos\ApiFestivos.sln --no-build --no-restore
dotnet run --project .\ApiFestivos\ApiFestivos.csproj --launch-profile https
```

Swagger: https://localhost:7041/swagger/index.html

Si falta la confianza del certificado de desarrollo, ejecutar una vez en la sesión
normal de Windows `dotnet dev-certs https --trust`. También se puede iniciar con
`--launch-profile http` y abrir http://localhost:5148/swagger/index.html.

Los puertos quedan guardados en el proyecto. `ApiFestivos/appsettings.json`, en
`Servidor:UrlPredeterminada`, establece `http://localhost:5148` para iniciar sin un
perfil. `ApiFestivos/Properties/launchSettings.json` mantiene HTTP 5148 y HTTPS 7041
en `applicationUrl` de los perfiles respectivos. Para cambiar permanentemente un
puerto, actualizar esos valores y reiniciar la API; Postman debe usar el mismo puerto.
No es necesario pasar `--urls` al ejecutar.

### Base de datos existente

La conexión en `ApiFestivos/appsettings.json` usa:

```text
Server=GARZON;Database=Festivos;Integrated Security=True;Encrypt=True;TrustServerCertificate=True
```

Es la instancia y autenticación confirmadas por el propietario del proyecto.
`TrustServerCertificate=True` corresponde a este servidor local. En un despliegue
con certificado válido, utilizar `TrustServerCertificate=False`. No hay contraseñas
en el repositorio. En otro equipo puede sobrescribirse la conexión con la variable
de entorno `ConnectionStrings__Festivos`.

La API **no recrea la base ni ejecuta migraciones o semillas al iniciar**.
`database/Inspeccionar.sql` permite revisar las tablas y datos. Si faltan las tablas
o los datos iniciales, revisar y ejecutar `database/PrepararFestivos.sql` desde SSMS
sobre la base **Festivos**: agrega lo que falta sin eliminar ni actualizar registros.
No es necesario ejecutarlo si el esquema y los datos ya están completos.

El esquema compartido desde SSMS confirma `dbo.Pais`, `dbo.TipoFestivo` y
`dbo.Festivo`. Sus columnas, salvo Id, admiten NULL; el modelo EF se adaptó a esa
nulabilidad manteniendo los nombres y las clases. Los contratos de escritura de la
API exigen nombres y relaciones válidos. Para los cálculos, un valor nulo en los
campos opcionales de fecha se interpreta como 0; una fecha fija incompleta se
rechaza con un mensaje de validación. Los Id se consideran autogenerados.
No se hacen cambios automáticos sobre tablas preexistentes.

Los datos compartidos ya incluyen los 19 registros de Colombia (Id=1), otros 20
países y un tipo 5 usado por Ecuador. No es necesario cargar de nuevo Colombia.
El usuario confirmó que todavía no se ha definido la fórmula del tipo 5.

### Endpoints

| Método | Ruta | Resultado |
| --- | --- | --- |
| GET / POST | `/api/paises` | Listar / crear países |
| GET / PUT / DELETE | `/api/paises/{Id}` | Consultar / modificar / eliminar |
| GET / POST | `/api/tiposfestivo` | Listar / crear tipos |
| GET / PUT / DELETE | `/api/tiposfestivo/{Id}` | Consultar / modificar / eliminar |
| GET / POST | `/api/festivos` | Listar / crear definiciones de festivos |
| GET / PUT / DELETE | `/api/festivos/{Id}` | Consultar / modificar / eliminar |
| GET | `/api/{recurso}/buscar/1/{Texto}` | Buscar por Nombre, o Tipo en tiposfestivo |
| GET | `/api/festivos/pais/{IdPais}` | Definiciones de un país |
| GET | `/api/calendario/festivos/{IdPais}/{Año}` | Listado anual ordenado, con festivo y fecha |
| GET | `/api/calendario/verificar/{IdPais}/{Año}/{Mes}/{Dia}` | Es Festivo / No es festivo |

POST devuelve 201 y la ubicación del registro; DELETE devuelve 204; los registros
inexistentes devuelven 404; los datos inválidos, 400; los conflictos de integridad,
409. Los errores de acceso a SQL Server se informan sin devolver detalles internos.
La fecha inválida devuelve HTTP 400 con título `Fecha No valida`, en lugar del 200
de la captura del enunciado. Los cuerpos POST/PUT no contienen Id ni navegaciones.

Ejemplo de festivo fijo:

```json
{ "idPais": 1, "nombre": "Festivo de ejemplo", "dia": 15, "mes": 5, "diasPascua": 0, "idTipo": 1 }
```

Ejemplo de festivo basado en Pascua:

```json
{ "idPais": 1, "nombre": "Jueves Santo", "dia": 0, "mes": 0, "diasPascua": -3, "idTipo": 3 }
```

`ApiFestivos/ApiFestivos.http` contiene las consultas del enunciado.

### Reglas de cálculo

1. Tipo 1: fecha fija.
2. Tipo 2: siguiente lunes; si ya es lunes, se conserva.
3. Tipo 3: domingo de Pascua más DiasPascua.
4. Tipo 4: cálculo anterior y traslado al lunes.

Se conserva la fórmula académica facilitada por el docente; no se sustituye por otro
algoritmo de Pascua. Su aceptación de años 1..9999 es un límite técnico, no una
garantía de exactitud histórica de esa fórmula para todos los siglos.
Los catálogos y las definiciones permiten gestionar también el tipo 5 existente
(`Ley Puente Festivo Viernes`). Los únicos IdTipo con algoritmo definido son 1..4.
Para un calendario o verificación que dependa del tipo 5 u otro tipo adicional, la
API devuelve HTTP 409 con `Regla de cálculo pendiente`, sin inventar fechas ni
omitir celebraciones. Definir la regla del tipo 5 con el docente queda pendiente;
Colombia funciona con sus cuatro tipos definidos. Los registros asociados impiden
eliminar un país o tipo. La lista conserva celebraciones que coinciden y contempla
traslados entre años y el 29 de febrero.

Los datos del PDF son 19 registros para Colombia, incluido Domingo de Pascua.
Se conservan sus desplazamientos 40, 61 y 68 para Ascensión, Corpus Christi y
Sagrado Corazón. Casos de comprobación:

```text
GET /api/calendario/verificar/1/2023/6/12  -> Es Festivo
GET /api/calendario/verificar/1/2023/2/28  -> No es festivo
GET /api/calendario/verificar/1/2023/2/35  -> HTTP 400, Fecha No valida
GET /api/calendario/festivos/1/2023       -> 19 entradas con los datos del PDF
```

### Validación y entrega

Validación realizada: 49 pruebas correctas, 0 errores y 0 advertencias de compilación; auditoría sin paquetes vulnerables reportados.

Las pruebas usan EF Core con SQLite en memoria y no modifican `GARZON/Festivos`.
Comprueban los cuatro modos, el ejemplo de Pascua de 1999, el calendario completo
de 2023, fechas inválidas, bisiestos, cruces de año, relaciones, CRUD y Swagger.
El proveedor de producción sigue siendo SQL Server.

Auditoría de dependencias directas y transitivas:

```powershell
dotnet list .\ApiFestivos\ApiFestivos.sln package --vulnerable --include-transitive
```

El entorno aislado de Codex no pudo usar la autenticación integrada ni la clave
privada del certificado HTTPS de Windows. La compilación y las pruebas aisladas
no sustituyen la comprobación final contra la base existente desde Visual Studio
o PowerShell de la sesión del usuario. Las columnas y los primeros registros se
contrastaron con la salida de SSMS compartida por el propietario. Probar la conexión
y las consultas anteriores en Swagger antes de entregar.

Pendiente por decisión del propietario: revisar el diff, autorizar commit/push y
enviar al docente el enlace del repositorio con los integrantes. No se realizan
commits, push ni envío de correo automáticamente.
