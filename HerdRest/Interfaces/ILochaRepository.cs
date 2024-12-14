using HerdRest.Model;

namespace HerdRest.Interfaces
{
    public interface ILochaRepository
    {
        bool CreateLocha(Locha locha);
        ICollection<Locha> GetLochy();
        Locha GetLocha(int lochaId);
        bool UpdateLocha(Locha locha);
        bool DeleteLocha(Locha locha);
        bool Save();
        bool LochaExists(int lochaId);
    }
}