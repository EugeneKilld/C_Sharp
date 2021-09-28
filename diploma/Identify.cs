using libzkfpcsharp;
using Npgsql;
using Sample;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Diplom
{
    public partial class Identify : Form
    {
        Bitmap fingerprint;
        bool[,] picture_user, picture_data;
        int[,] img_user, img_data;
        int flag = 0;
        NpgsqlDataReader data_reader;
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

        [DllImport("user32.dll", EntryPoint = "SendMessageA")]
        public static extern int SendMessage(IntPtr hwnd, int wMsg, IntPtr wParam, IntPtr lParam);
        public Identify()
        {
            InitializeComponent();
            //DB.LIST(ref data_reader);
        }

        private void Exit_Click(object sender, EventArgs e)
        {
            Form ifrm = Application.OpenForms[0];
            ifrm.StartPosition = FormStartPosition.Manual; // меняем параметр StartPosition у Form1, иначе она будет использовать тот, который у неё прописан в настройках и всегда будет открываться по центру экрана
            ifrm.Left = this.Left; // задаём открываемой форме позицию слева равную позиции текущей формы
            ifrm.Top = this.Top; // задаём открываемой форме позицию сверху равную позиции текущей формы
            ifrm.Show();
            this.Close();
        }

        private void Ident_Click(object sender, EventArgs e)
        {
            string selectedState = comboBox1.SelectedItem.ToString();
            if (selectedState == "Метод особых точек")
            {
                flag = 1;
            }
            else if (selectedState == "Собственные алгоритмы сканера")
            {
                flag = 2;
            }
            FPBuffer = new byte[mfpWidth * mfpHeight];
            Thread captureThread = new Thread(new ThreadStart(DoCapture));
            captureThread.IsBackground = true;
            captureThread.Start();
            bIsTimeToDie = false;
        }

        private void Save_Result_Click(object sender, EventArgs e)
        {

        }

        private void DoCapture()
        {
            while (!bIsTimeToDie)
            {

                cbCapTmp = 2048;
                int ret = zkfp2.AcquireFingerprint(mDevHandle, FPBuffer, CapTmp, ref cbCapTmp);
                if (ret == zkfp.ZKFP_ERR_OK)
                {
                    SendMessage(FormHandle, MESSAGE_CAPTURED_OK, IntPtr.Zero, IntPtr.Zero);
                }
                Thread.Sleep(200);
            }
        }

        protected override void DefWndProc(ref Message m)
        {
            switch (m.Msg)
            {
                case MESSAGE_CAPTURED_OK:
                    {
                        MemoryStream ms = new MemoryStream();
                        BitmapFormat.GetBitmap(FPBuffer, mfpWidth, mfpHeight, ref ms);
                        Bitmap bmp = new Bitmap(ms);
                        this.pictureBox1.Image = bmp;

                        /* обработка изображения - получение вектора признака, в зависимости от выбранного метода (flag)
                         * подключение к БД, получение списка всех студентов (ФИО + ссылки на изображения)
                         * получение шаблона
                         * сравнение шаблона с полученным отпечатком пальцев.
                         */
                        if (flag == 1)
                        {
                            /* Обработка захваченного изображения отпечатка пальца:
                            * Бинаризация;
                            * Скелетизация;
                            * Выделение особых точек;
                            * получение вектора признаков.
                            */
                            Bitmap bmp_user = new Bitmap(bmp);
                            FingerPrint fingerptint = new FingerPrint();
                            fingerptint.Binary(bmp_user, ref binary_fingerprint, ref picture_user, ref roi, ref pointModule, ref pointAngle, ref areaModule, ref areaAngle, ref grad_fingerprint);
                            fingerptint.Sceleton(ref picture_user, binary_fingerprint.Width, binary_fingerprint.Height);
                            fingerptint.SpecialPoints(picture_user, ref img_user);
                            int[] vector1 = new int[200];
                            int[] vector2 = new int[200];
                            fingerptint.vector_special_point(ref img_user, ref vector1, ref vector2, roi);
                            List<PersonMB> person = new List<PersonMB>();
                            DB.LIST(ref data_reader);

                            if (data_reader.HasRows)
                            {
                                foreach(var item in data_reader)
                                {
                                    int[] special_data_point_1 = new int[200];
                                    int[] special_data_point_2 = new int[200];
                                    for (int i = 1; i < 4; i++)
                                    {
                                        if (i == 1)
                                        {
                                            special_data_point_1 = (int[])data_reader[$"link1_1"];
                                            special_data_point_2 = (int[])data_reader[$"link1_2"];
                                        }
                                        if (i == 2)
                                        {
                                            special_data_point_1 = (int[])data_reader[$"link2_1"];
                                            special_data_point_2 = (int[])data_reader[$"link2_2"];
                                        }
                                        if (i == 3)
                                        {
                                            special_data_point_1 = (int[])data_reader[$"link3_1"];
                                            special_data_point_2 = (int[])data_reader[$"link3_2"];
                                        }
                                        int all = 0;
                                        int match = 0;
                                        int all_data = 0;
                                        int[,] new_img_user = new int[img_user.GetLength(0), img_user.GetLength(1)];
                                        for (int k = -20; k < 20; k++)
                                        {
                                            for (int j = -20; j < 20; j++)
                                            {
                                                for (int x = 20; x < img_user.GetLength(1) - 20; x++)
                                                {
                                                    for (int y = 20; y < img_user.GetLength(0) - 20; y++)
                                                    {
                                                        new_img_user[y + k, x + j] = img_user[y, x];
                                                    }
                                                }
                                                fingerptint.Compare(new_img_user, special_data_point_1, special_data_point_2, ref all, ref match, ref all_data);
                                                //textBox1.Text += $"всего - {all.ToString()}, всего д - {all_data.ToString()} совпало - {match.ToString()}, процент = {match * match * 100.0 / (all * all_data)}\n\r";
                                                if (match * match * 100.0 / (all * all_data) > 50)
                                                {
                                                    person.Add(new PersonMB(data_reader["first_name"].ToString(), data_reader["last_name"].ToString(), data_reader["o_name"].ToString(), match, all, all_data, match * match * 100.0 / (all * all_data)));
                                                    break;
                                                }
                                                for (int x = 20; x < img_user.GetLength(1) - 20; x++)
                                                {
                                                    for (int y = 20; y < img_user.GetLength(0) - 20; y++)
                                                    {
                                                        new_img_user[y + i, x + j] = 0;
                                                    }
                                                }

                                            }
                                            if (match * match * 100.0 / (all * all_data) > 50)
                                            {
                                                break;
                                            }
                                        }

                                    }
                                }
                            }
                            if (person.Count > 0)
                            {
                                int max_procent = 0;
                                for (int i = 0; i < person.Count; i++)
                                {
                                    if (person[i].procent > person[max_procent].procent)
                                    {
                                        max_procent = i;
                                    }
                                }
                                textBox1.Text += " ИМЯ - " + person[max_procent].name + person[max_procent].last_name + person[max_procent].o_name + $"процент = {person[max_procent].procent}\r\n";
                            }
                            
                        }
                    }
                    break;

                default:
                    base.DefWndProc(ref m);
                    break;
            }
        }

        public static void Set_Handle(IntPtr Handle)
        {
            mDevHandle = Handle;
        }

        private void Identify_Load(object sender, EventArgs e)
        {
            FormHandle = this.Handle;
        }
    }
}
