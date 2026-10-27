namespace ApiBancoDemo.Models;

public class CuentaBancaria
{
    public int Id { get; set; }
    public string NumeroCuenta { get; set; } = string.Empty;
    public string Titular { get; set; } = string.Empty;
    public decimal Saldo { get; set; }
    public string TipoCuenta { get; set; } = "Ahorros";
    public string Moneda { get; set; } = "USD";
    public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;
    public bool Activa { get; set; } = true;
    public string? AvatarUrl { get; set; }  // Imagen subida
}