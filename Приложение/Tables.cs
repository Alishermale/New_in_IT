using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace customs
{
    class Tables
    {
        public Tables(string Код, string Название, string Вероятность)
        {
            this.Код = Код;
            this.Название = Название;
            this.Вероятность = Вероятность;
        }
        public string Код { get; set; }
        public string Название { get; set; }
        public string Вероятность { get; set; }
    }
}
