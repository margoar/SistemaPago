using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SistemaPago.Strategies
{
    public interface IPaymentStrategy
    {
        void Pagar(decimal monto);

    }
}
