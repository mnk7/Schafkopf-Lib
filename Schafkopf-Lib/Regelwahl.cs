using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Schafkopf_Lib;

namespace Schafkopf_Lib {
    class Regelwahl {
        public IKontrolle Wahl( Modus m ) {
            Karte.Farbe Eichel = Karte.Farbe.EICHEL;
            Karte.Farbe Gras = Karte.Farbe.GRAS;
            Karte.Farbe Herz = Karte.Farbe.HERZ;
            Karte.Farbe Schellen = Karte.Farbe.SCHELLEN;

            switch ( m ) {
                case Modus.GEIEReichelDU:
                case Modus.GEIEReichel:
                    return new Farbgeier(Eichel);
                case Modus.GEIERgrasDU:
                case Modus.GEIERgras:
                    return new Farbgeier(Gras);
                case Modus.GEIERherzDU:
                case Modus.GEIERherz:
                    return new Farbgeier(Herz);
                case Modus.GEIERschellenDU:
                case Modus.GEIERschellen:
                    return new Farbgeier(Schellen);

                case Modus.WENZeichelDU:
                case Modus.WENZeichel:
                    return new Farbwenz(Eichel);
                case Modus.WENZgrasDU:
                case Modus.WENZgras:
                    return new Farbwenz(Gras);
                case Modus.WENZherzDU:
                case Modus.WENZherz:
                    return new Farbwenz(Herz);
                case Modus.WENZschellenDU:
                case Modus.WENZschellen:
                    return new Farbwenz(Schellen);

                case Modus.GEIERdu:
                case Modus.GEIER:
                    return new Geier();

                case Modus.WENZdu:
                case Modus.WENZ:
                    return new Wenz();

                case Modus.SOLOeichelDU:
                case Modus.SOLOeichel:
                    return new Solo(Eichel);
                case Modus.SOLOgrasDU:
                case Modus.SOLOgras:
                    return new Solo(Gras);
                case Modus.SOLOherzDU:
                case Modus.SOLOherz:
                    return new Solo(Herz);
                case Modus.SOLOschellenDU:
                case Modus.SOLOschellen:
                    return new Solo(Schellen);

                case Modus.SAUSPIELeichel:
                    return new Sauspiel(Eichel);
                case Modus.SAUSPIELgras:
                    return new Sauspiel(Gras);
                case Modus.SAUSPIELschellen:
                    return new Sauspiel(Schellen);

                case Modus.HOCHZEIT:
                    return new Hochzeit();
            }
            return null;
        }

        public bool darfGespieltWerden( Modus m, Modell modell, int ID, Karte.Farbe f ) {
            if ( m.Equals(Modus.SAUSPIELeichel)
                || m.Equals(Modus.SAUSPIELgras)
                || m.Equals(Modus.SAUSPIELschellen) ) {
                return sauspielMoeglich(f, modell, ID);
            }
            if ( m.Equals(Modus.SIE) ) {
                return sieMoeglich(modell, ID);
            }
            if ( m.Equals(Modus.HOCHZEIT) ) {
                return new Hochzeit().hochzeitMoeglich(modell.GibSpielerkarten(ID));
            }
            //Wenz, Geier und Solo dürfen immer gespielt werden
            return true;
        }

        public bool sieMoeglich( Modell modell, int ID ) {
            List<Karte> spielerhand = modell.GibSpielerkarten(ID);

            for ( int i = 0; i < spielerhand.Count(); i++ ) {
                if ( spielerhand[i].wert != Karte.Wert.OBER ) {
                    if ( spielerhand[i].wert == Karte.Wert.UNTER ) {
                        if ( modell.KurzesBlatt ) {
                            if ( spielerhand[i].farbe == Karte.Farbe.HERZ || spielerhand[i].farbe == Karte.Farbe.SCHELLEN ) {
                                //Es wird mit 6 Karten gespielt und einer der Unter ist zu niedrig
                                return false;
                            } else {
                                //Es wird mit 6 Karten gespielt und der Spieler hat die zwei hoechsten Unter
                                continue;
                            }
                        } else {
                            //Es wird mit 8 Karten gespielt und der Spieler hat alle Unter
                            continue;
                        }
                    } else {
                        //Der Spieler hat eine Karte, die weder Ober noch Unter ist
                        return false;
                    }
                }
            }
            //Der Spieler darf einen Sie spielen
            return true;
        }

        public bool sauspielMoeglich( Karte.Farbe farbe, Modell modell, int position ) {
            List<Karte> y = modell.GibSpielerkarten(position);
            Karte rufsau = new Karte(farbe, Karte.Wert.SAU);
            bool hatFarbe = false;
            bool hatSau = false;
            for ( int i = 0; i < y.Count(); i++ ) {
                if ( y[i].vergleiche(rufsau) ) {
                    //Der Spieler hat die Rufsau selbst
                    hatSau = true;
                }
                if ( y[i].farbe.Equals(farbe) ) {
                    if ( (y[i].wert != Karte.Wert.OBER) && (y[i].wert != Karte.Wert.UNTER) ) {
                        hatFarbe = true;
                    }
                }
            }
            if ( !hatSau && hatFarbe ) {
                return true;
            } else {
                return false;
            }
        }
    }
}
