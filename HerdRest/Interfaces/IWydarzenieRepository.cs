using HerdRest.Model;

namespace HerdRest.Interfaces
{
    public interface IWydarzenieRepository
    {
        bool CreateWydarzenie(Wydarzenie wydarzenie, int? miotId, int? lochaId);
        ICollection<Wydarzenie> GetWydarzenia();
        Wydarzenie GetWydarzenie(int wydarzenieId);
        bool UpdateWydarzenie(Wydarzenie wydarzenie);
        bool DeleteWydarzenie(Wydarzenie wydarzenie);
        bool Save();
        bool WydarzenieExists(int wydarzenieId);

    }
}