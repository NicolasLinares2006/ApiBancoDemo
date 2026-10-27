using System.Collections.Concurrent;
using ApiBancoDemo.Models;

namespace ApiBancoDemo.Services;

public class AlmacenEnMemoria
{
    public ConcurrentDictionary<int, CuentaBancaria> Cuentas { get; } = new(new[]
    {
        new KeyValuePair<int, CuentaBancaria>(1, new CuentaBancaria { Id = 1, NumeroCuenta = "ACC-001", Titular = "Carlos Pérez", Saldo = 1500.00m, TipoCuenta = "Ahorros" }),
        new KeyValuePair<int, CuentaBancaria>(2, new CuentaBancaria { Id = 2, NumeroCuenta = "ACC-002", Titular = "María Gómez", Saldo = 3200.50m, TipoCuenta = "Corriente" })
    });

    public ConcurrentDictionary<int, Usuario> Usuarios { get; } = new(new[]
    {
        new KeyValuePair<int, Usuario>(1, new Usuario { Id = 1, Username = "admin", PasswordHash = "admin123", Rol = "Admin" }),
        new KeyValuePair<int, Usuario>(2, new Usuario { Id = 2, Username = "carlos", PasswordHash = "carlos123", Rol = "User" })
    });

    public ConcurrentBag<Transferencia> Transferencias { get; } = new();
    public ConcurrentDictionary<string, byte[]> Imagenes { get; } = new();
    public ConcurrentDictionary<string, string> ImagenesMime { get; } = new();

    // Candados por cuenta para concurrencia en transferencias
    public ConcurrentDictionary<int, object> LocksCuenta { get; } = new();

    public object GetLock(int cuentaId) =>
        LocksCuenta.GetOrAdd(cuentaId, _ => new object());
}