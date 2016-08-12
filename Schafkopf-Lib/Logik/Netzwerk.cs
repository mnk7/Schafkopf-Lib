using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Schafkopf_Lib {
    class Netzwerk {
        public int SpielerID { get; set; }
        protected TcpClient Client;
        protected NetworkStream Stream;

        protected byte[] Buffer;

        public Netzwerk(TcpClient Client, int SpielerID ) {
            this.SpielerID = SpielerID;
            this.Client = Client;
            Stream = Client.GetStream();
            Buffer = new byte[2048];
        }

        //---------------------------Senden-----------------------------

        public void sende(string Data ) {
            if(Data.Equals("") || Data == null ) {
                Data = "§";
            }
            byte[] DataBytes = System.Text.Encoding.ASCII.GetBytes(Data);
            Stream.Write(DataBytes, 0, DataBytes.Length);
        }

        public void sende(string Flag, string Data ) {
            Flag += (";" + Data);
            sende(Flag);
        }

        public void sende(string Flag, string[] Data) {
            for(int i = 0; i < Data.Length; i++ ) {
                Flag += (";" + Data[i]);
            }
            sende(Flag);
        }

        //--------------------------Einlesen----------------------------

        public string lese() {
            int Length = Stream.Read(Buffer, 0, Buffer.Length);
            string Data = System.Text.Encoding.ASCII.GetString(Buffer, 0, Length);
            if ( Data.Equals("§") ) {
                Data = "";
            }
            return Data;
        }

        public string[] leseFeld() {

        }

        public void Beenden() {
            Client.Close();
        }
    }
}
