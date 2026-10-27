using Microsoft.AspNetCore.Mvc;
using System.Collections.Concurrent;

namespace ApiBancoDemo.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CuentasController : ControllerBase
{
    // Almacenamiento en memoria seguro para hilos (ideal para JMeter)
    private static readonly ConcurrentDictionary<int, CuentaBancaria> _cuentas = new(new[]
    {
        new KeyValuePair<int, CuentaBancaria>(1, new CuentaBancaria { Id = 1, NumeroCuenta = "ACC-001", Titular = "Carlos Pérez", Saldo = 1500.00m, TipoCuenta = "Ahorros" }),
        new KeyValuePair<int, CuentaBancaria>(2, new CuentaBancaria { Id = 2, NumeroCuenta = "ACC-002", Titular = "María Gómez", Saldo = 3200.50m, TipoCuenta = "Corriente" })
    });

    // GET: api/Cuentas
    [HttpGet]
    public IActionResult ObtenerTodas() => Ok(_cuentas.Values);

    // GET: api/Cuentas/5
    [HttpGet("{id}")]
    public IActionResult ObtenerPorId(int id)
    {
        if (_cuentas.TryGetValue(id, out var cuenta))
            return Ok(cuenta);

        return NotFound(new { mensaje = "Cuenta bancaria no encontrada." });
    }

    // POST: api/Cuentas
    [HttpPost]
    public IActionResult CrearCuenta([FromBody] CuentaBancaria nuevaCuenta)
    {
        int nuevoId = _cuentas.Keys.DefaultIfEmpty(0).Max() + 1;
        nuevaCuenta.Id = nuevoId;
        _cuentas[nuevoId] = nuevaCuenta;

        return CreatedAtAction(nameof(ObtenerPorId), new { id = nuevoId }, nuevaCuenta);
    }

    // PUT: api/Cuentas/5
    [HttpPut("{id}")]
    public IActionResult ActualizarCuenta(int id, [FromBody] CuentaBancaria cuentaActualizada)
    {
        if (!_cuentas.ContainsKey(id))
            return NotFound(new { mensaje = "Cuenta bancaria no encontrada." });

        cuentaActualizada.Id = id;
        _cuentas[id] = cuentaActualizada;

        return NoContent();
    }

    // DELETE: api/Cuentas/5
    [HttpDelete("{id}")]
    public IActionResult EliminarCuenta(int id)
    {
        if (_cuentas.TryRemove(id, out _))
            return NoContent();

        return NotFound(new { mensaje = "Cuenta bancaria no encontrada." });
    }
}