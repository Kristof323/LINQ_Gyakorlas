using System.Globalization;

namespace LINQ_Gyakorlas
{
    internal class Program
    {
        static void Main(string[] args)
        {
            List<Diak> diakok = [];
            bool isFirstLine = true;
            var hu = new CultureInfo("hu-HU");

            foreach (string diakLine in File.ReadLines("diakok.csv"))
            {
                if (isFirstLine)
                {
                    isFirstLine = false;
                    continue;
                }

                string[] diakStringArr = diakLine.Trim().Split(";");
                Diak diak = new Diak(
                    diakStringArr[0],
                    Convert.ToInt32(diakStringArr[1]),
                    diakStringArr[2],
                    Convert.ToDouble(diakStringArr[3], hu));
                diakok.Add(diak);
            }

            // 1. Mennyi az osztályátlag az egyes osztályokban?
            Console.WriteLine("1. Osztályátlagok:");
            var osztalyAtlagok = diakok
                .GroupBy(d => d.Osztaly)
                .Select(g => new { Osztaly = g.Key, Atlag = g.Average(d => d.Atlag) })
                .OrderBy(x => x.Osztaly);

            foreach (var oa in osztalyAtlagok)
            {
                Console.WriteLine($"{oa.Osztaly}: {oa.Atlag:F2}");
            }

            // 2. Rangsorolni az osztályokat a tanulmányi átlaguk alapján csökkenő sorrendben
            Console.WriteLine("\n2. Osztályok rangsorolva csökkenő átlag szerint:");
            var rangsor = diakok
                .GroupBy(d => d.Osztaly)
                .Select(g => new { Osztaly = g.Key, Atlag = g.Average(d => d.Atlag) })
                .OrderByDescending(x => x.Atlag);

            foreach (var r in rangsor)
            {
                Console.WriteLine($"{r.Osztaly}: {r.Atlag:F2}");
            }

            // 3. Ki a legjobb tanuló és mennyi az átlaga az egyes osztályokban?
            Console.WriteLine("\n3. Legjobb tanuló(osztályonként, egyenlő esetben mindenkinek):");
            var legjobbak = diakok
                .GroupBy(d => d.Osztaly)
                .SelectMany(g =>
                {
                    double max = g.Max(d => d.Atlag);
                    return g.Where(d => d.Atlag == max)
                        .Select(d => new { Osztaly = g.Key, Nev = d.Nev, Atlag = d.Atlag });
                })
                .OrderBy(x => x.Osztaly);

            foreach (var l in legjobbak)
            {
                Console.WriteLine($"{l.Osztaly}: {l.Nev} ({l.Atlag:F2})");
            }

            // 4. Statisztika, hogy hány adott életkorú tanuló van
            Console.WriteLine("\n4. Tanulók száma életkoronként:");
            var eletkorStat = diakok
                .GroupBy(d => d.Eletkor)
                .Select(g => new { Eletkor = g.Key, Darab = g.Count() })
                .OrderBy(x => x.Eletkor);

            foreach (var e in eletkorStat)
            {
                Console.WriteLine($"{e.Eletkor} éves: {e.Darab} fő");
            }

            // 5. Van-e olyan diák, akinek az átlaga 2,0 alatti?
            Console.WriteLine("\n5. Diákok 2,0 alatti átlaggal:");
            var alacsonyak = diakok.Where(d => d.Atlag < 2.0).ToList();

            if (alacsonyak.Any())
            {
                foreach (var d in alacsonyak)
                {
                    Console.WriteLine(d.Nev);
                }
            }
            else
            {
                Console.WriteLine("Nincs olyan diák, akinek az átlaga 2,0 alatti.");
            }
        }
    }
}
