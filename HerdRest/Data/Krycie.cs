namespace HerdRest.Data
{
    public class Krycie
    {

        public int Id { get; set; }
        public DateOnly DataPokrycia { get; set; }
        public DateTime DataCzasUtworzenia { get; set; }
        public DateTime DataCzasModyfikacji { get; set; }
    }
}