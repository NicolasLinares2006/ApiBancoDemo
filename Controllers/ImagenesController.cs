using ApiBancoDemo.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ApiBancoDemo.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ImagenesController : ControllerBase
{
    private readonly AlmacenEnMemoria _almacen;
    public ImagenesController(AlmacenEnMemoria almacen) => _almacen = almacen;

    [HttpGet("{id}")]
    public IActionResult Obtener(string id)
    {
        if (!_almacen.Imagenes.TryGetValue(id, out var bytes))
            return NotFound(new { mensaje = "Imagen no encontrada." });

        var mime = _almacen.ImagenesMime[id];
        return File(bytes, mime);
    }
}