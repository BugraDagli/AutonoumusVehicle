using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace Düzeltme
{
    class Sabitler
    {
        public static GameServer server = ((GameServer)Application.OpenForms.OfType<GameServer>().SingleOrDefault());
        public static Dictionary<int, Client> bagli_client = new Dictionary<int, Client>();


        public static void Oyuncu_Baglandi(string connectionID)
        {
            bagli_kullanici_sayisi++;
            Yazi.Log_Yaz("Kullanıcı sunucuya bağlandı : " + connectionID);
            Sabitler.server.listBox1.Items.Add(connectionID);
        }

        public static void Oyuncu_cikti(string connectionID)
        {
            bagli_kullanici_sayisi--;
            Yazi.Log_Yaz("Kullanıcı sunucudan ayrıldı : " + connectionID);
            Sabitler.server.listBox1.Items.Remove(connectionID);
        }

        private static int bagli_kullanici_sayisi_ = 0;
        public static int bagli_kullanici_sayisi
        {
            get
            {
                return bagli_kullanici_sayisi_;
            }

            set
            {
                bagli_kullanici_sayisi_ = value;
                server.label1.Text = "Bağlı kullanıcı sayısı : " + bagli_kullanici_sayisi.ToString();
            }
        }
    }
}
