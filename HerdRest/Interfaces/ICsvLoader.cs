using System;

namespace HerdRest.Interfaces;

public interface ICsvLoader
{
    bool LoadDataFromCsv(string data);
}
