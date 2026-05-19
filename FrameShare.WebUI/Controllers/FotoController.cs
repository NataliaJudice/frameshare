using FrameShare.Application.Interfaces;
using FrameShare.Domain.Entity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;

namespace FrameShare.WebUI.Controllers
{
    public class FotoController : Controller
    {
        private readonly IUploadService _uploadService;
        private readonly IFotoService _fotoService;
        public readonly IMissaoService _missaoService;
        private readonly IHttpClientFactory _httpClientFactory;

        private int UsuarioIdLogado =>
            int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

        public FotoController(IUploadService uploadService,
            IFotoService fotoService,
            IMissaoService missaoService,
            IHttpClientFactory httpClientFactory
            )
        {
            _uploadService = uploadService;
            _fotoService = fotoService;
            _missaoService = missaoService;
            _httpClientFactory = httpClientFactory;
        }

        [HttpGet]
        public async Task<IActionResult> BaixarFoto(string url, string nomeArquivo)
        {
            if (string.IsNullOrEmpty(url)) return BadRequest();

            try
            {
                var client = _httpClientFactory.CreateClient();
                var response = await client.GetAsync(url);

                if (!response.IsSuccessStatusCode) return NotFound();

                var bytes = await response.Content.ReadAsByteArrayAsync();
                var contentType = response.Content.Headers.ContentType?.ToString() ?? "image/jpeg";
                return File(bytes, contentType, nomeArquivo ?? "frameshare-foto.jpg");
            }
            catch
            {
                return Redirect(url);
            }
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var fotosIniciais = await _fotoService.BuscarRecentes(1, 5, 0);
                var model = new FrameShare.WebUI.ViewModels.IndexViewModel
                {
                    Fotos = fotosIniciais.ToList(),
                };
                return View(model);
            }
            catch
            {
                return View(new FrameShare.WebUI.ViewModels.IndexViewModel { Fotos = new System.Collections.Generic.List<Foto>() });
            }
        }

        public async Task<IActionResult> GaleriaPessoal()
        {
            try
            {
                var fotosIniciais = await _fotoService.BuscarFotosPorUsuario(1, UsuarioIdLogado, 5, 0);
                var model = new FrameShare.WebUI.ViewModels.IndexViewModel
                {
                    Fotos = fotosIniciais.ToList(),
                };
                return View(model);
            }
            catch
            {
                return View(new FrameShare.WebUI.ViewModels.IndexViewModel { Fotos = new System.Collections.Generic.List<Foto>() });
            }
        }

        [HttpGet]
        public async Task<IActionResult> BuscarFotos([FromQuery] int idEvento, [FromQuery] int pagina, [FromQuery] string ordem, [FromQuery] int? missaoId)
        {
            int tamanhoPagina = 5;
            if (missaoId == 0) missaoId = null;

            try
            {
                var fotos = await _fotoService.BuscarComFiltros(idEvento, pagina, tamanhoPagina, ordem, missaoId);

                if (fotos == null || !fotos.Any())
                    return NoContent();

                return PartialView("_FotosGaleriaPartial", fotos);
            }
            catch (ApplicationException ex)
            {
                // Retorna erro HTTP 400 para o JS disparar o SweetAlert de erro na paginação
                return BadRequest(new { erro = ex.Message });
            }
            catch
            {
                return BadRequest(new { erro = "Falha ao carregar mais fotos da galeria pública." });
            }
        }

        [HttpGet]
        public async Task<IActionResult> BuscarFotosPessoal([FromQuery] int idEvento, [FromQuery] int pagina, [FromQuery] string ordem, [FromQuery] int? missaoId)
        {
            int tamanhoPagina = 5;
            if (missaoId == 0) missaoId = null;

            try
            {
                var fotos = await _fotoService.BuscarComFiltrosPessoal(idEvento, pagina, tamanhoPagina, ordem, missaoId, UsuarioIdLogado);

                if (fotos == null || !fotos.Any())
                    return NoContent();

                return PartialView("_FotosGaleriaPartial", fotos);
            }
            catch (ApplicationException ex)
            {
                return BadRequest(new { erro = ex.Message });
            }
            catch
            {
                return BadRequest(new { erro = "Falha ao carregar mais fotos do seu histórico pessoal." });
            }
        }
    }
}