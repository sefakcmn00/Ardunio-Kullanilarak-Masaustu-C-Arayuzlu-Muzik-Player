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
using System.IO.Ports;
namespace MUSİCPLAYER_ARDUNİO
{
    public partial class Form1 : Form
    {

        bool uyarı = false;
        private DialogResult result;
        string[] dosya_yollari;
        string[] dosya_isimleri;
        bool media_oynatma = false;
        int play_buton_durumu = 0;
        string[] dosya_numaraları;
        double x = 0;
        int saat = 0,dakika=0,saniye=0;
        bool zamanlayici = false;
        string[] port_listesi;
        bool baglantı_durumu = false;
        char okunan_veri = '0';


        private void uyarilar(string uyarı_metni)
        {
            uyarı = true;
            DialogResult = MessageBox.Show(uyarı_metni,"MEDİA PLAYER UYARILAR",MessageBoxButtons.OK);
            if (result==DialogResult.OK)
            {
                uyarı = false;
            }
        }
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            DirectoryInfo klasor_bilgisi = new DirectoryInfo(@"C:\Users\ASUS\Downloads\MediaHuman\Music");
            if (klasor_bilgisi.Exists)
            {
                FileInfo[] dosyalar = klasor_bilgisi.GetFiles("*.mp3",SearchOption.AllDirectories);
                dosya_yollari = new string[dosyalar.Length];
                dosya_isimleri = new string[dosyalar.Length];
                for (int i = 0; i < dosyalar.Length; i++)
                {
                    listBox1.Items.Add(i+1+" : "+dosyalar[i].Name);
                    dosya_isimleri[i] = dosyalar[i].Name.ToString();
                    listBox2.Items.Add( dosyalar[i].FullName);
                    dosya_yollari[i] = dosyalar[i].FullName.ToString();
                }
            }
            else
            {
                uyarilar("C:/MEDİA FİLES DOSYASININA ERİŞİLEMİYOR");
            }
            button11.Enabled = false;
            trackBar1.Minimum = 5;
            trackBar1.Maximum = 150;
            portListele();
            panel4.Visible = false;
        }
        //PLAY BUTONU DURUMLARI PUASE VE PLAY ANINDA
        private void button1_Click(object sender, EventArgs e)
        {
            if (dosya_isimleri.Length>0)
            {
                if (play_buton_durumu==0)
                {
                    listBox2_yurut("Normal");
                    play_buton_durumu = 1;
                }
                else if (play_buton_durumu==1)
                {
                    media_oynatma = false;
                    axWindowsMediaPlayer1.Ctlcontrols.pause();
                    button1.ImageIndex = 0;
                    x = axWindowsMediaPlayer1.Ctlcontrols.currentPosition;
                    axWindowsMediaPlayer1.Ctlcontrols.currentPosition = x;
                    play_buton_durumu = 2;

                }
                else if (play_buton_durumu==2)
                {
                    media_oynatma = true;
                    axWindowsMediaPlayer1.Ctlcontrols.play();
                    button1.ImageIndex = 19;
                    play_buton_durumu = 1;
                }
               
            }
            else
            {
                uyarilar("OYNATILACAK MEDİA DOSYASI BULANAMADI");
            }

        }
        private void listBox2_yurut(string liste)
        {
            listBox1.Items.Clear();
            listBox2.Items.Clear();
            media_oynatma = true;
            button1.ImageIndex = 19;
            if (liste=="Normal")
            {
               var sıralı_liste= axWindowsMediaPlayer1.playlistCollection.newPlaylist("SIRALI LİSTE");
                for (int i = 0; i < dosya_isimleri.Length; i++)
                {
                    var media_item = axWindowsMediaPlayer1.newMedia(dosya_yollari[i].ToString());
                    sıralı_liste.appendItem(media_item);
                    listBox1.Items.Add(i + 1+" "+dosya_isimleri[i]);
                }
                axWindowsMediaPlayer1.currentPlaylist = sıralı_liste;

            }
            else if (liste=="Karışık")
            {
               
                    dosya_numaraları = new string[dosya_isimleri.Length];
                    Random rnd = new Random();
                    for (int i = 0; i < dosya_isimleri.Length; i++)
                    {
                        int sayi;
                        do
                        {
                            sayi = rnd.Next(0,dosya_isimleri.Length);
                            if (!dosya_numaraları.Contains(sayi.ToString()))
                            {
                                dosya_numaraları[i] = sayi.ToString();

                            }

                        } while (dosya_numaraları[i]!=sayi.ToString());
                    }
                    var karısık_liste = axWindowsMediaPlayer1.playlistCollection.newPlaylist("KARIŞIK");
                    for (int i = 0; i < dosya_isimleri.Length; i++)
                    {
                        var media_item = axWindowsMediaPlayer1.newMedia(dosya_yollari[int.Parse(dosya_numaraları[i])].ToString());
                        karısık_liste.appendItem(media_item);
                        listBox1.Items.Add(i + 1 + "" + dosya_isimleri[int.Parse(dosya_numaraları[i])]);
                        listBox2.Items.Add(i + 1 + "" + dosya_yollari[int.Parse(dosya_numaraları[i])]);


                    }
                    axWindowsMediaPlayer1.currentPlaylist = karısık_liste;
                
               
            }

        }

