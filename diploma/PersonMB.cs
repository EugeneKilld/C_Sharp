using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diplom
{
    class PersonMB
    {
        public string name { get; set; }
        public string last_name { get; set; }
        public string o_name { get; set; }
        public int match { get; set; }
        public int all { get; set; }
        public int all_v { get; set; }
        public double procent { get; set; }
        public PersonMB(string name, string last_name, string o_name, int match, int all, int all_v, double procent)
        {
            this.name = name;
            this.last_name = last_name;
            this.o_name = o_name;
            this.match = match;
            this.all = all;
            this.all_v = all_v;
            this.procent = procent;
        }
    }
}
