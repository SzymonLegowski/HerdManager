namespace HerdRest.Model
{
    public class WydarzenieLocha
    {
        public int LochaId { get; set; }
        public int WydarzenieId { get; set; }
        public Locha Locha { get; set; }
        public Wydarzenie Wydarzenie { get; set; }
    }
}