using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;


namespace Düzeltme
{
    class ClientManager
    {

        public static void CreateNewConnection(TcpClient tempClient )
        {
            Client newClient = new Client();// yeni connection oluşturuyoruz
            newClient.socket = tempClient; //bağlanan client'ı socket numarası olarak atıyoruz
            newClient.connectionID = ((IPEndPoint)tempClient.Client.RemoteEndPoint).Port; //Connection ID olarak da yeni Client'in bizim bilgisayarımıza bağlandığı port numarasını veriyoruz.
            newClient.Start();//client'i başlatıyoruz    Client -->>                      //rastgele numaralar da verebilirdik ama port numarası hem uniq hem de port numarsı tutmak için ayrı bir değişken tutmuyoruz.
            Sabitler.bagli_client.Add(newClient.connectionID, newClient);//sabitler class'ının içindeki bagli_client'lara ekleme yapıyoruz
            Console.WriteLine("Girdi");
            DataSender.SendHosgeldinMesaji(newClient.connectionID);
        }

        public static void SendDataTo(int connectionID ,byte[]data) //paketleme
        {
            Kaan_ByteBuffer buffer = new Kaan_ByteBuffer();
            buffer.Int_Yaz((data.GetUpperBound(0) - data.GetLowerBound(0)) + 1);
            buffer.Bytes_Yaz(data); //paketlediğimiz veriyi byte olarak buffer'ın içine yazıyoruz
            Sabitler.bagli_client[connectionID].stream.BeginWrite(buffer.ToArray(), 0, buffer.ToArray().Length, null, null); //bağlı_client'lerin içinde connectionID ile arama yapıyoruz.
            //O connectionID'nin buffer'ına dizi şeklinde gönderimini sağlıyoruz
            buffer.Dispose();
        }

        public static async void SendDataToInGameAll(int connectionID ,byte[] data)
        {
            Kaan_ByteBuffer buffer = new Kaan_ByteBuffer();
            buffer.Int_Yaz((data.GetUpperBound(0) - data.GetLowerBound(0)) + 1);
            buffer.Bytes_Yaz(data);

            foreach (Client oyuncu in Sabitler.bagli_client.Values)
            {
                if (oyuncu != null && oyuncu.connectionID != connectionID && oyuncu.oyunda_mi )
                {
                    Sabitler.bagli_client[oyuncu.connectionID].stream.BeginWrite(buffer.ToArray(), 0, buffer.ToArray().Length, null, null);
                }
            }
            await Task.Delay(20);
            buffer.Dispose();
        }

        public static void SendDataToAll(int connectionID, byte[] data)
        {
            Kaan_ByteBuffer buffer = new Kaan_ByteBuffer();
            buffer.Int_Yaz((data.GetUpperBound(0) - data.GetLowerBound(0)) + 1);
            buffer.Bytes_Yaz(data);

            foreach (Client oyuncu in Sabitler.bagli_client.Values)
            {
                if (oyuncu != null && oyuncu.connectionID != connectionID)
                {
                    Sabitler.bagli_client[oyuncu.connectionID].stream.BeginWrite(buffer.ToArray(), 0, buffer.ToArray().Length, null, null);
                }
            }
            buffer.Dispose();

        }




    }
}
