using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Compiladores_Proyecto_Deanosaurios
{

    /* PARA CONSTRUIR UNA COLECCION LR(0) CANONICA DE UNA GRAMATICA, DEFINIMOS:
     * 1.-Una gramatica aumentada
     * 2.-Una funcion CERRADURA
     * 3.-Una funcion ir_A
     */

    internal class A_LR
    {
        public List<String> Terminales;
        public List<String> NoTerminales;
        public List<String> SimbolosGramaticales;
        public List<LR_Estado> Estados;
        public Dictionary<String, String> Gramatica;
        public String PAumentada;
        public int Cont_Estado = 0;

        public A_LR(Dictionary<String, String> Gramatica, List<String> Terminales, List<String> NoTerminales, String GramaticaAumentada)
        {
            this.Gramatica = Gramatica;
            this.Terminales = Terminales;
            this.NoTerminales = NoTerminales;
            this.PAumentada = GramaticaAumentada;
            this.SimbolosGramaticales = new List<String>();

            foreach (String s in this.NoTerminales)
            {
                this.SimbolosGramaticales.Add(s);
            }
            foreach (String s in this.Terminales)
            {
                this.SimbolosGramaticales.Add(s);
            }

            Inicializar();
        }

        // Funcion para inicializar el AFD_LR
        public void Inicializar()
        { 
            Estados = new List<LR_Estado>();                                                            //Inicializar estados del AFD
            LR_Elemento Elemento_ini = new LR_Elemento(PAumentada , "programa'");                       //Elemento inicial es programa (. programa) (1.-Una gramatica aumentada)
            List<LR_Elemento> Elementos_iniciales = new List<LR_Elemento>{Elemento_ini};                //Insertar en lista de elementos de la produccion programa
            LR_Estado Estado_ini = new LR_Estado(CERRADURA(Elementos_iniciales), Cont_Estado);         //Crear primera cerradura en base a la lista inicial
            Estados.Add(Estado_ini);                                                                   //Definir estado l0

            for (int i = 0; i < Estados.Count; i++)
            {
                foreach (String c in SimbolosGramaticales)
                { 
                    List<LR_Elemento> Ir_A = ir_A(i, c);                                                //Agregar cada simbolo a ir_A
                    if (Ir_A.Count != 0 && ChecaNuevoEstado(Ir_A) == -1)
                    {
                        Cont_Estado++;
                        LR_Estado Nuevo = new LR_Estado(Ir_A, Cont_Estado);
                        Estados.Add(Nuevo);
                        Estados[i].setTransicion(c, Nuevo.Index_Estado);
                    }
                    else if (Ir_A.Count != 0 && ChecaNuevoEstado(Ir_A) != -1)
                    {
                        int indiceestado = ChecaNuevoEstado(Ir_A);
                        Estados[i].setTransicion(c, Estados[indiceestado].Index_Estado);
                    }
                }
            }
        }

        public int ChecaNuevoEstado(List<LR_Elemento> Candidato)
        { 
            for (int i = 0; i < Estados.Count; i++)
            {
                if (Estados[i].Igual(Candidato))
                    return i;
            }
            return -1;
        }

        // 2.-Una funcion CERRADURA
        public List<LR_Elemento> CERRADURA (List<LR_Elemento> ElementosEvaluar)
        { 
            
            List<LR_Elemento> J = new List<LR_Elemento>();  //Lista de elementos a retornar
            foreach (LR_Elemento e in ElementosEvaluar)     //Se agregan todos los elementos iniciales
            {
                J.Add(e);
            }

            for (int i = 0; i < J.Count; i++)               //Recorrer cada elemento de la lista
            {
                String[] DivisionCadena = J[i].Cuerpo.Split(' ');     //Separar strings con espacios preexistentes
                int indexPunto = -1;                                  //Reacomodar punto
                for (int j = 0; j < DivisionCadena.Length; j++)                                            
                {
                    if (DivisionCadena[j] == ".")
                    {
                        indexPunto = j;
                        break;
                    }
                }
                if (indexPunto != -1 && indexPunto != DivisionCadena.Length - 1)
                {
                    String CadenaEvaluar = DivisionCadena[indexPunto + 1];
                    if (NoTerminales.Contains(CadenaEvaluar))
                    {
                        List<LR_Elemento> Producciones = getProduccion(CadenaEvaluar);
                        foreach (LR_Elemento e in Producciones)
                        {
                            String P = e.Cuerpo;
                            String Aux = ". " + P;
                            Aux.TrimEnd();
                            if (!Contiene(J, Aux))
                            {
                                LR_Elemento NuevoElemento = new LR_Elemento(Aux, e.Encabezado);
                                J.Add(NuevoElemento);
                            }
                        }
                    }
                }
            }
            return J;
        }

        //Busca si contiene un elemento en especifico
        public bool Contiene(List<LR_Elemento> Elementos, String Cadena)
        {
            foreach (LR_Elemento e in Elementos)
            {
                if (e.Cuerpo == Cadena)
                    return true;
            }
            return false;
        }

        //Regresa una produccion determinada dependiendo del encabezado del elemento
        public List<LR_Elemento> getProduccion(String Encabezado)
        {
            String[] divisiones = Gramatica[Encabezado].Split('|'); //Separar los string de la gramatica de acuerdo al encabezado
            List<LR_Elemento> res = new List<LR_Elemento>();
            foreach (String s in divisiones)
            {
                LR_Elemento NuevoElemento = new LR_Elemento(s.TrimEnd(' '), Encabezado);
                res.Add(NuevoElemento);
            }
            return res;
        }

        // 3.-Una funcion ir_A
        public List<LR_Elemento> ir_A(int indiceEstado, String Simbolo)
        {
            LR_Estado Seleccionado = Estados[indiceEstado];
            List<LR_Elemento> ProduccionesCambiadas = Seleccionado.Ret_cadenas(Simbolo);
            List<LR_Elemento> Resultado = new List<LR_Elemento>();
            if (ProduccionesCambiadas.Count > 0)
            {
                Resultado = CERRADURA(ProduccionesCambiadas);
            }
            return Resultado;
        }

    }
}
