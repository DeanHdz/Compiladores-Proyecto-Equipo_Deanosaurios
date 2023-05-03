using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiladores_Proyecto_Deanosaurios
{
    internal class LR_Elemento
    {
        public string Cuerpo;
        public string Encabezado;

        public LR_Elemento (String Cuerpo, String Encabezado)
        {
            this.Cuerpo = Cuerpo;
            this.Encabezado = Encabezado;
        }
    }
}
