# SistemaPago — Patrones de diseño aplicados a pagos

Aplicación web mínima (ASP.NET Core 2.0, Razor Pages) que sirve como ejemplo didáctico
de **patrones de diseño** aplicados a un mismo problema: un sistema que necesita cobrar de
varias formas (tarjeta, PayPal, transferencia...).

La idea es resolver ese problema con distintos patrones y comparar qué aporta cada uno.
**Cada patrón vive en su propia rama**, con su propio código y su propio README que lo
explica en detalle.

## Índice de ramas

| Rama | Patrón | Carpeta | Estado | Qué resuelve |
|---|---|---|---|---|
| [`strategy`](https://github.com/margoar/SistemaPago/tree/strategy) | Strategy | `SistemaPago/Strategies/` | ✅ Completo | **Cómo se ejecuta** el cobro: cada medio de pago es un algoritmo intercambiable detrás de una misma interfaz |
| [`factory`](https://github.com/margoar/SistemaPago/tree/factory) | Factory | `SistemaPago/Payments/` | 🚧 En progreso | **Quién decide y construye** el medio de pago según lo que elige el usuario, centralizando los `new` en una sola clase |

> La rama `main` es la portada del repositorio. Su código corresponde al punto de partida
> (la implementación con Strategy), pero la explicación de cada patrón está en su rama.

## Cómo explorar un patrón

Clonar el repositorio y cambiar a la rama del patrón que se quiere estudiar:

```bash
git clone https://github.com/margoar/SistemaPago.git
cd SistemaPago

git checkout strategy   # o: git checkout factory
```

Luego leer el `README.md` de esa rama, que explica el problema, los actores del patrón,
el flujo de una petición y cómo extenderlo.

## Strategy vs. Factory en una frase

Se complementan y por eso suelen confundirse:

- **Strategy** → define *qué hace* cada medio de pago y permite intercambiarlos sin tocar
  el código que los usa.
- **Factory** → decide *cuál* medio de pago crear a partir de un dato de entrada, para que
  el resto del sistema nunca conozca las clases concretas.

## Ejecutar el proyecto

Requiere el SDK de .NET Core 2.0 (o Visual Studio con la carga de trabajo de ASP.NET).

```bash
dotnet restore
dotnet run --project SistemaPago/SistemaPago.csproj
```

Luego abrir la URL que indique la consola (por defecto `http://localhost:5000`).

## Estructura común

Todas las ramas comparten la misma base; lo que cambia es la carpeta del patrón.

```
SistemaPago/
├── <carpeta del patrón>/      Strategies/ o Payments/, según la rama
├── Pages/
│   ├── Index.cshtml           formulario de pago
│   └── Index.cshtml.cs        cliente que usa el patrón
├── Startup.cs                 configuración y registro de servicios (DI)
└── Program.cs                 arranque del host web
```
