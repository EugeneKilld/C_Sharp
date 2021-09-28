using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using libzkfpcsharp;
using System.Runtime.InteropServices;
using System.IO;
using Sample;
using System.Drawing.Imaging;

namespace Diplom
{
    public partial class Registration : Form
    {
        const int N = 200;
        int[] link1_1 = new int[N];
        int[] link1_2 = new int[N];
        int[] link2_1 = new int[N];
        int[] link2_2 = new int[N];
        int[] link3_1 = new int[N];
        int[] link3_2 = new int[N];
        Bitmap fingerprint;
        bool[,] picture_user;
        int[,] img_user;
        double[,] roi;
        Bitmap binary_fingerprint, grad_fingerprint;
        int[,] const_grad_p;
        double[,] pointModule, pointAngle, areaModule, areaAngle;

        int mfpWidth = 300;
        int Register_Cntr = 0;
        int mfpHeight = 375;
        int cbCapTmp = 2048;
        byte[] FPBuffer;
        byte[] CapTmp = new byte[2048];
        bool bIsTimeToDie = false;
        IntPtr FormHandle = IntPtr.Zero;
        public static IntPtr mDevHandle = IntPtr.Zero;
        const int MESSAGE_CAPTURED_OK = 0x0400 + 6;
        const int MESSAGE_EMPTY_FIELD = 0x0400 + 5;

        [DllImport("user32.dll", EntryPoint = "SendMessageA")]

        public static extern int SendMessage(IntPtr hwnd, int wMsg, IntPtr wParam, IntPtr lParam);
        public Registration()
        {
            InitializeComponent();

            FPBuffer = new byte[mfpWidth * mfpHeight];
            Thread captureThread = new Thread(new ThreadStart(DoCapture));
            captureThread.IsBackground = true;
            captureThread.Start();
            bIsTimeToDie = false;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Form ifrm = Application.OpenForms[0];
            ifrm.StartPosition = FormStartPosition.Manual; // меняем параметр StartPosition у Form1, иначе она будет использовать тот, который у неё прописан в настройках и всегда будет открываться по центру экрана
            ifrm.Left = this.Left; // задаём открываемой форме позицию слева равную позиции текущей формы
            ifrm.Top = this.Top; // задаём открываемой форме позицию сверху равную позиции текущей формы
            ifrm.Show();
            this.Close();
        }

        private void DoCapture()
        {
            while (!bIsTimeToDie)
            {
                if (Last_Name.Text == "" && First_Name.Text == "")
                {
                    SendMessage(FormHandle, MESSAGE_EMPTY_FIELD, IntPtr.Zero, IntPtr.Zero);
                }
                else
                {
                    cbCapTmp = 2048;
                    int ret = zkfp2.AcquireFingerprint(mDevHandle, FPBuffer, CapTmp, ref cbCapTmp);
                    if (ret == zkfp.ZKFP_ERR_OK)
                    {
                        SendMessage(FormHandle, MESSAGE_CAPTURED_OK, IntPtr.Zero, IntPtr.Zero);
                    }
                }
                Thread.Sleep(200);
            }
        }

