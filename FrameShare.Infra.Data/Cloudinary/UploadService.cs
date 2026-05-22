using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using FrameShare.Application.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Processing;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace FrameShare.Infra.Data.Cloudinary
{
    public class UploadService : IUploadService
    {
        private readonly CloudinaryDotNet.Cloudinary _cloudinary;

        public UploadService(IConfiguration configuration)
        {
            var cloudName = configuration["Cloudinary:CloudName"];
            var apiKey = configuration["Cloudinary:ApiKey"];
            var apiSecret = configuration["Cloudinary:ApiSecret"];

            var account = new Account(cloudName, apiKey, apiSecret);
            _cloudinary = new CloudinaryDotNet.Cloudinary(account);
            _cloudinary.Api.Secure = true;
        }

        public async Task<string> Upload(IFormFile file)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("Arquivo inválido.");

            var extensao = Path.GetExtension(file.FileName)?.ToLower() ?? "";
            var contentType = file.ContentType?.ToLower() ?? "";
            var extensoesPermitidas = new[] { ".jpg", ".jpeg", ".png", ".webp", ".heic", ".heif" };

            if (!extensoesPermitidas.Contains(extensao) && !contentType.StartsWith("image/") && contentType != "application/octet-stream")
            {
                throw new ApplicationException("Formato de arquivo inválido. Por favor, envie apenas imagens.");
            }

            try
            {
                // Criamos um MemoryStream para onde a imagem comprimida será gravada
                using var outputStream = new MemoryStream();

                // 🌟 PROCESSO DE COMPRESSÃO NO BACKEND (ImageSharp)
                using (var inputStream = file.OpenReadStream())
                {
                    // Carrega a imagem original vinda do upload
                    // 🌟 Correção 1: Especificamos "SixLabors.ImageSharp.Image" para tirar a ambiguidade
                    using (var image = await SixLabors.ImageSharp.Image.LoadAsync(inputStream))
                    {
                        // Se a imagem for maior que Full HD (1920px), redimensiona proporcionalmente
                        int maxDimensao = 1920;
                        if (image.Width > maxDimensao || image.Height > maxDimensao)
                        {
                            image.Mutate(x => x.Resize(new ResizeOptions
                            {
                                // 🌟 Correção 2: Especificamos "SixLabors.ImageSharp.Size" para o compilador saber que não é o Size do Cloudinary
                                Size = new SixLabors.ImageSharp.Size(maxDimensao, maxDimensao),
                                Mode = ResizeMode.Max
                            }));
                        }

                        // Configura o encoder para JPEG com 75% de qualidade
                        var encoder = new JpegEncoder
                        {
                            Quality = 75
                        };

                        // Salva a imagem processada no nosso MemoryStream
                        await image.SaveAsJpegAsync(outputStream, encoder);
                    }
                }

                // Reseta a posição do stream para o Cloudinary conseguir ler do começo
                outputStream.Position = 0;

                // Nome seguro padronizado com final .jpg (já que convertemos via código)
                string nomeArquivoSeguro = $"upload_{Guid.NewGuid()}.jpg";

                var uploadParams = new ImageUploadParams()
                {
                    // Passamos o novo outputStream contendo a imagem já comprimida
                    File = new FileDescription(nomeArquivoSeguro, outputStream),
                    DisplayName = "Foto Mural",
                    Folder = "FRAMESHARE_2026",
                    PublicId = Guid.NewGuid().ToString()
                };

                var uploadResult = await _cloudinary.UploadAsync(uploadParams);

                if (uploadResult.StatusCode == System.Net.HttpStatusCode.OK)
                    return uploadResult.SecureUrl.ToString();
                else
                    throw new Exception(uploadResult.Error.Message);
            }
            catch (ApplicationException)
            {
                throw;
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("format is not supported") || ex.Message.Contains("Invalid image file"))
                {
                    throw new ApplicationException("Não foi possível processar os dados desta imagem. Tente enviar outra foto.");
                }

                throw new Exception("Falha no processamento/upload da imagem: " + ex.Message);
            }
        }
    }
}