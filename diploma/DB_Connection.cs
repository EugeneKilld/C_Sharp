using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Diplom
{
    public partial class DB_Connection : Form
    {
        public DB_Connection()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string connectStr = "Host=" + Host.Text + ";Username=" + Username.Text + ";Port=" + Port.Text + ";Password=" + Password.Text + ";Database=" + Database.Text;
            DB.ConnectStr = connectStr;

            if (DB.TRYCONNECT())
            {
                MessageBox.Show("Подключение успешно!");
            }
            else
            {
                MessageBox.Show("Ошибка подключения!");
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Form ifrm = Application.OpenForms[0];
            ifrm.StartPosition = FormStartPosition.Manual; // меняем параметр StartPosition у Form1, иначе она будет использовать тот, который у неё прописан в настройках и всегда будет открываться по центру экрана
            ifrm.Left = this.Left; // задаём открываемой форме позицию слева равную позиции текущей формы
            ifrm.Top = this.Top; // задаём открываемой форме позицию сверху равную позиции текущей формы
            ifrm.Show();
            this.Close();
        }
    }
}
