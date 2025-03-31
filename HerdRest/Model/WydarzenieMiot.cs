using Microsoft.AspNetCore.SignalR;

namespace HerdRest.Model
{
    public class WydarzenieMiot
    {
        public int MiotId { get; set; }
        public int WydarzenieId { get; set; }
        public required Miot Miot { get; set; }
        public required Wydarzenie Wydarzenie { get; set; }
    }
}