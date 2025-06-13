using HerdRest.Data;
using HerdRest.Dto;
using HerdRest.Enums;
using HerdRest.Interfaces;
using HerdRest.Model;
using Microsoft.EntityFrameworkCore;

namespace HerdRest.Repository
{
    public class WydarzenieRepository(DataContext context) : IWydarzenieRepository
    {
        private readonly DataContext _context = context;

        public WydarzenieDto MapToDto(Wydarzenie wydarzenie) => new()
        {
            Id = wydarzenie.Id,
            TypWydarzenia = wydarzenie.TypWydarzenia,
            Uwagi = wydarzenie.Uwagi,
            Rasa = wydarzenie.Rasa,
            DataWydarzenia = wydarzenie.DataWydarzenia,
            DataCzasUtworzenia = wydarzenie.DataCzasUtworzenia.ToString("yyyy-MM-dd HH:mm:ss"),
            DataCzasModyfikacji = wydarzenie.DataCzasModyfikacji.ToString("yyyy-MM-dd HH:mm:ss"),
            LochyId = wydarzenie.WydarzeniaLoch?.Select(l => l.LochaId).ToList() ?? [],
            MiotyId = wydarzenie.WydarzeniaMioty?.Select(m => m.MiotId).ToList() ?? []
        };
        public List<WydarzenieDto> MapToDtoList(List<Wydarzenie> wydarzenia)
        {
            return [.. wydarzenia.Select(MapToDto)];
        }
        public Wydarzenie MapToModel(WydarzenieDto wydarzenieDto)
        {
            return new Wydarzenie
            {
                Id = wydarzenieDto.Id,
                TypWydarzenia = wydarzenieDto.TypWydarzenia,
                Uwagi = wydarzenieDto.Uwagi,
                Rasa = wydarzenieDto.Rasa,
                DataWydarzenia = wydarzenieDto.DataWydarzenia,
                DataCzasUtworzenia = DateTime.Now,
                DataCzasModyfikacji = DateTime.Now,
                WydarzeniaLoch = _context.WydarzeniaLochy.Where(w => w.WydarzenieId == wydarzenieDto.Id).ToList() ?? [],
                WydarzeniaMioty = _context.WydarzeniaMiotu.Where(w => w.WydarzenieId == wydarzenieDto.Id).ToList() ?? []
            };
        }
        public (bool, int) CreateWydarzenie(Wydarzenie wydarzenie, List<int>? miotId, List<int>? lochaId)
        {
            wydarzenie.Uwagi ??= "brak";
            if (miotId != null)
            {
                foreach (var id in miotId)
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
                foreach (var id in lochaId)
                {
                    var locha = _context.Lochy
                    .Where(a => a.Id == id)
                    .Include(l => l.Mioty)
                    .ThenInclude(m => m.WydarzeniaMiotu)
                    .FirstOrDefault();
                    var wydarzenieLocha = new WydarzenieLocha()
                    {
                        Locha = locha ?? throw new InvalidOperationException("Locha nie istnieje."),
                        Wydarzenie = wydarzenie,
                    };
                    if (wydarzenie.TypWydarzenia == TypWydarzenia.Krycie)
                    {
                        locha.Status = StatusLochy.Pokryta;
                        var wolnyMiot = locha.Mioty?.FirstOrDefault(m => m.DataProszenia == null);
                        if (wolnyMiot == null)
                        {
                            Miot miot = new(locha, [])
                            {
                                DataPrzewidywanegoProszenia = wydarzenie.DataWydarzenia.AddDays(114)
                            };
                            var wydarzenieMiot = new WydarzenieMiot()
                            {
                                Wydarzenie = wydarzenie,
                                Miot = miot
                            };
                            miot.WydarzeniaMiotu.Add(wydarzenieMiot);
                            _context.Add(miot);
                            _context.Add(wydarzenieMiot);
                        }
                        else
                        {
                            wolnyMiot.DataPrzewidywanegoProszenia = wydarzenie.DataWydarzenia.AddDays(114);
                            var wydarzenieMiotToRemove = wolnyMiot.WydarzeniaMiotu
                            .FirstOrDefault(w => w.Wydarzenie.TypWydarzenia == TypWydarzenia.Krycie);
                            wolnyMiot.WydarzeniaMiotu.Remove(wydarzenieMiotToRemove);
                            _context.Remove(wydarzenieMiotToRemove);
                            var wydarzenieMiot = new WydarzenieMiot()
                            {
                                Wydarzenie = wydarzenie,
                                Miot = wolnyMiot
                            };
                            wolnyMiot.WydarzeniaMiotu.Add(wydarzenieMiot);
                            _context.Update(wolnyMiot);
                            _context.Add(wydarzenieMiot);
                        }
                        _context.Update(locha);
                    }
                    _context.Add(wydarzenieLocha);
                }
            }
            _context.Add(wydarzenie);
            return (Save(), wydarzenie.Id);
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

        public ICollection<Wydarzenie> GetWydarzeniaMiesiaca(int miesiac, int rok)
        {
            return [.. _context.Wydarzenia
                .Where(w => w.DataWydarzenia.Month == miesiac && w.DataWydarzenia.Year == rok)
                .Include(w => w.WydarzeniaLoch)
                .ThenInclude(wl => wl.Locha)
                .Include(w => w.WydarzeniaMioty)
                .ThenInclude(wm => wm.Miot)
                .OrderBy(w => w.DataWydarzenia)];
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
        public (bool, int) UpdateWydarzenie(Wydarzenie wydarzenie, List<int>? miotId, List<int>? lochaId)
        {
            if(wydarzenie.DataWydarzenia == default)
                {
                    wydarzenie.DataWydarzenia = _context.Wydarzenia.Where(l => l.Id == wydarzenie.Id)
                    .Select(l => l.DataWydarzenia)
                    .FirstOrDefault();
                }
            wydarzenie.Uwagi ??= "";
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
                        var Miot = _context.Mioty.Where(a => a.Id == id).FirstOrDefault();
                        var wydarzenieMiot = new WydarzenieMiot()
                        {
                            Miot = Miot,
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
                        var locha = _context.Lochy.Where(a => a.Id == id)
                        .Include(l => l.Mioty)
                        .ThenInclude(m => m.WydarzeniaMiotu)
                        .FirstOrDefault();
                        var wydarzenieLocha = new WydarzenieLocha()
                        {
                            Locha = locha ?? throw new InvalidOperationException("Locha nie istnieje."),
                            Wydarzenie = wydarzenie,
                        };
                        if (wydarzenie.TypWydarzenia == TypWydarzenia.Krycie)
                        {
                            locha.Status = StatusLochy.Pokryta;
                            var wolnyMiot = locha.Mioty?.FirstOrDefault(m => m.DataProszenia == null);
                            if (wolnyMiot == null)
                            {
                                Miot miot = new(locha, [])
                                {
                                    DataPrzewidywanegoProszenia = wydarzenie.DataWydarzenia.AddDays(114)
                                };
                                var wydarzenieMiot = new WydarzenieMiot()
                                {
                                    Wydarzenie = wydarzenie,
                                    Miot = miot
                                };
                                miot.WydarzeniaMiotu.Add(wydarzenieMiot);
                                _context.Add(miot);
                                _context.Add(wydarzenieMiot);
                            }
                            else
                            {
                                wolnyMiot.DataPrzewidywanegoProszenia = wydarzenie.DataWydarzenia.AddDays(114);
                                var wydarzenieMiotToRemove = wolnyMiot.WydarzeniaMiotu
                                .FirstOrDefault(w => w.Wydarzenie.TypWydarzenia == TypWydarzenia.Krycie);
                                _context.Remove(wydarzenieMiotToRemove);
                                var wydarzenieMiot = new WydarzenieMiot()
                                {
                                    Wydarzenie = wydarzenie,
                                    Miot = wolnyMiot
                                };
                                wolnyMiot.WydarzeniaMiotu.Add(wydarzenieMiot);
                                _context.Update(wolnyMiot);
                                _context.Add(wydarzenieMiot);
                            }
                            _context.Update(locha);
                        }
                        _context.Add(wydarzenieLocha);
                    }
                }
            }
            _context.Update(wydarzenie);
            return (Save(), wydarzenie.Id);
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