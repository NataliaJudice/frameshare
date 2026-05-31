using CloudinaryDotNet.Actions;
using FrameShare.Application.Interfaces;
using FrameShare.Domain.Entity;
using FrameShare.WebUI.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

[Authorize]
public class DriveTestController : Controller
{
    private readonly IUploadService _uploadService;
    private readonly IFotoService _fotoService;
    public readonly IMissaoService _missaoService;
    private readonly IUsuarioService _usuarioService;

    private int UsuarioIdLogado =>
        int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
    private int UsuarioRoleLogado =>
        int.Parse(User.FindFirst(ClaimTypes.Role)?.Value ?? "0");

    public DriveTestController(IUploadService uploadService, IFotoService fotoService, IMissaoService missaoService, IUsuarioService usuarioService)
    {
        _uploadService = uploadService;
        _fotoService = fotoService;
        _missaoService = missaoService;
        _usuarioService = usuarioService;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        int eventoId = 1;
        var fotosTotal = await _fotoService.BuscarPorUsuario(UsuarioIdLogado);
        var fotos = await _fotoService.BuscarFotosPorUsuario(eventoId, UsuarioIdLogado, 10);

        var indexvm = new IndexViewModel
        {
            FotosJaEnviadas = fotosTotal,
            Fotos = fotos,
        };
        return View(indexvm);
    }

    // Adaptado para SweetAlert/AJAX
    [HttpPost]
    public async Task<IActionResult> Index(IFormFile file)
    {
        if (file == null || file.Length == 0)
            return BadRequest(new { mensagem = "Por favor, selecione uma imagem válida." });

        try
        {
            string imageUrl = await _uploadService.Upload(file);
            await _fotoService.Criar(null, 1, UsuarioIdLogado, imageUrl);

            // Retorna JSON de sucesso para o SweetAlert fechar o loading e atualizar a página
            return Ok(new { mensagem = "Upload concluído com sucesso!", url = imageUrl });
        }
        catch (ApplicationException ex)
        {
            // Captura as exceções controladas do FotoService
            return BadRequest(new { mensagem = ex.Message });
        }
        catch (Exception)
        {
            // Proteção para falhas críticas de infraestrutura desconhecidas
            return BadRequest(new { mensagem = "Não foi possível enviar a sua foto devido a uma instabilidade no servidor." });
        }
    }
    [HttpPost]
    public async Task<IActionResult> CriarUsuario(string nomeCompleto)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(nomeCompleto))
                return BadRequest(new { erro = "O nome completo não pode ser vazio." });

            await _usuarioService.CriarConvidado(nomeCompleto);
            return Ok(new { mensagem = "Convidado cadastrado e credencial gerada com sucesso!" });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { erro = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { erro = ex.Message });
        }
        catch (Exception ex)
        {
            return BadRequest(new { erro = ex.Message });
        }
    }

    // Adaptado para SweetAlert/AJAX
    [HttpPost]
    public async Task<IActionResult> UploadPorMissao(IFormFile file, int idMissao)
    {
        if (file == null || file.Length == 0)
            return BadRequest(new { mensagem = "Por favor, envie um arquivo de imagem válido para cumprir a missão." });

        int eventoId = 1;
        try
        {
            string imageUrl = await _uploadService.Upload(file);
            await _fotoService.Criar(idMissao, eventoId, UsuarioIdLogado, imageUrl);

            return Ok(new { mensagem = "Missão cumprida com sucesso!" });
        }
        catch (ApplicationException ex)
        {
            return BadRequest(new { mensagem = ex.Message });
        }
        catch (Exception)
        {
            return BadRequest(new { mensagem = "Ocorreu um problema ao registrar o cumprimento da missão no servidor." });
        }
    }

    [HttpGet]
    public async Task<IActionResult> AdminIndex()
    {
        var roleUsuario = User.FindFirst(ClaimTypes.Role)?.Value;

        // Se NÃO for admin, exibe a página de não autorizado na hora
        if (roleUsuario != "Admin")
        {
            return RedirectToAction("NaoAutorizado", "Foto");
        }
        int eventoId = 1;
        var fotosTotal = await _fotoService.Buscar(eventoId);
        var fotos = await _fotoService.BuscarRecentes(eventoId, 10);

        var viewmodel = new IndexAdminViewModel
        {
            FotosTotal = fotosTotal.Count(),
            Fotos = fotos
        };
        return View(viewmodel);
    }
}