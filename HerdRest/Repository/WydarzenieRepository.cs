using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HerdRest.Data;
using HerdRest.Dto;
using HerdRest.Interfaces;
using HerdRest.Model;
using Microsoft.EntityFrameworkCore;

namespace HerdRest.Repository
{
    public class WydarzenieRepository : IWydarzenieRepository
    {
        private readonly DataContext _context;

        public WydarzenieRepository(DataContext context)
        {
            _context = context;
        }
        public WydarzenieDto MapToDto(Wydarzenie wydarzenie)
        {
            return new WydarzenieDto
            {
                Id = wydarzenie.Id,
                TypWydarzenia = wydarzenie.TypWydarzenia,
                Uwagi = wydarzenie.Uwagi,
                DataWydarzenia = wydarzenie.DataWydarzenia,
                DataWykonania = wydarzenie.DataWykonania,
                DataCzasUtworzenia = wydarzenie.DataCzasUtworzenia,
                DataCzasModyfikacji = wydarzenie.DataCzasModyfikacji,
                LochyId = wydarzenie.WydarzeniaLoch?.Select(wl => wl.LochaId).ToList() ?? [],
                MiotyId = wydarzenie.WydarzeniaMioty?.Select(wm => wm.MiotId).ToList() ?? []
            };
        }
        public List<WydarzenieDto> MapToDtoList(List<Wydarzenie> wydarzenia)
        {
            return wydarzenia.Select(MapToDto).ToList();
        }

        public Wydarzenie MapToModel(WydarzenieDto wydarzenieDto)
        {
            return new Wydarzenie
            {
                Id = wydarzenieDto.Id,
                TypWydarzenia = wydarzenieDto.TypWydarzenia,
                Uwagi = wydarzenieDto.Uwagi,
                DataWydarzenia = wydarzenieDto.DataWydarzenia,
                DataWykonania = wydarzenieDto.DataWykonania,
                DataCzasUtworzenia = wydarzenieDto.DataCzasUtworzenia,
                DataCzasModyfikacji = wydarzenieDto.DataCzasModyfikacji,
                WydarzeniaLoch = wydarzenieDto.LochyId?.Select(id => new WydarzenieLocha { LochaId = id }).ToList(),
                WydarzeniaMioty = wydarzenieDto.MiotyId?.Select(id => new WydarzenieMiot { MiotId = id }).ToList()
            };
        }
        public bool CreateWydarzenie(Wydarzenie wydarzenie, int? miotId, int? wydarzenieId)
        {
            if(miotId != null)
            {
                var wydarzenieMiotEntity = _context.Mioty.Where(a => a.Id == miotId).FirstOrDefault();
                var wydarzenieMiot = new WydarzenieMiot()
                {
                    Miot = wydarzenieMiotEntity,
                    Wydarzenie = wydarzenie,
                };
                _context.Add(wydarzenieMiot);
            }

            if(wydarzenieId != null)
            {
                var wydarzenieLochaEntity = _context.Lochy.Where(a => a.Id == wydarzenieId).FirstOrDefault();
                var wydarzenieLocha = new WydarzenieLocha()
                {
                    Locha = wydarzenieLochaEntity,
                    Wydarzenie = wydarzenie,
                };
                _context.Add(wydarzenieLocha);
            }

            _context.Add(wydarzenie);
            return Save();

        }
        
        public ICollection<Wydarzenie> GetWydarzenia()
        {
            return [.. _context.Wydarzenia.OrderBy(w => w.Id)];
        }

        public Wydarzenie GetWydarzenie(int wydarzenieId)
        {
            return _context.Wydarzenia.Where(w => w.Id == wydarzenieId).FirstOrDefault();
        }
        public bool UpdateWydarzenie(Wydarzenie wydarzenie)
        {
            wydarzenie.DataCzasModyfikacji = DateTime.Now;
            wydarzenie.DataCzasUtworzenia = _context.Wydarzenia.Where(l => l.Id == wydarzenie.Id)
                .Select(l => l.DataCzasUtworzenia)
                .FirstOrDefault();

            _context.Update(wydarzenie);
            return Save();
        }

        public bool DeleteWydarzenie(Wydarzenie wydarzenie)
        {
             _context.Remove(wydarzenie);
            return Save();
        }

         public bool Save()
        {
            var Saved = _context.SaveChanges();
            return Saved > 0 ? true : false;
        }

        public bool WydarzenieExists(int wydarzenieId)
        {
            return _context.Lochy.Any(w => w.Id == wydarzenieId);
        }
    }
}