using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HerdRest.Model
{
    public class Miot
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int UrodzoneZywe { get; set; }
        public int UrodzoneMartwe { get; set; }
        public int Przygniecone { get; set; }
        public int Odsadzone { get; set; }
        public int Ocena { get; set;}
        public DateTime DataCzasUtworzenia { get; set; }
        public DateTime DataCzasModyfikacji { get; set; }
        public Locha Locha {get; set;}
        public ICollection<WydarzenieMiot> WydarzeniaMiotu { get; set; }
    }
}