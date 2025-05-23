using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HerdRest.Enums;

namespace HerdRest.Dto
{
    public class LochaDto
    {
        public int Id { get; set; }
        public int NumerLochy { get; set; } 
        public StatusLochy Status { get; set; }
        public string? Uwagi { get; set; }
        public int IndeksProdukcji365Dni { get; set; }
        public string? DataCzasUtworzenia { get; set; }
        public string? DataCzasModyfikacji { get; set; }
        public DateOnly? DataBrakowania { get; set; }
        public List<int>? MiotyId { get; set; }
        public List<int>? WydarzeniaLochyId { get; set; }
    }
}