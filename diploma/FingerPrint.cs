using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diplom
{
    public class FingerPrint
    {
        public void Compare(int[,] img_user, int[] vector_data_1, int[] vector_data_2, ref int all, ref int match, ref int all_data)
        {
            all = 0;
            match = 0;
            all_data = 0;
            for (int i = 0; ;)
            {
                bool flag = false;
                if (vector_data_1[i] == 0)
                {
                    all_data += i - 1;
                    break;
                }
                for (int x = vector_data_1[i] - 15; x < vector_data_1[i] + 15; x++)
                {
                    for (int y = vector_data_1[i + 1] - 15; y < vector_data_1[i + 1] + 15; y++)
                    {
                        try
                        {
                            if (img_user[y, x] == 1)
                            {
                                match++;
                                flag = true;
                                break;
                            }
                        }
                        catch { };
                    }
                    if (flag) break;
                }
                i += 2;
            }

            for (int i = 0; ;)
            {
                bool flag = false;
                if (vector_data_2[i] == 0)
                {
                    all_data += i - 1;
                    break;
                }
                for (int x = vector_data_2[i] - 15; x < vector_data_2[i] + 15; x++)
                {
                    for (int y = vector_data_2[i + 1] - 15; y < vector_data_2[i + 1] + 15; y++)
                    {
                        try
                        {
                            if (img_user[y, x] == 2)
                            {
                                match++;
                                flag = true;
                                break;
                            }
                        }
                        catch { };
                    }
                    if (flag) break;
                }
                i += 2;
            }
            all_data = all_data / 2;
            for (int i = 0; i < img_user.GetLength(0); i++)
            {
                for (int j = 0; j < img_user.GetLength(1); j++)
                {
                    if (img_user[i, j] != 0)
                    {
                        all++;
                    }
                }
            }
        }
        public void Binary(Bitmap fingerprint, ref Bitmap binary_fingerprint, ref bool[,] picture, ref double[,] roi, 
            ref double[,] pointModule, ref double[,] pointAngle, ref double[,] areaModule, ref double[,] areaAngle, ref Bitmap grad_fingerprint)
        {
            /* Создается матрица изображения буллевского типа,
             * где черный пиксель = true; а белый = false.
             */
            string path = Directory.GetCurrentDirectory();
            //string path_save = path + "\\Fingerprint-Enhancement-Python-develop\\src\\images\\1.png";
            string path_python_script = path + @"\Fingerprint-Enhancement-Python-develop\src\example.py";
            Directory.CreateDirectory(@"C:\Images");

            
            fingerprint.Save(@"C:\Images\1.png", ImageFormat.Png);
            Process p = new Process();
            p.StartInfo = new ProcessStartInfo(@"C:\Users\drjac\AppData\Local\Programs\Python\Python38\python.exe", path_python_script)
            {
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = false
            };
            p.Start();

            string output = p.StandardOutput.ReadToEnd();
            p.WaitForExit();
            p.Close();

            string filepath = @"C:\Images\2.png";
            using (var file = new FileStream(filepath, FileMode.Open, FileAccess.Read, FileShare.Inheritable))
            {
                binary_fingerprint = new Bitmap(file);
            }
            
            roi = new double[binary_fingerprint.Height, binary_fingerprint.Width];
            roi = ROI(binary_fingerprint, ref pointModule, ref pointAngle, ref areaModule, ref areaAngle, ref grad_fingerprint);
            picture = new bool[binary_fingerprint.Height, binary_fingerprint.Width];
            for (int x = 0; x < binary_fingerprint.Width; x++)
                for (int y = 0; y < binary_fingerprint.Height; y++)
                {
                    Color c = binary_fingerprint.GetPixel(x, y);
                    byte rgb = (byte)(c.R * 0.3 + c.G * 0.59 + c.B * 0.11);
                    if (rgb < 50)
                        picture[y, x] = true;
                    else
                        picture[y, x] = false;
                }
        }
        public void Sceleton(ref bool[,] picture, int w, int h)
        {
            /*скелетизация изображения.
             * используются методы другого класса
             */
            int count = 1;
            while (count != 0) //повторять пок удалялся хотя бы один пиксель
            {
                count = Scelet.delete(w, h, ref picture);
                if (count > 0)
                    Scelet.delete2(w, h, ref picture);
            };            
        }
        public void SpecialPoints(bool[,] picture, ref int[,] img)
        {
            /*поиск особых точек.
             * создание матрицы img, гдк 1 - конечная точка, 2 - точка ветвления.
             */
            img = new int[picture.GetLength(0), picture.GetLength(1)];
            int t = 0;
            for (int i = 0; i < picture.GetLength(0); i++)
                for (int j = 0; j < picture.GetLength(1); j++)
                {
                    //point_fingerprint.SetPixel(j, i, Color.FromArgb(255, 255, 255, 255));
                    img[i, j] = 0;
                    if (picture[i, j])
                    {
                        t = checkThisPoint(picture.GetLength(0), picture.GetLength(1), picture, i, j);
                        if (t == 2)
                        {
                            img[i, j] = 1; //конечная точка
                        }
                        else if (t == 74 || t == 146 || t == 164 || t == 41 || t == 138 || t == 162 || t == 168 || t == 42 || t == 37 || t == 73 || t == 82 || t == 148 || t == 170) //точка ветвления
                        {
                            img[i, j] = 2;
                        }
                    }
                }

            for (int i = 0; i < picture.GetLength(0); i++)
                for (int j = 0; j < picture.GetLength(1); j++)
                {
                    if (img[i, j] == 1)
                    {
                        bool flag = FindT2(picture, ref img, i, j, 0);
                        if (!flag)
                        {
                            img[i, j] = 0;
                        }
                    }
                }
            DeleteNoizePoint(picture.GetLength(0), picture.GetLength(1), ref img);

        }
        int checkThisPoint(int h, int w, bool[,] picture, int y, int x) //подсчет количества черных в окрестности
        {
            int c = 0;
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    if (-1 + y + i > 0 && -1 + y + i < h && -1 + x + j > 0 && -1 + x + j < w)
                        if (picture[-1 + y + i, -1 + x + j]) c++;
                }
            }
            if (c == 4)
            {
                c = 0;
                try { if (picture[y - 1, x - 1]) c += 128; } catch { };
                try { if (picture[y - 1, x]) c += 1; } catch { };
                try { if (picture[y - 1, x + 1]) c += 2; } catch { };
                try { if (picture[y, x - 1]) c += 64; } catch { };
                try { if (picture[y, x + 1]) c += 4; } catch { };
                try { if (picture[y + 1, x - 1]) c += 32; } catch { };
                try { if (picture[y + 1, x]) c += 16; } catch { };
                try { if (picture[y + 1, x + 1]) c += 8; } catch { };
            }
            return c;
        }
        void DeleteNoizePoint(int h, int w, ref int[,] img)
        {
            for (int i = 0; i < h; i++)
                for (int j = 0; j < w; j++)
                {
                    if (img[i, j] == 1)
                    {
                        for (int k = i - 10; k < i + 10; k++)
                        {
                            for (int m = j - 10; m < j + 10; m++)
                            {
                                if (k > 0 && k < h && m > 0 && m < w)
                                    if (img[k, m] == 1 && (k != i || m != j))
                                    {
                                        img[k, m] = 0;
                                        img[i, j] = 0;
                                    }
                            }
                        }

                    }
                }
        }
        double[,] ROI(Bitmap binary_fingerprint, ref double[,] pointModule, ref double[,] pointAngle, ref double[,] areaModule, ref double[,] areaAngle, ref Bitmap grad_fingerprint)
        {
            grad_fingerprint = new Bitmap(binary_fingerprint.Width, binary_fingerprint.Height, binary_fingerprint.PixelFormat);
            pointModule = new double[binary_fingerprint.Width, binary_fingerprint.Height];
            pointAngle = new double[binary_fingerprint.Width, binary_fingerprint.Height];
            areaModule = new double[binary_fingerprint.Width, binary_fingerprint.Height];
            areaAngle = new double[binary_fingerprint.Width, binary_fingerprint.Height];
            int[,] grad_p = new int[binary_fingerprint.Width, binary_fingerprint.Height];
            for (int y = 0; y < binary_fingerprint.Height; ++y)
                for (int x = 0; x < binary_fingerprint.Width; ++x)
                {
                    Color c = binary_fingerprint.GetPixel(x, y);
                    int rgb = c.R;
                    grad_p[x, y] = rgb;
                }
            PreProccessing.Point_Grad(grad_p, ref pointModule, ref pointAngle);
            PreProccessing.areaGrad(grad_p, pointModule, pointAngle, ref areaModule, ref areaAngle, 15);
            double max = 255;
            for (int y = 0; y < binary_fingerprint.Height; ++y)
                for (int x = 0; x < binary_fingerprint.Width; ++x)
                {
                    if (areaModule[x, y] > max)
                        max = areaModule[x, y];
                }
            for (int y = 0; y < binary_fingerprint.Height; ++y)
                for (int x = 0; x < binary_fingerprint.Width; ++x)
                {
                    areaModule[x, y] = areaModule[x, y] * 255 / max;
                }
            int treshold = otsuThreshold(areaModule, binary_fingerprint.Height, binary_fingerprint.Width);
            for (int y = 0; y < binary_fingerprint.Height; ++y)
                for (int x = 0; x < binary_fingerprint.Width; ++x)
                {
                    if (areaModule[x, y] < treshold * 0.7)
                        areaModule[x, y] = 0;
                    else
                        areaModule[x, y] = 255;
                }
            return areaModule;
        }
        int otsuThreshold(double[,] image, int h, int w)
        {
            // Проверки на NULL и проч. опустим, чтобы сконцетрироваться
            // на работе метода

            // Посчитаем минимальную и максимальную яркость всех пикселей
            int min = Convert.ToInt32(image[0, 0]);
            int max = Convert.ToInt32(image[0, 0]);

            for (int x = 0; x < w; x++)
                for (int y = 0; y < h; y++)
                {
                    int value = Convert.ToInt32(image[x, y]);

                    if (value < min)
                        min = value;

                    if (value > max)
                        max = value;
                }

            // Гистограмма будет ограничена снизу и сверху значениями min и max,
            // поэтому нет смысла создавать гистограмму размером 256 бинов
            int histSize = max - min + 1;
            int[] hist = new int[histSize];

            // Заполним гистограмму нулями
            for (int t = 0; t < histSize; t++)
                hist[t] = 0;

            // И вычислим высоту бинов
            for (int x = 0; x < w; x++)
                for (int y = 0; y < h; y++)
                    hist[Convert.ToInt32(image[x, y]) - min]++;

            // Введем два вспомогательных числа:
            int m = 0; // m - сумма высот всех бинов, домноженных на положение их середины
            int n = 0; // n - сумма высот всех бинов
            for (int t = 0; t <= max - min; t++)
            {
                m += t * hist[t];
                n += hist[t];
            }

            float maxSigma = -1; // Максимальное значение межклассовой дисперсии
            int threshold = 0; // Порог, соответствующий maxSigma

            int alpha1 = 0; // Сумма высот всех бинов для класса 1
            int beta1 = 0; // Сумма высот всех бинов для класса 1, домноженных на положение их середины

            // Переменная alpha2 не нужна, т.к. она равна m - alpha1
            // Переменная beta2 не нужна, т.к. она равна n - alpha1

            // t пробегается по всем возможным значениям порога
            for (int t = 0; t < max - min; t++)
            {
                alpha1 += t * hist[t];
                beta1 += hist[t];

                // Считаем вероятность класса 1.
                float w1 = (float)beta1 / n;
                // Нетрудно догадаться, что w2 тоже не нужна, т.к. она равна 1 - w1

                // a = a1 - a2, где a1, a2 - средние арифметические для классов 1 и 2
                float a = (float)alpha1 / beta1 - (float)(m - alpha1) / (n - beta1);

                // Наконец, считаем sigma
                float sigma = w1 * (1 - w1) * a * a;

                // Если sigma больше текущей максимальной, то обновляем maxSigma и порог
                if (sigma > maxSigma)
                {
                    maxSigma = sigma;
                    threshold = t;
                }
            }

            // Не забудем, что порог отсчитывался от min, а не от нуля
            threshold += min;

            // Все, порог посчитан, возвращаем его наверх :)
            return threshold;
        }
        public void vector_special_point(ref int [,] img, ref int[] vector1, ref int[] vector2, double [,] roi)
        {
            bool flag = true;
            int count1 = 0;
            int count3 = 0;
            for (int i = 0; i < img.GetLength(0); i++)
                for (int j = 0; j < img.GetLength(1); j++)
                {
                    if (img[i, j] == 1)
                    {
                        for (int x = -10; x < 10; x++)
                            for (int y = -10; y < 10; y++)
                            {
                                if (roi[j + x, i + y] == 0)
                                {
                                    flag = false;
                                    img[i, j] = 0;
                                }
                            }
                        if (flag)
                        {
                            vector1[count1] = j;
                            vector1[count1 + 1] = i;
                            count1 += 2;
                        }
                        flag = true;
                    }
                    if (img[i, j] == 2)
                    {
                        for (int x = -5; x < 5; x++)
                            for (int y = -5; y < 5; y++)
                            {
                                if (roi[j + x, i + y] == 0)
                                {
                                    flag = false;
                                    img[i, j] = 0;
                                }
                            }
                        if (flag)
                        {
                            vector2[count3] = j;
                            vector2[count3 + 1] = i;
                            count3 += 2;
                        }
                        flag = true;
                    }
                }
        }
        bool FindT2(bool[,] picture, ref int[,] img, int y, int x, int step)
        {
            if (step < 10)
            {
                for (int i = y - 1; i < y + 2; i++)
                    for (int j = x - 1; j < x + 2; j++)
                        if (picture[i, j] && (y != i || x != j))
                        {
                            if (img[i, j] == 2)
                            {
                                img[i, j] = 0;
                                return false;
                            }
                            else
                            {
                                picture[i, j] = false;
                                step++;
                                if (!FindT2(picture, ref img, i, j, step))
                                    return false;
                                step--;
                            }
                        }
            }
            return true;
        }
    }
}
