using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrameShare.Domain.Entity
{
    public class Usuario
    {
        public int Id { get; set; }
        public string NomeCompleto { get; set; } = string.Empty;

        // O SlugLogin é o que o convidado usará (ex: natalianovais)
        public string SlugLogin { get;  set; } = string.Empty;

        // "Admin" ou "Convidado"
        public string Role { get; set; } = "Convidado";

        // Relacionamento com o Evento (Casamento, Aniversário, etc)
        public int EventoId { get; set; }

        // Construtor vazio para o Entity Framework
        public Usuario() { }

        public Usuario(string nomeCompleto, int eventoId, string slugLogin , string role = "Convidado")
        {
            NomeCompleto = nomeCompleto;
            EventoId = eventoId;
            Role = role;
            SlugLogin = slugLogin;
        }
    }
}
