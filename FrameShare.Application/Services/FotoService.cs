using FrameShare.Application.DTOs;
using FrameShare.Application.Interfaces;
using FrameShare.Domain.Entity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrameShare.Application.Services
{
    public class FotoService : IFotoService
    {
        private readonly IApplicationDbContext _context;

        public FotoService(IApplicationDbContext context)
        {
            _context = context;
        }

        public Task<bool> Apagar(Guid idFoto)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<Foto>> Buscar(int idEvento)
        {
            try
            {
                return await _context.Foto
                    .Where(f => f.EventId == idEvento)
                    .OrderByDescending(f => f.DataUpload)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                // Aqui você pode injetar um ILogger se quiser salvar o log: _logger.LogError(ex, "Erro ao buscar fotos...");
                throw new ApplicationException("Ocorreu um erro ao carregar as fotos do evento no banco de dados.", ex);
            }
        }

        public async Task<int> BuscarPorUsuario(int idUsuario)
        {
            try
            {
                return await _context.Foto
                    .Where(x => x.UserId == idUsuario)
                    .CountAsync(); // Modificado para CountAsync() direta: muito mais rápido do que dar ToListAsync e contar na memória
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Erro ao contabilizar as fotos do usuário.", ex);
            }
        }

        public async Task<IEnumerable<Foto>> BuscarFotosPorUsuario(int idEvento, int idUsuario, int tamanhoPagina, int pagina = 0)
        {
            try
            {
                return await _context.Foto
                    .Where(f => f.EventId == idEvento && f.UserId == idUsuario)
                    .OrderByDescending(f => f.DataUpload)
                    .Skip(pagina * tamanhoPagina)
                    .Take(tamanhoPagina)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Erro ao buscar a lista de fotos do usuário logado.", ex);
            }
        }

        public async Task<IEnumerable<Foto>> BuscarComFiltrosPessoal(int idEvento, int pagina, int tamanho, string ordem, int? missaoId, int idusuario)
        {
            try
            {
                var query = _context.Foto.AsQueryable().Where(f => f.EventId == idEvento && f.UserId == idusuario);

                if (missaoId.HasValue && missaoId > 0)
                    query = query.Where(f => f.MissionId == missaoId);

                if (ordem == "asc")
                    query = query.OrderBy(f => f.DataUpload);
                else
                    query = query.OrderByDescending(f => f.DataUpload);

                return await query
                    .Skip(pagina * tamanho)
                    .Take(tamanho)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Erro ao aplicar filtros na galeria pessoal.", ex);
            }
        }

        public async Task<IEnumerable<Foto>> BuscarRecentes(int idEvento, int tamanhoPagina, int pagina = 0)
        {
            try
            {
                return await _context.Foto
                    .Where(f => f.EventId == idEvento)
                    .OrderByDescending(f => f.DataUpload)
                    .Skip(pagina * tamanhoPagina)
                    .Take(tamanhoPagina)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Erro ao carregar as fotos recentes do mural.", ex);
            }
        }

        public async Task<IEnumerable<Foto>> BuscarComFiltros(int idEvento, int pagina, int tamanho, string ordem, int? missaoId)
        {
            try
            {
                var query = _context.Foto.AsQueryable().Where(f => f.EventId == idEvento);

                if (missaoId.HasValue && missaoId > 0)
                    query = query.Where(f => f.MissionId == missaoId);

                if (ordem == "asc")
                    query = query.OrderBy(f => f.DataUpload);
                else
                    query = query.OrderByDescending(f => f.DataUpload);

                return await query
                    .Skip(pagina * tamanho)
                    .Take(tamanho)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Erro ao aplicar filtros na galeria pública.", ex);
            }
        }

        public Task<bool> BuscarPorId(int idEvento, Guid idFoto)
        {
            throw new NotImplementedException();
        }

        public Task<bool> BuscarPorMissao(int idEvento, int idMissao)
        {
            throw new NotImplementedException();
        }

        // CORRIGIDO: Retorno alterado para Task para gerenciar corretamente o fluxo assíncrono
        public async Task Criar(int? idMissao, int idEvent, int userid, string urlDrive)
        {
            try
            {
                if (string.IsNullOrEmpty(urlDrive))
                    throw new ArgumentException("A URL do Drive da imagem não pode estar vazia.");

                var foto = new Foto(idMissao, idEvent, userid, urlDrive);

                _context.Foto.Add(foto);

                // CORRIGIDO: Inclusão do await necessária para garantir a persistência segura no Banco
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException dbEx)
            {
                // Captura erros específicos de violação de chave estrangeira ou banco indisponível
                throw new ApplicationException("Falha de persistência: Não foi possível salvar o registro da foto devido a uma inconsistência de dados.", dbEx);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Ocorreu um erro inesperado ao salvar a nova memória.", ex);
            }
        }

        public Task<bool> Editar(FotoDTO fotoDTO, Guid idFoto)
        {
            throw new NotImplementedException();
        }
    }
}