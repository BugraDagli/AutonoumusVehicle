using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Düzeltme
{
    static class ServerHandleData
    {
        public delegate void Packet(int connectionID, byte[] data);
        public static Dictionary<int, Packet> packets = new Dictionary<int, Packet>(); //İnt türünde key veriyoruz ve o keye göre packette arama yap diyoruz

        public static void InitializePackets()// Paketler buraya eklenecek
        {
            packets.Add((int)ClientPackets.CMerhabaServer, DataReceiver.HandleMerhabaServer); // -->>DataReceiver.clientpackets  / client bir şey göndermek istediğinde keyi yolluyor sunucumuz
            //o keye göre fonksiyona yönlendiriyor /yani client'tan gelen verileri neye göre işleyeceğimizi anlıyoruz
        }

        public static void HandleData (int connectionID,byte[] data)
        {
            byte[] buffer = (byte[])data.Clone(); //gelen data'yı clone'layıp buffer'ın içine atıyor, olduğu gibi atarsa data'nın içini boşaltır
            int pLength = 0;

            if(Sabitler.bagli_client[connectionID].buffer == null)
            {
                Sabitler.bagli_client[connectionID].buffer = new Kaan_ByteBuffer();
            }

            Sabitler.bagli_client[connectionID].buffer.Bytes_Yaz(buffer);

            if(Sabitler.bagli_client[connectionID].buffer.Count() == 0)
            {
                Sabitler.bagli_client[connectionID].buffer.Clear();
                return;
            }

            if(Sabitler.bagli_client[connectionID].buffer.Length() >= 4)
            {
                pLength = Sabitler.bagli_client[connectionID].buffer.Int_Oku(false);
                if(pLength <= 0)
                {
                    Sabitler.bagli_client[connectionID].buffer.Clear();
                    return;
                }
            }

            while(pLength > 0 & pLength <= Sabitler.bagli_client[connectionID].buffer.Length() -4)
            {
                if(pLength <= Sabitler.bagli_client[connectionID].buffer.Length() - 4 )
                {
                    Sabitler.bagli_client[connectionID].buffer.Int_Oku();
                    data = Sabitler.bagli_client[connectionID].buffer.Bytes_Oku(pLength);
                }
                pLength = 0;

                if(Sabitler.bagli_client[connectionID].buffer.Length() >= 4)
                {
                    pLength = Sabitler.bagli_client[connectionID].buffer.Int_Oku(false);

                    if(pLength <= 0)
                    {
                        Sabitler.bagli_client[connectionID].buffer.Clear();
                        return;
                    }
                }

                if(pLength <= 1)
                {
                    Sabitler.bagli_client[connectionID].buffer.Clear();
                }

                HandleDataPackets(connectionID , data);
            }
        }

        private static void HandleDataPackets(int connectionID , byte[] data) //packet numaramızı alıyor
        {
            Kaan_ByteBuffer buffer = new Kaan_ByteBuffer();
            buffer.Bytes_Yaz(data);//paketi açıyor
            int packetID = buffer.Int_Oku();//ilk satırı okuyor sonra kapatıyor
            buffer.Dispose();
            
            if(packets.TryGetValue(packetID, out Packet packet)) //sonra bulduğu key numarasına göre arama yapıyor
            {
                packet.Invoke(connectionID, data);//invoke ile o fonksiyonun içerisine bağlanmamızı sağlıyor / DataReceiver -->> işlemler
            }
        }
    }
}
