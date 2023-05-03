using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiladores_Proyecto_Deanosaurios
{
    internal class LR_TransicionD
    {
        public int Index_Destado;
        public String S;

        public LR_TransicionD(int index_destino, String transicion)
        {
            this.S = transicion;
            this.Index_Destado = index_destino;
        }

    }
}
