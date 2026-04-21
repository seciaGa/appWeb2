using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace appWeb2.Models
{
    public class VideoJuegos
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(150)]
        public string Titulo { get; set; }

        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal Precio { get; set; }

        [Required]
        public int CategoriaId { get; set; }
        [ForeignKey("CategoriaId")]
        public Categoria? Categoria { get; set; }

        [StringLength(500)]
        public string Descripcion { get; set; }

        public string? Imagen { get; set; }

        public ICollection<Compra>? Compras { get; set; }

        public DateTime? FechaRegistro { get; set; } = DateTime.Now;

        public bool EnPromocion { get; set; } = false;

        public int? EdadMinima { get; set; }
    }
}