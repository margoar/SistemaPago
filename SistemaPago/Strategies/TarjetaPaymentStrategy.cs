using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SistemaPago.Strategies
{
    public class TarjetaPaymentStrategy : IPaymentStrategy
    {
        void IPaymentStrategy.Pagar(decimal monto)
        {
            Console.WriteLine($"Pagando ${monto} con tarjeta...");
        }
    }
}
