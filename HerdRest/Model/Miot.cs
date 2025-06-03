using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HerdRest.Model
{
    public class Miot
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int? UrodzoneZywe { get; set; }
        public int? UrodzoneMartwe { get; set; }
        public int? Przygniecone { get; set; }
        public int? Odsadzone { get; set; }
        public int? Ocena { get; set;}
        public DateOnly DataPrzewidywanegoProszenia { get; set;}
        public DateOnly? DataProszenia { get; set; }
        public DateOnly? DataOdsadzenia { get; set; }
        [Column(TypeName = "timestamp without time zone")]
        public DateTime DataCzasUtworzenia { get; set; }
        [Column(TypeName = "timestamp without time zone")]
        public DateTime DataCzasModyfikacji { get; set; }
        public Locha Locha {get; set;}
        public ICollection<WydarzenieMiot> WydarzeniaMiotu { get; set; }
    }
}