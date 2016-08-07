using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Schafkopf_Lib
{
    class Regelwahl
    {
        public IKontrolle Wahl(Modus m)
        {
            return null;
        }

        public bool darfGespieltWerden(Modus m, Modell modell, int ID, Karte.Farbe f)
        {
            return false;
        }

        public bool sieMoeglich(Modell modell, int ID)
        {
            return false;
        }

        public bool sauspielMoeglich(Karte.Farbe farbe, Modell modell, int position)
        {
            return false;
        }
    }
}
