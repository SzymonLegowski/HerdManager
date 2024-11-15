namespace HerdRest.Data
{
    public class Locha
    {
        public int Id { get; set; }
        public int NumerLochy { get; set; } 
        public required string Status { get; set; }
        public int IndeksProdukcji365Dni { get; set; }
        public DateTime DataCzasUtworzenia { get; set; }
        public DateTime DataCzasModyfikacji { get; set; }

    }
}