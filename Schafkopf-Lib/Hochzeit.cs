﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Schafkopf_Lib {
    class Hochzeit : IKontrolle {
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
            //Es wurde Trumpf angespielt
            if ( istTrumpf(angespielt.wert, angespielt.farbe) ) {
                if ( istTrumpf(tisch[ID].wert, tisch[ID].farbe) ) {
                    return true;
                } else {
                    if ( hatKeinenTrumpf(m, ID) ) {
                        return true;
                    }
                }
                return false;
            }
            //Es wurde eine Farbe angespielt
            if ( tisch[ID].farbe.Equals(angespielt.farbe)
                    && !istTrumpf(tisch[ID].wert, tisch[ID].farbe) ) {
                //Es wurde die passende Farbe gespielt
                return true;
            } else {
                if ( keineFarbe(angespielt.farbe, m, ID) ) {
                    //Der Spieler hat die Farbe nicht
                    return true;
                }
            }
            return false;
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
                if ( karte.farbe.Equals(farbe) && !istTrumpf(karte.wert, karte.farbe) ) {
                    return false;
                }
            }

            karte = m.Tisch[ID];
            if ( karte.farbe.Equals(farbe) && !istTrumpf(karte.wert, karte.farbe) ) {
                return false;
            }

            return true;
        }

        public bool istTrumpf( Karte.Wert wert, Karte.Farbe farbe ) {
            if ( wert.Equals(Karte.Wert.OBER)
                || wert.Equals(Karte.Wert.UNTER)
                || farbe.Equals(Karte.Farbe.HERZ) ) {
                return true;
            }
            return false;
        }

        public int Laufende( int spieler, int mitspieler, Modell modell ) {
            List<Karte> spielerkarten = modell.GibSpielerkarten(spieler);
            spielerkarten.AddRange(modell.GibSpielerkarten(mitspieler));

            //Für jeden enthaltenen Trumpf gibt es ein Feld
            bool[] enthalten = new bool[14];
            for ( int i = 0; i < 14; i++ ) {
                enthalten[i] = false;
            }

            Karte k;
            int stelle;
            for ( int i = 0; i < spielerkarten.Count(); i++ ) {
                k = spielerkarten[i];

                if ( k.wert.Equals(Karte.Wert.OBER) ) {
                    stelle = ( int )k.farbe;
                    enthalten[stelle] = true;
                } else if ( k.wert.Equals(Karte.Wert.UNTER) ) {
                    stelle = 4 + ( int )k.farbe;
                    enthalten[stelle] = true;
                } else if ( k.farbe.Equals(Karte.Farbe.HERZ) ) {
                    //Farbtrumpf
                    //Enum für Wert ist umgedreht aufgestellt -> siehe lib.Karte
                    stelle = 8 + (5 - ( int )k.wert);
                    enthalten[stelle] = true;
                }
            }

            int laufende = 0;
            for ( int i = 0; i < 14; i++ ) {
                if ( enthalten[i] ) {
                    laufende++;
                } else {
                    break;
                }
            }

            return laufende;
        }

        public int Mitspieler( Modell m ) {
            //Kein Mitspieler
            return -1;
        }

        public int Sieger( Modell m, int erster ) {
            bool trumpf = false;
            Karte[] gespielt = m.Tisch;
            for ( int i = 0; i < 4; i++ ) {
                if ( istTrumpf(gespielt[i].wert, gespielt[i].farbe) ) {
                    trumpf = true;
                }
            }
            if ( !trumpf ) {
                return keinTrumpf(gespielt, erster);
            }
            return schonTrumpf(gespielt);
        }

        /**
	     * Ermittelt den Sieger eines Stichs, wenn ein Trumpf gespielt wurde
	     * @param gespielt
         * @return
	     */
        private int schonTrumpf( Karte[] tisch ) {
            int sieger = -1;

            for ( int i = 0; i < 4; i++ ) {
                //Es muss ein Trumpf vorhanden sein
                if ( istTrumpf(tisch[i].wert, tisch[i].farbe) ) {
                    if ( sieger == -1 ) {
                        sieger = i;
                    } else {
                        int diff = kartenRangliste(tisch[i].wert) - kartenRangliste(tisch[sieger].wert);
                        //Wenn der Wert der Karte höher als der der alten ist
                        if ( diff > 0 ) {
                            sieger = i;
                        } else {
                            //Beide Karten haben den gleichen Wert 
                            if ( diff == 0 ) {
                                //Wenn die Farbe der Karte höherwertig ist
                                //siehe lib.Karte: 1 = EICHEL, 2 = GRAS, 3 = HERZ, 4 = SCHELLEN
                                if ( ( int )tisch[i].farbe < ( int )tisch[sieger].farbe ) {
                                    sieger = i;
                                }
                            }
                            //Ansonsten sticht die Karte tisch[i] nicht
                        }
                    }
                }
            }

            return sieger;
        }

        /**
         * Ermittelt den Sieger, wenn kein Trumpf gespielt wurde
         * @param tisch
         * @return
         */
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
        * Untersucht, ob der Spieler einen Trumpf auf der Hand hat
        * @param m
        * @param ID
        * @return
        */
        private bool hatKeinenTrumpf( Modell m, int ID ) {
            List<Karte> y = m.GibSpielerkarten(ID);

            for ( int i = 0; i < y.Count(); i++ ) {
                if ( istTrumpf(y[i].wert, y[i].farbe) ) {
                    return false;
                }
            }
            if ( istTrumpf(m.Tisch[ID].wert, m.Tisch[ID].farbe) ) {
                return false;
            }

            return true;
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
                case Karte.Wert.KONIG:
                    return 3;
                case Karte.Wert.ZEHN:
                    return 4;
                case Karte.Wert.SAU:
                    return 5;
                case Karte.Wert.UNTER:
                    return 6;
                case Karte.Wert.OBER:
                    return 7;
            }

            return -1;
        }

        public bool hochzeitMoeglich( List<Karte> spielerhand ) {
            int anzahl_Truempfe = 0;
            for ( int i = 0; i < spielerhand.Count(); i++ ) {
                if ( istTrumpf(spielerhand[i].wert, spielerhand[i].farbe) ) {
                    anzahl_Truempfe++;
                }
            }

            if ( anzahl_Truempfe == 1 ) {
                return true;
            }

            return false;
        }

        public bool hochzeitMoeglich( Modell m, int position, Karte angebot ) {
            if ( !istTrumpf(angebot.wert, angebot.farbe) ) {
                return false;
            }
            List<Karte> hand = m.GibSpielerkarten(position);
            for ( int i = 0; i < hand.Count(); i++ ) {
                if ( istTrumpf(hand[i].wert, hand[i].farbe)
                        && !hand[i].vergleiche(angebot) ) {
                    return false;
                }
            }
            return true;
        }
    }
}
