using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Schafkopf_Lib {
    public class Karte {
        public enum Farbe { EICHEL, GRAS, HERZ, SCHELLEN };
        public enum Wert { SIEBEN, ACHT, NEUN, ZEHN, KONIG, SAU, UNTER, OBER };

        private static int[] Punkte = { 0, 0, 0, 10, 4, 11, 2, 3 };

        public Farbe farbe { get; }
        public Wert wert { get; }

        public Karte( Farbe farbe, Wert wert ) {
            this.farbe = farbe;
            this.wert = wert;
        }

        public int gibPunkte() {
            return Punkte[( int )wert];
        }

        public string gibString() {
            return farbe.ToString() + wert.ToString();
        }

        public bool vergleiche( Karte k ) {
            if ( k.farbe.Equals(this.farbe) &&
                k.wert.Equals(this.wert) ) {
                return true;
            } else {
                return false;
            }
        }
    }
}
