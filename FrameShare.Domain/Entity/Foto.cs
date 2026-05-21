using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrameShare.Domain.Entity
{
    public class Foto
    {
        public Guid Id { get; set; }
        public int EventId { get; set; }
        public Evento Evento { get; set; }
        public int UserId { get; set; } 
        public Usuario Usuario { get; set; }
        public int? MissionId { get; set; } 
        public Missao? Missao { get; set; }
        public string UrlDrive { get; set; }
        public bool? Status { get; set; }
        public DateTime DataUpload { get; set; }

        public Foto()
        {
            Id = Guid.NewGuid();
            DataUpload = DateTime.Now;
        }

        public Foto(int? idMissao, int idEvent, int userid, string urlDrive)
        {
            Id = Guid.NewGuid();
            MissionId = idMissao;
            EventId = idEvent;
            UserId = userid;
            UrlDrive = urlDrive;
            DataUpload = DateTime.Now;
        }
    }

}
