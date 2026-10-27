using System.Security.Cryptography;
using ApiBancoDemo.Services;

namespace ApiBancoDemo.Services;

public class ImageStorageService
{
    private readonly AlmacenEnMemoria _almacen;

    public ImageStorageService(AlmacenEnMemoria almacen)
    {
        _almacen = almacen;
    }

    private static readonly string[] MimesPermitidos =
        { "image/jpeg", "image/png", "image/gif", "image/webp" };

    private const long MaxBytes = 2 * 1024 * 1024; // 2 MB

    public (bool ok, string mensaje, string? id) Guardar(IFormFile archivo)
    {
        if (archivo == null || archivo.Length == 0)
            return (false, "Archivo vacío.", null);

        if (archivo.Length > MaxBytes)
            return (false, "El archivo excede el tamaño máximo de 2 MB.", null);

        if (!MimesPermitidos.Contains(archivo.ContentType))
            return (false, "Formato no permitido. Use JPEG, PNG, GIF o WEBP.", null);

        using var ms = new MemoryStream();
        archivo.CopyTo(ms);
        var bytes = ms.ToArray();

        // Verificación de firma binaria (magic numbers)
        if (!EsImagenValida(bytes))
            return (false, "El contenido del archivo no es una imagen válida.", null);

        var id = Convert.ToHexString(RandomNumberGenerator.GetBytes(16));
        _almacen.Imagenes[id] = bytes;
        _almacen.ImagenesMime[id] = archivo.ContentType;

        return (true, "OK", id);
    }

    private static bool EsImagenValida(byte[] b)
    {
        if (b.Length < 12) return false;
        // JPEG
        if (b[0] == 0xFF && b[1] == 0xD8) return true;
        // PNG
        if (b[0] == 0x89 && b[1] == 0x50 && b[2] == 0x4E && b[3] == 0x47) return true;
        // GIF
        if (b[0] == 0x47 && b[1] == 0x49 && b[2] == 0x46) return true;
        // WEBP
        if (b[0] == 0x52 && b[1] == 0x49 && b[2] == 0x46 && b[3] == 0x46 &&
            b[8] == 0x57 && b[9] == 0x45 && b[10] == 0x42 && b[11] == 0x50) return true;
        return false;
    }
}