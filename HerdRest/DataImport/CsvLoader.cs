using System;
using System.Collections.Generic;
using HerdRest.Data;
using HerdRest.Enums;
using HerdRest.Interfaces;
using HerdRest.Model;
using HerdRest.Repository;

namespace HerdRest.DataImport;

public class CsvLoader(ILochaRepository lochaRepository, IMiotRepository miotRepository, IWydarzenieRepository wydarzenieRepository, DataContext context) : ICsvLoader
{
    private readonly ILochaRepository _lochaRepository = lochaRepository;
    private readonly IMiotRepository _miotRepository = miotRepository;
    private readonly IWydarzenieRepository _wydarzenieRepository = wydarzenieRepository;
    private readonly DataContext _context = context;
    

    public string LoadDataFromCsv(string filePath)
    {
        int idLochy = 0;
        int idWydarzenia = 0;
        bool Zgon = false;
        bool Sprzedana = false;
        foreach (string line in File.ReadLines(filePath))
        {
            if (!line.Contains('/'))
            {
                Zgon = false;
                Sprzedana = false;
                string[] parts = line.Split(';');
                int.TryParse(parts[0], out int numerLochy);
                Locha locha = new();
                locha.Uwagi = parts[1];
                if (parts.Length > 2)
                {
                    DateOnly.TryParse(parts[2], out DateOnly dataBrakowania);
                    locha.DataBrakowania = dataBrakowania;
                    if (parts[3] == "Zgon") Zgon = true; else Sprzedana = true;
                }
                else
                {
                    locha.Status = StatusLochy.Luzna;
                }
                locha.NumerLochy = numerLochy;
                var CreateOutput = _lochaRepository.CreateLocha(locha, null);
                if (!CreateOutput.Item1)
                {
                    return $"Błąd przy zapisywaniu lochy nr {parts[0]}";
                }
                idLochy = CreateOutput.Item2;
            }
            else
            {
                string[] Sektory = line.Split('/');
                string[] SektorKrycia = Sektory[0].Split(';');
                string[] SektorPorodowki = Sektory[1].Split(';');
                int SektorKryciaDlugosc = SektorKrycia.Length;
                int SektorPorodowkiDlugosc = SektorPorodowki.Length;
                for (int index = 0; index < SektorKryciaDlugosc; index++)
                {

                    if (DateOnly.TryParse(SektorKrycia[index], out DateOnly DataKrycia))
                    {
                        var CzyIstnieje = _context.Wydarzenia.Where(w => w.DataWydarzenia == DataKrycia).FirstOrDefault();
                        if (CzyIstnieje != null)
                        {
                            idWydarzenia = CzyIstnieje.Id;
                            WydarzenieLocha wydarzenieLocha = new()
                            {
                                Locha = _context.Lochy.Where(l => l.Id == idLochy).FirstOrDefault(),
                                Wydarzenie = CzyIstnieje,
                            };
                            _context.Add(wydarzenieLocha);
                            _context.SaveChanges();
                        }
                        else
                        {
                            Wydarzenie wydarzenie = new();
                            wydarzenie.DataWydarzenia = DataKrycia;
                            wydarzenie.TypWydarzenia = TypWydarzenia.Krycie;
                            if (SektorKryciaDlugosc - 1 > index)
                            {
                                if (!DateOnly.TryParse(SektorKrycia[index + 1], out DateOnly NastepnaData))
                                {
                                    wydarzenie.Uwagi = SektorKrycia[index + 1];
                                }
                            }
                            List<int> idLoch = [idLochy];
                            var createOutput = _wydarzenieRepository.CreateWydarzenie(wydarzenie, null, idLoch);
                            if (!createOutput.Item1)
                            {
                                return $"Wystąpił problem przy zapisie krycia z datą {wydarzenie.DataWydarzenia}";
                            }
                            idWydarzenia = createOutput.Item2;
                        }
                    }
                }
                if (SektorPorodowkiDlugosc > 0 && SektorPorodowki[0] != "")
                {
                    Miot miot = new();
                    miot.Locha = _context.Lochy.Where(l => l.Id == idLochy).FirstOrDefault();
                    if (DateOnly.TryParse(SektorPorodowki[0], out DateOnly DataProszenia))
                    {
                        miot.DataProszenia = DataProszenia;
                        miot.Locha.Status = StatusLochy.Karmiaca;
                    }
                    if (SektorPorodowkiDlugosc > 1)
                    {
                        if (DateOnly.TryParse(SektorPorodowki[1], out DateOnly DataOdsadzenia))
                        {
                            miot.DataOdsadzenia = DataOdsadzenia;
                            miot.Locha.Status = StatusLochy.Luzna;
                        }
                        if (SektorPorodowkiDlugosc > 2)
                        {
                            _ = int.TryParse(SektorPorodowki[2], out int urodzoneZywe);
                            miot.UrodzoneZywe = urodzoneZywe;
                            if (SektorPorodowkiDlugosc > 3)
                            {
                                _ = int.TryParse(SektorPorodowki[3], out int urodzoneMartwe);
                                miot.UrodzoneMartwe = urodzoneMartwe;
                                if (SektorPorodowkiDlugosc > 4)
                                {
                                    _ = int.TryParse(SektorPorodowki[4], out int przygniecione);
                                    miot.Przygniecone = przygniecione;
                                    if (SektorPorodowkiDlugosc > 5)
                                    {
                                        _ = int.TryParse(SektorPorodowki[5], out int odsadzone);
                                        miot.Odsadzone = odsadzone;
                                        if (SektorPorodowkiDlugosc > 6)
                                        {
                                            _ = int.TryParse(SektorPorodowki[6], out int ocena);
                                            miot.Ocena = ocena;
                                        }
                                    }
                                }
                            }
                        }
                    }
                    var createOutput = _miotRepository.CreateMiot(miot, idWydarzenia);
                    if (!createOutput.Item1) return $"Błąd podczas dodawania miotu do lochy nr {miot.Locha.Id}";
                    if (Sprzedana) miot.Locha.Status = StatusLochy.Sprzedana;
                    if (Zgon) miot.Locha.Status = StatusLochy.Zgon;
                    _context.Lochy.Update(miot.Locha);
                }
            }
        }
        return "Dodano pomyślnie";
    }
}
