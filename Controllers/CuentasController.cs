using ApiBancoDemo.Models;
using ApiBancoDemo.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ApiBancoDemo.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CuentasController : ControllerBase
{
    private readonly AlmacenEnMemoria _almacen;

    public CuentasController(AlmacenEnMemoria almacen) => _almacen = almacen;

    // GET: api/Cuentas?pagina=1&tamano=10&buscar=maria
    [HttpGet]
    public IActionResult ObtenerTodas(int pagina = 1, int tamano = 10, string? buscar = null)
    {
        if (pagina < 1) pagina = 1;
        if (tamano < 1 || tamano > 100) tamano = 10;

        var query = _almacen.Cuentas.Values.AsQueryable();

        if (!string.IsNullOrWhiteSpace(buscar))
            query = query.Where(c => c.Titular.Contains(buscar, StringComparison.OrdinalIgnoreCase)
                                   || c.NumeroCuenta.Contains(buscar, StringComparison.OrdinalIgnoreCase));

        var total = query.Count();
        var items = query.OrderBy(c => c.Id).Skip((pagina - 1) * tamano).Take(tamano).ToList();

        return Ok(new CuentaPaginada(items, total, pagina, tamano));
    }

    // GET: api/Cuentas/5
    [HttpGet("{id}")]
    public IActionResult ObtenerPorId(int id)
    {
        if (_almacen.Cuentas.TryGetValue(id, out var cuenta))
            return Ok(cuenta);
        return NotFound(new { mensaje = "Cuenta bancaria no encontrada." });
    }

    // POST: api/Cuentas
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public IActionResult CrearCuenta([FromBody] CuentaBancaria nuevaCuenta)
    {
        int nuevoId = _almacen.Cuentas.Keys.DefaultIfEmpty(0).Max() + 1;
        nuevaCuenta.Id = nuevoId;
        nuevaCuenta.FechaCreacion = DateTime.UtcNow;
        _almacen.Cuentas[nuevoId] = nuevaCuenta;
        return CreatedAtAction(nameof(ObtenerPorId), new { id = nuevoId }, nuevaCuenta);
    }

    // PUT: api/Cuentas/5
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public IActionResult ActualizarCuenta(int id, [FromBody] CuentaBancaria cuentaActualizada)
    {
        if (!_almacen.Cuentas.ContainsKey(id))
            return NotFound(new { mensaje = "Cuenta bancaria no encontrada." });

        cuentaActualizada.Id = id;
        _almacen.Cuentas[id] = cuentaActualizada;
        return NoContent();
    }

    // DELETE: api/Cuentas/5
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public IActionResult EliminarCuenta(int id)
    {
        if (_almacen.Cuentas.TryRemove(id, out _))
            return NoContent();
        return NotFound(new { mensaje = "Cuenta bancaria no encontrada." });
    }

    // POST: api/Cuentas/5/avatar  (multipart/form-data)
    [HttpPost("{id}/avatar")]
    [RequestSizeLimit(3 * 1024 * 1024)]
    public IActionResult SubirAvatar(int id, IFormFile archivo, [FromServices] ImageStorageService storage)
    {
        if (!_almacen.Cuentas.TryGetValue(id, out var cuenta))
            return NotFound(new { mensaje = "Cuenta bancaria no encontrada." });

        var (ok, msg, imgId) = storage.Guardar(archivo);
        if (!ok) return BadRequest(new { mensaje = msg });

        cuenta.AvatarUrl = $"/api/imagenes/{imgId}";
        return Ok(new { mensaje = "Imagen subida.", avatarUrl = cuenta.AvatarUrl });
    }
}