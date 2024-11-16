using System.Collections;
using System.Diagnostics.CodeAnalysis;

struct Lepes
{
    public int Csoport;   // melyik csoportbol 
    public int X;       // hanyadik elemig (0..Group - 1) - 0: egy elemet nyomunk be, Group - 1: az osszeset elemet benyomjuk
    public int JatekosLep;
    public bool Veszit;
    public int LepesekSzamaAVegeig;

    public void Print()
    {
        Console.WriteLine($"Lepes --- Csoport: {Csoport}, X: {X}, JatekosLep: {JatekosLep}, Veszit: {Veszit}, LepesekSzamaAVegeig: {LepesekSzamaAVegeig}");
    }
}

struct Allas
{
    public List<int> Csoportok = new List<int>();
    public int JatekosLep;

    public Allas()
    {
    }

    public override bool Equals(object obj)
    {
        Allas that = ((Allas)obj);
        return this.Csoportok.SequenceEqual(that.Csoportok) || this.JatekosLep == that.JatekosLep;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Csoportok, JatekosLep);
    }
}

class Program
{
    private Dictionary<Allas, Lepes> map = new Dictionary<Allas, Lepes>();
    private int counter = 0;

    public Lepes megold(Allas allas)
    {
        if (map.ContainsKey(allas))
        {
            return map[allas];
        }

        try
        {

            // ha pontosan egy olyan csoport van, amelyikbol 1 van, a tobbi 0
            // ha a 0. elem nem 0, es a tobbi 0

            int hanyNem0Csoport = allas.Csoportok.Count(x => x != 0);
            int elsoNem0Csoport = allas.Csoportok.FindIndex(x => x != 0);

            if (hanyNem0Csoport == 1)
            {
                if (elsoNem0Csoport == 0)
                {
                    Lepes lepes1;
                    lepes1.Csoport = 0;
                    lepes1.X = 0;
                    lepes1.JatekosLep = allas.JatekosLep;
                    lepes1.Veszit = allas.Csoportok[0] % 2 == 1;
                    lepes1.LepesekSzamaAVegeig = allas.Csoportok[0] - 1;

                    map[allas] = lepes1;
                    return lepes1;
                }

                if (allas.Csoportok[elsoNem0Csoport] == 1)
                {
                    Lepes lepes2;
                    lepes2.Csoport = elsoNem0Csoport;
                    lepes2.X = elsoNem0Csoport - 1;
                    lepes2.Veszit = false;
                    lepes2.JatekosLep = allas.JatekosLep;
                    lepes2.LepesekSzamaAVegeig = 1;

                    map[allas] = lepes2;
                    return lepes2;
                }
            }

            int veszitMax = int.MinValue;
            int nyerMin = int.MaxValue;
            int veszitI = -1;
            int nyerI = -1;
            int veszitJ = -1;
            int nyerJ = -1;

            for (int i = allas.Csoportok.Count - 1; i>=0;  i--)
            {
                if (allas.Csoportok[i] == 0) // 
                {
                    continue;
                }

                for (int j = i; j >= 0; j--)
                {
                    Allas ujAllas;
                    ujAllas.Csoportok = allas.Csoportok.ToList();
                    ujAllas.Csoportok[i]--;

                    if (ujAllas.Csoportok[allas.Csoportok.Count - 1] == 0)
                    {
                        ujAllas.Csoportok.RemoveAt(ujAllas.Csoportok.Count - 1);
                    }

                    ujAllas.JatekosLep = 1 - allas.JatekosLep;

                    // az eredeti i + 1-es csoport szetesik ket csoportra
                    // egy (0..j - 1) es (j+1..i) csoportra
                    int cs1 = j - 1;
                    if (cs1 >= 0)
                    {
                        ujAllas.Csoportok[cs1]++;
                    }

                    int cs2 = i - j - 1;
                    if (cs2 >= 0)
                    {
                        ujAllas.Csoportok[cs2]++;
                    }

                    Lepes megoldasLepes = megold(ujAllas);

                    if (megoldasLepes.Veszit && veszitMax < megoldasLepes.LepesekSzamaAVegeig)
                    {
                        veszitMax = megoldasLepes.LepesekSzamaAVegeig;
                        veszitI = i;
                        veszitJ = j;
                    }

                    if (!megoldasLepes.Veszit && nyerMin > megoldasLepes.LepesekSzamaAVegeig)
                    {
                        nyerMin = megoldasLepes.LepesekSzamaAVegeig;
                        nyerI = i;
                        nyerJ = j;

                        if (nyerI >= 0)
                        {// ha talalunk nyero lepest, akkor nem keresunk tovabb. igy sokkal hamarabb befejezi az algoritmus, ellenben nem feltetlenul a legkevesebb lepesszamu nyero megoldast adja majd vissza
                            Lepes nyeroLepes;
                            nyeroLepes.Csoport = nyerI;
                            nyeroLepes.X = nyerJ;
                            nyeroLepes.JatekosLep = allas.JatekosLep;
                            nyeroLepes.Veszit = false;
                            nyeroLepes.LepesekSzamaAVegeig = nyerMin + 1;

                            map[allas] = nyeroLepes;
                            return nyeroLepes;
                        }
                    }
                }
            }

            if (veszitI >= 0)
            {
                Lepes vesztesLepes;
                vesztesLepes.Csoport = veszitI;
                vesztesLepes.X = veszitJ;
                vesztesLepes.JatekosLep = allas.JatekosLep;
                vesztesLepes.Veszit = true;
                vesztesLepes.LepesekSzamaAVegeig = veszitMax + 1;

                map[allas] = vesztesLepes;
                return vesztesLepes;
            }

            throw new Exception();
        }
        finally
        {
            if (map.Count % 100000 == 0)
            {
                Console.WriteLine(map.Count);
            }
        }
    }

    static void Main(string[] args)
    {
        Allas kiinduloAllas = new Allas()
        {
                        Csoportok = new List<int>() { 1, 2 }, // 2, 1, 2
            // Csoportok = new List<int>() { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 15 }, // 15, 15, 15, 15..., 15
            //Csoportok = new List<int>() { 2, 0, 2, 3, 0, 0, 0, 0, 1 }, // 9, 3, 3, 4, 4, 4, 1, 1
            JatekosLep = 0
        };

        Program p = new Program();
        Lepes l = p.megold(kiinduloAllas);
        l.Print();
    }
}