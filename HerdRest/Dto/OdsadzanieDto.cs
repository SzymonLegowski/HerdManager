namespace HerdRest.Dto
{
    public class OdsadzanieDto
    {
        public required List<int> MiotyId { get; set; }
        public required DateOnly DataOdsadzenia { get; set; }
    }
}