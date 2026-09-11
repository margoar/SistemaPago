namespace SistemaPago.Payments
{
    public class TarjetaPayment : IPayment
    {
        public string Procesar(decimal monto)
        {
            return $"Pago de ${monto} procesado con tarjeta.";
        }
    }
}
