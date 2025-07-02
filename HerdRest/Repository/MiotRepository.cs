using HerdRest.Data;
using HerdRest.Dto;
using HerdRest.Enums;
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
                DataProszenia = miot.DataProszenia,
                DataOdsadzenia = miot.DataOdsadzenia,
                DataPrzewidywanegoProszenia = miot.DataPrzewidywanegoProszenia,
                DataCzasUtworzenia = miot.DataCzasUtworzenia,
                DataCzasModyfikacji = miot.DataCzasModyfikacji,
                LochaId = miot.Locha.Id,
                NumerLochy = miot.Locha.NumerLochy,
                WydarzeniaMiotuId = miot.WydarzeniaMiotu.Select(wm => wm.WydarzenieId).ToList() ?? []
            };
        }
        public List<MiotDto> MapToDtoList(List<Miot> mioty)
        {
            return mioty.Select(MapToDto).ToList();
        }

        public Miot MapToModel(MiotDto miotDto) => new()
        {
            Id = miotDto.Id,
            UrodzoneZywe = miotDto.UrodzoneZywe,
            UrodzoneMartwe = miotDto.UrodzoneMartwe,
            Przygniecone = miotDto.Przygniecone,
            Odsadzone = miotDto.Odsadzone,
            Ocena = miotDto.Ocena,
            DataPrzewidywanegoProszenia = miotDto.DataPrzewidywanegoProszenia,
            DataProszenia = miotDto.DataProszenia,
            DataOdsadzenia = miotDto.DataOdsadzenia,
            DataCzasUtworzenia = miotDto.DataCzasUtworzenia,
            DataCzasModyfikacji = miotDto.DataCzasModyfikacji,
            Locha = _context.Lochy.Where(l => l.Id == miotDto.LochaId).FirstOrDefault() ?? throw new InvalidOperationException("Podany Miot nie ma lochy."),
            WydarzeniaMiotu = [.. _context.WydarzeniaMiotu.Where(w => w.MiotId == miotDto.Id)]
        };

        public (bool, int) CreateMiot(Miot miot, int wydarzenieKrycieId)
        {
            var wydarzenieMiotEntity = _context.Wydarzenia.Where(a => a.Id == wydarzenieKrycieId).FirstOrDefault();
            var wydarzenieMiot = new WydarzenieMiot
            {
                Miot = miot,
                Wydarzenie = wydarzenieMiotEntity ?? throw new InvalidOperationException("Wydarzenie nie istnieje.") 
            };
            miot.DataPrzewidywanegoProszenia = wydarzenieMiotEntity.DataWydarzenia.AddDays(114);

            miot.Locha.Status = StatusLochy.Prosna;

            if (miot.DataProszenia != null)
                miot.Locha.Status = StatusLochy.Karmiaca;

            if (miot.DataOdsadzenia != null)
                miot.Locha.Status = StatusLochy.Luzna;

            _context.Update(miot.Locha);
            _context.Add(wydarzenieMiot);
            _context.Add(miot);
            return (Save(), miot.Id);
        }
        public ICollection<Miot> GetMioty()
        {
            var mioty = _context.Mioty
                .Include(m => m.Locha)
                .Include(m => m.WydarzeniaMiotu)
                .ThenInclude(m => m.Wydarzenie)
                .ToList();
            return mioty;
        }

        public ICollection<Miot> GetMiotyWDanymMiesiacu(int rok, int miesiac)
        {
            return [.. _context.Mioty
                .Include(m => m.Locha)
                .Include(m => m.WydarzeniaMiotu)
                .Where(m => 
                    (m.DataPrzewidywanegoProszenia.Year == rok &&
                     m.DataPrzewidywanegoProszenia.Month == miesiac) ||
                    (m.DataProszenia.HasValue &&
                     m.DataProszenia.Value.Year == rok &&
                     m.DataProszenia.Value.Month == miesiac) ||
                    (m.DataOdsadzenia.HasValue &&
                     m.DataOdsadzenia.Value.Year == rok &&
                     m.DataOdsadzenia.Value.Month == miesiac))];
        }
        public Miot GetMiot(int miotId)
        {
            var miot = _context.Mioty.Include(m => m.Locha)
                .Include(m => m.WydarzeniaMiotu)
                .ThenInclude(l => l.Wydarzenie)
                .FirstOrDefault(m => m.Id == miotId) ?? throw new InvalidOperationException("Miot nie istnieje.");
            return miot;
        }
        public (bool, int) UpdateMiot(Miot miot, List<int>? wydarzeniaMiotuId)
        {
            if (wydarzeniaMiotuId?.Count > 0){
                foreach(var wydarzenieId in wydarzeniaMiotuId)
                {
                    var CzyIstnieje = _context.WydarzeniaMiotu.Where(wm => wm.WydarzenieId == wydarzenieId && wm.MiotId == miot.Id).FirstOrDefault();
                    if(CzyIstnieje == null)
                    {
                        var wydarzenieMiotEntity = _context.Wydarzenia.Where(a => a.Id == wydarzenieId).FirstOrDefault();
                        var wydarzenieMiot = new WydarzenieMiot
                        {
                            Miot = miot,
                            Wydarzenie = wydarzenieMiotEntity ?? throw new InvalidOperationException("Wydarzenie nie istnieje.")
                        };
                        
                        _context.Add(wydarzenieMiot);

                        if(wydarzenieMiotEntity?.TypWydarzenia == TypWydarzenia.Krycie)
                        {
                            miot.DataPrzewidywanegoProszenia = wydarzenieMiotEntity.DataWydarzenia.AddDays(114);
                        }   
                    }
                    else
                    {
                        _context.WydarzeniaMiotu.Remove(CzyIstnieje);
                    }

                }
            }

            miot.DataCzasModyfikacji = DateTime.Now;
            miot.DataCzasUtworzenia = _context.Mioty.Where(l => l.Id == miot.Id)
                .Select(l => l.DataCzasUtworzenia)
                .FirstOrDefault();
            miot.Locha.Status = StatusLochy.Prosna;

            if (miot.DataProszenia != null)
                miot.Locha.Status = StatusLochy.Karmiaca;

            if (miot.DataOdsadzenia != null)
                miot.Locha.Status = StatusLochy.Luzna;

            _context.Update(miot.Locha);
            _context.Update(miot);
            return (Save(), miot.Id);
        }
        public bool AddOdsadzenia(OdsadzanieDto odsadzanieDto)
        {
            foreach (int miotId in odsadzanieDto.MiotyId)
            {
                var miot = GetMiot(miotId);
                miot.DataOdsadzenia = odsadzanieDto.DataOdsadzenia;
                miot.Locha.Status = StatusLochy.Luzna;
                _context.Update(miot);
            }
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