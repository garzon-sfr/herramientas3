# Herramientas 3 — Ingeniería de Software

Este repositorio contiene las actividades, prácticas y proyectos desarrollados en la asignatura **Herramientas 3**, correspondiente al programa de **Ingeniería de Software** de la **Institución Universitaria Pascual Bravo**.

## Información general

- **Estudiante:** Andrés Felipe Ramírez Garzón
- **Institución:** Institución Universitaria Pascual Bravo
- **Programa:** Ingeniería de Software
- **Asignatura:** Herramientas 3
- **Docente:** Frain León Osorio Rivera

## Contenido del repositorio

En este repositorio se encuentran los talleres, actividades y entregables correspondientes al desarrollo de la asignatura.

---

## Entregas realizadas

**Primera entrega**: desarrollo de la capa de Dominio y del contexto de base de datos.

**Segunda entrega**: desarrollo de la capa Core, incluyendo las interfaces de repositorios y servicios, junto con la implementación de repositorios en Infraestructura.

**Entrega final — 8 de octubre de 2026**: presentación de la aplicación completa, integración de sus componentes, consulta y cálculo de días festivos, y disponibilidad de las operaciones mediante Swagger.

## Presentación del proyecto

**ApiFestivos** es una aplicación REST desarrollada en .NET 10 que permite administrar y consultar los días festivos de diferentes países, considerando las reglas establecidas para determinar sus fechas.

La aplicación permite registrar países, tipos de festivos y días festivos, consultar el calendario anual y verificar si una fecha específica corresponde a un día festivo.

## Contenido de la entrega

El proyecto comprende las siguientes capas:

- **Dominio:** entidades y relaciones del sistema.
- **Core:** interfaces de repositorios y servicios.
- **Infraestructura:** persistencia y acceso a la base de datos.
- **Aplicación:** implementación de la lógica de negocio y cálculo de festivos.
- **Presentación:** API REST disponible para su ejecución y comprobación mediante Swagger.

También se incluye el **script SQL de la base de datos Festivos**, que permite preparar la base de datos en otro equipo.

## Ejecución local

**Requisitos:** .NET SDK 10 y SQL Server.

La solución se encuentra en `ApiFestivos/ApiFestivos.sln`.

Desde la carpeta raíz del proyecto, ejecutar:

```powershell
dotnet restore .\ApiFestivos\ApiFestivos.sln
dotnet build .\ApiFestivos\ApiFestivos.sln
dotnet run --project .\ApiFestivos\ApiFestivos.csproj --launch-profile https
```

Una vez iniciada la aplicación, acceder a Swagger:

**https://localhost:7041/swagger/index.html**

También está disponible el perfil HTTP:

**http://localhost:5148/swagger/index.html**

Antes de ejecutar, importar el script SQL incluido en el repositorio y configurar la conexión a SQL Server en `ApiFestivos/appsettings.json`, de acuerdo con el servidor local.

## Funcionalidades y endpoints

| Recurso | Endpoint principal | Funcionalidad |
|---|---|---|
| Países | `/api/paises` | Consultar, registrar, modificar y eliminar países |
| Tipos de festivos | `/api/tiposfestivo` | Administrar los tipos de festivos |
| Festivos | `/api/festivos` | Administrar las definiciones de festivos |
| Festivos por país | `/api/festivos/pais/{IdPais}` | Consultar festivos de un país |
| Calendario anual | `/api/calendario/festivos/{IdPais}/{Año}` | Obtener los festivos de un año |
| Verificación de fecha | `/api/calendario/verificar/{IdPais}/{Año}/{Mes}/{Dia}` | Determinar si una fecha es festiva |

La aplicación dispone de operaciones **GET, POST, PUT y DELETE**, según el recurso, que pueden probarse directamente desde Swagger.

## Reglas de cálculo de festivos

El sistema contempla cuatro modalidades:

1. **Fecha fija:** conserva el día y mes establecidos.
2. **Fecha trasladable:** traslada el festivo al lunes siguiente, si corresponde.
3. **Basado en Pascua:** calcula la fecha a partir del Domingo de Pascua.
4. **Basado en Pascua trasladable:** calcula la fecha respecto a Pascua y la traslada al lunes correspondiente.

Estas reglas permiten generar calendarios de festivos y verificar fechas de acuerdo con el país seleccionado.

## Entregas realizadas

**Primera entrega:** desarrollo de la capa de Dominio y contexto de base de datos.

**Segunda entrega:** desarrollo de Core, interfaces de repositorios y servicios, e implementación de repositorios en Infraestructura.

**Entrega final — 8 de octubre de 2026:** integración de la aplicación, implementación de servicios, controladores, cálculo de festivos y presentación funcional mediante Swagger.


---

**Institución Universitaria Pascual Bravo**  
**Ingeniería de Software — Herramientas 3**  
**Octubre de 2026**