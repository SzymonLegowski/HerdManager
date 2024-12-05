
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HerdRest.Enums;

namespace HerdRest.Data
{
    public class Locha
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int NumerLochy { get; set; } 
        public StatusLochy Status { get; set; }
        public int? IndeksProdukcji365Dni { get; set; }
        public DateTime DataCzasUtworzenia { get; set; }
        public DateTime DataCzasModyfikacji { get; set; }
        public ICollection<Miot>? Mioty { get; set; }
        public ICollection<Wydarzenie>? Wydarzenia { get; set; }

        public Locha(){
            this.DataCzasModyfikacji = DateTime.UtcNow;
            this.DataCzasUtworzenia = DateTime.UtcNow;
        }

    }
}