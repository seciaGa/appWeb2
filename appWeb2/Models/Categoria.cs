using System.ComponentModel.DataAnnotations;

namespace appWeb2.Models
{
    public class Categoria
    {
        [Key]
        public int Id { get; set; }
        public string Nombre { get; set; }
        public ICollection<VideoJuegos> VideoJuegos { get; set; }
    }
}
