using HerdRest.Dto;
using HerdRest.Model;

namespace HerdRest.Interfaces
{
    public interface IWydarzenieRepository
    {
        WydarzenieDto MapToDto(Wydarzenie wydarzenie);
        List<WydarzenieDto> MapToDtoList(List<Wydarzenie> wydarzenia);
        bool CreateWydarzenie(Wydarzenie wydarzenie, int? miotId, int? lochaId);
        ICollection<Wydarzenie> GetWydarzenia();
        Wydarzenie GetWydarzenie(int wydarzenieId);
        // bool UpdateWydarzenie(Wydarzenie wydarzenie, int? miotId, int? lochaId);
        ICollection<Wydarzenie> GetWydarzeniaLochy(int lochaId);
        ICollection<Wydarzenie> GetWydarzeniaMiotu(int miotId);
        bool UpdateWydarzenie(Wydarzenie wydarzenie);
        bool DeleteWydarzenie(Wydarzenie wydarzenie);
        bool Save();
        bool WydarzenieExists(int wydarzenieId);

    }
}