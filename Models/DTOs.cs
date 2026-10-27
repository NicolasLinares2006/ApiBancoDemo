namespace ApiBancoDemo.Models;

public record LoginRequest(string Username, string Password);
public record LoginResponse(string Token, string Rol, DateTime Expira);
public record TransferenciaRequest(int CuentaOrigenId, int CuentaDestinoId, decimal Monto);
public record CuentaPaginada(IEnumerable<CuentaBancaria> Items, int Total, int Pagina, int TamanoPagina);