using Microsoft.AspNetCore.SignalR;

namespace HerdRest.Model
{
    public class WydarzenieMiot
    {
        public int MiotId { get; set; }
        public int WydarzenieId { get; set; }
        public Miot Miot { get; set; }
        public Wydarzenie Wydarzenie { get; set; }
    }
}