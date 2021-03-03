using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Net;
using System.Net.Sockets;
using Emgu.CV.Structure;
using Emgu.CV;
using Emgu.CV.UI;
using Emgu.CV.OCR;

namespace Düzeltme
{
    public partial class GameServer : Form
    {


		public GameServer()
        {
            InitializeComponent();
        }

		private void pictureBox3_Click(object sender, EventArgs e)
		{

		}

		private void button1_Click(object sender, EventArgs e)
		{
			
			timer1.Start();  

		}

		Image<Bgr, byte> imgInput;
		Image<Gray, byte> imgGray;
		Image<Gray, byte> imgBinarize;
		Image<Gray, byte> imgCombined;

		int counter = 1;
		private void timer1_Tick(object sender, EventArgs e)
		{
			counter++;
			
			pictureBox3.Image = Image.FromFile(@"C:\Users\Bugra Daglı\Desktop\Snapshots\snap_"+counter+".png");
			// kaynak görüntünün kare olduğunu ve genişliğinin eşit sayıda piksel içerdiğini varsayalım
			Bitmap bm = new Bitmap(pictureBox3.Image);
			int l = bm.Width / 2;
			Bitmap bmKaynak = new Bitmap(4 * l, l);
			Bitmap bmBilinear = new Bitmap(4 * l, l);
			int i, j;
			int x, y;
			double radyan, teta;

			// kartezyen koordinatlarda komşu dizinlerde kullanım için 
			int iFloorX, iCeilingX, iFloorY, iCeilingY;
			// sonlu ondalık sayılarla Kartezyen koordinatlarda hesaplanan endeksler
			double fTrueX, fTrueY;
			// enterpolasyon için 
			double fDeltaX, fDeltaY;
			// pixel renkleri
			Color solUstRenk, sagUstRenk, solAltRenk, sagAltRenk;
			// enterpolasyonlu "üst" pikseller
			double fUstRed, fUstGreen, fUstBlue;
			// enterpolasyonlu "alt" pikseller 
			double fAltRed, fAltGreen, fAltBlue;
			// son enterpolasyonlu renk bileşenleri
			int iRed, iGreen, iBlue;

			for (i = 0; i < bmKaynak.Height; ++i)
			{
				for (j = 0; j < bmKaynak.Width; ++j)
				{
					radyan = (double)(l - i);
					// teta = 2.0 * Math.PI * (double) (4.0 * l - j) / (double) (4.0 * l);
					teta = 2.0 * Math.PI * (double)(-j) / (double)(4.0 * l);

					fTrueX = radyan * Math.Cos(teta);
					fTrueY = radyan * Math.Sin(teta);

					// "normal" mod
					x = (int)(Math.Round(fTrueX)) + l;
					y = l - (int)(Math.Round(fTrueY));
					// sınırları kontrol et
					if (x >= 0 && x < (2 * l) && y >= 0 && y < (2 * l))
					{
						bmKaynak.SetPixel(j, i, bm.GetPixel(x, y));
					}

					// bilinear mod
					fTrueX = fTrueX + (double)l;
					fTrueY = (double)l - fTrueY;

					iFloorX = (int)(Math.Floor(fTrueX));
					iFloorY = (int)(Math.Floor(fTrueY));
					iCeilingX = (int)(Math.Ceiling(fTrueX));
					iCeilingY = (int)(Math.Ceiling(fTrueY));

					// sınırları kontrol et
					if (iFloorX < 0 || iCeilingX < 0 ||
						iFloorX >= (2 * l) || iCeilingX >= (2 * l) ||
						iFloorY < 0 || iCeilingY < 0 ||
						iFloorY >= (2 * l) || iCeilingY >= (2 * l)) continue;

					fDeltaX = fTrueX - (double)iFloorX;
					fDeltaY = fTrueY - (double)iFloorY;

					solUstRenk = bm.GetPixel(iFloorX, iFloorY);
					sagUstRenk = bm.GetPixel(iCeilingX, iFloorY);
					solAltRenk = bm.GetPixel(iFloorX, iCeilingY);
					sagAltRenk = bm.GetPixel(iCeilingX, iCeilingY);

					// üst komşular arasında yatay olarak enterpolasyon
					fUstRed = (1 - fDeltaX) * solUstRenk.R + fDeltaX * sagUstRenk.R;
					fUstGreen = (1 - fDeltaX) * solUstRenk.G + fDeltaX * sagUstRenk.G;
					fUstBlue = (1 - fDeltaX) * solUstRenk.B + fDeltaX * sagUstRenk.B;

					// alt komşular arasında yatay olarak enterpolasyon
					fAltRed = (1 - fDeltaX) * solAltRenk.R + fDeltaX * sagAltRenk.R;
					fAltGreen = (1 - fDeltaX) * solAltRenk.G + fDeltaX * sagAltRenk.G;
					fAltBlue = (1 - fDeltaX) * solAltRenk.B + fDeltaX * sagAltRenk.B;

					// üst ve alt enterpolasyonlu sonuçlar arasında dikey olarak enterpolasyon
					iRed = (int)(Math.Round((1 - fDeltaY) * fUstRed + fDeltaY * fAltRed));
					iGreen = (int)(Math.Round((1 - fDeltaY) * fUstGreen + fDeltaY * fAltGreen));
					iBlue = (int)(Math.Round((1 - fDeltaY) * fUstBlue + fDeltaY * fAltBlue));

					// renk değerlerinin geçerli olduğunun kontrolü
					if (iRed < 0) iRed = 0;
					if (iRed > 255) iRed = 255;
					if (iGreen < 0) iGreen = 0;
					if (iGreen > 255) iGreen = 255;
					if (iBlue < 0) iBlue = 0;
					if (iBlue > 255) iBlue = 255;

					bmBilinear.SetPixel(j, i, Color.FromArgb(iRed, iGreen, iBlue));
				}
			}

			bmKaynak.Save(@"C:\Users\Bugra Daglı\Desktop\Duzelmis\bilinearsiz.png", System.Drawing.Imaging.ImageFormat.Bmp);
			bmBilinear.Save(@"C:\Users\Bugra Daglı\Desktop\Duzelmis\duzeltilmis.png", System.Drawing.Imaging.ImageFormat.Bmp);

			//pictureBox4.Image = bmKaynak;
			pictureBox4.Image = bmBilinear;
		}

