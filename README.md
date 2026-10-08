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
'''scrip SQL llamada Festivos para crear base de datos en propia maquina'''

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

Casos de comprobación:

```text
GET /api/calendario/verificar/1/2023/6/12  -> Es Festivo
GET /api/calendario/verificar/1/2023/2/28  -> No es festivo
GET /api/calendario/verificar/1/2023/2/35  -> HTTP 400, Fecha No valida
GET /api/calendario/festivos/1/2023       -> 19 entradas con los datos del PDF
```