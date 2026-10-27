using System.Diagnostics;
using ApiBancoDemo.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ApiBancoDemo.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ReportesController : ControllerBase
{
    private readonly AlmacenEnMemoria _almacen;
    public ReportesController(AlmacenEnMemoria almacen) => _almacen = almacen;

    // GET: api/Reportes/resumen
    [HttpGet("resumen")]
    public IActionResult Resumen()
    {
        var cuentas = _almacen.Cuentas.Values.ToList();
        var transferencias = _almacen.Transferencias.ToList();

        return Ok(new
        {
            totalCuentas = cuentas.Count,
            saldoTotal = cuentas.Sum(c => c.Saldo),
            saldoPromedio = cuentas.Any() ? cuentas.Average(c => c.Saldo) : 0,
            totalTransferencias = transferencias.Count,
            montoTransferido = transferencias.Sum(t => t.Monto),
            cuentasPorTipo = cuentas.GroupBy(c => c.TipoCuenta)
                                    .ToDictionary(g => g.Key, g => g.Count())
        });
    }

    // GET: api/Reportes/procesar?iteraciones=100000  (prueba de estrés)
    [HttpGet("procesar")]
    public IActionResult Procesar(int iteraciones = 10000)
    {
        if (iteraciones < 1 || iteraciones > 5_000_000)
            return BadRequest(new { mensaje = "iteraciones fuera de rango (1-5,000,000)." });

        var sw = Stopwatch.StartNew();
        double suma = 0;
        for (int i = 0; i < iteraciones; i++)
            suma += Math.Sqrt(i) * Math.Sin(i);

        sw.Stop();
        return Ok(new
        {
            iteraciones,
            resultado = suma,
            milisegundos = sw.ElapsedMilliseconds
        });
    }
}