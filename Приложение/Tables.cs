using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace customs
{
    class Tables
    {
        public Tables(string Код, string Запрос, string Вероятность)
        {
            this.Код = Код;
            this.Запрос = Запрос;
            this.Вероятность = Вероятность;
        }
        public string Код { get; set; }
        public string Запрос { get; set; }
        public string Вероятность { get; set; }
    }
}
