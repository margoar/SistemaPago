# SistemaPago — Patrón Strategy en ASP.NET Core

Aplicación web mínima (ASP.NET Core 2.0, Razor Pages) que sirve como ejemplo didáctico
del **patrón de diseño Strategy** aplicado a un sistema de pagos.

## ¿Qué problema resuelve?

Un sistema de pagos necesita cobrar de varias formas: tarjeta, PayPal, transferencia,
criptomonedas... La solución ingenua es un `if/else` o un `switch` gigante:

```csharp
// ❌ Sin Strategy: el método crece cada vez que aparece un medio de pago nuevo
public string ProcesarPago(decimal monto, string tipo)
{
    if (tipo == "tarjeta")      return $"Pago de ${monto} con tarjeta.";
    else if (tipo == "paypal")  return $"Pago de ${monto} con PayPal";
    else if (tipo == "cripto")  return ...;   // hay que EDITAR esta clase otra vez
}
```

Ese código viola el **principio abierto/cerrado**: para agregar un comportamiento hay que
modificar una clase que ya funcionaba (y volver a probarla toda).

**Strategy** propone lo contrario: encapsular cada algoritmo en su propia clase, hacer que
todas cumplan un mismo contrato, y que el código cliente reciba la que necesite sin
saber cuál es.

## Los actores del patrón en este proyecto

| Rol en el patrón | Clase en el proyecto | Qué hace |
|---|---|---|
| **Strategy** (contrato) | [IPaymentStrategy.cs](SistemaPago/Strategies/IPaymentStrategy.cs) | Declara `string Pagar(decimal monto)` |
| **Concrete Strategy** | [TarjetaPaymentStrategy.cs](SistemaPago/Strategies/TarjetaPaymentStrategy.cs) | Implementa el cobro con tarjeta |
| **Concrete Strategy** | [PayPalPaymentStrategy.cs](SistemaPago/Strategies/PayPalPaymentStrategy.cs) | Implementa el cobro con PayPal |
| **Context** | [PaymentService.cs](SistemaPago/Strategies/PaymentService.cs) | Usa una estrategia sin saber cuál es |
| **Cliente / configuración** | [Startup.cs](SistemaPago/Startup.cs) | Decide qué estrategia se inyecta |
| **Interfaz de usuario** | [Index.cshtml.cs](SistemaPago/Pages/Index.cshtml.cs) | Botón que dispara el pago y muestra el resultado |

## Cómo fluye una petición

```
Index.cshtml  (botón "Pagar $100.000")
      │  POST
      ▼
IndexModel.OnPost()                 ← recibe PaymentService por constructor
      │  _paymentService.ProcesarPago(100000)
      ▼
PaymentService.ProcesarPago()       ← el CONTEXTO: sólo conoce IPaymentStrategy
      │  _paymentStrategy.Pagar(monto)
      ▼
PayPalPaymentStrategy.Pagar()       ← la estrategia concreta que Startup registró
      │
      ▼
"Pago de $100000 realizado con PayPal"
```

Lo importante: **`PaymentService` nunca menciona PayPal ni tarjeta**. Sólo depende de la
interfaz. Quién es la implementación real lo decide el contenedor de dependencias.

### 1. El contrato

```csharp
public interface IPaymentStrategy
{
    string Pagar(decimal monto);
}
```

### 2. Las estrategias concretas

```csharp
public class TarjetaPaymentStrategy : IPaymentStrategy
{
    public string Pagar(decimal monto) => $"Pago de ${monto} realizado con tarjeta.";
}

public class PayPalPaymentStrategy : IPaymentStrategy
{
    public string Pagar(decimal monto) => $"Pago de ${monto} realizado con PayPal";
}
```

### 3. El contexto

```csharp
public class PaymentService
{
    private readonly IPaymentStrategy _paymentStrategy;

    public PaymentService(IPaymentStrategy paymentStrategy)   // inyección por constructor
    {
        _paymentStrategy = paymentStrategy;
    }

    public string ProcesarPago(decimal monto) => _paymentStrategy.Pagar(monto);
}
```

### 4. La elección de la estrategia

En `Startup.ConfigureServices` se registra cuál implementación resuelve `IPaymentStrategy`:

```csharp
//services.AddScoped<IPaymentStrategy, TarjetaPaymentStrategy>();
services.AddScoped<IPaymentStrategy, PayPalPaymentStrategy>();
services.AddScoped<PaymentService>();
```

**Para cambiar el medio de pago de toda la aplicación basta con mover el comentario de una
línea a la otra.** Ninguna otra clase se toca. Eso es exactamente lo que el patrón busca.

## Cómo agregar un medio de pago nuevo

1. Crear `SistemaPago/Strategies/TransferenciaPaymentStrategy.cs`:

   ```csharp
   namespace SistemaPago.Strategies
   {
       public class TransferenciaPaymentStrategy : IPaymentStrategy
       {
           public string Pagar(decimal monto)
               => $"Pago de ${monto} realizado por transferencia bancaria.";
       }
   }
   ```

2. Registrarla en `Startup.ConfigureServices`:

   ```csharp
   services.AddScoped<IPaymentStrategy, TransferenciaPaymentStrategy>();
   ```

No se modificó `PaymentService`, ni `IndexModel`, ni las estrategias existentes:
**abierto a la extensión, cerrado a la modificación**.

## Ejecutar el proyecto

Requiere el SDK de .NET Core 2.0 (o Visual Studio 2017+ con la carga de trabajo de ASP.NET).

```bash
dotnet restore
dotnet run --project SistemaPago/SistemaPago.csproj
```

Luego abrir la URL que indique la consola (por defecto `http://localhost:5000`) y presionar
el botón **Pagar $100.000**. El mensaje que aparece revela qué estrategia está activa.

## Estructura del repositorio

```
SistemaPago/
├── Pages/
│   ├── Index.cshtml           formulario con el botón de pago
│   └── Index.cshtml.cs        cliente: pide PaymentService y muestra el mensaje
├── Strategies/
│   ├── IPaymentStrategy.cs        contrato común
│   ├── TarjetaPaymentStrategy.cs  estrategia concreta
│   ├── PayPalPaymentStrategy.cs   estrategia concreta
│   └── PaymentService.cs          contexto
├── Startup.cs                 registro de la estrategia activa (DI)
└── Program.cs                 arranque del host web
```

## Ideas para seguir practicando

- **Elegir la estrategia en tiempo de ejecución**: que el usuario seleccione el medio de
  pago en un `<select>` y una fábrica (`Func<string, IPaymentStrategy>` o un
  `IEnumerable<IPaymentStrategy>` inyectado) devuelva la correspondiente.
- **Datos reales**: que `Pagar` reciba un objeto `Pago` (monto, moneda, cliente) en vez de
  un `decimal` suelto, y devuelva un resultado con estado y número de transacción.
- **Pruebas unitarias**: `PaymentService` es trivial de testear porque se le puede pasar
  una estrategia falsa (*mock*) — otra ventaja directa del patrón.
