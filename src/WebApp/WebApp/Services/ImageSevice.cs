using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;

namespace WebApp.Services;

public class ImageSevice
{
    private readonly string _imageDirectory;

    public ImageSevice(IWebHostEnvironment env)
    {
        if (env == null) throw new ArgumentNullException(nameof(env));

        if (env.EnvironmentName == "Production")
        {
            _imageDirectory = "/home/data/uploads/images";
        }
        else
        {
            _imageDirectory = Path.Combine(env.WebRootPath ?? string.Empty, "Asserts", "Images");
        }

        if (!Directory.Exists(_imageDirectory))
        {
            Directory.CreateDirectory(_imageDirectory);
        }
    }

    public string GetImagePath(string fileName)
    {
        return Path.Combine(_imageDirectory, fileName);
    }

    public async Task UploadImageAsync(Stream imageStream, string fileName)
    {
        ArgumentNullException.ThrowIfNull(imageStream);
        if (string.IsNullOrWhiteSpace(fileName)) throw new ArgumentException("fileName não pode ser vazio.", nameof(fileName));

        if (!Directory.Exists(_imageDirectory))
        {
            Directory.CreateDirectory(_imageDirectory);
        }

        var filePath = Path.Combine(_imageDirectory, fileName);

        await using var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None, 4096, useAsync: true);
        await imageStream.CopyToAsync(fileStream).ConfigureAwait(false);
        await fileStream.FlushAsync().ConfigureAwait(false);
    }
}
