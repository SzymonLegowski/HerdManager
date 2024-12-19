using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HerdRest.Data;
using HerdRest.Dto;
using HerdRest.Model;

namespace HerdRest.Repository
{
    public class MiotRepository
    {
        private readonly DataContext _context;
        public MiotRepository(DataContext context)
        {
            _context = context;
        }  
        public MiotDto MapToDto(Miot miot)
        {
            return new MiotDto
            {
                Id = miot.Id,
                UrodzoneZywe = miot.UrodzoneZywe,
                UrodzoneMartwe = miot.UrodzoneMartwe,
                Przygniecone = miot.Przygniecone,
                Odsadzone = miot.Odsadzone,
                Ocena = miot.Ocena,
                DataCzasUtworzenia = miot.DataCzasUtworzenia,
                DataCzasModyfikacji = miot.DataCzasModyfikacji,
                LochaId = miot.Locha.Id,
                WydarzeniaMiotuId = miot.WydarzeniaMiotu.Select(wm => wm.WydarzenieId).ToList()
            };

        }
        public List<MiotDto> MapToDtoList(List<Miot> mioty)
        {
            return mioty.Select(MapToDto).ToList();
        }

        public Miot MapToModel(MiotDto miotDto)
        {
            return new Miot
            {
                Id = miotDto.Id,
                UrodzoneZywe = miotDto.UrodzoneZywe,
                UrodzoneMartwe = miotDto.UrodzoneMartwe,
                Przygniecone = miotDto.Przygniecone,
                Odsadzone = miotDto.Odsadzone,
                Ocena = miotDto.Ocena,
                DataCzasUtworzenia = miotDto.DataCzasUtworzenia,
                DataCzasModyfikacji = miotDto.DataCzasModyfikacji,
                Locha = new Locha { Id = miotDto.LochaId },
                WydarzeniaMiotu = miotDto.WydarzeniaMiotuId?.Select(id => new WydarzenieMiot { WydarzenieId = id }).ToList()
            };
        }

        public bool CreateMiot(Miot miot)
        {
            _context.Add(miot);
            return Save();
        }
        public ICollection<Miot> GetMioty()
        {
            return [.. _context.Mioty.OrderBy(m => m.Id)];
        }
        public Miot GetMiot(int miotId)
        {
            return _context.Mioty.FirstOrDefault(m => m.Id == miotId);
        }
        public bool UpdateMiot(Miot miot)
        {
            miot.DataCzasModyfikacji = DateTime.Now;
            miot.DataCzasUtworzenia = _context.Mioty.Where(l => l.Id == miot.Id).Select(l => l.DataCzasUtworzenia).FirstOrDefault();
            _context.Update(miot);
            return Save();
        }
        public bool DeleteMiot(Miot miot)
        {
            _context.Remove(miot);
            return Save();
        }
        public bool Save()
        {
             var Saved = _context.SaveChanges();
            return Saved > 0 ? true : false;
        }
        public bool MiotExists(int miotId)
        {
            return _context.Mioty.Any(e => e.Id == miotId);
        }
    }
}