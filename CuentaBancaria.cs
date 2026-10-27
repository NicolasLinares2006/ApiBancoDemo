namespace ApiBancoDemo;

public class CuentaBancaria
{
    public int Id { get; set; }
    public string NumeroCuenta { get; set; } = string.Empty;
    public string Titular { get; set; } = string.Empty;
    public decimal Saldo { get; set; }
    public string TipoCuenta { get; set; } = "Ahorros"; // Ahorros o Corriente
}