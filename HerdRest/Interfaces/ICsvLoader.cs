using System;

namespace HerdRest.Interfaces;

public interface ICsvLoader
{
    string LoadDataFromCsv(string data);
}
