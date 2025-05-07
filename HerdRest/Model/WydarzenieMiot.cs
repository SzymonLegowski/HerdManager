using Microsoft.AspNetCore.SignalR;

namespace HerdRest.Model
{
    public class WydarzenieMiot
    {
        public int MiotId { get; set; }
        public int WydarzenieId { get; set; }
        public virtual Miot Miot { get; set; }
        public virtual Wydarzenie Wydarzenie { get; set; }
    }
}