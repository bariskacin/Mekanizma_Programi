using System;
using System.Drawing;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Tab;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        Graphics grafikMekanizma;
        Graphics grafikEkran;

        Pen KalemBeyaz10 = new Pen(System.Drawing.Color.White, 10);
        Pen KalemMavi6 = new Pen(System.Drawing.Color.Blue, 6);
        Pen KalemSari6 = new Pen(System.Drawing.Color.Yellow, 6);
        Pen KalemKirmizi6 = new Pen(System.Drawing.Color.Red, 6);
        Pen KalemTurkuaz3 = new Pen(System.Drawing.Color.Turquoise, 3);
        Pen KalemPembe3 = new Pen(System.Drawing.Color.Pink, 3);
        Pen KalemTuruncu3 = new Pen(System.Drawing.Color.Orange, 3);
        Pen KalemYesil3 = new Pen(System.Drawing.Color.Green, 3);
        Pen KalemSari3 = new Pen(System.Drawing.Color.Yellow, 3);
        Pen KalemKirmizi3 = new Pen(System.Drawing.Color.Red, 3);
        Pen KalemMavi3 = new Pen(System.Drawing.Color.Blue, 3);

        //Başlangıç değerleri. Tüm fonksiyonlarda kullanılacağından Global tanımlandı.
        double OrijinX = 200;
        double OrijinY = 300;

        double Ax = 0;
        double Ay = 0;

        double Bx = 0;
        double By = 0;

        double Cx = 0;
        double Cy = 0;

        double Aci1 = 180.0, Aci2 = 60, Aci3 = 0, Aci4 = 0; //Dereceler gösterimlerde kullanılıyor

        double Aci2Radyan = 60, Aci3Radyan = 0, Aci4Radyan = 0; //Radyanlar hesaplamalarda kullanılıyor

        double R1 = 0;  //AD şasi çubuğu
        double R2 = 125;  //AB motora bağlı çubuk
        double R3 = 200;  //BC ortadaki çubk

        private void Form1_Load(object sender, EventArgs e)
        {
            grafikMekanizma = pictureBox1.CreateGraphics();
            grafikEkran = pictureBoxGrafikCizdir.CreateGraphics();

            this.WindowState = FormWindowState.Maximized; // Formu tam ekran aç
        }

        bool MekanizmaCalismaDurumu = false; //başlangıçta mekanizma çalışmıyor olacak

        private void MEKANIZMAYI_CALISTIR_DURDUR(object sender, EventArgs e)
        {
            if (MekanizmaCalismaDurumu == false) //mekanizma çalışmıyorsa çalıştıracak.
            {
                MekanizmaCalismaDurumu = true;
                timer1.Enabled = true;
                btn_MEKANIZMAYI_CALISTIR_DURDUR.Text = "DURDUR";
                btn_MEKANIZMAYI_CALISTIR_DURDUR.BackColor = Color.Red;

            }
            else
            {
                MekanizmaCalismaDurumu = false;
                timer1.Enabled = false;
                btn_MEKANIZMAYI_CALISTIR_DURDUR.Text = "BAŞLAT";
                btn_MEKANIZMAYI_CALISTIR_DURDUR.BackColor = Color.Magenta;
            }
        }

        //************** BÜTÜN HESAPLAR BURADAN ÇIKIŞ YAPIYOR ************* TİMER
        private void timer1_Tick(object sender, EventArgs e)
        {
            KONUM_HESAPLAMA();

            STATIK_HESAPLAMA();

            HIZ_ANALIZI();

            İVME_Analizi();

            Aci2++; //Aci2 motorun bağlı olduğu açıdır. Burada saat her tik attığında açıyı artırıyor. 
            Aci2 = Aci2 % 360; //360 dereceyi geçtiğinde tekrar sıfırlamak için Mod (kalan) kullanıldı.
            txtTahrikAcisi.Text = Aci2.ToString(); //Mekanizmayı döndüren tahrik açısı. Yani motorun açısı. 

            timer1.Interval = trackBar1.Value * trackBar1.Value; //Timer hızını trackbardan alıyor.
        }

        //****************** BELLİ BİR AÇIDA ÇALIŞTIR ****************************
        private void BELLI_BIR_ACIDA_HESAPLA_Click(object sender, EventArgs e)
        {
            Aci2 = Convert.ToDouble(txtTahrikAcisi.Text);

            KONUM_HESAPLAMA();

            STATIK_HESAPLAMA();

            HIZ_ANALIZI();

            İVME_Analizi();
        }

        //========================= KONUM HESAPLAMA FONKSİYONLARI =============================
        //========================= KONUM HESAPLAMA FONKSİYONLARI =============================
        //========================= KONUM HESAPLAMA FONKSİYONLARI =============================
        //========================= KONUM HESAPLAMA FONKSİYONLARI =============================
        //========================= KONUM HESAPLAMA FONKSİYONLARI =============================

        public void KONUM_HESAPLAMA()
        {
            AcilariVeKonumlariHesapla();

            MekanizmaCiziminiGoster();
            VektorelKonumTablosunuGoster();
            MafsalKonumTablosunuGoster();
            MafsalNoktalariniCizimleGoster();
        }

        //************************* HESAPLA VE GÖSTER *********************
        public void AcilariVeKonumlariHesapla()
        {
            Bx = Ax + R2 * Math.Cos(Radyan(Aci2));
            By = Ay + R2 * Math.Sin(Radyan(Aci2));
            Aci3Radyan = Aci3Hesapla(Radyan(Aci2)); //Alt fonksiyona giderken Aci2 yi radyana çevirip götürüyor.

            R1 = R2 * Math.Sin(AciAlfaRadyan) + R3 * Math.Sin(AciBetaRadyan);
            Cx = Ax + R1;
        }
        //************ ACI3 HESAPLANIYOR ************************** 
        double m = 0; //Bu değişken Aci4 hesaplarken de kullanılacağından globalde tanımlandı.
        double AciAlfaRadyan = 0; //Bu değişken Aci4 hesaplarken de kullanıldığından tanımlama Globalde yapıldı.
        double AciBetaRadyan = 0;
        double AciGamaRadyan = 0;
        double h = 0;

        public double Aci3Hesapla(double Aci2Radyan)
        {

            h = R2 * Math.Sin(Aci2Radyan);

            AciAlfaRadyan = (Math.PI / 2) - Aci2Radyan;
            AciBetaRadyan = Math.Acos(h / R3);
            AciGamaRadyan = Math.Asin(h / R3);

            Aci3Radyan = 2 * Math.PI - AciGamaRadyan;

            return Aci3Radyan;
        }

        //SONUÇLARI GÖSTERME*****************
        public void MekanizmaCiziminiGoster()
        {
            //Her çizimden önce ekranı temizleyip mekanızmayı yeniden çiziyor. Böylece mekanizma hareket etmiş görünüyor.
            pictureBox1.Refresh();

            //MEKANİZMA çubuklarını grafik ekranda ÇİZDİRİYOR.
            CizgiCiz(KalemKirmizi6, Ax, Ay, Bx, By);
            CizgiCiz(KalemKirmizi6, Bx, By, Cx, Cy);

            //Vektörel KONUM TABLOSUNU Ekranda GÖSTERİYOR
            txtTahrikAcisi.Text = Aci2.ToString(); //Mekanizmayı döndüren tahrik açısı. Yani motorun açısı. 

            txtR1_L.Text = R1.ToString();
            txtR1_θ.Text = Aci1.ToString();

            txtR2_L.Text = R2.ToString();
            txtR2_θ.Text = Aci2.ToString();

            txtR3_L.Text = R3.ToString();
            Aci3 = Derece(Aci3Radyan) % 360;//Acilar derece olarak ve 360ın modu şeklinde (kalanı şeklinde) gösteriliyor
            txtR3_θ.Text = Aci3.ToString();  
        }


        public void VektorelKonumTablosunuGoster()
        {
            //Vektörel KONUM TABLOSUNU Ekranda GÖSTERİYOR

            txtR1_L.Text = R1.ToString();
            txtR1_θ.Text = Aci1.ToString();

            txtR2_L.Text = R2.ToString();
            txtR2_θ.Text = Aci2.ToString();

            txtR3_L.Text = R3.ToString();
            Aci3 = Derece(Aci3Radyan) % 360;//Acilar derece olarak ve 360ın modu şeklinde (kalanı şeklinde) gösteriliyor
            txtR3_θ.Text = Aci3.ToString(); 
        }


        public void MafsalKonumTablosunuGoster()
        {
            txtAx.Text = Ax.ToString();
            txtAy.Text = Ay.ToString();

            txtBx.Text = Bx.ToString();
            txtBy.Text = By.ToString();

            txtCx.Text = Cx.ToString();
            txtCy.Text = Cy.ToString();
        }

        public void MafsalNoktalariniCizimleGoster()
        {
            DaireCiz(KalemBeyaz10, Ax, Ay);
            DaireCiz(KalemBeyaz10, Bx, By);
            DaireCiz(KalemBeyaz10, Cx, Cy);
        }

        public void CizgiCiz(Pen Kalem, double X1, double Y1, double X2, double Y2)
        {
            if (double.IsNaN(X1) || double.IsNaN(Y1) || double.IsNaN(X2) || double.IsNaN(Y2))
            {
                // NaN Sonuçları değerlendiriliyor.
                Console.WriteLine("Input değerlerde NaN bulunmakta. Cizgi oluşturulamadı.");
                return;
            }
            //Dikkat orijini ekranın sol üst köşesinden ortaya taşıyor (OrijinX, OrijinY). Ayrıca ekranda y eksenleri aşağı baktığından bunları yukarı çevirmek için -1 ile çarpıyor) 
            grafikMekanizma.DrawLine(Kalem, (int)(X1 + OrijinX), (int)(Y1 * (-1) + OrijinY), (int)(X2 + OrijinX), (int)(Y2 * (-1) + OrijinY));
        }

        public void DaireCiz(Pen Kalem, double X, double Y)
        {
            if (double.IsNaN(X) || double.IsNaN(Y))
            {
                // NaN Sonuçları değerlendiriliyor.
                Console.WriteLine("Input değerlerde NaN bulunmakta. Daire oluşturulamadı.");
                return;
            }
            //Dikkat orijini ekranın sol üst köşesinden ortaya taşıyor (OrijinX, OrijinY). Ayrıca ekranda y eksenleri aşağı baktığından bunları yukarı çevirmek için -1 ile çarpıyor) 
            grafikMekanizma.DrawEllipse(Kalem, (int)((X) + OrijinX), (int)((Y) * (-1) + OrijinY), 1, 1); //Genişliği ve yüksekliği 1 piksel bir daire çiziyor. Daire gözükmesi için Kalem çapı yukarıda 10 piksel yapıldı.
        }

        //Derece gelen açıları Radyan'a dönüştürürüyor
        public double Radyan(double AciDerece)
        {
            double AciRadyan = AciDerece * 2 * Math.PI / 360.0;
            return AciRadyan;
        }

        //Radyan gelen açıları Dereceye dönüştürüyor
        public double Derece(double AciRadyan)
        {
            double AciDerece = (AciRadyan * 360.0) / (2 * Math.PI);
            return AciDerece;
        }

        //======================= STATİK HESAPLAMA FONKSİYONLARI =================
        //======================= STATİK HESAPLAMA FONKSİYONLARI =================
        //======================= STATİK HESAPLAMA FONKSİYONLARI =================
        //======================= STATİK HESAPLAMA FONKSİYONLARI =================

        //Yukarıda Timer içinden buraya adres veriliyor. Konum hesabındaki tüm değişkenler global olduğu için burada da geçerli. 
        //Statik hesapta kullandığımız büyüklükleri tanımlıyoruz. 
        double F = 0;
        double ϕ = 0;

        double T = 0;

        double F12 = 0;
        double θ12 = 0;

        double F32 = 0;
        double θ32 = 0;

        double F23 = 0;
        double θ23 = 0;

        double F43 = 0;
        double θ43 = 0;

        double F34 = 0;
        double θ34 = 0;

        double F14 = 0;
        double θ14 = 0;

        double DG4 = 0;
        double θDG4 = 0;

        double DC = 0;
        double θDC = 0;

        double AB = 0;

        double θ2 = 0;
        double T_Nm = 0;

        public void STATIK_HESAPLAMA()
        {
            StatikKuvvetVektorleriniHesapla();
            StatikKuvvetVektorleriniEkrandaGoster();
            StatikKuvvetVektorleriniGrafikEkrandaCizdir();
            TorkGrafiginiGoster();
        }

        public void StatikKuvvetVektorleriniHesapla()
        {
            //Notlardaki hesaplama sırasına göre işlemleri burada yapalım. 
            F = 500; //Problemde veriliyor. Mekanizmaya dışarıdan etki eden kuvvet ve açısı
            ϕ = 45;  

            DC = R3;
            DG4 = R3 / 2.0;
            θDG4 = Aci4 - 180;
            θDC = Aci4 - 180;
            θ34 = Aci3 + 180;

            F34 = DG4 * F * Math.Sin(Radyan(θDG4) - Radyan(ϕ)) / (DC * Math.Sin(Radyan(θDC) - Radyan(θ34)));

            F43 = F34;
            F23 = F34;
            F32 = F34;
            F12 = F34;

            θ43 = Aci3;
            θ23 = Aci3 + 180;
            θ32 = Aci3;
            θ12 = Aci3 + 180;

            double θ14radyan = Math.Atan((F * Math.Sin(Radyan(ϕ)) + F34 * Math.Sin(Radyan(θ34))) / (F * Math.Cos(Radyan(ϕ)) + F34 * Math.Cos(Radyan(θ34))));

            θ14 = Derece(θ14radyan);

            F14 = (-F * Math.Cos(Radyan(ϕ)) - F34 * Math.Cos(Radyan(θ34))) / Math.Cos(Radyan(θ14));

            if (F14 < 0) //kuvvet negatif çıkarsa pozitife çevirip açısını 180 derece döndürecek. 
            {
                F14 = -1 * F14;
                θ14 = θ14 + 180;
            }

            AB = R2;
            θ2 = Aci2;
            T = -AB * F32 * Math.Sin(Radyan(θ32) - Radyan(θ2));

            T_Nm = T / 1000;
        }

        public void StatikKuvvetVektorleriniEkrandaGoster()
        {
            txt_F_L.Text = F.ToString();
            txt_F_ϕ.Text = ϕ.ToString();

            txt_T_L.Text = T_Nm.ToString();

            if (T_Nm > 0)//momentin yönü  
                txt_T_θ.Text = "Sola";
            else
                txt_T_θ.Text = "Sağa";

            txt_F12_L.Text = F12.ToString();
            txt_F12_θ.Text = θ12.ToString();

            txt_F32_L.Text = F32.ToString();
            txt_F32_θ.Text = θ32.ToString();

            txt_F23_L.Text = F23.ToString();
            txt_F23_θ.Text = θ23.ToString();

            txt_F43_L.Text = F43.ToString();
            txt_F43_θ.Text = θ43.ToString();

            txt_F34_L.Text = F34.ToString();
            txt_F34_θ.Text = θ34.ToString();

            txt_F14_L.Text = F14.ToString();
            txt_F14_θ.Text = θ14.ToString();

        }

        public void StatikKuvvetVektorleriniGrafikEkrandaCizdir()
        {
            double BoyOlcegi = 0.2; //Vektöleri gösterirken boylarını ayarlamak için kullanılıyor.  

            VektorCiz(KalemSari3, F43 * BoyOlcegi, θ43, Cx, Cy); //F43 vektörü C noktasındadır.
            VektorCiz(KalemMavi3, F34 * BoyOlcegi, θ34, Cx, Cy); //F43 vektörü C noktasındadır.

            VektorCiz(KalemSari3, F23 * BoyOlcegi, θ23, Bx, By); //F23 vektörü B noktasındadır.
            VektorCiz(KalemMavi3, F32 * BoyOlcegi, θ32, Bx, By); //F32 vektörü B noktasındadır.

            VektorCiz(KalemMavi3, F12 * BoyOlcegi, θ12, Ax, Ay); //F12 vektörü A noktasındadır.

            //F kuvvetinin vektörünü çizdiriyor. G4 noktasının koordinatı hesaplanmalı
            double G4x = Cx + (DC * 0.5) * Math.Cos(Radyan(θDC));
            double G4y = Cy + (DC * 0.5) * Math.Sin(Radyan(θDC));

            VektorCiz(KalemMavi6, F * BoyOlcegi, ϕ, G4x, G4y);

            //T moment Vektörünü Çiziyor. Yönü renkle gösterecek.

            if(T_Nm>0)
                MomentVektorCiz(KalemYesil3, T_Nm, Ax, Ay);
            else
                MomentVektorCiz(KalemPembe3, T_Nm, Ax, Ay);
        }

        public void VektorCiz(Pen Kalem, double Boy, double Aci, double Mx, double My) //Vektörün boy, açı ve merkez noktasının koordinatları
        {
            int X1 = Convert.ToInt32(Mx);
            int Y1 = Convert.ToInt32(My);
            //int X2 = Convert.ToInt32(X1 + Boy * Math.Cos(Radyan(Aci)));
            //int Y2 = Convert.ToInt32(Y1 + Boy * Math.Sin(Radyan(Aci)));
            double tempX2 = X1 + Boy * Math.Cos(Radyan(Aci));
            double tempY2 = Y1 + Boy * Math.Sin(Radyan(Aci));

            // Handle potential overflow by clamping the values within Int32 range
            int X2 = (tempX2 > int.MaxValue) ? int.MaxValue : (tempX2 < int.MinValue) ? int.MinValue : (int)tempX2;
            int Y2 = (tempY2 > int.MaxValue) ? int.MaxValue : (tempY2 < int.MinValue) ? int.MinValue : (int)tempY2;

            try
            {
                grafikMekanizma.DrawLine(Kalem, (int)(X1 + OrijinX), (int)(Y1 * (-1) + OrijinY), (int)(X2 + OrijinX), (int)(Y2 * (-1) + OrijinY));
            }
            catch (OverflowException ex)
            {
                // Overflow hatası oluştuğunda yapılacak işlemler
                Console.WriteLine("Overflow hatası oluştu: " + ex.Message);
                // Hata oluştuğunda devam etmek için bu bloğu boş bırakabilirsiniz.
            }
        }

        public void MomentVektorCiz(Pen Kalem, double Moment, double Mx, double My)
        {
            grafikMekanizma.DrawEllipse(Kalem, (int)((Mx + OrijinX)-Moment), (int)((My * (-1) + OrijinY)-Moment), (int)(Moment*2), (int)(Moment*2));
        }

        double x1 = 0;
        double y1 = 0;
        double x2 = 0;
        double y2 = 0;
        double OlcekX = 2;
        double OlcekY = 3;

        public void TorkGrafiginiGoster()
        {
            int OrijinOtelemeX = 50;
            int OrijinOtelemeY = 200;

            //x ve y eksenlerini gösteriyor.
            grafikEkran.DrawLine(KalemKirmizi3, (int)(0 + OrijinOtelemeX), (int)(0 + OrijinOtelemeY), (int)( 360 * OlcekX + OrijinOtelemeX), (int)(0 + OrijinOtelemeY));
            grafikEkran.DrawLine(KalemKirmizi3, (int)(0 + OrijinOtelemeX), (int)(0), (int)(0 + OrijinOtelemeX), (int)(OrijinOtelemeY*2));

            //Grafiği çizdirirken x ekseni Aci2 (motor açısı) olacak. Y ekseni de ortaya çıkan Tork'un değeri olacak. 
            x2 = (Aci2 % 360)* OlcekX;
            y2 = T_Nm * OlcekY;

            grafikEkran.DrawLine(KalemSari3, (int)(x1 + OrijinOtelemeX), (int)(y1 * (-1) + OrijinOtelemeY), (int)(x2 + OrijinOtelemeX), (int)(y2 * (-1) + OrijinOtelemeY));
            x1 = (Aci2 % 360)* OlcekX;
            y1 = T_Nm * OlcekY;

            if((Aci2 % 360) == 0)
            {
                pictureBoxGrafikCizdir.Refresh();
            }
        }


        //======================= HIZ HESAPLAMA FONKSİYONLARI =================
        //======================= HIZ HESAPLAMA FONKSİYONLARI =================
        //======================= HIZ HESAPLAMA FONKSİYONLARI =================

        double acisalhiz1 = 0;
        double acisalhiz2 = 15;
        double acisalhiz3 = 0;


        public void HIZ_ANALIZI()
        {
            double R3_θ = Convert.ToDouble(txtR3_θ.Text);
            double R2_θ = Convert.ToDouble(txtR2_θ.Text);
            double R1_θ = Convert.ToDouble(txtR1_θ.Text);

            acisalhiz1 = -R2 * acisalhiz2 * Math.Sin(Radyan(R3_θ) - Radyan(R2_θ)) / (R1 * Math.Sin(Radyan(R3_θ) - Radyan(R1_θ)));
            acisalhiz3 = -R2 * acisalhiz2 * Math.Sin(Radyan(R1_θ) - Radyan(R2_θ)) / (R3 * Math.Sin(Radyan(R1_θ) - Radyan(R3_θ)));

            txtAcisalHiz1.Text = acisalhiz1.ToString();
            txtAcisalHiz2.Text = acisalhiz2.ToString();
            txtAcisalHiz3.Text = acisalhiz3.ToString();
        }

        //======================= İVME HESAPLAMA FONKSİYONLARI =================
        //======================= İVME HESAPLAMA FONKSİYONLARI =================
        //======================= İVME HESAPLAMA FONKSİYONLARI =================

        double acisalivme2 = 0;
        double acisalivme3 = 0;

        public void İVME_Analizi()
        {
            double R3_θ = Convert.ToDouble(txtR3_θ.Text);
            double R2_θ = Convert.ToDouble(txtR2_θ.Text);
            double R1_θ = Convert.ToDouble(txtR1_θ.Text);

            acisalhiz1 = -R2 * acisalhiz2 * Math.Sin(Radyan(R3_θ) - Radyan(R2_θ)) / (R1 * Math.Sin(Radyan(R3_θ) - Radyan(R1_θ)));
            acisalhiz3 = -R2 * acisalhiz2 * Math.Sin(Radyan(R1_θ) - Radyan(R2_θ)) / (R3 * Math.Sin(Radyan(R1_θ) - Radyan(R3_θ)));

            acisalivme2 = R2 * acisalhiz2 * acisalhiz2 * Math.Cos(Radyan(R2_θ) - Radyan(R3_θ)) - R3 * acisalhiz3 * acisalhiz3 / (R2 * Math.Sin(Radyan(R2_θ) - Radyan(R3_θ)));
            acisalivme3 = R2 * acisalhiz2 * acisalhiz2 * Math.Cos(Radyan(R3_θ) - Radyan(R2_θ)) / (R3 * Math.Sin(Radyan(R3_θ) - Radyan(R2_θ)));

            txtIvme2.Text = acisalivme2.ToString();
            txtIvme3.Text = acisalivme3.ToString();
        }
    }
}
