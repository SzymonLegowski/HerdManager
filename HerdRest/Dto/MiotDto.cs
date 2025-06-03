namespace HerdRest.Dto
{
    public class MiotDto
    {
        public int Id { get; set; }
        public int? UrodzoneZywe { get; set; }
        public int? UrodzoneMartwe { get; set; }
        public int? Przygniecone { get; set; }
        public int? Odsadzone { get; set; }
        public int? Ocena { get; set;}
        public DateOnly DataPrzewidywanegoProszenia { get; set; }
        public DateOnly? DataProszenia { get; set; }
        public DateOnly? DataOdsadzenia { get; set; }
        public DateTime DataCzasUtworzenia { get; set; }
        public DateTime DataCzasModyfikacji { get; set; }
        public int LochaId {get; set;}
        public List<int>? WydarzeniaMiotuId { get; set; }
    }
}