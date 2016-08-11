using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Schafkopf_Lib {
    class Wenz : IKontrolle {
        public bool Erlaubt( Modell m, int ID ) {
            Karte[] tisch = m.Tisch;
            Karte angespielt;
            int spieler0 = m.GibAusspieler(ID);

            //Findet die Karte, die zuerst gespielt wurde
            angespielt = tisch[spieler0];

            //Es wurde nichts angespielt
            if ( angespielt == null || ID == spieler0 ) {
                return true;
            }
            if ( m.GibSpielerkarten(ID).Count() == 0 ) {
                return true;
            }
            //Es wurde ein Unter angespielt
            if ( angespielt.wert.Equals(Karte.Wert.UNTER) ) {
                if ( tisch[ID].wert.Equals(Karte.Wert.UNTER) ) {
                    return true;
                } else if ( hatKeinenTrumpf(m, ID) ) {
                    return true;
                }
                return false;
            }
            //Es wurde eine Farbe angespielt
            if ( tisch[ID].farbe.Equals(angespielt.farbe)
                    && !tisch[ID].wert.Equals(Karte.Wert.UNTER) ) {
                //Es wurde die passende Farbe gespielt
                return true;
            } else if ( keineFarbe(angespielt.farbe, m, ID) ) {
                //Der Spieler hat die Farbe nicht
                return true;
            }
            return false;
        }

        /**
        * Untersucht, ob der Spieler einen Trumpf auf der Hand hat
        * @param m
        * @param ID
        * @return
        */
        private bool hatKeinenTrumpf( Modell m, int ID ) {
            List<Karte> y = m.GibSpielerkarten(ID);

            for ( int i = 0; i < y.Count(); i++ ) {
                if ( y[i].wert.Equals(Karte.Wert.UNTER) ) {
                    return false;
                }
            }
            if ( m.Tisch[ID].wert.Equals(Karte.Wert.UNTER) ) {
                return false;
            }

            return true;
        }

        /**
         * Untersucht, ob der Spieler die angespielte Farbe nicht auf der Hand hat
         * @param farbe
         * @param m
         * @param ID
         * @return
         */
        private bool keineFarbe( Karte.Farbe farbe, Modell m, int ID ) {
            List<Karte> y = m.GibSpielerkarten(ID);

            Karte karte;
            for ( int i = 0; i < y.Count(); i++ ) {
                karte = y[i];
                if ( karte.farbe.Equals(farbe) && !karte.wert.Equals(Karte.Wert.UNTER) ) {
                    return false;
                }
            }

            karte = m.Tisch[ID];
            if ( karte.farbe.Equals(farbe) && !karte.wert.Equals(Karte.Wert.UNTER) ) {
                return false;
            }

            return true;
        }

        public bool istTrumpf( Karte.Wert wert, Karte.Farbe farbe ) {
            if ( wert.Equals(Karte.Wert.UNTER) ) {
                return true;
            }
            return false;
        }

        public int Laufende( int spieler, int mitspieler, Modell model ) {
            List<Karte> spielerkarten = model.GibSpielerkarten(spieler);

            //Für jeden enthaltenen Trumpf gibt es ein Feld
            bool[] enthalten = new bool[4];
            for ( int i = 0; i < 4; i++ ) {
                enthalten[i] = false;
            }

            Karte k;
            for ( int i = 0; i < spielerkarten.Count(); i++ ) {
                k = spielerkarten[i];
                int stelle;

                if ( k.wert.Equals(Karte.Wert.UNTER) ) {
                    stelle = ( int )k.farbe;
                    enthalten[stelle] = true;
                }
            }

            int laufende = 0;
            for ( int i = 0; i < 4; i++ ) {
                if ( enthalten[i] ) {
                    laufende++;
                } else {
                    break;
                }
            }

            return laufende;
        }

        public int Mitspieler( Modell m ) {
            return 4;
        }

        public int Sieger( Modell m, int erster ) {
            bool unter= false;
            Karte[] gespielt = m.Tisch;
            for ( int i = 0; i < 4; i++ ) {
                if ( gespielt[i].wert.Equals(Karte.Wert.UNTER) ) {
                    unter = true;
                }
            }
            if ( !unter ) {
                return keinTrumpf(gespielt, erster);
            }
            return schonTrumpf(gespielt);
        }

        private int schonTrumpf( Karte[] gespielt ) {
            int spieler = -1;
            for ( int i = 0; i < 4; i++ ) {
                if ( gespielt[i].wert.Equals(Karte.Wert.UNTER) ) {

                    if ( spieler == -1 ) {
                        spieler = i;
                    } else {
                        //siehe lib.Karte: 1 = EICHEL, 2 = GRAS, 3 = HERZ, 4 = SCHELLEN
                        if ( ( int )gespielt[i].farbe < ( int )gespielt[spieler].farbe ) {
                            spieler = i;
                        }
                    }
                }
            }
            return spieler;
        }

        private int keinTrumpf( Karte[] gespielt, int erster ) {
            //Derjenige, der ausgekartet hat wird zuerst abgerufen
            int spieler = erster;

            for ( int i = 1; i < 4; i++ ) {

                if ( gespielt[(i + erster) % 4].farbe.Equals(gespielt[erster].farbe) ) {
                    if ( kartenRangliste(gespielt[(i + erster) % 4].wert)
                            > kartenRangliste(gespielt[spieler].wert) ) {
                        spieler = (i + erster) % 4;
                    }
                }
            }
            return spieler;
        }

        /**
	    * Gibt einen Wert zurück, der dem Stellenwert der Karte entspricht
	    * @param wert
	    * @return
	    */
        private int kartenRangliste( Karte.Wert wert ) {
            switch ( wert ) {
                case Karte.Wert.SIEBEN:
                    return 0;
                case Karte.Wert.ACHT:
                    return 1;
                case Karte.Wert.NEUN:
                    return 2;
                case Karte.Wert.OBER:
                    return 3;
                case Karte.Wert.KONIG:
                    return 4;
                case Karte.Wert.ZEHN:
                    return 5;
                case Karte.Wert.SAU:
                    return 6;
                case Karte.Wert.UNTER:
                    return 7;
            }

            return -1;
        }
    }
}
