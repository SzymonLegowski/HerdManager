using HerdRest.Data;
using HerdRest.Dto;
using HerdRest.Interfaces;
using HerdRest.Model;
using Microsoft.EntityFrameworkCore;

namespace HerdRest.Repository
{
    public class WydarzenieRepository(DataContext context) : IWydarzenieRepository
    {
        private readonly DataContext _context = context;

        public WydarzenieDto MapToDto(Wydarzenie wydarzenie)
        {
            return new WydarzenieDto
            {
                Id = wydarzenie.Id,
                TypWydarzenia = wydarzenie.TypWydarzenia,
                Uwagi = wydarzenie.Uwagi,
                DataWydarzenia = wydarzenie.DataWydarzenia,
                DataWykonania = wydarzenie.DataWykonania,
                DataCzasUtworzenia = wydarzenie.DataCzasUtworzenia.ToString("yyyy-MM-dd HH:mm:ss"),
                DataCzasModyfikacji = wydarzenie.DataCzasModyfikacji.ToString("yyyy-MM-dd HH:mm:ss"),
                LochyId = wydarzenie.WydarzeniaLoch?.Select(l => l.LochaId).ToList() ?? [],
                MiotyId = wydarzenie.WydarzeniaMioty?.Select(m => m.MiotId).ToList() ?? []
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
                DataCzasUtworzenia = DateTime.Now,
                DataCzasModyfikacji = DateTime.Now,
                WydarzeniaLoch = _context.WydarzeniaLochy.Where(w => w.WydarzenieId == wydarzenieDto.Id).ToList() ?? [],
                WydarzeniaMioty = _context.WydarzeniaMiotu.Where(w => w.WydarzenieId == wydarzenieDto.Id).ToList() ?? []
            };
        }
        public bool CreateWydarzenie(Wydarzenie wydarzenie, List<int>? miotId, List<int>? lochaId)
        {
            wydarzenie.Uwagi ??= "brak";
            if(miotId != null)
            {
                foreach(var id in miotId)
                {
                    var wydarzenieMiotEntity = _context.Mioty.Where(a => a.Id == id).FirstOrDefault();
                    var wydarzenieMiot = new WydarzenieMiot()
                    {
                        Miot = wydarzenieMiotEntity ?? throw new InvalidOperationException("Miot nie istnieje."),
                        Wydarzenie = wydarzenie,
                    };
                    _context.Add(wydarzenieMiot);
                }
            }

            if(lochaId != null)
            {
               foreach(var id in lochaId)
                {
                    var wydarzenieLochaEntity = _context.Lochy
                    .Where(a => a.Id == id).FirstOrDefault();
                    var wydarzenieLocha = new WydarzenieLocha()
                    {
                        Locha = wydarzenieLochaEntity ?? throw new InvalidOperationException("Locha nie istnieje."),
                        Wydarzenie = wydarzenie,
                    };
                    _context.Add(wydarzenieLocha);
                }
            }
            if(wydarzenie.DataWykonania == default)
            {
                wydarzenie.DataWykonania = wydarzenie.DataWydarzenia;
            }
            _context.Add(wydarzenie);
            return Save();
        }
        
        public ICollection<Wydarzenie> GetWydarzenia()
        {
            return [.. _context.Wydarzenia
                .Include(w => w.WydarzeniaLoch)
                .ThenInclude(wl => wl.Locha)
                .Include(w => w.WydarzeniaMioty)
                .ThenInclude(wm => wm.Miot)
                .OrderBy(w => w.Id)];
        }

        public Wydarzenie GetWydarzenie(int wydarzenieId)
        {
            var wydarzenie = _context.Wydarzenia.Where(w => w.Id == wydarzenieId)
                .Include(w => w.WydarzeniaLoch)
                .ThenInclude(wl => wl.Locha)
                .Include(w => w.WydarzeniaMioty)
                .ThenInclude(wm => wm.Miot)
                .FirstOrDefault() ?? throw new InvalidOperationException("Wydarzenie nie istnieje.");
            return wydarzenie;
        }
        public bool UpdateWydarzenie(Wydarzenie wydarzenie, List<int>? miotId, List<int>? lochaId)
        {
            wydarzenie.Uwagi ??= "brak";
            wydarzenie.DataCzasModyfikacji = DateTime.Now;
            wydarzenie.DataCzasUtworzenia = _context.Wydarzenia.Where(l => l.Id == wydarzenie.Id)
                .Select(l => l.DataCzasUtworzenia)
                .FirstOrDefault();

            foreach(var wydarzenieMiot in wydarzenie.WydarzeniaMioty)
            {
                if(miotId == null || !miotId.Contains(wydarzenieMiot.MiotId))
                {
                    _context.Remove(wydarzenieMiot);
                }
            }
            foreach(var wydarzenieLocha in wydarzenie.WydarzeniaLoch)
            {
                if(lochaId == null || !lochaId.Contains(wydarzenieLocha.LochaId))
                {
                    _context.Remove(wydarzenieLocha);
                }
            }
            if(miotId != null)
            {
                foreach(var id in miotId)
                {
                    if(_context.WydarzeniaMiotu.Where(wm => wm.MiotId == id && wm.WydarzenieId == wydarzenie.Id).FirstOrDefault() == null)
                        {
                        var wydarzenieMiotEntity = _context.Mioty.Where(a => a.Id == id).FirstOrDefault();
                        var wydarzenieMiot = new WydarzenieMiot()
                        {
                            Miot = wydarzenieMiotEntity,
                            Wydarzenie = wydarzenie,
                        };
                        _context.Add(wydarzenieMiot);
                    }
               }
            }

            if(lochaId != null)
            {
                foreach(var id in lochaId)
                {
                    if(_context.WydarzeniaLochy.Where(wl => wl.LochaId == id && wl.WydarzenieId == wydarzenie.Id).FirstOrDefault() == null)
                    {
                        var wydarzenieLochaEntity = _context.Lochy.Where(a => a.Id == id).FirstOrDefault();
                        var wydarzenieLocha = new WydarzenieLocha()
                        {
                            Locha = wydarzenieLochaEntity ?? throw new InvalidOperationException("Locha nie istnieje."),
                            Wydarzenie = wydarzenie,
                        };
                        _context.Add(wydarzenieLocha);
                    }
                }
            }
            if(wydarzenie.DataWykonania == default)
            {
                wydarzenie.DataWykonania = wydarzenie.DataWydarzenia;
            }
            
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
            return _context.Wydarzenia.Any(w => w.Id == wydarzenieId);
        }
    }
}