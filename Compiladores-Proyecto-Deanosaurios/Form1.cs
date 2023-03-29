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
        AFN afn = new AFN();
        AFD afd = new AFD();
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


            //Conversion de Expresion regular a infija
            String infija = "";

            int i = 0; //Iterador del string input

            while(i < input.Length)
            {
                //Case #1
                if (input[i] == '(' || input[i] == '|')
                {
                    infija += input[i]; // Letra/Numero/Operador
                }
                else
                {
                    //Case #2-1
                    if (input[i] == '[') //Intervalo iniciado
                    {
                        infija += '('; //Sustituir el corchete
                        i++;//Apunta al caracter despues del corchete
                        infija += input[i];
                        if (input[i + 1] == '-')
                        {
                            for (int j = (int)input[i]; j < (int)input[i + 2]; j++)
                            {
                                infija += '|';
                                infija += (Char)(j + 1);
                            }
                            infija += ')';
                            i += 3; //Mover iterador apuntando al reciente ')'
                        }
                        else
                        {
                            i++;
                            while (input[i] != ']')
                            {
                                infija += '|';
                                infija += input[i];
                                i++;
                            }
                            infija += ')';
                        }
                    }
                    //Case #2-2
                    else
                    {
                        infija += input[i];
                    }

                    if (i + 1 < input.Length)
                    {
                        if (input[i + 1] != '*' && input[i + 1] != '?' && input[i + 1] != '+' && input[i + 1] != '|' && input[i + 1] != ')')
                        {
                            infija += '&';
                        }
                    }
                }

                i++; //Apuntar al siguiente caracter    
            }

            textBox1.Text = infija;

            //Realizar Conversion (CONVERTIR EXPRESIONES INFIJAS EN POSFIJAS)

            //1.Definir la prioridad del conjunto de operaciones... ke?

            //2 Inicializar una pila
            Stack<char> pila = new Stack<char>();

            //3 Inicializar posfija
            String posfija = "";
            i = 0; //iterador del string infija

            try
            {
                while(i < infija.Length) //Mienstras no sea fin de la expresión infija
                {
                    switch (infija[i])
                    {
                        case '(': pila.Push(infija[i]); //Insertar en la pila
                            break;
                        case ')':
                            while (pila.Peek() != '(') // Extraer de la pila y desplegar en posfija hasta encontrar “paréntesis izquierdo” (no desplegarlo)
                            {
                                posfija += pila.Pop();
                            }
                            _ = pila.Pop(); //No desplegar parentesis izquierdo
                            break;
                        default:
                            //(Operando) Numero, minuscula, mayuscula, ñ
                            if(infija[i] >= 48 && infija[i] <= 57 || infija[i] >= 97 && infija[i] <= 122 || infija[i] >= 65 && infija[i] <= 90 || infija[i] == 'ñ' || infija[i] == 'Ñ')
                            {
                                posfija += infija[i]; // Desplegar en posfija
                            }
                            //Para el "verdadero" default, se asume que se tiene un operador
                            else
                            {
                                bool band = true;
                                while (band)
                                {
                                    if (pila.Count == 0 || pila.Peek() == '(' || ChecarPrioridad(pila, infija[i]))
                                    {
                                        pila.Push(infija[i]); //Insertar el operador en la pila;
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

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            int contEpsilon = 0;
            int contEstados = 0;
            afn.conviertePosfijaEnAFN(tb2_al.Text);
            dataGridView1.Columns.Clear();

            DataGridViewColumn columnEDO = new DataGridViewColumn();
            columnEDO.Name = "EDO";
            columnEDO.HeaderText = "EDO";
            DataGridViewCell dgvcell1 = new DataGridViewTextBoxCell();
            columnEDO.CellTemplate = dgvcell1;
            dataGridView1.Columns.Add(columnEDO);

            foreach (string s in afn.alfabeto)
            {
                DataGridViewColumn column = new DataGridViewColumn();
                column.Name = s;
                column.HeaderText = s;
                DataGridViewCell dgvcell = new DataGridViewTextBoxCell();
                column.CellTemplate = dgvcell;
                dataGridView1.Columns.Add(column);

            }
            DataGridViewColumn columnEpsilon = new DataGridViewColumn();
            columnEpsilon.Name = "£";
            columnEpsilon.HeaderText = "£";
            DataGridViewCell dgvcellEpsilon = new DataGridViewTextBoxCell();
            columnEpsilon.CellTemplate = dgvcellEpsilon;
            dataGridView1.Columns.Add(columnEpsilon);
            for(int i = 0; i < afn.estados.Count; i ++)
            {
                contEstados++;
                DataGridViewRow r = new DataGridViewRow();
                r.CreateCells(dataGridView1);
                r.Cells[0].Value = afn.estados[i].nombre;
                for (int j = 0; j <= afn.alfabeto.Count; j++)
                {
                    if(j == afn.alfabeto.Count)
                    {
                        foreach (Transicion t in afn.estados[i].transiciones)
                        {
                            if (t.valor == "")
                            {
                                contEpsilon++;
                                r.Cells[j + 1].Value = r.Cells[j + 1].Value + " " + t.destino.nombre.ToString();
                            }
                        }
                    }
                    else
                    {
                        foreach (Transicion t in afn.estados[i].transiciones)
                        {
                            if (t.valor == afn.alfabeto[j])
                            {
                                r.Cells[j + 1].Value = r.Cells[j + 1].Value + " " + t.destino.nombre.ToString();
                            }
                        }
                    }
                }
                dataGridView1.Rows.Add(r);
            }
            textBox2.Text = contEpsilon.ToString();
            textBox3.Text = contEstados.ToString();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            afd.construyeAFD(afn);
            textBox5.Text = "";
            textBox6.Text = "";
            dataGridView2.Columns.Clear();

            DataGridViewColumn columnEDO = new DataGridViewColumn();
            columnEDO.Name = "EDO";
            columnEDO.HeaderText = "EDO";
            DataGridViewCell dgvcell1 = new DataGridViewTextBoxCell();
            columnEDO.CellTemplate = dgvcell1;
            dataGridView2.Columns.Add(columnEDO);

            foreach (string s in afn.alfabeto)
            {
                DataGridViewColumn column = new DataGridViewColumn();
                column.Name = s;
                column.HeaderText = s;
                DataGridViewCell dgvcell = new DataGridViewTextBoxCell();
                column.CellTemplate = dgvcell;
                dataGridView2.Columns.Add(column);
            }
            int contadorDestados = 0;
            for (int i = 0; i < afd.dEstados.Count; i++)
            {
                if(afd.dEstados[i].estadoAceptacion)
                {
                    textBox5.Text += afd.dEstados[i].name + " ";
                }
                if (afd.dEstados[i].estados.Contains(afn.estados[afn.estados.Count - 1]))
                {
                    textBox5.Text += afd.dEstados[i].name + " ";
                }
                contadorDestados++;
                DataGridViewRow r = new DataGridViewRow();
                r.CreateCells(dataGridView2);
                r.Cells[0].Value = afd.dEstados[i].name;
                for (int j = 0; j < afd.alfabeto.Count; j++)
                {
                    
                        foreach (TransicionDestado t in afd.dEstados[i].transiciones)
                        {
                            if (t.valor == afn.alfabeto[j])
                            {
                                r.Cells[j + 1].Value = r.Cells[j + 1].Value + " " + t.destino.name.ToString();
                            }
                        }
                    
                }
                dataGridView2.Rows.Add(r);
            }
            textBox4.Text = contadorDestados.ToString();
            textBox5.Text = "";
            foreach (DEstado d in afd.dEstados)
            {
                if (d.estadoAceptacion)
                {
                    textBox5.Text += " " +d.name;
                }
            }
            foreach(EDO edo in afn.estados)
            {
                if (edo.estadoAceptacion)
                {
                    textBox7.Text += " " + edo.nombre;
                }
            }
        }

        private void label6_Click(object sender, EventArgs e)
        {

        }

        private void label7_Click(object sender, EventArgs e)
        {

        }

        private void label9_Click(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (afd.lexemaValido(textBox6.Text, afd.dEstados[0]))
            {
                label9.Text = "Si pertenece al lenguaje de la ER";
            }
            else
            {
                label9.Text = "NO pertenece al lenguaje de la ER";
            }
        }

        private void textBox7_TextChanged(object sender, EventArgs e)
        {

        }

        private void button4_Click(object sender, EventArgs e)
        {
            textBox9.Text = "";
            foreach (DEstado d in afd.dEstados)
            {
                if(d.name == textBox8.Text)
                {
                    foreach (EDO edo in d.estados)
                    {
                        textBox9.Text += " " + edo.nombre;
                    }
                }
            }
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