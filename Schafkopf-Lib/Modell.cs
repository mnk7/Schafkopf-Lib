using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Schafkopf_Lib {
    /*
     * Spielbare Modi 
     */
    public enum Modus {
        SAUSPIELeichel, SAUSPIELgras, SAUSPIELschellen,
        HOCHZEIT,
        GEIEReichel, GEIERgras, GEIERherz, GEIERschellen,
        WENZeichel, WENZgras, WENZherz, WENZschellen,
        GEIER,
        WENZ,
        SOLOeichel, SOLOgras, SOLOherz, SOLOschellen,
        GEIEReichelDU, GEIERgrasDU, GEIERherzDU, GEIERschellenDU,
        WENZeichelDU, WENZgrasDU, WENZherzDU, WENZschellenDU,
        GEIERdu,
        WENZdu,
        SOLOeichelDU, SOLOgrasDU, SOLOherzDU, SOLOschellenDU,
        SIE,
        NICHTS
    }

    class Modell {
        //Speichert wie viele Karten ein Spieler auf der Hand hat: True entspr. 6 Karten, False entspr. 8 Karten
        public bool KurzesBlatt;

        private List<Karte> AlleKarten;

        public string[] Spielernamen { get; }
        public int[] Punkte { get; set; }
        private List<Karte>[] Spielerhand;
        public Karte[] Tisch { get; }
        public Karte[] LetzterStich { get; }
        public int LetzterStichGewinner { get; set; }
        public int LetzterStichPunkte { get; set; }

        public int Laufende { get; set; }

        public Modell( bool KurzesBlatt ) {
            this.KurzesBlatt = KurzesBlatt;
            this.Spielerhand = new List<Karte>[4];
            for ( int i = 0; i < 4; i++ ) {
                Spielerhand[i] = new List<Karte>();
            }
            this.Tisch = new Karte[4];
            this.LetzterStich = new Karte[4];
            this.Punkte = new int[4];
            Laufende = 0;

            InitialisiereKarten();
        }

        public Modell( bool KurzesBlatt,
            List<Karte>[] Spielerhand,
            Karte[] Tisch,
            Karte[] LetzterStich,
            int[] Punkte ) {

            this.KurzesBlatt = KurzesBlatt;
            this.Spielerhand = Spielerhand;
            this.Tisch = Tisch;
            this.LetzterStich = LetzterStich;
            this.Punkte = Punkte;
            Laufende = 0;
        }

        void InitialisiereKarten() {
            if ( KurzesBlatt ) {
                for ( int i = 0; i < 4; i++ ) {
                    for ( int j = 0; j < 6; j++ ) {
                        //Siebener und Achter werden uebergangen
                        AlleKarten.Add(new Karte(( Karte.Farbe )i, ( Karte.Wert )(j + 2)));
                    }
                }
            } else {
                for ( int i = 0; i < 32; i++ ) {
                    AlleKarten.Add(new Karte(( Karte.Farbe )(i / 8), ( Karte.Wert )(i % 8)));
                }
            }
        }

        public void Mischen( int MischWiederholungen ) {
            Random Zufall = new Random(67);
            int ZufallsWert1;
            int ZufallsWert2;
            Karte Speicher;
            for ( int i = 0; i < AlleKarten.Count * MischWiederholungen; i++ ) {
                ZufallsWert1 = Zufall.Next(0, AlleKarten.Count);
                ZufallsWert2 = Zufall.Next(0, AlleKarten.Count);

                //Dreieckstausch
                Speicher = AlleKarten[ZufallsWert2];
                AlleKarten[ZufallsWert2] = AlleKarten[ZufallsWert1];
                AlleKarten[ZufallsWert1] = Speicher;
            }
        }

        public void ErsteKartenGeben( int AnzahlErsteKarten ) {
            if ( AnzahlErsteKarten < 0 ) { AnzahlErsteKarten = 0; }
            if ( AnzahlErsteKarten > 6 && KurzesBlatt ) { AnzahlErsteKarten = 6; }
            if ( AnzahlErsteKarten > 8 && !KurzesBlatt ) { AnzahlErsteKarten = 8; }

            for ( int i = 0; i < 4; i++ ) {
                Spielerhand[i].AddRange(AlleKarten.GetRange(i * AnzahlErsteKarten, AnzahlErsteKarten));
            }
        }

        public void RestlicheKartenGeben() {
            int AnzahlRestlicheKarten;
            int AnzahlErsteKarten = Spielerhand[0].Count;
            if ( KurzesBlatt ) {
                AnzahlRestlicheKarten = 6 - AnzahlErsteKarten;
            } else {
                AnzahlRestlicheKarten = 8 - AnzahlErsteKarten;
            }

            for ( int i = 0; i < 4; i++ ) {
                Spielerhand[i].AddRange(AlleKarten.GetRange(4 * AnzahlErsteKarten + i * AnzahlRestlicheKarten, AnzahlRestlicheKarten));
            }
        }

        public List<Karte> GibSpielerkarten( int SpielerID ) {
            return Spielerhand[SpielerID];
        }

        public List<Karte> LegeKarte( int SpielerID, Karte GespielteKarte ) {
            bool gefunden = false;
            for ( int i = 0; i < Spielerhand[SpielerID].Count; i++ ) {
                if ( GespielteKarte.vergleiche(Spielerhand[SpielerID][i]) ) {
                    Spielerhand[SpielerID].RemoveAt(i);
                    gefunden = true;
                }
            }
            if ( !gefunden ) {
                throw new Exception("Fehlerhafte Karte gespielt");
            } else {
                Tisch[SpielerID] = GespielteKarte;
                return Spielerhand[SpielerID];
            }
        }

        public List<Karte> LegeZurueck( int SpielerID ) {
            Spielerhand[SpielerID].Add(Tisch[SpielerID]);
            Tisch[SpielerID] = null;
            return Spielerhand[SpielerID];
        }

        public void Stechen( int GewinnerID ) {
            int PunkteStich = 0;

            for ( int i = 0; i < 4; i++ ) {
                PunkteStich += Tisch[i].gibPunkte();
                LetzterStich[i] = Tisch[i];
                Tisch[i] = null;
            }

            Punkte[GewinnerID] += PunkteStich;

            LetzterStichGewinner = GewinnerID;
            LetzterStichPunkte = PunkteStich;
        }

        public int GibAusspieler( int ID ) {
            int ZahlGespielteKarten = 0;
            for ( int i = 0; i < 4; i++ ) {
                if ( Tisch[i] != null ) {
                    ZahlGespielteKarten++;
                }
            }
            int Spieler0 = ID + 1 - ZahlGespielteKarten;
            if ( Spieler0 < 0 ) {
                Spieler0 += 4;
            }
            if ( Spieler0 > 3 ) {
                Spieler0 -= 4;
            }

            return Spieler0;
        }

        public Modus WerSpielt( Modus m1, Modus m2 ) {
            //Ist einer der Modi null wird der andere zurückgegeben
            if ( m1 == null )
                return m2;
            if ( m2 == null )
                return m1;

            //Wenn jemand nichts spielt
            //Achtung!! spielen beide nichts, so kann es sein, dass am Ende 
            //Modus.NICHTS an den Server zurückgegeben wird!
            if ( m1.Equals(Modus.NICHTS) )
                return m2;
            if ( m2.Equals(Modus.NICHTS) )
                return m1;

            int nr1 = ( int )m1;
            int nr2 = ( int )m2;

            //stellt alle Sauspiele und Hochzeiten gleich
            if ( nr1 < 4 )
                nr1 = 0;
            if ( nr2 < 4 )
                nr2 = 0;

            //stellt alle Farbgeier und Farbwenzen gleich
            if ( nr1 > 3 && nr1 < 8 )
                nr1 = 4;
            if ( nr2 > 3 && nr2 < 8 )
                nr2 = 4;

            if ( nr1 > 7 && nr1 < 12 )
                nr1 = 8;
            if ( nr2 > 7 && nr2 < 12 )
                nr2 = 8;

            //stellt alle Solos gleich
            if ( nr1 > 13 && nr1 < 18 )
                nr1 = 14;
            if ( nr2 > 13 && nr2 < 18 )
                nr2 = 14;

            //stellt alle FarbgeierDu's und FarbwenzDu's gleich
            if ( nr1 > 17 && nr1 < 22 )
                nr1 = 18;
            if ( nr2 > 17 && nr2 < 22 )
                nr2 = 18;

            if ( nr1 > 21 && nr1 < 26 )
                nr1 = 22;
            if ( nr2 > 21 && nr2 < 26 )
                nr2 = 22;

            //stellt alle SoloDu's gleich
            if ( nr1 > 27 && nr1 < 32 )
                nr1 = 28;
            if ( nr2 > 27 && nr2 < 32 )
                nr2 = 28;

            //Nur wenn der zweite Modus besser ist wird dieser zurückgegeben
            if ( nr1 > nr2 )
                return m1;
            else
                return m2;
        }

        public void Hochzeit( int Spielt, int Mitspieler, Karte Angebot, Karte Angenommen ) {

            //Position von angebotener und angenommener Karte
            int An = -1;
            int Bn = -1;

            Karte a = new Karte(Angebot.farbe, Angebot.wert);
            Karte Test = null;
            for ( int i = 0; i < 6; i++ ) {
                Test = new Karte(Spielerhand[Spielt][i].farbe, Spielerhand[Spielt][i].wert);
                if ( Test.vergleiche(a) ) {
                    //Karte von der Hand des Spielers nehmen
                    An = i;
                    break;
                }
            }

            Karte b = new Karte(Angenommen.farbe, Angenommen.wert);
            Karte GegenTest = null;
            for ( int i = 0; i < 6; i++ ) {
                GegenTest = new Karte(Spielerhand[Mitspieler][i].farbe, Spielerhand[Mitspieler][i].wert);
                if ( GegenTest.vergleiche(b) ) {
                    //Karte von der Hand des Spielers nehmen
                    Bn = i;
                    break;
                }
            }

            Spielerhand[Spielt][An] = GegenTest;
            Spielerhand[Mitspieler][Bn] = Test;
        }

        public void SetzeNamen( int SpielerID, string name ) {
            Spielernamen[SpielerID] = name;
        }

        public void ErrechneLaufende( int Spieler, int Mitspieler, Modus modus ) {
            IKontrolle regeln = new Regelwahl().Wahl(modus);
            Laufende = regeln.Laufende(Spieler, Mitspieler, this);
            if ( Laufende < 3 ) {
                Laufende = 0;
            }
        }

        public void Debug( int SpielerID ) {
            for ( int i = 0; i < Spielerhand[SpielerID].Count; i++ ) {
                Console.WriteLine(Spielerhand[SpielerID][i].ToString());
            }
        }
    }
}
