using ApiBancoDemo.Models;
using ApiBancoDemo.Services;
using Microsoft.AspNetCore.Mvc;

namespace ApiBancoDemo.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AlmacenEnMemoria _almacen;
    private readonly TokenService _tokens;

    public AuthController(AlmacenEnMemoria almacen, TokenService tokens)
    {
        _almacen = almacen;
        _tokens = tokens;
    }

    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginRequest req)
    {
        var usuario = _almacen.Usuarios.Values
            .FirstOrDefault(u => u.Username == req.Username && u.PasswordHash == req.Password);

        if (usuario is null)
            return Unauthorized(new { mensaje = "Credenciales inválidas." });

        var token = _tokens.GenerarToken(usuario.Username, usuario.Rol, out var expira);
        return Ok(new LoginResponse(token, usuario.Rol, expira));
    }
}