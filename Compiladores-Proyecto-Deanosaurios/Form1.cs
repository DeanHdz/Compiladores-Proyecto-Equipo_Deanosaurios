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

        private void btn_ConP_Click(object sender, EventArgs e)
        {
            //Inicializar ambos variables string
            String output = "", input = tb1_al.Text;

            //Realizar Conversion (CONVERTIR EXPRESIONES INFIJAS EN POSFIJAS)


            //Retornar output string a tb2_al
            tb2_al.Text = output;
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