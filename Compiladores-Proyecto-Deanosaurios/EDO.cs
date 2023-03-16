using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiladores_Proyecto_Deanosaurios
{
    internal class EDO
    {
        public int nombre;
        public List<Transicion> transiciones= new List<Transicion>();
        public bool estadoAceptacion = false;

        public EDO(int nombre)
        {
            this.nombre = nombre;
        }

        public void agregarTransicion(Transicion transicion)
        {
            transiciones.Add(transicion);
        }

        public void setEDOaceptacion(bool aceptacion)
        {
           estadoAceptacion = aceptacion;
        }

    }
}
