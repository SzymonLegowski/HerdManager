using HerdRest.Dto;
using HerdRest.Model;

namespace HerdRest.Interfaces
{
    public interface IMiotRepository
    {
        MiotDto MapToDto(Miot miot);
        List<MiotDto> MapToDtoList(List<Miot> mioty);
        Miot MapToModel(MiotDto miotDto);
        (bool, int) CreateMiot(Miot miot, int wydarzenieKrycie);
        ICollection<Miot> GetMioty();
        ICollection<Miot> GetMiotyWDanymMiesiacu(int rok, int miesiac);
        Miot GetMiot(int miotId);
        (bool, int) UpdateMiot(Miot miot, List<int> wydarzeniaMiotuId);
        bool AddOdsadzenia(OdsadzanieDto odsadzanieDto);
        bool DeleteMiot(Miot miot);
        bool Save();
        bool MiotExists(int miotId);
    }
}