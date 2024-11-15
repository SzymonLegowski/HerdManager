namespace HerdRest.Data
{
    public class Wydarzenie
    {
        public int Id { get; set; }
        public required string Uwagi { get; set; }
        public DateOnly DataWydarzenia { get; set; }
        public DateOnly DataWykonania { get; set; }
        public DateTime DataCzasUtworzenia { get; set; }
        public DateTime DataCzasModyfikacji { get; set; }

    }
}