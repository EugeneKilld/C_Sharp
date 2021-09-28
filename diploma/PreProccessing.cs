using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diplom
{
    class PreProccessing
    {
        public static void Point_Grad(int[,] sourceImage, ref double[,] pointModule, ref double[,] pointAngle)
        {
            /* градиент в точке
             * 
             */
            double z1, z2, z3, z4, z5, z6, z7, z8, z9;
            double gx, gy;
            for (int x = 1; x < sourceImage.GetLength(0) - 1; x++)
            {
                for (int y = 1; y < sourceImage.GetLength(1) - 1; y++)
                {
                    z1 = sourceImage[x - 1, y - 1];
                    z2 = sourceImage[x - 1, y];
                    z3 = sourceImage[x - 1, y + 1];
                    z4 = sourceImage[x, y - 1];
                    z5 = sourceImage[x, y];
                    z6 = sourceImage[x, y + 1];
                    z7 = sourceImage[x + 1, y - 1];
                    z8 = sourceImage[x + 1, y];
                    z9 = sourceImage[x + 1, y + 1];

                    //оператор собеля
                    gx = (z7 + 2 * z8 + z9) - (z1 + 2 * z2 + z3);
                    gy = (z3 + 2 * z6 + z9) - (z1 + 2 * z4 + z7);

                    pointModule[x, y] = Math.Sqrt(gx * gx + gy * gy);
                    pointAngle[x, y] = Trigon.Atan2(gy, gx);
                }
            }
        }
        public static void areaGrad(int[,] sourceImage, double[,] pointModule, double[,] pointAngle, ref double[,] areaModule, ref double[,] areaAngle, int size)
        {
            int outer = (size + 1) / 2;
            int inner = (size - 1) / 2;
            for (int x = outer; x < sourceImage.GetLength(0) - outer; x++)
            {
                for (int y = outer; y < sourceImage.GetLength(1) - outer; y++)
                {
                    double sumX = 0;
                    double sumY = 0;
                    double sumModules = 0;
                    for (int w = x - inner; w < x + inner; w++)
                    {
                        for (int z = y - inner; z < y + inner; z++)
                        {
                            sumX += pointModule[w, z] * Trigon.Cos(2 * pointAngle[w, z]);
                            sumY += pointModule[w, z] * Trigon.Sin(2 * pointAngle[w, z]);
                            sumModules += pointModule[w, z];
                        }
                    }
                    areaModule[x, y] = Math.Sqrt(sumX * sumX + sumY * sumY);
                    areaAngle[x, y] = Trigon.Atan2(sumY, sumX) / 2;
                }
            }
        }
    }
}
