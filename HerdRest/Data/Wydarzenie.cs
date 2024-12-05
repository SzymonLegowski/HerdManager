using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HerdRest.Enums;

namespace HerdRest.Data
{
    public class Wydarzenie
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public TypWydarzenia TypWydarzenia { get; set; }
        public required string Uwagi { get; set; }
        public DateOnly DataWydarzenia { get; set; }
        public DateOnly DataWykonania { get; set; }
        public DateTime DataCzasUtworzenia { get; set; }
        public DateTime DataCzasModyfikacji { get; set; }
        public ICollection<Locha>? Lochy { get; set;}
        public ICollection<Miot>? Mioty {get; set;}

        public Wydarzenie(){
            this.DataCzasModyfikacji = DateTime.Now;
            this.DataCzasUtworzenia = DateTime.Now;
        }

    }
}