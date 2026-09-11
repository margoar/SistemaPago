namespace SistemaPago.Payments
{
    public class PayPalPayment : IPayment
    {
        public string Procesar(decimal monto)
        {
            return $"Procesando pago de {monto} a través de PayPal.";
        }
    }
}
