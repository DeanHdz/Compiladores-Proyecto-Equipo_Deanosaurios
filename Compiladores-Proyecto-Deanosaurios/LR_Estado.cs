using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Compiladores_Proyecto_Deanosaurios
{
    internal class LR_Estado
    {

        public List<LR_Elemento> Elementos_Estado;
        public List<LR_TransicionD> Transiciones;
        public int Index_Estado;

        public LR_Estado(List<LR_Elemento> ElementosEstado, int index)
        {
            this.Elementos_Estado = ElementosEstado;
            this.Index_Estado = index;
            this.Transiciones = new List<LR_TransicionD>();
        }

        public List<LR_Elemento> Ret_cadenas(String SimboloABuscar)
        {
            List<LR_Elemento> res = new List<LR_Elemento>();

            foreach (LR_Elemento e in Elementos_Estado)
            {
                String c = e.Cuerpo.TrimEnd(' ');
                c.TrimEnd(' ');
                String[] Split = c.Split(' ');

                int indexPunto = -1;
                for (int i = 0; i < Split.Length; i++)
                {
                    if (Split[i] == ".")
                    {
                        indexPunto = i;
                        break;
                    }
                }
                if (indexPunto != -1 && indexPunto != Split.Length - 1) 
                {
                    if (Split[indexPunto + 1] == SimboloABuscar) 
                    {
                        String R = RecorrerPunto(Split, indexPunto);
                        R.TrimEnd(' ');
                        res.Add(new LR_Elemento(R, e.Encabezado));
                    }
                }
            }
            return res;
        }

        public String RecorrerPunto(String[] Cadena, int Index_Punto)
        {
            String[] res = new String[Cadena.Length];
            String R = "";
            for (int i = 0; i < Cadena.Length; i++)
            {
                res[i] = Cadena[i];
            }

            res[Index_Punto] = Cadena[Index_Punto + 1];
            res[Index_Punto + 1] = Cadena[Index_Punto];

            foreach (String c in res)
            {
                R += c + " ";
            }
            String Aux = R.TrimEnd(' ');
            return (Aux);
        }

        public bool Igual(List<LR_Elemento> Candidato)
        {
            int Contador = 0;
            foreach (LR_Elemento e in Candidato)
            {
                if (Contiene(e.Cuerpo))
                    Contador++;
            }

            if (Contador == Elementos_Estado.Count)
                return true;
            else
                return false;
        }

        public String getEstado()
        {
            String res = "";
            foreach (LR_Elemento e in Elementos_Estado)
            {
                res += e.Encabezado + "  -->  " + e.Cuerpo + Environment.NewLine;
            }
            return res;
        }

        public bool Contiene(String Cadena)
        {
            foreach (LR_Elemento e in Elementos_Estado)
            {
                if (Cadena == e.Cuerpo)
                    return true;
            }
            return false;
        }

        public void setTransicion(String Simbolo, int id)
        {
            LR_TransicionD NuevaTransicion = new LR_TransicionD(id, Simbolo);
            Transiciones.Add(NuevaTransicion);
        }

        public LR_TransicionD getTransicion(String T)
        {
            foreach (LR_TransicionD TD in this.Transiciones)
            {
                if (TD.S == T)
                    return TD;
            }
            return null;
        }

    }
}
