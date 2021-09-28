using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Npgsql;

namespace Diplom
{
    class DB
    {
        public static string ConnectStr = "Host=localhost;Username=postgres;Port=5432;Password=postgres;Database=Fingerprint"; //строка запроса для подключения
        public static void ADD(string Last_Name, string First_Name, string O_Name, int[] Link1_1, int[] Link1_2, int[] Link2_1, int[] Link2_2, int[] Link3_1, int[] Link3_2)
        {
            var connect = new NpgsqlConnection(ConnectStr); //объект для подключения
            connect.Open();// подключение к серверу

            string l1_1 = "{", l1_2 = "{", l2_1 = "{", l2_2 = "{", l3_1 = "{", l3_2 = "{";
            for (int i = 0; i < 200; i++)
            {
                l1_1 += Link1_1[i].ToString() + ", ";
                l1_2 += Link1_2[i].ToString() + ", ";
                l2_1 += Link2_1[i].ToString() + ", ";
                l2_2 += Link2_2[i].ToString() + ", ";
                l3_1 += Link3_1[i].ToString() + ", ";
                l3_2 += Link3_2[i].ToString() + ", ";
            }
            l1_1 += Link1_1[99].ToString() + "} ";
            l1_2 += Link1_2[99].ToString() + "} ";
            l2_1 += Link2_1[99].ToString() + "} ";
            l2_2 += Link2_2[99].ToString() + "} ";
            l3_1 += Link3_1[99].ToString() + "} ";
            l3_2 += Link3_2[99].ToString() + "} ";
            var cmdADD = new NpgsqlCommand($"insert into student(last_name, first_name, o_name, link1_1, link1_2, link2_1, link2_2, link3_1, link3_2) " +
                $"values('{Last_Name}','{First_Name}','{O_Name}','{l1_1}','{l1_2}','{l2_1}','{l2_2}','{l3_1}','{l3_2}')", connect);
            if (cmdADD.ExecuteNonQuery() == 1)
            {
                MessageBox.Show("Запись добавлена успешно!");
            }
            else
            {
                MessageBox.Show("Ошибка при вставке данных!");
            }
            connect.Close();// завершаем соединение
        }
        public static void LIST(ref NpgsqlDataReader reader)
        {
            var ConnectStr = "Host=localhost;Username=postgres;Port=5432;Password=postgres;Database=Fingerprint"; //строка запроса для подключения
            var connect = new NpgsqlConnection(ConnectStr); //объект для подключения
            connect.Open();// подключение к серверу

            var cmdLIST = new NpgsqlCommand("SELECT * FROM student", connect);
            reader = cmdLIST.ExecuteReader();

            //connect.Close();// завершаем соединение
        }
        public static void DELETE(int SID)
        {
            var ConnectStr = "Host=localhost;Username=postgres;Port=5432;Password=postgres;Database=Fingerprint"; //строка запроса для подключения
            var connect = new NpgsqlConnection(ConnectStr); //объект для подключения
            connect.Open();// подключение к серверу

            var cmdDELETE = new NpgsqlCommand($"DELETE FROM student WHERE id = '{SID.ToString()}'", connect); //объект для выполнения комманды delete
            if (cmdDELETE.ExecuteNonQuery() == 1)
            {
                MessageBox.Show("Запись удалена!");
            }
            else
            {
                MessageBox.Show("Ошибка при удалении!");
            }
            connect.Close();// завершаем соединение
        }
        public static void DELETE(string Last_Name, string First_Name)
        {
            var ConnectStr = "Host=localhost;Username=postgres;Port=5432;Password=postgres;Database=Fingerprint"; //строка запроса для подключения
            var connect = new NpgsqlConnection(ConnectStr); //объект для подключения
            connect.Open();// подключение к серверу

            var cmdDELETE = new NpgsqlCommand($"DELETE FROM student WHERE Last_Name = '{Last_Name}' AND First_Name = '{First_Name}'", connect); //объект для выполнения комманды delete
            if (cmdDELETE.ExecuteNonQuery() == 1)
            {
                MessageBox.Show("Запись удалена!");
            }
            else
            {
                MessageBox.Show("Ошибка при удалении!");
            }
            connect.Close();// завершаем соединение
        }
        public static bool SEARCH(string Last_Name, string First_Name)
        {
            var ConnectStr = "Host=localhost;Username=postgres;Port=5432;Password=postgres;Database=Fingerprint"; //строка запроса для подключения
            var connect = new NpgsqlConnection(ConnectStr); //объект для подключения
            connect.Open();// подключение к серверу

            var cmdLIST = new NpgsqlCommand($"SELECT * FROM student WHERE Last_Name = '{Last_Name}' AND First_Name = '{First_Name}'", connect);
            //зарегистрирован ли чел-к в БД
            var dataReader = cmdLIST.ExecuteReader();
            if (dataReader.HasRows)
            {
                connect.Close();// завершаем соединение
                return true;
            }

            connect.Close();// завершаем соединение
            return false;
        }
        public static bool TRYCONNECT()
        {
            var connect = new NpgsqlConnection(ConnectStr); //объект для подключения
            bool flag = true;
            try
            {
                connect.Open();// подключение к серверу
            }
            catch
            {
                flag = false;
            }
            return flag;
        }
    }
}
