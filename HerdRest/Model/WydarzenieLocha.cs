namespace HerdRest.Model
{
    public class WydarzenieLocha
    {
        public int LochaId { get; set; }
        public int WydarzenieId { get; set; }
        public virtual required Locha Locha { get; set; }
        public virtual required Wydarzenie Wydarzenie { get; set; }
    }
}