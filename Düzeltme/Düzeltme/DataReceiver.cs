using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Düzeltme
{
    public enum ClientPackets
    {
        CMerhabaServer = 1,
    }
    static class DataReceiver
    {

        public static void HandleMerhabaServer(int connectionID ,byte[] data)
        {
            Kaan_ByteBuffer buffer = new Kaan_ByteBuffer();
            buffer.Bytes_Yaz(data); //tekrar paketleri açıyoruz
            int packetID = buffer.Int_Oku(); //packetID'yi tekrar okuyoruz ki sıralamadan çıksın
            string msg = buffer.String_Oku(); //sonra mesajı okuyup gelen_mesaj olarak gösteriyoruz
            buffer.Dispose();
            Yazi.Gelen_Mesaj(connectionID + " " + msg);
        }
    }
}
