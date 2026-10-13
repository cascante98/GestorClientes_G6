# Práctica programada 1 – Gestor de Clientes (SC-701 Programación Avanzada)

## 1. Integrantes finales del grupo
* Hazel Aguilar Quirós
* Jovan Ortiz Blanco 
* Oscar Cascante Diaz
* Kevin Mena Chinchilla

## 2. Repositorio
N/A

## 3. Especificación básica del proyecto

### a. Arquitectura (por capas, estilo Clean Architecture)
| Proyecto | Tipo | Responsabilidad |
|---|---|---|
| `GestorClientes.Domain` | Class Library | Entidades `Cliente` y `Telefono` y sus reglas de negocio. Sin dependencias. |
| `GestorClientes.Infrastructure` | Class Library | **Capa de acceso a datos**: `ApplicationDbContext` (EF Core + SQLite) y repositorios. |
| `GestorClientes.Application` | Class Library | **Capa lógica de negocio**: servicios, DTOs, `Respuesta<T>` y mapeos AutoMapper. |
| `GestorClientes` | ASP.NET Core MVC | Presentación: controlador, vistas Razor, JavaScript (jQuery + DataTables). |

Funcionalidades: listado de clientes con sus teléfonos, ver detalle, registrar, modificar y borrar (elimina también los teléfonos ligados).

Ejecución: `dotnet run --project GestorClientes`. La base de datos SQLite (`clientes.db`) se crea sola al iniciar.

### b. Paquetes NuGet
- `AutoMapper` 16.2.0
- `Microsoft.EntityFrameworkCore` 10.0.12
- `Microsoft.EntityFrameworkCore.Sqlite` 10.0.12

### c. Principios SOLID y patrones de diseño
- **S – Responsabilidad única**: controlador (HTTP), servicio (negocio), repositorio (datos), entidad (reglas del dominio).
- **O – Abierto/cerrado**: nuevas reglas se agregan como métodos de validación en la entidad sin cambiar los consumidores.
- **L – Sustitución de Liskov**: cualquier implementación de `IClienteRepository`/`IClienteServicio` es intercambiable.
- **I – Segregación de interfaces**: interfaces pequeñas y específicas por entidad.
- **D – Inversión de dependencias**: servicio y controlador dependen de interfaces, registradas en `Program.cs`.
- Patrones: **Repository**, **Dependency Injection**, **DTO**, **Result** (`Respuesta<T>`), **Mapper** (AutoMapper), **MVC**.
- Buenas prácticas: métodos asíncronos con `CancellationToken`, `AsNoTracking` en consultas, sin nulos en BD (valores por defecto), validación de cédula única.
