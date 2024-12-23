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
    public class MiotRepository(DataContext context) : IMiotRepository
    {
        private readonly DataContext _context = context;

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
                WydarzeniaMiotuId = miot.WydarzeniaMiotu.Select(wm => wm.WydarzenieId).ToList() ?? []
                
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
                Locha = _context.Lochy.Where(l => l.Id == miotDto.LochaId).FirstOrDefault(),
                WydarzeniaMiotu = [.. _context.WydarzeniaMiotu.Where(w => w.MiotId == miotDto.Id)]
            };
        }

        public bool CreateMiot(Miot miot, int wydarzenieKrycie)
        {
            var wydarzenieMiotEntity = _context.Wydarzenia.Where(a => a.Id == wydarzenieKrycie).FirstOrDefault();
            var wydarzenieMiot = new WydarzenieMiot
            {
                Miot = miot,
                Wydarzenie = wydarzenieMiotEntity 
            };
            
            var PrzewidywaneProszenie = new Wydarzenie
            {
                TypWydarzenia = Enums.TypWydarzenia.PrzewidywaneProszenie,
                DataWydarzenia = wydarzenieMiotEntity.DataWydarzenia.AddDays(114),
                DataCzasUtworzenia = DateTime.Now,
                DataCzasModyfikacji = DateTime.Now
            };

            var Odsadzanie = new Wydarzenie
            {
                TypWydarzenia = Enums.TypWydarzenia.Odsadzanie,
                DataWydarzenia = wydarzenieMiotEntity.DataWydarzenia.AddDays(35),
                DataCzasUtworzenia = DateTime.Now,
                DataCzasModyfikacji = DateTime.Now
            };

            var PrzewidywaneProszenieLocha = new WydarzenieLocha
            {
                Locha = miot.Locha,
                Wydarzenie = PrzewidywaneProszenie
            };

            var OdsadzanieLocha = new WydarzenieLocha
            {
                Locha = miot.Locha,
                Wydarzenie = Odsadzanie
            };

            var PrzewidywaneProszenieMiot = new WydarzenieMiot
            {
                Miot = miot,
                Wydarzenie = PrzewidywaneProszenie
            };

            var OdsadzanieMiot = new WydarzenieMiot
            {
                Miot = miot,
                Wydarzenie = Odsadzanie
            };  
            
            _context.Add(miot);
            _context.Add(wydarzenieMiot);
            _context.Add(PrzewidywaneProszenie);
            _context.Add(Odsadzanie);
            _context.Add(PrzewidywaneProszenieMiot);
            _context.Add(OdsadzanieMiot);
            _context.Add(PrzewidywaneProszenieLocha);
            _context.Add(OdsadzanieLocha);
            return Save();
        }
        public ICollection<Miot> GetMioty()
        {
            return [.. _context.Mioty
                .Include(m => m.Locha)
                .Include(m => m.WydarzeniaMiotu)
                .ThenInclude(m => m.Wydarzenie)
                .OrderBy(m => m.Id)];
        }
        public Miot GetMiot(int miotId)
        {
            var miot = _context.Mioty
                .Include(m => m.Locha)
                .Include(m => m.WydarzeniaMiotu)
                .ThenInclude(m => m.Wydarzenie)
                .FirstOrDefault(m => m.Id == miotId) ?? throw new InvalidOperationException("Miot nie istnieje.");
            return miot;
        }
        public bool UpdateMiot(Miot miot, List<int> wydarzeniaMiotuId)
        {
            foreach(var wydarzenieId in wydarzeniaMiotuId)
            {
                var wydarzenieMiotEntity = _context.Wydarzenia.Where(a => a.Id == wydarzenieId).FirstOrDefault();
                var wydarzenieMiot = new WydarzenieMiot
                {
                    Miot = miot,
                    Wydarzenie = wydarzenieMiotEntity 
                };
                _context.Add(wydarzenieMiot);
            }

            miot.DataCzasModyfikacji = DateTime.Now;
            miot.DataCzasUtworzenia = _context.Mioty.Where(l => l.Id == miot.Id)
                .Select(l => l.DataCzasUtworzenia)
                .FirstOrDefault();

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
            return Saved > 0;
        }
        public bool MiotExists(int miotId)
        {
            return _context.Mioty.Any(e => e.Id == miotId);
        }
    }
}