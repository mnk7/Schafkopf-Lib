using Schafkopf_Lib;

interface IKontrolle
{
    /**
	 * Bestimmt den Sieger eines Spiels
	 * @param model
	 * @return SpielerID des Siegers
	 */
    int Sieger(Modell m, int erster);

    /**
	 * Bestimmt, ob ein Spielzug erlaubt ist und gibt das Ergebnis zurück
	 * @param model
	 * @param ID
	 * @return erlaubt
	 */
    bool Erlaubt(Modell m, int ID);

    /**
	 * Bestimmt einen eventuellen Mitspieler
	 * @param model
	 * @return mitspieler oder null
	 */
    int Mitspieler(Modell m);

    /**
	 * Bestimmt, ob eine Karte Trumpf ist
	 * @param wert
	 * @param farbe
	 * @return
	 */
    bool istTrumpf(Karte.Wert wert, Karte.Farbe farbe);

    /**
	 * Errechnet die Laufenden der Spieler.
	 * @param spieler
	 * @param mitspieler
	 * @param model
	 * @return
	 */
    int Laufende(int spieler, int mitspieler, Modell model);
}