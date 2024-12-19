using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HerdRest.Dto;
using HerdRest.Model;

namespace HerdRest.Interfaces
{
    public interface IMiotRepository
    {
        MiotDto MapToDto(Miot miot);
        List<MiotDto> MapToDtoList(List<Miot> mioty);
        bool CreateMiot(Miot miot);
        ICollection<Miot> GetMioty();
        Miot GetMiot(int miotId);
        bool UpdateMiot(Miot miot);
        bool DeleteMiot(Miot miot);
        bool Save();
        bool MiotExists(int miotId);
    }
}