using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Düzeltme
{
    class ServerTCP// HAZIR
    {
        static TcpListener serverSocket = new TcpListener(IPAddress.Any, 6060);

        public static void InitializeNetwork()
        {
            Yazi.Log_Yaz("Paketleriniz Başlatılıyor");
            ServerHandleData.InitializePackets();
            serverSocket.Start();
            serverSocket.BeginAcceptTcpClient(new AsyncCallback(OnClientConnect), null);

        }

        private static void OnClientConnect(IAsyncResult result)
        {
            TcpClient client = serverSocket.EndAcceptTcpClient(result);
            serverSocket.BeginAcceptTcpClient(new AsyncCallback(OnClientConnect), null);
            ClientManager.CreateNewConnection(client);
        }



    }
}
