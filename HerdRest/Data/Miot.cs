namespace HerdRest.Data
{
    public class Miot
    {
        public int Id { get; set; }
        public int UrodzoneZywe { get; set; }
        public int UrodzoneMartwe { get; set; }
        public int Przygniecone { get; set; }
        public int Odsadzone { get; set; }
        public int Ocena { get; set;}
        public DateOnly DataPokrycia { get; set; } 
        public DateOnly PrzewidywanaDataProszenia { get; set; }
        public DateOnly DataProszenia { get; set;}
        public DateOnly DataOdsadzenia { get; set; }
        public DateTime DataCzasUtworzenia { get; set; }
        public DateTime DataCzasModyfikacji { get; set; }

        public Miot(){}
    }
}