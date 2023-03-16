using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiladores_Proyecto_Deanosaurios
{
    internal class AFN
    {
        public List<EDO> estados = new List<EDO>();
       
        public List<string> alfabeto = new List<string>();
        
        public int contEps = 0;

        
        /*public void agregarEDO(EDO edo)
        {
            NEstados++;
            estados.Add(edo);
        }*/

        public void agregaAlAlfabeto(string nuevoElemento)
        {
            bool band = true;
            for (int i = 0; i < alfabeto.Count; i++)
            {
                if(nuevoElemento == alfabeto[i])
                {
                    band = false;
                }
            }
            if(band)
                alfabeto.Add(nuevoElemento);
        }

        public void conviertePosfijaEnAFN(string posfija)
        {
            estados.Clear();
            alfabeto.Clear();
            if(posfija != "")
                Thompson(posfija);
            else
                return;
            //Aqui va el codigo que genera la tabla
        }

        public void Thompson(string posfija)
        {
            int cont = 0;
            Stack<EDO> pila = new Stack<EDO>();
            Stack<AFN> pilaAFN = new Stack<AFN>();
            AFN auxAFN = new AFN();
            AFN auxAFN2 = new AFN();
            AFN auxAFN3 = new AFN();//se utiliza para operaciones | y &
            EDO aux1 = new EDO(0);
            EDO aux2 = new EDO(0);
            //estados.Add(pila.Peek());
            for (int i = 0; i < posfija.Length; i++)
            {
                switch (posfija[i])
                {
                    case '*':
                        auxAFN = pilaAFN.Pop();
                        auxAFN2 = new AFN();
                        //auxAFN.estados.Count - 1 este es el nodo que tiene siempre el estado de aceptación (El ultimo agregado)

                        //inicio
                        aux1 = new EDO(cont);
                        cont++;
                        aux2 = new EDO(cont);
                        aux2.setEDOaceptacion(true);
                        cont++;
                        auxAFN.estados[auxAFN.estados.Count - 1].setEDOaceptacion(false); //quita el ultimo estado de aceptacion del AFN anterior
                        
                        aux1.agregarTransicion(new Transicion(auxAFN.estados[0], ""));//agrega un estado a la izquierda con transicion epsilon al primer estado del AFN anterior
                        auxAFN.estados[auxAFN.estados.Count - 1].agregarTransicion(new Transicion(aux2,""));//agrega la transicion al estado de aceptacion final
                        aux1.agregarTransicion(new Transicion(aux2,""));//agrega una transicion del estado inicial al final por ser *
                        auxAFN.estados[auxAFN.estados.Count - 1].agregarTransicion(new Transicion(auxAFN.estados[0], "")); //agrega la transicion del estado final de el AFN anterior al estado inicial
                        //fin
                        //Agrega los estados en orden
                        auxAFN2.estados.Add(aux1);
                        foreach (EDO edo in auxAFN.estados)
                        {
                            auxAFN2.estados.Add(edo);
                        }
                        auxAFN2.estados.Add(aux2);
                        pilaAFN.Push(auxAFN2);
                        break;
                    case '?':
                        auxAFN = pilaAFN.Pop();
                        auxAFN2 = new AFN();
                        //auxAFN.estados.Count - 1 este es el nodo que tiene siempre el estado de aceptación (El ultimo agregado)

                        //inicio
                        aux1 = new EDO(cont);
                        cont++;
                        aux2 = new EDO(cont);
                        aux2.setEDOaceptacion(true);
                        cont++;
                        auxAFN.estados[auxAFN.estados.Count - 1].setEDOaceptacion(false); //quita el ultimo estado de aceptacion del AFN anterior
                        aux1.agregarTransicion(new Transicion(auxAFN.estados[0], ""));//agrega un estado a la izquierda con transicion epsilon al primer estado del AFN anterior
                        auxAFN.estados[auxAFN.estados.Count - 1].agregarTransicion(new Transicion(aux2, ""));//agrega la transicion al estado de aceptacion final
                        aux1.agregarTransicion(new Transicion(aux2,""));//agrega una transicion del estado inicial al final por ser ?
                        //fin
                        //Agrega los estados en orden
                        auxAFN2.estados.Add(aux1);
                        foreach(EDO edo in auxAFN.estados)
                        {
                            auxAFN2.estados.Add(edo);
                        }
                        auxAFN2.estados.Add(aux2);
                        pilaAFN.Push(auxAFN2);
                        break;
                    case '+':
                        auxAFN = pilaAFN.Pop();
                        auxAFN2 = new AFN();
                        //auxAFN.estados.Count - 1 este es el nodo que tiene siempre el estado de aceptación (El ultimo agregado)

                        //inicio
                        aux1 = new EDO(cont);
                        cont++;
                        aux2 = new EDO(cont);
                        aux2.setEDOaceptacion(true);
                        cont++;
                        auxAFN.estados[auxAFN.estados.Count - 1].setEDOaceptacion(false); //quita el ultimo estado de aceptacion del AFN anterior
                        aux1.agregarTransicion(new Transicion(auxAFN.estados[0], ""));//agrega un estado a la izquierda con transicion epsilon al primer estado del AFN anterior
                        auxAFN.estados[auxAFN.estados.Count - 1].agregarTransicion(new Transicion(aux2, ""));//agrega la transicion al estado de aceptacion final
                        auxAFN.estados[auxAFN.estados.Count - 1].agregarTransicion(new Transicion(auxAFN.estados[0], "")); //agrega la transicion del estado final de el AFN anterior al estado inicial
                        //fin
                        //Agrega los estados en orden
                        auxAFN2.estados.Add(aux1);
                        foreach (EDO edo in auxAFN.estados)
                        {
                            auxAFN2.estados.Add(edo);
                        }
                        auxAFN2.estados.Add(aux2);
                        pilaAFN.Push(auxAFN2);
                        break;
                    case '|':
                        auxAFN = new AFN();
                        auxAFN3 = pilaAFN.Pop();
                        auxAFN2 = pilaAFN.Pop();
                        aux1 = new EDO(cont);
                        cont++;
                        aux2 = new EDO(cont);
                        aux2.setEDOaceptacion(true);
                        cont++;
                        //quita el ultimo estado de aceptacion de los AFN anteriores
                        auxAFN2.estados[auxAFN2.estados.Count - 1].setEDOaceptacion(false); 
                        auxAFN3.estados[auxAFN3.estados.Count - 1].setEDOaceptacion(false);
                        //Agregar la transicion del ultimo estado de los dos AFN anteriores al estado de aceptacion final
                        auxAFN2.estados[auxAFN2.estados.Count - 1].agregarTransicion(new Transicion(aux2, ""));
                        auxAFN3.estados[auxAFN3.estados.Count - 1].agregarTransicion(new Transicion(aux2, ""));
                        //agrega las transiciones del estado inicial a los dos estados iniciales de los AFN anteriores en la pila
                        aux1.agregarTransicion(new Transicion(auxAFN2.estados[0], ""));
                        aux1.agregarTransicion(new Transicion(auxAFN3.estados[0], ""));
                        //Agrega los estados en orden
                        auxAFN.estados.Add(aux1);
                        foreach (EDO edo in auxAFN2.estados)
                        {
                            auxAFN.estados.Add(edo);
                        }
                        foreach (EDO edo in auxAFN3.estados)
                        {
                            auxAFN.estados.Add(edo);
                        }
                        auxAFN.estados.Add(aux2);
                        pilaAFN.Push(auxAFN);
                        break;
                    case '&':
                        auxAFN = new AFN();
                        auxAFN3 = pilaAFN.Pop();
                        auxAFN2 = pilaAFN.Pop();
                        auxAFN2.estados[auxAFN2.estados.Count - 1].setEDOaceptacion(false);
                        foreach(Transicion t in auxAFN3.estados[0].transiciones)
                        {
                            auxAFN2.estados[auxAFN2.estados.Count -1].transiciones.Add(t);
                        }
                        foreach (EDO edo in auxAFN3.estados)
                        {
                            foreach (Transicion t in edo.transiciones)
                            {
                                if (t.destino == auxAFN3.estados[0])
                                {
                                    t.destino = auxAFN2.estados[auxAFN2.estados.Count - 1];
                                }
                            }
                        }
                        foreach (EDO edo in auxAFN2.estados)
                        {
                            auxAFN.estados.Add(edo);
                        }
                        auxAFN3.estados.Remove(auxAFN3.estados[0]);
                        foreach (EDO edo in auxAFN3.estados)
                        {
                            auxAFN.estados.Add(edo);
                        }
                        pilaAFN.Push(auxAFN);
                        break;
                    default:
                        auxAFN = new AFN();
                        agregaAlAlfabeto(posfija[i].ToString());
                        //inicio
                        aux1 = new EDO(cont);
                        cont++;
                        aux2 = new EDO(cont);
                        aux2.setEDOaceptacion(true);
                        cont++;
                        aux1.agregarTransicion(new Transicion(aux2, posfija[i].ToString()));
                        //fin
                        auxAFN.estados.Add(aux1);
                        auxAFN.estados.Add(aux2);
                        pilaAFN.Push(auxAFN);
                        break;
                }
            }
            AFN AFNaux = pilaAFN.Pop();
            estados = AFNaux.estados;
        }
    }
}
