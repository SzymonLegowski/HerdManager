using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HerdRest.Enums;

namespace HerdRest.Model
{
    public class Wydarzenie
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public TypWydarzenia TypWydarzenia { get; set; }
        public string Uwagi { get; set; }
        public DateOnly DataWydarzenia { get; set; }
        public DateOnly DataWykonania { get; set; }
        public DateTime DataCzasUtworzenia { get; set; }
        public DateTime DataCzasModyfikacji { get; set; }
        public ICollection<WydarzenieLocha> WydarzeniaLoch { get; set;}
        public ICollection<WydarzenieMiot> WydarzeniaMioty {get; set;}

    }
}