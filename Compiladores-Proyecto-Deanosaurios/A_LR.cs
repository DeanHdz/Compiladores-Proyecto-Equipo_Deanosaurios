using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Compiladores_Proyecto_Deanosaurios
{

    internal class A_LR
    {
        public List<String> Terminales;
        public List<String> NoTerminales;
        public List<String> SimbolosGramaticales;
        public List<LR_Estado> Estados;
        public Dictionary<String, String> Gramatica;
        public String PAumentada;
        public int Cont_Estado = 0;

        /*************************** INICIO AVANCE 7  ******************************/
        public String[,] ir_a;
        public String[,] Accion;
        /************************** FIN AVANCE 7 ********************************/


        public A_LR(Dictionary<String, String> Gramatica, List<String> Terminales, List<String> NoTerminales, String GramaticaAumentada)
        {
            this.Gramatica = Gramatica;
            this.Terminales = Terminales;
            this.NoTerminales = NoTerminales;
            this.PAumentada = GramaticaAumentada;
            this.SimbolosGramaticales = new List<String>();

            foreach (String s in this.NoTerminales)
                this.SimbolosGramaticales.Add(s);
            foreach (String s in this.Terminales)
                this.SimbolosGramaticales.Add(s);
            Inicializar();
        }
        
        /*************************************** INICIO AVANCE 7 *********************************************/

        public void CrearTablaDeAnalisis(Dictionary<String, String> Siguientes)
        {
            foreach (LR_Estado estado in this.Estados)                      // RECORRER CADA ESTADO
            {
                foreach (LR_Elemento elemento in estado.Elementos_Estado)   //RECORRER CADA ELEMENTO EN EL ESTADO
                {
                    String Aux = elemento.Encabezado.TrimEnd(' ');
                    elemento.Encabezado = Aux;

                    //(DIRIGE / DESPLAZAR)
                    //Inciso A)  Si [ A → α.aβ ] está en Ii e ir_A(Ii , a ) = Ij 
                    if (elemento.Cuerpo.IndexOf(".") != elemento.Cuerpo.Length - 1)
                    {
                        int punto = elemento.Cuerpo.IndexOf(".");
                        int length = elemento.Cuerpo.Length;
                        String aux = TerminalDespuesDelPunto(elemento.Cuerpo, elemento.Cuerpo.IndexOf("."));
                        if (aux != null)
                        {
                            int indiceIrA = -1;
                            foreach (LR_TransicionD t in estado.Transiciones)
                            {
                                if (t.S == aux)
                                    indiceIrA = t.Index_Destado;
                            }
                            if (indiceIrA != -1)
                            {
                                int indiceSimbolo = Terminales.IndexOf(aux);
                                Accion[estado.Index_Estado, Terminales.IndexOf(aux)] = "d" + indiceIrA.ToString(); //Guardar en arreglo accion
                            }
                        }
                    }

                    //(REDUCIR)
                    //Inciso B) Si [ A → α. ] está en Ii,
                    if (!elemento.Encabezado.Contains("'") && elemento.Cuerpo.IndexOf(".") == elemento.Cuerpo.Length - 1)
                    {// si el punto esta al ultimo se hacen los reducir
                        String[] siguientesCadena = Siguientes[(string)elemento.Encabezado].Split(' ');
                        int indicePunto = elemento.Cuerpo.IndexOf(".");
                        String elemento_limpio = elemento.Cuerpo.Remove((int)elemento.Cuerpo.IndexOf(".")).TrimEnd(' ');

                        int indiceProd = Produccion_Indice(elemento_limpio);
                        foreach (String s in siguientesCadena)
                        {
                            if (s == "$")
                                Accion[estado.Index_Estado, Terminales.Count] = "r" + indiceProd.ToString();        //Guardar en arreglo accion
                            else
                                Accion[estado.Index_Estado, Terminales.IndexOf(s)] = "r" + indiceProd.ToString();   //Guardar en arreglo accion
                        }
                    }

                    //(ESTADO DE ACEPTACION)
                    //Inciso C) Si [ S’ → S. ] está en Ii entonces, ACCION[ i, $ ] = “aceptar” (ac).
                    if (elemento.Encabezado.Contains("'") && elemento.Cuerpo.IndexOf(".") == elemento.Cuerpo.Length - 1)
                        Accion[estado.Index_Estado, Terminales.Count()] = "ac";     //Guardar en arreglo accion                                           

                }
                foreach (LR_TransicionD tD in estado.Transiciones)
                {
                    if (NoTerminales.Contains(tD.S))
                        this.ir_a[estado.Index_Estado, NoTerminales.IndexOf(tD.S)] = tD.Index_Destado.ToString();   //Guardar en arreglo ir_a
                }
            }
        }

        public String TerminalDespuesDelPunto(String elemento, int index)
        {
            foreach (String s in this.Terminales)
                if (elemento.IndexOf(s, index) == index + 2)
                    return s;
            return null;
        }

        public int Produccion_Indice(String produccionBus)
        {
            int aux = 0;
            foreach (KeyValuePair<String, String> EntradaD in this.Gramatica)
            {
                String[] ArregloCadenas = EntradaD.Value.Split('|');
                foreach (String c in ArregloCadenas)
                {
                    aux++;
                    if (produccionBus == c)
                        return aux;
                }
            }
            return 0;
        }

        /****************************************** FIN AVANCE 7 ****************************************************/


        // Funcion para inicializar el AFD_LR
        public void Inicializar()
        {
            Cont_Estado = 0;
            Estados = new List<LR_Estado>();                                                           //Inicializar estados del AFD
            LR_Elemento Elemento_ini = new LR_Elemento(PAumentada , "programa'");                      //Elemento inicial es programa (. programa) (1.-Una gramatica aumentada)
            List<LR_Elemento> Elementos_iniciales = new List<LR_Elemento> { Elemento_ini };            //Insertar la gramatica aumentada
            LR_Estado Estado_ini = new LR_Estado(CERRADURA(Elementos_iniciales), Cont_Estado);         //Crear primera cerradura en base a la lista inicial
            Estados.Add(Estado_ini);                                                                   //Definir estado l0

            for (int i = 0; i < Estados.Count; i++)
            {
                foreach (String c in SimbolosGramaticales)
                { 
                    List<LR_Elemento> Ir_A = ir_A(i, c);
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

            /*************************** INICIO AVANCE 7 ******************************/

            // Preparar las tablas de Accion e ir_a
            this.ir_a = new String[Estados.Count, NoTerminales.Count];
            this.Accion = new String[Estados.Count, Terminales.Count + 1]; // +1 por el $
            for (int i = 0; i < Estados.Count; i++)
            {
                for (int j = 0; j < NoTerminales.Count; j++)
                    this.ir_a[i, j] = "";
                for (int z = 0; z < Terminales.Count + 1; z++)
                    this.Accion[i, z] = "";
            }
            /************************** FIN AVANCE 7 ********************************/

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
