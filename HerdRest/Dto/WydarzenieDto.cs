using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HerdRest.Enums;

namespace HerdRest.Dto
{
    public class WydarzenieDto
    {
        public int Id { get; set; }
        public TypWydarzenia TypWydarzenia { get; set; }
        public string? Uwagi { get; set; }
        public DateOnly DataWydarzenia { get; set; }
        public DateOnly DataWykonania { get; set; }
        public DateTime DataCzasUtworzenia { get; set; }
        public DateTime DataCzasModyfikacji { get; set; }
        public List<int>? LochyId { get; set;}
        public List<int>? MiotyId {get; set;}
    }
}