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
                DataPrzewidywanegoProszenia = miotDto.DataPrzewidywanegoProszenia,
                DataProszenia = miotDto.DataProszenia,
                DataOdsadzenia = miotDto.DataOdsadzenia,
                DataCzasUtworzenia = miotDto.DataCzasUtworzenia,
                DataCzasModyfikacji = miotDto.DataCzasModyfikacji,
                Locha = _context.Lochy.Where(l => l.Id == miotDto.LochaId).FirstOrDefault() ?? throw new InvalidOperationException("Podany Miot nie ma lochy."),
                WydarzeniaMiotu = [.. _context.WydarzeniaMiotu.Where(w => w.MiotId == miotDto.Id)]
            };
        }

        public bool CreateMiot(Miot miot, int wydarzenieKrycieId)
        {
            var wydarzenieMiotEntity = _context.Wydarzenia.Where(a => a.Id == wydarzenieKrycieId).FirstOrDefault();
            var wydarzenieMiot = new WydarzenieMiot
            {
                Miot = miot,
                Wydarzenie = wydarzenieMiotEntity ?? throw new InvalidOperationException("Wydarzenie nie istnieje.") 
            };
            miot.DataPrzewidywanegoProszenia = wydarzenieMiotEntity.DataWydarzenia.AddDays(114);

            _context.Add(wydarzenieMiot);
            _context.Add(miot);
            return Save();
        }

          public bool ImportMiotyFromFile(string FilePath)
        {
            if(!File.Exists(FilePath))
                return false;

            List<Miot> Mioty = [];
            string[] lines = File.ReadAllLines(FilePath);
            foreach(string line in lines){
                string[] parts = line.Split(';');
                if(parts.Length == 9){
                    Miot Miot = new()
                    {
                        UrodzoneZywe = int.Parse(parts[2]),
                        UrodzoneMartwe = int.Parse(parts[3]),
                        Przygniecone = int.Parse(parts[4]),
                        Odsadzone = int.Parse(parts[5]),
                        Ocena = int.Parse(parts[6]),
                        DataProszenia = DateOnly.Parse(parts[0]),
                        Locha = _context.Lochy.Where(l => l.Id == int.Parse(parts[7])).FirstOrDefault() ?? throw new InvalidOperationException("Podana locha nie istnieje.")
                    };
                    if(parts[1] != "0")
                    {
                        Miot.DataOdsadzenia = DateOnly.Parse(parts[1]);
                    }
                    var krycie = _context.Wydarzenia.Where(w => w.Id == int.Parse(parts[8])).FirstOrDefault() ?? throw new InvalidOperationException("Podane wydarzenie nie istnieje.");
                    var wydarzenieMiot = new WydarzenieMiot()
                    {
                        Miot = Miot,
                        Wydarzenie = krycie
                    };
                    Miot.DataPrzewidywanegoProszenia = krycie.DataWydarzenia.AddDays(114);
                    Mioty.Add(Miot);
                    _context.Add(wydarzenieMiot);
                }
            }
            _context.AddRange(Mioty);
            return Save();
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
        public Miot GetMiot(int miotId)
        {
            var miot = _context.Mioty.Include(m => m.Locha)
                .Include(m => m.WydarzeniaMiotu)
                .ThenInclude(m => m.Wydarzenie)
                .FirstOrDefault(m => m.Id == miotId) ?? throw new InvalidOperationException("Miot nie istnieje.");
            return miot;
        }
        public bool UpdateMiot(Miot miot, List<int> wydarzeniaMiotuId)
        {
            if (wydarzeniaMiotuId != null)
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
            if(wydarzeniaMiotuId.Count == 0)
            {
                miot.DataPrzewidywanegoProszenia = _context.Mioty.Where(m => m.Id == miot.Id)
                    .Select(m => m.DataPrzewidywanegoProszenia)
                    .FirstOrDefault();
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