        private void listBox1_MouseClick(object sender, MouseEventArgs e)
        {
            
        }
        //KARIŞIK BUTONU
        private void button13_Click(object sender, EventArgs e)
        {
            if (dosya_isimleri.Length>5)
            {
                listBox2_yurut("Karışık");
            }
            else
            {
                uyarilar("5 ve Altında mp3 dosyalarımız");
            }
            
        }
        //stop butonu
        private void button2_Click(object sender, EventArgs e)
        {
            if (media_oynatma==true)
            {
                media_oynatma = false;
                axWindowsMediaPlayer1.Ctlcontrols.stop();
                button1.ImageIndex = 0;
                play_buton_durumu = 0;

            }
            else
            {
                uyarilar("MEDİA OYNATILMIYOR!");
            }
        }
        //DOSYA EKLE
        private void button12_Click(object sender, EventArgs e)
        {
            OpenFileDialog mydialog = new OpenFileDialog() {Multiselect=true,Filter="MP3|*.mp3|MP4|*.mp4|MKV|*.mkv"};
            mydialog.ShowDialog();
            int secilen_dosya_sayısı = mydialog.SafeFileNames.Length;
            if (secilen_dosya_sayısı>0)
            {
                listBox1.Items.Clear();
                listBox2.Items.Clear();
                dosya_isimleri = new string[secilen_dosya_sayısı];
                dosya_yollari = new string[secilen_dosya_sayısı];
                for (int i = 0; i < dosya_isimleri.Length; i++)
                {
                    dosya_isimleri[i] = mydialog.SafeFileNames[i];
                    dosya_yollari[i] = mydialog.SafeFileNames[i].ToString();
                    
                    
                    

                }

                listBox2_yurut("Normal");

            }
            else
            {
                uyarilar("Dosya Seçimi yapılmadı");
            }


        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (media_oynatma==false)
            {
                uyarilar("Media Oynatılamıyor");
            }
            else if (listBox1.Items.Count>1)
            {
                axWindowsMediaPlayer1.Ctlcontrols.previous();
            }
            else if (listBox1.Items.Count<2)
            {
                uyarilar("Listede 2'den az dosya var");
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (media_oynatma == false)
            {
                uyarilar("Media Oynatılamıyor");
            }
            else if (listBox1.Items.Count > 1)
            {
                axWindowsMediaPlayer1.Ctlcontrols.next();
            }
            else if (listBox1.Items.Count < 2)
            {
                uyarilar("Listede 2'den az dosya var");
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (media_oynatma==true)
            {
                axWindowsMediaPlayer1.Ctlcontrols.currentPosition -= 10;
            }
            else
            {
                uyarilar("MEDİA OYNATILMIYOR");
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {

            if (media_oynatma == true)
            {
                axWindowsMediaPlayer1.Ctlcontrols.currentPosition += 10;
            }
            else
            {
                uyarilar("MEDİA OYNATILMIYOR");
            }

        }

        private void button7_Click(object sender, EventArgs e)
        {
            axWindowsMediaPlayer1.settings.volume -= 5;
        }

        private void button8_Click(object sender, EventArgs e)
        {
            axWindowsMediaPlayer1.settings.volume += 5;
        }
        bool ses = true;
        private void button9_Click(object sender, EventArgs e)
        {
            if (ses==true)
            {
                axWindowsMediaPlayer1.settings.mute = true;
                button9.ImageIndex = 9;
                ses = false;
            }
            else 
            {
                button9.ImageIndex = 8;

                axWindowsMediaPlayer1.settings.mute = false;
                ses = true;
            }
            
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            if (trackBar1.Value<60)
            {
                saat = 0;
                textBox1.Text =saat.ToString();

            }
           else if (trackBar1.Value < 120)
            {
                saat = 1;
                textBox1.Text = saat.ToString();

            }
            else
            {
                saat = 2;
                textBox1.Text = saat.ToString();

            }
            textBox2.Text = Convert.ToString(trackBar1.Value % 60);
            textBox3.Text = "0";
        }
        //ZAMANLAYICI BAŞLATMA
        private void button10_Click(object sender, EventArgs e)
        {
            saniye = 60;
            dakika = trackBar1.Value % 60 - 1;
            textBox1.Text = saat.ToString();
            textBox2.Text = dakika.ToString();
            textBox3.Text = saniye.ToString();

            timer1.Interval = 1000;
            timer1.Start();
            button10.Enabled = false;
            button11.Enabled = true;
            trackBar1.Enabled = false;
        }

        private void button11_Click(object sender, EventArgs e)
        {
            timer1.Stop();
            button10.Enabled = true;
            button11.Enabled = false;
            saniye = 60;
            textBox1.Clear();
            textBox2.Clear();
            textBox3.Clear();
            trackBar1.Enabled = true;
            trackBar1.Value = 5;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            saniye--;
            if (saniye==0 && dakika!=0)
            {
                dakika--;
                saniye = 60;

            }
            if (dakika==0 && saat!=60)
            {
                saat--;
                dakika = 60;
            }
            textBox1.Text = saat.ToString();
            textBox2.Text = dakika.ToString();
            textBox3.Text = saniye.ToString();
            if (saat==0 && dakika==0 && saniye==0)
            {
                System.Diagnostics.Process.Start("shutdown", "/s /t 0");
            }
        }

        // tam ekran
        private void button14_Click(object sender, EventArgs e)
        {
            if (media_oynatma==true)
            {
                axWindowsMediaPlayer1.fullScreen = true;
            }
            else
            {
                uyarilar("MEDİA OYNATILAMIYOR");
            }
        }

        private void button17_Click(object sender, EventArgs e)
        {
            portListele();
        }
        // baglanbutonu
        private void button16_Click(object sender, EventArgs e)
        {
            if (comboBox1.Text != "")


            if (baglantı_durumu==false)
            {
                
                serialPort1.PortName = comboBox1.GetItemText(comboBox1.SelectedItem);
                serialPort1.BaudRate = 9600;
                if (serialPort1.IsOpen==false)
                
                    serialPort1.Open();
                    comboBox1.Enabled = false;
                    baglantı_durumu = true;
                    button16.ImageIndex = 17;
                    panel4.Visible = true;
                    button17.Enabled = false;
                    timer2.Start();
                    timer2.Interval = 20;
                
               
            }
            else
            {
                if (serialPort1.IsOpen == true)

                    serialPort1.Close();
                timer2.Stop();
                baglantı_durumu = false;
                panel4.Visible = false;
                button16.ImageIndex = 16;
                button17.Enabled = true;
                comboBox1.Enabled = true;



            }
            else
            {
                MessageBox.Show("port Seçilmedi");
            }
        }
        //CH - : A CH + : C PLAY-PAUSE: F VOL - :G VOL+ : H YİLDİZ(KARIŞIK OYNAT): I TUS1(MUTE):B
        // TUS2(ZAMANLAYICI BAŞ) : N TUS3(ZAMANLAYICI DUR ): O TUS4(ZAM DAKİKA -) : K TUS5(ZAM DAKİKA +) :L 
        //TUS6(TAM EKRAN) : R TUS7(TAM EKRAN ÇIKIS) : S 
        private void timer2_Tick(object sender, EventArgs e)
        {
            okunan_veri = Convert.ToChar(serialPort1.ReadChar());
            switch (okunan_veri)
            {
                // CH- : ÖNCEKİ DOSYAYI YÜRÜT
                case 'A': button3.PerformClick();
                    break;
                // ch + : sonraki dosyayı yürüt
                case 'C': button4.PerformClick();
                    break;
                // play and pause 
                case 'F': button1.PerformClick();
                    break;
                // sesi azalt
                case 'G':
                    button7.PerformClick();
                    break;
                // sesi ARTIR
                case 'H':
                    button8.PerformClick();
                    break;
                    
                // KARIŞIK ÇAL
                case 'I':
                    button13.PerformClick();
                    break;


                // ZAMANLAYICI - 5 DAKİKA
                case 'K':
                    if (trackBar1.Value >= 10)
                    {
                        trackBar1.Value -= 5;
                        if (trackBar1.Value < 60)
                        {
                            saat = 0;
                            textBox1.Text = saat.ToString();

                        }
                        else if (trackBar1.Value < 120)
                        {
                            saat = 1;
                            textBox1.Text = saat.ToString();

                        }
                        else
                        {
                            saat = 2;
                            textBox1.Text = saat.ToString();

                        }
                        textBox2.Text = Convert.ToString(trackBar1.Value % 60);
                        textBox3.Text = "0";
                    }
                    break;
                // ZAMANLAYICI + 5 DAKİKA
                case 'L':
                    if (trackBar1.Value <= 145)
                    {
                        trackBar1.Value += 5;
                        if (trackBar1.Value < 60)
                        {
                            saat = 0;
                            textBox1.Text = saat.ToString();

                        }
                        else if (trackBar1.Value < 120)
                        {
                            saat = 1;
                            textBox1.Text = saat.ToString();

                        }
                        else
                        {
                            saat = 2;
                            textBox1.Text = saat.ToString();

                        }
                        textBox2.Text = Convert.ToString(trackBar1.Value % 60);
                        textBox3.Text = "0";
                    }
                    break;
                // ZAMANLAYICIYI BAŞLAT
                case 'N':
                    button10.PerformClick();
                    break;
                // ZAMANLAYICIYI DURDUR
                case 'O':
                    button11.PerformClick();
                    break;
                // SES MUTE
                case 'B':
                    button9.PerformClick();
                    break;
                // TAM EKRAN
                case 'R':
                    button14.PerformClick();
                    break;
                // TAM EKRAN ÇIKIŞ
                case 'S':
                    axWindowsMediaPlayer1.fullScreen = false;   
                    break;





            }
            serialPort1.DiscardInBuffer();
        }

        private void listBox1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (listBox1.SelectedIndex!=-1)
            {
                media_oynatma = true;
                axWindowsMediaPlayer1.URL = dosya_yollari[listBox1.SelectedIndex];
                axWindowsMediaPlayer1.Ctlcontrols.play();
            }
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (serialPort1.IsOpen)
            {
                serialPort1.Close();
            }
        }

        private void button15_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("shutdown", "/s /t 0");
        }
        void portListele()
        {
            comboBox1.Items.Clear();
            port_listesi = SerialPort.GetPortNames();
            foreach (string port_adi in port_listesi)
            {
                comboBox1.Items.Add(port_adi);
                if (port_listesi[0]!=null)
                {
                    comboBox1.SelectedItem = port_listesi[0];
                }
            }
        }
    }
}
