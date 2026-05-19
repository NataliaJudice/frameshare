using FrameShare.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrameShare.Application.Interfaces
{
    public interface IUsuarioService
    {
       Task<Usuario> BuscarPorSlug(string slug) ;
        Task CriarConvidado(string nomeCompleto);
    }
}
