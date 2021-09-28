using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diplom
{
    class Trigon
    {
        static int[] values =
        {
            0,  0,  0,  1,  1,  1,  1,  1,  1,  2,  2,  2,  2,  2,  2,  2,
            3,  3,  3,  3,  3,  3,  4,  4,  4,  4,  4,  4,  5,  5,  5,  5,
            5,  5,  5,  6,  6,  6,  6,  6,  6,  7,  7,  7,  7,  7,  7,  7,
            8,  8,  8,  8,  8,  8,  9,  9,  9,  9,  9,  9,  9,  10, 10, 10,
            10, 10, 10, 11, 11, 11, 11, 11, 11, 11, 12, 12, 12, 12, 12, 12,
            12, 13, 13, 13, 13, 13, 13, 13, 14, 14, 14, 14, 14, 14, 14, 15,
            15, 15, 15, 15, 15, 15, 16, 16, 16, 16, 16, 16, 16, 16, 17, 17,
            17, 17, 17, 17, 17, 18, 18, 18, 18, 18, 18, 18, 18, 19, 19, 19,
            19, 19, 19, 19, 19, 20, 20, 20, 20, 20, 20, 20, 20, 21, 21, 21,
            21, 21, 21, 21, 21, 22, 22, 22, 22, 22, 22, 22, 22, 22, 23, 23,
            23, 23, 23, 23, 23, 23, 23, 24, 24, 24, 24, 24, 24, 24, 24, 24,
            25, 25, 25, 25, 25, 25, 25, 25, 25, 26, 26, 26, 26, 26, 26, 26,
            26, 26, 26, 27, 27, 27, 27, 27, 27, 27, 27, 27, 27, 28, 28, 28,
            28, 28, 28, 28, 28, 28, 28, 29, 29, 29, 29, 29, 29, 29, 29, 29,
            29, 29, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 31, 31,
            31, 31, 31, 31, 31, 31, 31, 31, 31, 31, 32, 32, 32, 32, 32, 32,
            32 };
        internal static double Cos(double angle)
        {
            return Math.Cos(angle * Math.PI / 128);
        }

        internal static double Sin(double angle)
        {
            return Math.Sin(angle * Math.PI / 128);
        }

        internal static int Atan2(double y, double x)
        {
            return Atan2(Convert.ToInt32(y), Convert.ToInt32(x));
        }

        internal static int Atan2(int y, int x)
        {
            int abx = Math.Abs(x);
            int aby = Math.Abs(y);
            if (abx + aby == 0)
            {
                return 0; //-1
            }
            int isNegX = (x < 0) ? 1 : 0;
            int isNegY = (y < 0) ? 1 : 0;
            int more = (aby > abx) ? 1 : 0;
            switch ((isNegY << 2) + (isNegX << 1) + (more))
            {
                default: return (values[at(aby, abx)]);
                case 1: return (64 - values[at(abx, aby)]);
                case 2: return (128 - values[at(aby, abx)]);
                case 3: return (64 + values[at(abx, aby)]);
                case 4: return (256 - values[at(aby, abx)]) & 255;
                case 5: return (192 + values[at(abx, aby)]);
                case 6: return (128 + values[at(aby, abx)]);
                case 7: return (192 - values[at(abx, aby)]);
            }
        }

        private static int at(int aby, int abx)
        {
            return Convert.ToInt32(Math.Atan2(aby, abx) * 1024 / Math.PI);
        }
    }
}
