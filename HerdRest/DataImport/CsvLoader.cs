using System;
using HerdRest.Data;
using HerdRest.Interfaces;

namespace HerdRest.DataImport;

public class CsvLoader : ICsvLoader
{
    private readonly DataContext _context;
    public CsvLoader(DataContext context)
    {
        _context = context;
    }

    public bool LoadDataFromCsv(string data)
    {
        Console.WriteLine(data);
        return true;
    }
}
