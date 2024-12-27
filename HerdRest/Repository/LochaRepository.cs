using HerdRest.Data;
using HerdRest.Dto;
using HerdRest.Interfaces;
using HerdRest.Model;
using Microsoft.EntityFrameworkCore;
using Npgsql.Internal;

namespace HerdRest.Repository
{
    public class LochaRepository : ILochaRepository
    {
        private readonly DataContext _context;
        
        public LochaRepository(DataContext context)
        {
            _context = context;
        }
        public LochaDto MapToDto(Locha locha) => new()
        {
            Id = locha.Id,
            NumerLochy = locha.NumerLochy,
            Status = locha.Status,
            IndeksProdukcji365Dni = _context.Mioty.Where(m => m.Locha.Id == locha.Id).Select(m => m.Odsadzone).Sum(),
            DataCzasUtworzenia = locha.DataCzasUtworzenia,
            DataCzasModyfikacji = locha.DataCzasModyfikacji,
            MiotyId = locha.Mioty.Select(m => m.Id).ToList() ?? [],
            WydarzeniaLochyId = _context.WydarzeniaLochy.Where(w => w.LochaId == locha.Id).Select(w => w.WydarzenieId).ToList() ?? []
        };
        public List<LochaDto> MapToDtoList(List<Locha> lochy)
        {
            return lochy.Select(MapToDto).ToList();
        }
        public Locha MapToModel(LochaDto lochaDto) => new()
        {
            Id = lochaDto.Id,
            NumerLochy = lochaDto.NumerLochy,
            Status = lochaDto.Status,
            DataCzasUtworzenia = lochaDto.DataCzasUtworzenia,
            DataCzasModyfikacji = lochaDto.DataCzasModyfikacji,
            Mioty = _context.Mioty.Where(m => m.Locha.Id == lochaDto.Id).ToList() ?? [],
            WydarzeniaLoch = _context.WydarzeniaLochy.Where(w => w.LochaId == lochaDto.Id).ToList() ?? []
        };
        public bool CreateLocha(Locha locha, List<int>? wydarzenieId)
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
            return Save();
        }

        public ICollection<Locha> GetLochy()
        {
            var lochy = _context.Lochy.OrderBy(p => p.Id).ToList();
            foreach(var locha in lochy)
            {
                locha.Mioty = [.. _context.Mioty.Where(m => m.Locha.Id == locha.Id)];
            }
            return lochy;
        }

        public Locha GetLocha(int lochaId)
        {
            var locha = _context.Lochy.Where(l => l.Id == lochaId).FirstOrDefault();
            locha.Mioty = [.. _context.Mioty.Where(m => m.Locha.Id == locha.Id)];
            return locha;
        }

        public ICollection<Wydarzenie> GetWydarzeniaLochy(int lochaId)
        {
            return [.. _context.WydarzeniaLochy.Where(w => w.LochaId == lochaId).Select(w => w.Wydarzenie)];
        }

        public bool UpdateLocha(Locha locha, List<int>? wydarzenieId)
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
            return Save();
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