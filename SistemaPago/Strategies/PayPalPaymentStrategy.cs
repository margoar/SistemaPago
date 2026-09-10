using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SistemaPago.Strategies
{
    public class PayPalPaymentStrategy : IPaymentStrategy
    {
        public string Pagar(decimal monto)
        {
            return $"Pago de ${monto} realizado con PayPal";

        }
    }
}
