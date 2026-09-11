namespace SistemaPago.Payments
{
    public interface IPayment
    {
        string Procesar(decimal monto);

    }
}
