using FrameShare.Domain.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrameShare.Infra.Data.EntitiesConfigration
{
    public class FotoConfiguration : IEntityTypeConfiguration<Foto>
    {
        public void Configure(EntityTypeBuilder<Foto> builder)
        {
            builder.HasKey(x => x.Id);
            builder.HasOne(f => f.Missao).WithMany(m => m.Fotos).HasForeignKey(f => f.MissionId);
            builder.HasOne(f => f.Evento).WithMany(e => e.Fotos).HasForeignKey(f => f.EventId);
            builder.HasOne(f => f.Usuario).WithMany(f => f.Fotos).HasForeignKey(f => f.UserId);

        }
    }
}
