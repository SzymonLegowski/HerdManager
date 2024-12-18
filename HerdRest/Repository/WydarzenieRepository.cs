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
                WydarzeniaLochId = wydarzenie.WydarzeniaLoch?.Select(wl => wl.LochaId).ToList() ?? [],
                WydarzeniaMiotyId = wydarzenie.WydarzeniaMioty?.Select(wm => wm.MiotId).ToList() ?? []
            };
        }
        public List<WydarzenieDto> MapToDtoList(List<Wydarzenie> wydarzenia)
        {
            return wydarzenia.Select(MapToDto).ToList();
        }
        public bool CreateWydarzenie(Wydarzenie wydarzenie, int? miotId, int? lochaId)
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

            if(lochaId != null)
            {
                var wydarzenieLochaEntity = _context.Lochy.Where(a => a.Id == lochaId).FirstOrDefault();
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
            return _context.Wydarzenia.OrderBy(w => w.Id).ToList();
        }

        public Wydarzenie GetWydarzenie(int wydarzenieId)
        {
            return _context.Wydarzenia.Where(w => w.Id == wydarzenieId).FirstOrDefault();
        }

        // public bool UpdateWydarzenie(Wydarzenie wydarzenie, int? miotId, int? lochaId)
        // {
        //      if(miotId != null)
        //     {
        //         var wydarzenieMiotEntity = _context.Mioty.Where(a => a.Id == miotId).FirstOrDefault();
        //         var wydarzenieMiot = new WydarzenieMiot()
        //         {
        //             Miot = wydarzenieMiotEntity,
        //             Wydarzenie = wydarzenie,
        //         };
        //         _context.Add(wydarzenieMiot);
        //     }

        //     if(lochaId != null)
        //     {
        //         var wydarzenieLochaEntity = _context.Lochy.Where(a => a.Id == lochaId).FirstOrDefault();
        //         var wydarzenieLocha = new WydarzenieLocha()
        //         {
        //             Locha = wydarzenieLochaEntity,
        //             Wydarzenie = wydarzenie,
        //         };
        //         _context.Add(wydarzenieLocha);
        //     }

        //     _context.Update(wydarzenie);
        //     return Save();

        // }
        public ICollection<Wydarzenie> GetWydarzeniaLochy(int lochaId)
        {
            return [.. _context.Wydarzenia.Where(w => w.WydarzeniaLoch.Any(wl => wl.LochaId == lochaId))];
        }
        public ICollection<Wydarzenie> GetWydarzeniaMiotu(int miotId)
        {
            return [.. _context.Wydarzenia.Where(w => w.WydarzeniaMioty.Any(wm => wm.MiotId == miotId))];
        }
        public bool UpdateWydarzenie(Wydarzenie wydarzenie)
        {
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