namespace HerdRest.Model
{
    public class WydarzenieLocha
    {
        public int LochaId { get; set; }
        public int WydarzenieId { get; set; }
        public required Locha Locha { get; set; }
        public required Wydarzenie Wydarzenie { get; set; }
    }
}