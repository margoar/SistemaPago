namespace SistemaPago.Payments
{
    public class TransferenciaPayment : IPayment
    {
        public string Procesar(decimal monto)
        {
            return $"Pago de ${monto} procesado mediante transferencia.";
        }
    }
}
