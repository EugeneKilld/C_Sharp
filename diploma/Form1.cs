using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using libzkfpcsharp;

namespace Diplom
{
    public partial class Form1 : Form
    {
        IntPtr mDevHandle = IntPtr.Zero;
        public Form1()
        {
            InitializeComponent();

        }

        private void Capture_Click(object sender, EventArgs e)
        {
            //инициализация сканера
            int ret = zkfperrdef.ZKFP_ERR_OK;
            if ((ret = zkfp2.Init()) == zkfperrdef.ZKFP_ERR_OK)
            {
                int nCount = zkfp2.GetDeviceCount();
                if (nCount > 0)
                {
                    Reg.Enabled = true;
                    Ident.Enabled = true;
                    ConnectDB.Enabled = true;
                }
                else
                {
                    zkfp2.Terminate();
                    MessageBox.Show("Устройства не найдены!");
                }
            }
            else
            {
                MessageBox.Show("Ошибка инициализации, код ошибки = " + ret + " !");
            }
            //подключение к сканеру
            if (IntPtr.Zero == (mDevHandle = zkfp2.OpenDevice(0)))
            {
                MessageBox.Show("Ошибка подключения к устройству");
                return;
            }
        }

        private void Registration_Click(object sender, EventArgs e)
        {
            Form ifrm = new Registration();
            ifrm.Left = this.Left; // задаём открываемой форме позицию слева равную позиции текущей формы
            ifrm.Top = this.Top;
            Registration.Set_Handle(mDevHandle);
            ifrm.Show();
            this.Hide();
        }

        private void Identify_Click(object sender, EventArgs e)
        {
            Form ifrm = new Identify();
            ifrm.Left = this.Left; // задаём открываемой форме позицию слева равную позиции текущей формы
            ifrm.Top = this.Top;
            Identify.Set_Handle(mDevHandle);
            ifrm.Show();
            this.Hide();


        }

        private void Close_Click(object sender, EventArgs e)
        {
            zkfp2.CloseDevice(mDevHandle);
            zkfp2.Terminate();
            this.Close();
        }

        private void ConnectDB_Click(object sender, EventArgs e)
        {
            Form ifrm = new DB_Connection();
            ifrm.Left = this.Left; // задаём открываемой форме позицию слева равную позиции текущей формы
            ifrm.Top = this.Top;
            ifrm.Show();
            this.Hide();
        }
    }
}