using HerdRest.Dto;
using HerdRest.Model;

namespace HerdRest.Interfaces
{
    public interface ILochaRepository
    {
        LochaDto MapToDto(Locha locha);
        List<LochaDto> MapToDtoList(List<Locha> lochy);
        Locha MapToModel(LochaDto lochaDto);
        bool CreateLocha(Locha locha, List<int>? wydarzenieId);
        ICollection<Locha> GetLochy();
        Locha GetLocha(int lochaId);
        bool UpdateLocha(Locha locha, List<int>? wydarzenieId);
        bool DeleteLocha(Locha locha);
        bool Save();
        bool LochaExists(int lochaId);
    }
}