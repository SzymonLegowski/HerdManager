namespace HerdRest.Model
{
    public class WydarzenieLocha
    {
        public int LochaId { get; set; }
        public int WydarzenieId { get; set; }
        public virtual Locha Locha { get; set; }
        public virtual Wydarzenie Wydarzenie { get; set; }
    }
}