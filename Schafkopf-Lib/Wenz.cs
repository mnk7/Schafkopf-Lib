using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Schafkopf_Lib
{
    class Wenz : IKontrolle
    {
        public bool Erlaubt(Modell m, int ID)
        {
            throw new NotImplementedException();
        }

        public bool istTrumpf(Karte.Wert wert, Karte.Farbe farbe)
        {
            throw new NotImplementedException();
        }

        public int Laufende(int spieler, int mitspieler, Modell model)
        {
            throw new NotImplementedException();
        }

        public int Mitspieler(Modell m)
        {
            throw new NotImplementedException();
        }

        public int Sieger(Modell m, int erster)
        {
            throw new NotImplementedException();
        }
    }
}
