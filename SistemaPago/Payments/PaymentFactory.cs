using System;
using System.Runtime.InteropServices.WindowsRuntime;

namespace SistemaPago.Payments
{
    public class PaymentFactory
    {
        public IPayment Create(string tipoPago)
        {
            switch (tipoPago)
            {
                case "tarjeta":
                    return new TarjetaPayment();
                case "paypal":
                    return new PayPalPayment();
                case "transferencia":
                    return new TransferenciaPayment();

                default:
                    throw new ArgumentException($"Tipo de pago no válido: {tipoPago}");
            }
        }
    }
}
