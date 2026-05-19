using FrameShare.Domain.Entity;

namespace FrameShare.WebUI.ViewModels
{
    public class IndexAdminViewModel
    {
        public int FotosTotal { get; set; }
        public IEnumerable<Foto>? Fotos { get; set; }
    }
}
