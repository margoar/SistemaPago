# SistemaPago — Patrón Factory

Aplicación web mínima (ASP.NET Core 2.0, Razor Pages) que sirve como ejemplo didáctico
de patrones de diseño aplicados a un sistema de pagos.

**Este repositorio trabaja un patrón por rama, y cada patrón vive en su propia carpeta:**

| Rama | Carpeta | Patrón | Documentación |
|---|---|---|---|
| `strategy` | `SistemaPago/Strategies/` | Strategy | README de esa rama |
| `factory` ← *estás aquí* | `SistemaPago/Payments/` | Factory | este archivo |

---

## ¿Qué problema resuelve el Factory?

El sistema debe cobrar de varias formas (tarjeta, PayPal, transferencia) y **el medio de pago
lo elige el usuario en tiempo de ejecución**. Sin el patrón, cada parte del código que
necesita un medio de pago tiene que saber construirlo:

```csharp
// ❌ Sin Factory: el PageModel conoce TODAS las clases concretas
if (tipo == "tarjeta")            pago = new TarjetaPayment();
else if (tipo == "paypal")        pago = new PayPalPayment();
else if (tipo == "transferencia") pago = new TransferenciaPayment();
```

Ese bloque termina copiado en cada página, servicio o controlador que necesite cobrar. Si
mañana se agrega un medio de pago hay que buscar y editar **todas** las copias.

**Factory** propone concentrar esa decisión en un solo lugar: una clase cuya única
responsabilidad es *saber construir* el objeto correcto a partir de un dato de entrada. El
resto del sistema pide el objeto y recibe la interfaz, sin conocer jamás las clases concretas.

> **Strategy vs. Factory** — se complementan y por eso se confunden:
> Strategy resuelve **cómo se ejecuta** un algoritmo intercambiable;
> Factory resuelve **quién decide y construye** cuál instancia se usa.
> Aquí `IPayment` define el comportamiento y `PaymentFactory` decide la implementación.

## Los actores del patrón en este proyecto

| Rol en el patrón | Clase en el proyecto | Qué hace |
|---|---|---|
| **Product** (contrato) | [IPayment.cs](SistemaPago/Payments/IPayment.cs) | Declara `string Procesar(decimal monto)` |
| **Concrete Product** | [TarjetaPayment.cs](SistemaPago/Payments/TarjetaPayment.cs) | Cobro con tarjeta |
| **Concrete Product** | [PayPalPayment.cs](SistemaPago/Payments/PayPalPayment.cs) | Cobro con PayPal |
| **Concrete Product** | [TransferenciaPayment.cs](SistemaPago/Payments/TransferenciaPayment.cs) | Cobro por transferencia |
| **Factory** | [PaymentFactory.cs](SistemaPago/Payments/PaymentFactory.cs) | Recibe un `string` y devuelve el `IPayment` que corresponde |
| **Cliente** | [Index.cshtml.cs](SistemaPago/Pages/Index.cshtml.cs) | Pide el pago a la fábrica y muestra el resultado |

## Cómo fluye una petición

```
Index.cshtml  (el usuario elige el medio de pago y envía el formulario)
      │  POST  tipoPago = "paypal"
      ▼
IndexModel.OnPost()
      │  _factory.Create("paypal")   ← sólo conoce el string y la interfaz
      ▼
PaymentFactory.Create()             ← LA FÁBRICA: único lugar con los "new"
      │  return new PayPalPayment();
      ▼
IPayment.Procesar(100000)           ← se invoca a través de la INTERFAZ
      ▼
"Procesando pago de 100000 a través de PayPal."
```

Lo importante: **`IndexModel` nunca escribe `new PayPalPayment()`**. Recibe un `IPayment` y
llama a `Procesar`. Si mañana PayPal se reemplaza por otra clase, el cliente ni se entera.

### 1. El contrato (Product)

```csharp
namespace SistemaPago.Payments
{
    public interface IPayment
    {
        string Procesar(decimal monto);
    }
}
```

### 2. Los productos concretos

```csharp
public class TarjetaPayment : IPayment
{
    public string Procesar(decimal monto) => $"Pago de ${monto} procesado con tarjeta.";
}

public class PayPalPayment : IPayment
{
    public string Procesar(decimal monto) => $"Procesando pago de {monto} a través de PayPal.";
}

public class TransferenciaPayment : IPayment
{
    public string Procesar(decimal monto) => $"Pago de ${monto} procesado mediante transferencia.";
}
```

