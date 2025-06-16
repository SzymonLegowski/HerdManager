using HerdRest.Data;
using HerdRest.Dto;
using HerdRest.Enums;
using HerdRest.Interfaces;
using HerdRest.Model;
using Microsoft.EntityFrameworkCore;

namespace HerdRest.Repository
{
    public class LochaRepository : ILochaRepository
    {
        private readonly DataContext _context;
        public LochaRepository(DataContext context)
        {
            _context = context;
        }
        public LochaDto MapToDto(Locha locha, bool sortWydarzenia) => new()
        {
            Id = locha.Id,
            NumerLochy = locha.NumerLochy,
            Status = locha.Status,
            Uwagi = locha.Uwagi,
            IndeksProdukcji365Dni = locha.IndeksProdukcji365Dni,
            DataCzasUtworzenia = locha.DataCzasUtworzenia.ToString("yyyy-MM-dd HH:mm:ss"),
            DataCzasModyfikacji = locha.DataCzasModyfikacji.ToString("yyyy-MM-dd HH:mm:ss"),
            DataBrakowania = locha.DataBrakowania,
            MiotyId = locha.Mioty?.Select(m => m.Id).ToList() ?? [],
            WydarzeniaLochyId = sortWydarzenia
                ? locha.WydarzeniaLochy?
                    .Where(wl => wl.Wydarzenie != null)
                    .OrderBy(wl => wl.Wydarzenie.DataWydarzenia)
                    .Select(w => w.WydarzenieId)
                    .ToList() ?? []
                : locha.WydarzeniaLochy?
                    .Select(wl => wl.WydarzenieId)
                    .ToList() ?? []
        };
        public List<LochaDto> MapToDtoList(List<Locha> lochy)
        {
            return [.. lochy.Select(locha => MapToDto(locha, false))];
        }
        public Locha MapToModel(LochaDto lochaDto) => new()
        {
            Id = lochaDto.Id,
            NumerLochy = lochaDto.NumerLochy,
            Status = lochaDto.Status,
            Uwagi = lochaDto.Uwagi,
            DataCzasUtworzenia = DateTime.Now,
            DataCzasModyfikacji = DateTime.Now,
            DataBrakowania = lochaDto.DataBrakowania,
            Mioty = _context.Mioty.Where(m => m.Locha.Id == lochaDto.Id).ToList() ?? [],
            WydarzeniaLochy = _context.WydarzeniaLochy.Where(w => w.LochaId == lochaDto.Id).ToList() ?? []
        };
        public (bool, int) CreateLocha(Locha locha, List<int>? wydarzenieId)
        {
            if(wydarzenieId != null)
            {
                foreach(var id in wydarzenieId)
                {
                    var wydarzenieLochaEntity = _context.Wydarzenia
                    .Where(w => w.Id == id).FirstOrDefault();
                    var wydarzenieLocha = new WydarzenieLocha()
                    {
                        Locha = locha,
                        Wydarzenie = wydarzenieLochaEntity,
                    };
                    _context.Add(wydarzenieLocha);
                }
            }
            _context.Add(locha);
            return (Save(), locha.Id);
        }

        public ICollection<Locha> GetLochy()
        {
            var lochy = _context.Lochy.OrderBy(p => p.Id)
                        .Include(l => l.Mioty)
                        .Include(l => l.WydarzeniaLochy)
                        .AsSingleQuery()
                        .ToList();
            return lochy;
        }

        public ICollection<Locha> GetLochyByStatus(int status)
        {
            var lochy = _context.Lochy
                        .Where(l => l.Status == (StatusLochy)status)
                        .Include(l => l.Mioty)
                        .Include(l => l.WydarzeniaLochy)
                        .ToList();
            return lochy;
        }

        public Locha GetLocha(int lochaId)
        {
            var locha = _context.Lochy.Include(l => l.Mioty)
                        .Include(l => l.WydarzeniaLochy)
                        .ThenInclude(wl => wl.Wydarzenie)
                        .FirstOrDefault(l => l.Id == lochaId) ?? throw new InvalidOperationException("Locha nie istnieje.");
            return locha;
        }
        public (bool, int) UpdateLocha(Locha locha, List<int>? wydarzenieId)
        {
            locha.DataCzasModyfikacji = DateTime.Now;
            locha.DataCzasUtworzenia = _context.Lochy.Where(l => l.Id == locha.Id)
                .Select(l => l.DataCzasUtworzenia)
                .FirstOrDefault();
        
            if(wydarzenieId != null)
            {
                foreach(var id in wydarzenieId)
                {
                    var wydarzenieLochaEntity = _context.Wydarzenia
                    .Where(w => w.Id == id).FirstOrDefault();
                    var wydarzenieLocha = new WydarzenieLocha()
                    {
                        Locha = locha,
                        Wydarzenie = wydarzenieLochaEntity,
                    };
                    _context.Add(wydarzenieLocha);
                }
            }
            _context.Update(locha);
            return (Save(), locha.Id);
        }

        public bool DeleteLocha(Locha locha)
        {
            _context.Remove(locha);
            return Save();
        }

        public bool Save()
        {
            var Saved = _context.SaveChanges();
            return Saved > 0;
        }

        public bool LochaExists(int lochaId)
        {
            return _context.Lochy.Any(l => l.Id == lochaId);
        }
    }
    
}