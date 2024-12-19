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
        public LochaDto MapToDto(Locha locha)
        {
            return new LochaDto
            {   
                Id = locha.Id,
                NumerLochy = locha.NumerLochy,
                Status = locha.Status,
                IndeksProdukcji365Dni = locha.IndeksProdukcji365Dni,
                DataCzasUtworzenia = locha.DataCzasUtworzenia,
                DataCzasModyfikacji = locha.DataCzasModyfikacji,
                MiotyId = locha.Mioty?.Select(m => m.Id).ToList() ?? [],
                WydarzeniaLochId = locha.WydarzeniaLoch?.Select(w => w.WydarzenieId).ToList() ?? []
            };
        }
        public List<LochaDto> MapToDtoList(List<Locha> lochy)
        {
            return lochy.Select(MapToDto).ToList();
        }
        public Locha MapToModel(LochaDto lochaDto)
        {
            return new Locha
            {
                Id = lochaDto.Id,
                NumerLochy = lochaDto.NumerLochy,
                Status = lochaDto.Status,
                IndeksProdukcji365Dni = lochaDto.IndeksProdukcji365Dni,
                DataCzasUtworzenia = lochaDto.DataCzasUtworzenia,
                DataCzasModyfikacji = lochaDto.DataCzasModyfikacji,
                Mioty = lochaDto.MiotyId?.Select(id => new Miot { Id = id }).ToList(),
                WydarzeniaLoch = lochaDto.WydarzeniaLochId?.Select(id => new WydarzenieLocha { WydarzenieId = id }).ToList()
            };
        }
        public bool CreateLocha(Locha locha)
        {
            _context.Add(locha);
            return Save();
        }

        public ICollection<Locha> GetLochy()
        {
            return _context.Lochy.OrderBy(p => p.Id).ToList();
        }

        public Locha GetLocha(int lochaId)
        {
            return _context.Lochy.Where(l => l.Id == lochaId).FirstOrDefault();
        }

        public bool UpdateLocha(Locha locha)
        {
            locha.DataCzasModyfikacji = DateTime.Now;
            locha.DataCzasUtworzenia = _context.Lochy.Where(l => l.Id == locha.Id).Select(l => l.DataCzasUtworzenia).FirstOrDefault();
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
            return Saved > 0 ? true : false;
        }

        public bool LochaExists(int lochaId)
        {
            return _context.Lochy.Any(l => l.Id == lochaId);
        }
    }
    
}