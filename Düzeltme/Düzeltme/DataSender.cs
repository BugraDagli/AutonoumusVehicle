using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Düzeltme
{

        public enum ServerPackets
        {
            SHosgeldinMesaji = 1,
        }

        static class DataSender
        {

            public static void SendHosgeldinMesaji(int connectionID)
            {
                Kaan_ByteBuffer buffer = new Kaan_ByteBuffer();
                buffer.Int_Yaz((int)ServerPackets.SHosgeldinMesaji);//İlk key numarası ,yani server'ın bunu hangi fonksiyona yönlendireceğini yazdık ,ilk byte'ı okuması gereken yer burası / ServerHandleData->Handledatapackets -->> 
                buffer.Int_Yaz(connectionID); //connection ID numarası yani port numarası
                buffer.String_Yaz("Merhaba, Sunucuya Hoşgeldiniz..");
                ClientManager.SendDataTo(connectionID, buffer.ToArray());
                buffer.Dispose();
                Sabitler.bagli_client[connectionID].oyunda_mi = true;

            }
        }

}
