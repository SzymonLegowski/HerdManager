using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HerdRest.Data
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
        public required DateTime DataCzasUtworzenia { get; set; }
        public required DateTime DataCzasModyfikacji { get; set; }
        public required int LochaId { get; set; }
        public required Locha Locha {get; set;}
        public required ICollection<Wydarzenie> Wydarzenia;

        public Miot(){
            this.DataCzasModyfikacji = DateTime.Now;
            this.DataCzasUtworzenia = DateTime.Now;
        }

    }
}