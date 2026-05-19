using FrameShare.Application.DTOs;
using FrameShare.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrameShare.Application.Interfaces
{
    public interface IFotoService
    {
        Task<IEnumerable<Foto>> BuscarComFiltrosPessoal(int idEvento, int pagina, int tamanho, string ordem, int? missaoId, int idusuario);
        Task<IEnumerable<Foto>> BuscarFotosPorUsuario(int idEvento, int idUsuario, int tamanhoPagina, int pagina = 0);
        Task<int> BuscarPorUsuario(int idUsuario);
        Task Criar(int? idMissao, int idEvent, int userid, string urlDrive);
        Task<bool> Editar(FotoDTO fotoDTO, Guid idFoto);
        Task<bool> Apagar(Guid idFoto);
        Task<IEnumerable<Foto>> Buscar(int idEvento);
        Task<bool> BuscarPorId(int idEvento, Guid idFoto);
        Task<bool> BuscarPorMissao(int idEvento, int idMissao);
        Task<IEnumerable<Foto>> BuscarRecentes(int idEvento, int tamanhoPagina, int pagina = 0);
        Task<IEnumerable<Foto>> BuscarComFiltros(int idEvento, int pagina, int tamanho, string ordem, int? missaoId);
    }
}
