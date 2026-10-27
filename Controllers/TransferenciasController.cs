using ApiBancoDemo.Models;
using ApiBancoDemo.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ApiBancoDemo.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TransferenciasController : ControllerBase
{
    private readonly AlmacenEnMemoria _almacen;
    public TransferenciasController(AlmacenEnMemoria almacen) => _almacen = almacen;

    [HttpPost]
    public IActionResult Transferir([FromBody] TransferenciaRequest req)
    {
        if (req.Monto <= 0)
            return BadRequest(new { mensaje = "El monto debe ser mayor a cero." });
        if (req.CuentaOrigenId == req.CuentaDestinoId)
            return BadRequest(new { mensaje = "Cuenta origen y destino no pueden ser iguales." });

        // Bloqueo ordenado para evitar deadlocks
        var primero = Math.Min(req.CuentaOrigenId, req.CuentaDestinoId);
        var segundo = Math.Max(req.CuentaOrigenId, req.CuentaDestinoId);

        lock (_almacen.GetLock(primero))
        lock (_almacen.GetLock(segundo))
        {
            if (!_almacen.Cuentas.TryGetValue(req.CuentaOrigenId, out var origen))
                return NotFound(new { mensaje = "Cuenta origen no encontrada." });
            if (!_almacen.Cuentas.TryGetValue(req.CuentaDestinoId, out var destino))
                return NotFound(new { mensaje = "Cuenta destino no encontrada." });
            if (!origen.Activa || !destino.Activa)
                return BadRequest(new { mensaje = "Una de las cuentas está inactiva." });
            if (origen.Saldo < req.Monto)
                return BadRequest(new { mensaje = "Saldo insuficiente." });

            origen.Saldo -= req.Monto;
            destino.Saldo += req.Monto;

            var t = new Transferencia
            {
                CuentaOrigenId = origen.Id,
                CuentaDestinoId = destino.Id,
                Monto = req.Monto
            };
            _almacen.Transferencias.Add(t);

            return Ok(new
            {
                mensaje = "Transferencia realizada.",
                transferencia = t,
                saldoOrigen = origen.Saldo
            });
        }
    }

    [HttpGet("historial/{cuentaId}")]
    public IActionResult Historial(int cuentaId)
    {
        var historial = _almacen.Transferencias
            .Where(t => t.CuentaOrigenId == cuentaId || t.CuentaDestinoId == cuentaId)
            .OrderByDescending(t => t.Fecha)
            .ToList();

        return Ok(historial);
    }
}