using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SistemaPago.Strategies
{
    public class PaymentService
    {
        private readonly IPaymentStrategy _paymentStrategy;

        public PaymentService(IPaymentStrategy paymentStrategy)
        {
            _paymentStrategy = paymentStrategy;
        }

        public string ProcesarPago(decimal monto)
        {
            return _paymentStrategy.Pagar(monto);
        }
    }
}