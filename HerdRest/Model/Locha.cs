
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HerdRest.Enums;

namespace HerdRest.Model
{
    public class Locha
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int NumerLochy { get; set; } 
        public StatusLochy Status { get; set; }
        public int IndeksProdukcji365Dni { get; set; }
        [Column(TypeName = "timestamp without time zone")]
        public DateTime? DataCzasUtworzenia { get; set; }
        [Column(TypeName = "timestamp without time zone")]
        public DateTime? DataCzasModyfikacji { get; set; }
        public ICollection<Miot>? Mioty { get; set; }
        public ICollection<WydarzenieLocha>? WydarzeniaLoch { get; set; }
    }
}