Las tres cumplen el mismo contrato, así que son intercambiables para quien las use.

### 3. La fábrica

```csharp
public class PaymentFactory
{
    public IPayment Create(string tipoPago)
    {
        switch (tipoPago)
        {
            case "tarjeta":       return new TarjetaPayment();
            case "paypal":        return new PayPalPayment();
            case "transferencia": return new TransferenciaPayment();

            default:
                throw new ArgumentException($"Tipo de pago no válido: {tipoPago}");
        }
    }
}
```

Sí, **el `switch` sigue existiendo** — pero ahora vive en *un solo* archivo, cuyo nombre dice
exactamente para qué sirve. Esa es la ganancia del patrón: no eliminar la decisión, sino
**centralizarla**. Nótese también el `default`, que convierte un tipo desconocido en un error
explícito en vez de en un `null` que reventaría más adelante.

## Cómo agregar un medio de pago nuevo

1. Crear `SistemaPago/Payments/CriptoPayment.cs`:

   ```csharp
   namespace SistemaPago.Payments
   {
       public class CriptoPayment : IPayment
       {
           public string Procesar(decimal monto)
               => $"Pago de ${monto} procesado en criptomonedas.";
       }
   }
   ```

2. Agregar un `case` en `PaymentFactory.Create`:

   ```csharp
   case "cripto": return new CriptoPayment();
   ```

Un archivo nuevo y una línea. Ninguna página, ningún servicio y ninguna otra clase de pago
se modifica.

## Ejecutar el proyecto

Requiere el SDK de .NET Core 2.0 (o Visual Studio con la carga de trabajo de ASP.NET).

```bash
dotnet restore
dotnet run --project SistemaPago/SistemaPago.csproj
```

Luego abrir la URL que indique la consola (por defecto `http://localhost:5000`).

## Estructura del repositorio

```
SistemaPago/
├── Payments/                      ← el patrón Factory
│   ├── IPayment.cs                    contrato común (Product)
│   ├── TarjetaPayment.cs              producto concreto
│   ├── PayPalPayment.cs               producto concreto
│   ├── TransferenciaPayment.cs        producto concreto
│   └── PaymentFactory.cs              la fábrica: decide y construye
├── Pages/
│   ├── Index.cshtml               formulario de pago
│   └── Index.cshtml.cs            cliente: pide el IPayment a la fábrica
├── Startup.cs                     configuración y registro de servicios
└── Program.cs                     arranque del host web
```

## Estado actual y siguientes pasos

La carpeta `Payments/` ya está completa. Falta conectarla con la aplicación:

- [ ] **`Startup.cs`** todavía registra los servicios del patrón Strategy
  (`IPaymentStrategy`, `PaymentService`), que ya no existen en esta rama.
  Reemplazar ese registro por el de la fábrica:

  ```csharp
  services.AddScoped<PaymentFactory>();
  ```

- [ ] **`Index.cshtml.cs`** todavía usa `SistemaPago.Strategies` y `PaymentService`.
  Cambiarlo para que reciba `PaymentFactory` e invoque `Create(tipoPago).Procesar(monto)`.

- [ ] **`Index.cshtml`** necesita un `<select name="tipoPago">` con las opciones
  `tarjeta`, `paypal` y `transferencia`, para que la elección sea del usuario — que es
  justamente lo que justifica tener una fábrica.

> ⚠️ Mientras esos tres puntos estén pendientes **el proyecto no compila**, porque
> `Startup.cs` e `Index.cshtml.cs` referencian clases del patrón Strategy que fueron
> eliminadas de esta rama.

## Ideas para seguir practicando

- **Factory Method clásico**: convertir `PaymentFactory` en una clase abstracta con
  subclases que decidan el producto, en vez de resolverlo con un `switch`.
- **Abstract Factory**: que la fábrica devuelva además un validador y un generador de
  comprobante coherentes con cada medio de pago.
- **Sin `switch`**: registrar los `IPayment` en el contenedor de dependencias y resolverlos
  por clave, o usar un `Dictionary<string, Func<IPayment>>`.
- **Pruebas unitarias**: verificar que `Create("paypal")` devuelve un `PayPalPayment` y que
  un tipo inválido lanza `ArgumentException`.
