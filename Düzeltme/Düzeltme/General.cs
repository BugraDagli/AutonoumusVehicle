using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Düzeltme
{
    class General
    {
        public static void Sunucuyu_Baslat()
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();

            ServerTCP.InitializeNetwork();
            Yazi.Log_Yaz("Sunucu Başlatıldı");
            sw.Stop();
            Yazi.Log_Yaz("Sunucu Başlama Süresi " + sw.ElapsedMilliseconds.ToString() + "ms");
        }
    }
}