		private void Durdur_Click(object sender, EventArgs e)
		{
			timer1.Stop();
		}

		private void GameServer_Load(object sender, EventArgs e)
		{
			richTextBox1.AppendText("Sunucu Başlatılıyor");
			richTextBox2.AppendText("Sunucu Başlatılıyor");
		}

		private void button2_Click(object sender, EventArgs e)
		{
			General.Sunucuyu_Baslat();
			button2.Enabled = false;
		}


		private void button4_Click(object sender, EventArgs e)
        {
			Image<Gray, byte> gray = new Image<Gray, byte>(@"C:\Users\Bugra Daglı\Desktop\Duzelmis\duzeltilmis.png");
			Image<Gray, float> sobel = gray.Sobel(0, 1, 3).Add(gray.Sobel(1, 0, 3)).AbsDiff(new Gray(0.0));
			pictureBox1.Image = sobel.ToBitmap();


		}

		
		private void button5_Click(object sender, EventArgs e)
        {
			Image<Gray, byte> imgInput = new Image<Gray, byte>(@"C:\Users\Bugra Daglı\Desktop\Duzelmis\duzeltilmis.png");


			Image<Gray, byte> imgOutput = imgInput.Convert<Gray, byte>().ThresholdBinary(new Gray(100), new Gray(255));
			Emgu.CV.Util.VectorOfVectorOfPoint contours = new Emgu.CV.Util.VectorOfVectorOfPoint();
			Mat hier = new Mat();

			Image<Gray, byte> imgout = new Image<Gray, byte>(imgInput.Width, imgInput.Height, new Gray(0));

			CvInvoke.FindContours(imgOutput, contours, hier, Emgu.CV.CvEnum.RetrType.External, Emgu.CV.CvEnum.ChainApproxMethod.ChainApproxSimple);
			CvInvoke.DrawContours(imgout, contours, -1, new MCvScalar(255, 0, 0));

			pictureBox1.Image = imgout.ToBitmap();
		}
		

        private void button6_Click(object sender, EventArgs e)
        {
			Image<Gray, byte> imgInput = new Image<Gray, byte>(@"C:\Users\Bugra Daglı\Desktop\Duzelmis\duzeltilmis.png");

			imgGray = imgInput.Convert<Gray, byte>();

			//binarization thresholding
			imgBinarize = new Image<Gray, byte>(imgGray.Width, imgGray.Height, new Gray(0));
			CvInvoke.Threshold(imgGray, imgBinarize, 500, 225, Emgu.CV.CvEnum.ThresholdType.Otsu);
			pictureBox1.Image = imgBinarize.ToBitmap();
		}
    }
}
