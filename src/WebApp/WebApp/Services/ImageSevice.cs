using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;

namespace WebApp.Services;

public class ImageSevice
{
    private readonly IWebHostEnvironment _env;
    private const string ImageDirectory = "Asserts/Images";

    // Construtor que injeta IWebHostEnvironment
    public ImageSevice(IWebHostEnvironment env)
    {
        _env = env ?? throw new ArgumentNullException(nameof(env));
    }

    public async Task UploadImageAsync(Stream imageStream, string fileName)
    {
        if (imageStream == null) throw new ArgumentNullException(nameof(imageStream));
        if (string.IsNullOrWhiteSpace(fileName)) throw new ArgumentException("fileName não pode ser vazio.", nameof(fileName));

        // Define o caminho completo para a pasta de imagens
        var postsPath = Path.Combine(_env.WebRootPath ?? string.Empty, ImageDirectory);

        // Se o diretório não existir, cria-o
        if (!Directory.Exists(postsPath))
        {
            Directory.CreateDirectory(postsPath);
        }

        // Define o caminho completo do arquivo incluindo o nome
        var filePath = Path.Combine(postsPath, fileName);

        // Copia o fluxo da imagem para o arquivo físico de forma assíncrona
        using (var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None, 4096, useAsync: true))
        {
            await imageStream.CopyToAsync(fileStream).ConfigureAwait(false);
            await fileStream.FlushAsync().ConfigureAwait(false);
        }
    }
}