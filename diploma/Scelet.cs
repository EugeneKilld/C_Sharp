using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diplom
{
    public class Scelet
    {
       static public int delete(int w, int h, ref bool[,] picture) //удаление пикселя по основному набору, возврат кол-ва удаленных
        {
            int count = 0;
            for (int i = 0; i < h; i++)
                for (int j = 0; j < w; j++)
                    if (picture[i, j])
                        if (deletable(i, j, h, w, picture))
                        {
                            picture[i, j] = false;
                            count += 1;
                        }
            return count;
        }
        static bool deletable(int y, int x, int h, int w, bool[,] picture) //получение 3*3, передача на проверку для осн.
        {
            bool[] a = new bool[9];
            for (int i = 0; i < 3; i++)
                for (int j = 0; j < 3; j++)
                    if (-1 + y + i < 0 || -1 + y + i >= h || -1 + x + j < 0 || -1 + x + j >= w)
                        a[3 * i + j] = false;
                    else
                        a[3 * i + j] = picture[-1 + y + i, -1 + x + j];
            return check(a);
        }
        static bool check(bool[] a) //определение принадлежности 3*3 к основным шаблонам
        {
            bool[] t123457 = { false, false, true, true, false, true };
            bool[] t013457 = { false, false, false, true, true, true };
            bool[] t134567 = { true, false, true, true, false, false };
            bool[] t134578 = { true, true, true, false, false, false };
            bool[] t0123457 = { false, false, false, true, true, true, true };
            bool[] t0134567 = { false, true, false, true, true, false, true };
            bool[] t1345678 = { true, true, true, true, false, false, false };
            bool[] t1234578 = { true, false, true, true, false, true, false };
            bool[] t = { a[1], a[2], a[3], a[4], a[5], a[7] };
            if (Sravnenie(t, t123457))
                return true;
            bool[] t1 = { a[0], a[1], a[3], a[4], a[5], a[7] };
            if (Sravnenie(t1, t013457))
                return true;
            bool[] t2 = { a[1], a[3], a[4], a[5], a[6], a[7] };
            if (Sravnenie(t2, t134567))
                return true;
            bool[] t3 = { a[1], a[3], a[4], a[5], a[7], a[8] };
            if (Sravnenie(t3, t134578))
                return true;
            bool[] t4 = { a[0], a[1], a[2], a[3], a[4], a[5], a[7] };
            if (Sravnenie(t4, t0123457))
                return true;
            bool[] t5 = { a[1], a[3], a[4], a[5], a[6], a[7], a[8] };
            if (Sravnenie(t5, t1345678))
                return true;
            bool[] t6 = { a[0], a[1], a[3], a[4], a[5], a[6], a[7] };
            if (Sravnenie(t6, t0134567))
                return true;
            bool[] t7 = { a[1], a[2], a[3], a[4], a[5], a[7], a[8] };
            if (Sravnenie(t7, t1234578))
                return true;
            return false;
        }
        public static void delete2(int w, int h, ref bool[,] picture)  //удаление пикселя по шумовому набору
        {
            for (int i = 1; i < h; i++)
                for (int j = 1; j < w; j++)
                    if (picture[i, j] == true)
                        if (deletable2(i, j, h, w, picture))
                            picture[i, j] = false;
        }
        static bool deletable2(int y, int x, int h, int w, bool[,] picture)  //получение 3*3, передача на проверку для шумов
        {
            bool[] a = new bool[9];
            for (int i = 0; i < 3; i++)
                for (int j = 0; j < 3; j++)
                    if (-1 + y + i < 0 || -1 + y + i >= h || -1 + x + j < 0 || -1 + x + j >= w)
                        a[3 * i + j] = false;
                    else
                        a[3 * i + j] = picture[-1 + y + i, -1 + x + j];
            return fringe(a);
        }
        static bool fringe(bool[] a) //определение принадлежности 3*3 к шумам
        {
            bool[,] t ={
                    {false, false, false, false, true, false, false, false, false},

           {false,false,false,false,true,false,false,true,true},
           {false,false,false,true,true,false,true,false,false},
           {true,true,false,false,true,false,false,false,false},
           {false,false,true,false,true,true,false,false,false},

           {false,false,false,false,true,false,true,true,false},
           { true,false,false,true,true,false,false,false,false},
           {false,true,true,false,true,false,false,false,false},
           {false,false,false,false,true,true,false,false,true},

           {false,false,false,false,true,false,true,true,true},
           {true,false,false,true,true,false,true,false,false},
           {true,true,true,false,true,false,false,false,false},
           {false,false,true,false,true,true,false,false,true}};

            bool[] check = new bool[9];
            for (int i = 0; i < 13; i++)
            {
                for (int j = 0; j < 9; j++)
                    check[j] = t[i, j];
                if (Sravnenie(a, check))
                    return true;
            }
            return false;
        }
        static bool Sravnenie(bool[] arr1, bool[] arr2)
        {
            if (arr1.Length != arr2.Length)
                return false;
            for (int i = 0; i < arr1.Length; i++)
                if (arr1[i] != arr2[i])
                    return false;
            return true;
        }
    }
}