        protected override void DefWndProc(ref Message m)
        {
            switch (m.Msg)
            {
                case MESSAGE_EMPTY_FIELD:
                    {
                        Note.Text = "Заполните обязательные поля \"Фамилия\" и \"Имя\"";
                    }
                    break;
                case MESSAGE_CAPTURED_OK:
                    {
                        MemoryStream ms = new MemoryStream();
                        BitmapFormat.GetBitmap(FPBuffer, mfpWidth, mfpHeight, ref ms);
                        Bitmap bmp = new Bitmap(ms);
                        this.pictureBox1.Image = bmp;
                        //сохраняем изображение пальчика
                        //запрос на сервер - зарегистрирован ли челикс, если да, то вывод сообщения в note и завершение switch
                        if(DB.SEARCH(Last_Name.Text, First_Name.Text))
                        {
                            Note.Text = "Человек уже зарегистрирован в БД";
                            break;
                        }
                        //если пальца нет то сохраняем изображение до 3ех раз
                        Register_Cntr++;
                        string line = Last_Name.Text + "_" + First_Name.Text + Register_Cntr.ToString() + ".png";
                        Bitmap img = new Bitmap(bmp);
                        img.Save(line, ImageFormat.Png);


                        
                        if (Register_Cntr == 1)
                        {
                            //обработка пальца
                            FingerPrint fingerptint1 = new FingerPrint();
                            fingerptint1.Binary(img, ref binary_fingerprint, ref picture_user, ref roi, ref pointModule, ref pointAngle, ref areaModule, ref areaAngle, ref grad_fingerprint);
                            fingerptint1.Sceleton(ref picture_user, binary_fingerprint.Width, binary_fingerprint.Height);
                            fingerptint1.SpecialPoints(picture_user, ref img_user);
                            fingerptint1.vector_special_point(ref img_user, ref link1_1, ref link1_2, roi);
                            binary_fingerprint = null;
                            picture_user = null;
                            roi = null;
                            pointModule = null;
                            pointAngle = null;
                            areaModule = null;
                            areaAngle = null;
                            grad_fingerprint = null;
                            img_user = null;
                            fingerptint1 = null;
                        }
                        if (Register_Cntr == 2)
                        {
                            FingerPrint fingerptint2 = new FingerPrint();
                            fingerptint2.Binary(img, ref binary_fingerprint, ref picture_user, ref roi, ref pointModule, ref pointAngle, ref areaModule, ref areaAngle, ref grad_fingerprint);
                            fingerptint2.Sceleton(ref picture_user, binary_fingerprint.Width, binary_fingerprint.Height);
                            fingerptint2.SpecialPoints(picture_user, ref img_user);
                            fingerptint2.vector_special_point(ref img_user, ref link2_1, ref link2_2, roi);
                            binary_fingerprint = null;
                            picture_user = null;
                            roi = null;
                            pointModule = null;
                            pointAngle = null;
                            areaModule = null;
                            areaAngle = null;
                            grad_fingerprint = null;
                            img_user = null;
                            fingerptint2 = null;

                        }
                        if (Register_Cntr == 3)
                        {
                            FingerPrint fingerptint3 = new FingerPrint();
                            fingerptint3.Binary(img, ref binary_fingerprint, ref picture_user, ref roi, ref pointModule, ref pointAngle, ref areaModule, ref areaAngle, ref grad_fingerprint);
                            fingerptint3.Sceleton(ref picture_user, binary_fingerprint.Width, binary_fingerprint.Height);
                            fingerptint3.SpecialPoints(picture_user, ref img_user);
                            fingerptint3.vector_special_point(ref img_user, ref link3_1, ref link3_2, roi);
                            binary_fingerprint = null;
                            picture_user = null;
                            roi = null;
                            pointModule = null;
                            pointAngle = null;
                            areaModule = null;
                            areaAngle = null;
                            grad_fingerprint = null;
                            img_user = null;
                            fingerptint3 = null;

                        }
                        //если Register_Cntr == 3 составляем запрос на сервер о добавлении пальчика
                        if (Register_Cntr == 3)
                        {
                            DB.ADD(Last_Name.Text, First_Name.Text, Title.Text,
                                 link1_1,
                                 link1_2,
                                 link2_1,
                                 link2_2,
                                 link3_1,
                                 link3_2);
                            Register_Cntr = 0;
                        }
                    }
                    break;

                default:
                    base.DefWndProc(ref m);
                    break;
            }
        }

        private void Registration_Load(object sender, EventArgs e)
        {
            FormHandle = this.Handle;
        }

        public static void Set_Handle(IntPtr Handle)
        {
            mDevHandle = Handle;
        }
    }
}
