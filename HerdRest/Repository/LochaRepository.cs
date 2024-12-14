using HerdRest.Data;
using HerdRest.Interfaces;
using HerdRest.Model;

namespace HerdRest.Repository
{
    public class LochaRepository : ILochaRepository
    {
        private readonly DataContext _context;
        
        public LochaRepository(DataContext context)
        {
            _context = context;
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