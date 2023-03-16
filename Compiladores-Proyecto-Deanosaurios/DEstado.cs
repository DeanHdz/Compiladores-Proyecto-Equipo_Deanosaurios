using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiladores_Proyecto_Deanosaurios
{
    internal class DEstado
    {
        public String name;
        public List<EDO> estados = new List<EDO>();
        public List<TransicionDestado> transiciones = new List<TransicionDestado>();
        public bool visited = false;
        public bool estadoAceptacion = false;

        public DEstado(List<EDO> estados, String name)
        {
            this.estados = estados;
            this.name = name;
        }

        public void agregarTransicion(TransicionDestado transicion)
        {
            transiciones.Add(transicion);
        }

        public void setEDOaceptacion(bool aceptacion)
        {
            estadoAceptacion = aceptacion;
        }
    }
}
