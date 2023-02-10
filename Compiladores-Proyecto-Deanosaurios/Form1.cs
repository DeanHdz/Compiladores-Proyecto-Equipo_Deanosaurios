using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Compiladores_Proyecto_Deanosaurios
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        bool ChecarPrioridad(Stack<char> pila, char input)
        {
            //El operador tiene mas prioridad que el tope de la pila
            bool res = true;

            if (pila.Peek() == '*' || pila.Peek() == '+' || pila.Peek() == '?')
                res = false;
            else if (input == '&' && pila.Peek() == '&')
                res = false;
            else if (input == '|')
                res = false;

            return res;
        }

        private void btn_ConP_Click(object sender, EventArgs e)
        {
            //Tarea de luih -> 1.- Cambiar nombre de input a infija en todo el codigo abajo (Excepto: String input = tb1_al.Text; )
            //2.- hacer la transformacion necesaria del input a infija... EJEMPLO -> cont(0|1) -> c&o&n&t&(0|1)




            //Inicializar string que recupere el input del textbox
            String input = tb1_al.Text;

            //Realizar Conversion (CONVERTIR EXPRESIONES INFIJAS EN POSFIJAS) Tarea de luih

            //1.Definir la prioridad del conjunto de operaciones... ke?

            //2 Inicializar una pila
            Stack<char> pila = new Stack<char>();

            //3 Inicializar posfija
            String posfija = "";
            int i = 0; //iterador del string input

            try
            {
                while(i < input.Length) //Mienstras no sea fin de la expresión infija
                {
                    switch (input[i])
                    {
                        case '(': pila.Push(input[i]); //Insertar en la pila
                            break;
                        case ')':
                            while (pila.Peek() != '(') // Extraer de la pila y desplegar en posfija hasta encontrar “paréntesis izquierdo” (no desplegarlo)
                            {
                                posfija += pila.Pop();
                            }
                            _ = pila.Pop(); //No desplegar parentesis izquierdo
                            break;
                        default:
                            if(input[i] >= 97 && input[i] <= 122 || input[i] >= 65 && input[i] <= 90 || input[i] == 'ñ' || input[i] == 'Ñ')
                            {
                                posfija += input[i]; // Desplegar en posfija
                            }
                            else
                            {
                                bool band = true;
                                while (band)
                                {
                                    if (pila.Count == 0 || pila.Peek() == '(' || ChecarPrioridad(pila, input[i]))
                                    {
                                        pila.Push(input[i]); //Insertar el operador en la pila;
                                        band = false;
                                    }
                                    else
                                        posfija += pila.Pop(); //Extraer el tope de la pila y desplegar en posfija;
                                }
                            }
                            break;
                    }
                    i++; //Apuntar al siguiente carácter de la expresión infija
                }
                while (pila.Count > 0)
                    posfija += pila.Pop(); // Extraer y desplegar en posfija los elementos de la pila hasta que se vacíe
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.ToString() + " ... La cadena inicial no es valido");
            }

            //Retornar output string a tb2_al
            tb2_al.Text = posfija;
        }
    }
}

/*
    CONVERTIR EXPRESIONES INFIJAS EN POSFIJAS.

1.Definir la prioridad del conjunto de operaciones.
2.Inicializar una pila.
3.Inicializar posfija.

Apuntar al primer carácter de la expresión infija.
while(no ocurra un error && no sea fin de la expresión infija)
{
    switch(carácter)
    {
        Paréntesis izquierdo:   Insertar en la pila;
                                break;
        Paréntesis derecho:     Extraer de la pila y desplegar en posfija hasta encontrar “paréntesis izquierdo” (no desplegarlo);
                                break;
        Operando:               Desplegar en posfija;
                                break;
        Operador:               band = true;
                                while(band)
                                {
                                    if( la pila está vacía || 
                                        el tope de la pila es un “paréntesis izquierdo” || 
                                        el operador tiene mayor prioridad que el tope de la pila)
                                    {
                                        Insertar el operador en la pila;
                                        band = false;
                                    }else
                                        Extraer el tope de la pila y desplegar en posfija;
                                }
                                break;
    }
    Apuntar al siguiente carácter de la expresión infija;
}
Extraer y desplegar en posfija los elementos de la pila hasta que se vacíe;
*/