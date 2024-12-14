using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HerdRest.Data;
using HerdRest.Interfaces;
using HerdRest.Model;

namespace HerdRest.Repository
{
    public class WydarzenieRepository : IWydarzenieRepository
    {
        private readonly DataContext _context;

        public WydarzenieRepository(DataContext context)
        {
            context = _context;
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

        public bool UpdateWydarzenie(Wydarzenie wydarzenie)
        {
            throw new NotImplementedException();
        }

        public bool DeleteWydarzenie(Wydarzenie wydarzenie)
        {
            throw new NotImplementedException();
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