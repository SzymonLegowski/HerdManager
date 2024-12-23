using HerdRest.Dto;
using HerdRest.Model;

namespace HerdRest.Interfaces
{
    public interface IWydarzenieRepository
    {
        WydarzenieDto MapToDto(Wydarzenie wydarzenie);
        List<WydarzenieDto> MapToDtoList(List<Wydarzenie> wydarzenia);
        Wydarzenie MapToModel(WydarzenieDto wydarzenieDto);
        bool CreateWydarzenie(Wydarzenie wydarzenie, List<int>? miotId, List<int>? lochaId);
        ICollection<Wydarzenie> GetWydarzenia();
        Wydarzenie GetWydarzenie(int wydarzenieId);
        bool UpdateWydarzenie(Wydarzenie wydarzenie, List<int>? miotId, List<int>? lochaId);
        bool DeleteWydarzenie(Wydarzenie wydarzenie);
        bool Save();
        bool WydarzenieExists(int wydarzenieId);

    }
}