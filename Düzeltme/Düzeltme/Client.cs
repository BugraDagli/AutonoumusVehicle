using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Düzeltme
{
    class Client
    {
        //socket bilgileri ,her client sınıfı oluşturduğumuzda onun için bir socket numarası atıyoruz
        public TcpClient socket; // socket no
        public NetworkStream stream; //Network Streamde yeni bir ID tanımlıyoruz
        private byte[] recBuffer;
        public Kaan_ByteBuffer buffer;
        public int connectionID;
        public bool oyunda_mi = false;
        // -->> Client Manager
        public void Start()
        {
            socket.SendBufferSize = 4096;    //socketin göndereceği bytesize'ı  / tek seferde göndermiyor böle böle gönderiyor
            socket.ReceiveBufferSize = 4096; //alabileceği bytesize'ı
            stream = socket.GetStream();
            recBuffer = new byte[4096];
            stream.BeginRead(recBuffer, 0, socket.ReceiveBufferSize, OnReceiveData, null);
            Sabitler.Oyuncu_Baglandi(connectionID.ToString()); // ------
        }
        
        private void OnReceiveData(IAsyncResult result) //Tcp asenkron
        {
            try
            {
                int length = stream.EndRead(result);
                if (length <= 0) //eğer gelen byte 0'dan küçükse connection'ı kapatıyoruz çünkü hiçbir zaman 0'dan küçük byte gelmez.
                {
                    CloseConnection();
                    return;
                }
                //Client Manager
                byte[] newBytes = new byte[length];
                Array.Copy(recBuffer, newBytes, length);
                ServerHandleData.HandleData(connectionID, newBytes);
                stream.BeginRead(recBuffer, 0, socket.ReceiveBufferSize, OnReceiveData, null);
            }

            catch (Exception)
            {
                CloseConnection();
                return;
            }
        }

        private void CloseConnection()
        {
            Sabitler.bagli_client.Remove(connectionID);
            Sabitler.Oyuncu_cikti(connectionID.ToString());
            socket.Close();
        }
    }



   
}

