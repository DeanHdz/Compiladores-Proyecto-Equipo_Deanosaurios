using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Security;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace Compiladores_Proyecto_Deanosaurios
{
    public partial class Form1 : Form
    {
        AFN afn = new AFN();
        AFD afd = new AFD();

        static List<string> PalabrasReservadas = new List<string>() { "if","then","else","end","repeat","until","read","write"};
        static List<string> SimbolosEspeciales = new List<string>() { "+","-","*","/","=","<",">","(",")",";",":="};

        public Form1()
        {
            InitializeComponent();
        }

        #region 1er entrega

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

        #endregion

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

        #region 5ta entrega
        //Funciones para Tokens
        public string ConvPosfija(string input)
        {
            //Conversion de Expresion regular a infija
            String infija = "";

            int i = 0; //Iterador del string input

            while (i < input.Length)
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

            //Realizar Conversion (CONVERTIR EXPRESIONES INFIJAS EN POSFIJAS)

            //1.Definir la prioridad del conjunto de operaciones... ke?

            //2 Inicializar una pila
            Stack<char> pila = new Stack<char>();

            //3 Inicializar posfija
            String posfija = "";
            i = 0; //iterador del string infija

            try
            {
                while (i < infija.Length) //Mienstras no sea fin de la expresión infija
                {
                    switch (infija[i])
                    {
                        case '(':
                            pila.Push(infija[i]); //Insertar en la pila
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
                            if (infija[i] >= 48 && infija[i] <= 57 || infija[i] >= 97 && infija[i] <= 122 || infija[i] >= 65 && infija[i] <= 90 || infija[i] == 'ñ' || infija[i] == 'Ñ')
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
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString() + " ... La cadena inicial no es valido");
            }

            return posfija;
        }

        //Boton clasifica tokens
        private void button5_Click(object sender, EventArgs e)
        {
            try
            {
                if(textBox10.Text != "" && textBox11.Text != "" && textBox12.Text != "")
                {
                    dataGridView3.Rows.Clear(); //Reiniciar contenido de tabla

                    //Iniciar proceso de AFN/AFD del identificador y numero
                    //Posfijas
                    string posID = ConvPosfija(textBox10.Text);
                    string posNum = ConvPosfija(textBox11.Text);
                    //AFNs
                    AFN AFN_ID = new AFN();
                    AFN_ID.conviertePosfijaEnAFN(posID);
                    AFN AFN_Num = new AFN();
                    AFN_Num.conviertePosfijaEnAFN(posNum);
                    //AFDs
                    AFD AFD_ID = new AFD();
                    AFD_ID.construyeAFD(AFN_ID);
                    AFD AFD_Num = new AFD();
                    AFD_Num.construyeAFD(AFN_Num);

                    List<String[]> Code = new List<String[]>(); //Cachitos de codigo en cada linea
                    int Line = 0; //Linea actual al imprimir en grid

                    for (int i = 0; i < textBox12.Lines.Length; i++)
                    {
                        // Leer linea por linea textbox y separar codigo
                        string Trim = textBox12.Lines[i].Trim();
                        String[] lineArr = Trim.Split(' ');
                        Code.Add(lineArr);
                        foreach (string s in lineArr)
                        {
                            // Leer cada renglon e identificar el codigo 
                            
                            // Identificar renglon vacio
                            if(s == null || s == "") { Line--; }//Hacer nada, contrarestar recorrimiento de linea en tabla
                            // Prueba palabra reservada
                            else if (PalabrasReservadas.Contains(s)){
                                    dataGridView3.Rows.Add(s, s);
                            }
                            // Prueba Simbolo especial
                            else if (SimbolosEspeciales.Contains(s)){
                                    dataGridView3.Rows.Add(s, s);
                            }
                            // Pprueba AFD Numero
                            else if ( AFD_Num.lexemaValido(s, AFD_Num.dEstados[0]) ){
                                    dataGridView3.Rows.Add("número", s);
                            }
                            // Prueba AFD Identificador
                            else if ( AFD_ID.lexemaValido(s, AFD_ID.dEstados[0]) ){
                                    dataGridView3.Rows.Add("ídentificador", s);
                            }
                            // Error lexico
                            else if (s != ""){
                                    dataGridView3.Rows.Add("Error Léxico", s); 
                                    dataGridView3.Rows[Line].Cells[0].Style.ForeColor = Color.Red;
                                    dataGridView3.Rows[Line].Cells[1].Style.ForeColor = Color.Red;
                            }
                            Line++;
                        }
                    }
                }
                else
                {
                    MessageBox.Show("No estan rellenados todos los campos.");
                }
            }
            catch(Exception E)
            {
                MessageBox.Show("Error: " + E.Message);
            }
        }

        #endregion

        #region Entrega 6
        //Boton Construir Colección LR(0) Canónica
        private void button6_Click(object sender, EventArgs e)
        {
            GramTINY G = new GramTINY();                                                                                //TINY ya con su AFD
            dataGridView4.Rows.Clear(); dataGridView4.Columns.Clear(); dataGridView4.Columns.Add("Estados", "Estados"); //Vaciar la tabla AFD
            textBox13.Text = "";       

            List<String> Transiciones = G.Afd_lr.SimbolosGramaticales;                                                  //Rescatar las transiciones (T,NT)
            foreach (String s in Transiciones){dataGridView4.Columns.Add(s, s); }                                       //Agregar columnas de transicion a datagrid AFD

            foreach (LR_Estado est in G.Afd_lr.Estados) //Por cada estado en la lista de estados imprimirlo en el textbox con su respectivo lista de elementos
            {
                String EstadoString = est.getEstado();                                                                  //get del string estado
                textBox13.Text += "l" + est.Index_Estado + " =" + Environment.NewLine + "{" + Environment.NewLine + EstadoString + "} " + Environment.NewLine + Environment.NewLine; //Imprimir estado en textbox

                List<String> Lista_Elementos = new List<String>{ "l" + est.Index_Estado.ToString()};
                foreach (String s in Transiciones)
                {
                    LR_TransicionD transicionDAux = est.getTransicion(s);
                    //Verificar si existe el estado destino
                    if (transicionDAux == null)
                        Lista_Elementos.Add("vacio");
                    else 
                        Lista_Elementos.Add(transicionDAux.Index_Destado.ToString()); 
                }
                dataGridView4.Rows.Add(Lista_Elementos.ToArray());// Agregar a la lista de elementos del AFD
            }

            /*************************** INICIO AVANCE 7 ******************************/
            dataGridView5.Columns.Clear(); dataGridView6.Columns.Clear();                                       // Vaciar la TABLA DE TRANSICION (vaciar Accion e ir_A)

            G.Afd_lr.CrearTablaDeAnalisis(G.Siguientes);                                                        // INICIAR LA TABLA DE ANALISIS SINTACTICO COMO TAL


            dataGridView5.Columns.Add("Estados", "Estados");                                                    // Agregar la columna de estados a la tabla

            for (int i = 0; i < G.Terminales.Count; i++)                                                        // Por cada Terminal se agrega una columna en tabla Accion
                dataGridView5.Columns.Add(G.Terminales[i], G.Terminales[i]);        
            dataGridView5.Columns.Add("$", "$");                                                                // Agregar la columna $

            for (int i = 0; i < G.NoTerminales.Count; i++)
                dataGridView6.Columns.Add(G.NoTerminales[i], G.NoTerminales[i]);                                // Por cada NoTerminal se agrega una columna en tabla Ir_A

            // (TERMINALES) Iterar estados e indicar accion
            for (int i = 0; i < G.Afd_lr.Estados.Count; i++){
                List<String> GridRow = new List<String>{ i.ToString() };
                for (int j = 0; j <= G.Terminales.Count; j++){
                    if (G.Afd_lr.Accion[i, j] != null)
                        GridRow.Add(G.Afd_lr.Accion[i, j]);
                    else
                        GridRow.Add("vacio");
                }
                dataGridView5.Rows.Add(GridRow.ToArray());
            }

            // (NO TERMINALES) Iterar estados e indicar accion
            for (int i = 0; i < G.Afd_lr.Estados.Count; i++){
                List<String> GridRow = new List<String>();
                for (int j = 0; j < G.NoTerminales.Count; j++){
                    if (G.Afd_lr.ir_a[i, j] != null)
                        GridRow.Add(G.Afd_lr.ir_a[i, j]);
                    else
                        GridRow.Add("vacio");
                }
                dataGridView6.Rows.Add(GridRow.ToArray());                          
             }

            /************************** FIN AVANCE 7 ********************************/

        }
        #endregion

        #region EntregaFinal

        //Copiado y pegado de la entrega 5
        private List<String> AnalizadorLexicoTINY_Entrega5(AFD AFD_Identificador, AFD AFD_Numero)
        {
            List<String> Code = new List<String>(); //Retornar programa para analisis sintactico
            int Line = 0; //Linea actual al imprimir en grid

            for (int i = 0; i < codigo_Textbox.Lines.Length; i++)
            {
                // Leer linea por linea textbox y separar codigo
                string Trim = codigo_Textbox.Lines[i].Trim();
                String[] lineArr = Trim.Split(' ');
                foreach (string s in lineArr)
                {
                    // Leer cada renglon e identificar el codigo 

                    // Identificar renglon vacio
                    if (s == null || s == "") { Line--; }//Hacer nada, contrarestar recorrimiento de linea en tabla
                                                         // Prueba palabra reservada
                    else if (PalabrasReservadas.Contains(s))
                    {
                        Tabla_Tokens.Rows.Add(s, s);
                        Code.Add(s);
                    }
                    // Prueba Simbolo especial
                    else if (SimbolosEspeciales.Contains(s))
                    {
                        Tabla_Tokens.Rows.Add(s, s);
                        Code.Add(s);
                    }
                    // prueba AFD Numero
                    else if (AFD_Numero.lexemaValido(s, AFD_Numero.dEstados[0]))
                    {
                        Tabla_Tokens.Rows.Add("número", s);
                        Code.Add("numero");
                    }
                    // Prueba AFD Identificador
                    else if (AFD_Identificador.lexemaValido(s, AFD_Identificador.dEstados[0]))
                    {
                        Tabla_Tokens.Rows.Add("ídentificador", s);
                        Code.Add("identificador");
                    }
                    // Error lexico
                    else if (s != "")
                    {
                        Tabla_Tokens.Rows.Add("Error Léxico", s);
                            Errores_Textbox.Text += "Linea " + (i+1).ToString() + ". " + s + " no se reconoce." + Environment.NewLine;
                        Tabla_Tokens.Rows[Line].Cells[0].Style.ForeColor = Color.Red;
                        Tabla_Tokens.Rows[Line].Cells[1].Style.ForeColor = Color.Red;
                    }
                    Line++;
                }
            }
            Code.Add("$"); //Agregar el delimitador de entrada para analisis sintactico
            return Code;
        }

        //Copiado y pegado de la entrega 6-7
        private A_LR CrearLR_Entrega6y7()
        {
            GramTINY g = new GramTINY(); //Al usar el constructor default ya se hace el LR
            g.Afd_lr.CrearTablaDeAnalisis(g.Siguientes); //A partir del LR se hace la tabla de analisis con siguientes (Hacer ir_a y Accion)
            return g.Afd_lr; //Retornar el LR completo
        }

        //Se procede con el analizador sintactico si no se hallan errores lexicos
        private void AnalizadorSintacticoTINY(A_LR LR, List<String> w)
        {
            // ENTRADA: Una cadena de entrada w y una tabla de análisis sintáctico LR con las funciones ACCION e ir_A, para una gramática G.

            /** Limpiar y preparar Area de Trabajo **/
            //Dejar arbol sintactico en blanco
            Arbol.Nodes.Clear(); Arbol.Refresh();           //Quitar nodos y redibujar
            //Inicializar estructuras
            Stack<int> PilaDeEstados = new Stack<int>();    //Crear pila de estados
            PilaDeEstados.Push(0);                          //Iniciar pila de estados con 0
            List<TreeNode> Hijos = new List<TreeNode>();


            // MÉTODO: Al principio, el analizador sintáctico tiene s0 en su pila, en donde s0 es el estado inicial y w$ está en el búfer de entrada.
            /** Algoritmo del PDF **/

            int a = 0; // hacer que "a" sea el primer símbolo de w$;
            while (true)  /* repetir indefinidamente */
            {
                int s = PilaDeEstados.Peek();   // hacer que "s" sea el estado en la parte superior de la pila;
                string simbolo = w[a];          //"a" apunta al simbolo correspondiente en w

                int Nodoactual_index = Hijos.Count;
                int simbolo_index = -1; //Por default se considera que no encuentra el simbolo en la tabla

                //Buscar la posicion de columna que ocupa el simbolo en la tabla
                if (LR.Terminales.Contains(simbolo))
                    simbolo_index = LR.Terminales.IndexOf(simbolo);     //Posicion del simbolo en tabla ACCION
                else if (simbolo == "$")
                    simbolo_index = LR.Terminales.Count;                //En la tabla ACCION siempre ocupa la ultima posicion el simbolo "$"


                if (simbolo_index != -1) //No busco el simbolo en la tabla
                {
                    // 1) if (ACCION[s, a] = desplazar t)
                    if (LR.Accion[s,simbolo_index].Contains("d"))
                    { 
                        string EstadoDesplazar = LR.Accion[s, simbolo_index]; // Realiza ACCION de estado con el simbolo evaluado
                        int estado_inicio = EstadoDesplazar.IndexOf("d") + 1; // Determinar "t" despues del d de "desplazar"
                        int Estado = int.Parse(EstadoDesplazar.Substring(estado_inicio).ToString()); //Rescatar el numero de estado partiendo de su indice
                        PilaDeEstados.Push(Estado); //meter estado "t" en la pila;
                        TreeNode nodo = new TreeNode(); //Crear y agregar nodo
                        nodo.Text = simbolo;
                        Hijos.Add(nodo);
                        a++; //hacer que "a" sea el siguiente símbolo de entrada;
                    }
                    // 2) else if (ACCION[s, a] = reducir A->β)
                    else if (LR.Accion[s,simbolo_index].Contains("r"))
                    { 
                        string rEstado = LR.Accion[s,simbolo_index]; // ACCION[s, a]
                        int indexR = rEstado.IndexOf("r")+1; // Clasificacion
                        int Estado = int.Parse(rEstado.Substring(indexR).ToString());
                        int BetaLength = LR.CantidadSimbolos_Beta(Estado);  //Tamaño de Beta

                        for (int i = 0; i < BetaLength; i++)
                            PilaDeEstados.Pop(); // retira elementos de la pila dependiendo del tamaño del cuerpo de la produccion

                        s = PilaDeEstados.Peek(); // Apunta al tope de la pila
                        string Padre = LR.ObtenPadreProduccion(Estado);
                        PilaDeEstados.Push(int.Parse(LR.ir_a[s, LR.NoTerminales.IndexOf(Padre)])); // Agregar el resultado de ir_A[t,A] a la PILA
 
                        List<TreeNode> nodos = new List<TreeNode>();
                        List<int> indexAux = new List<int>();
                        for (int i = Nodoactual_index - BetaLength; i < Nodoactual_index; i++)
                        {
                            nodos.Add(Hijos[i]);
                            indexAux.Add(i);
                        }
                        indexAux.Sort(); indexAux.Reverse();                            // Ordenar los indices para borrar datos innecesarios
                        foreach (int i in indexAux)                                      // Remover nodos repetidos
                            Hijos.RemoveAt(i);
                        TreeNode NodoPadre = new TreeNode(Padre);                        // Guardar hijos en padre
                        foreach (TreeNode T in nodos)
                            NodoPadre.Nodes.Add(T);

                        Hijos.Add(NodoPadre);
                    }
                    // 3) else if(ACCION[s,a] = aceptar )
                    else if (LR.Accion[s,simbolo_index] == "ac")
                    { 
                        foreach (TreeNode T in Hijos)
                            Arbol.Nodes.Add(T);     // Pasar todos los nodos obtenidos al arbol y representarlos
                        break;  /* terminó el análisis sintáctico */
                    }
                    else // 4) llamar a la rutina de recuperación de errores;
                    {
                        Errores_Textbox.Text = "Error sintáctico";
                        break; /* terminó el análisis sintáctico */
                    }
                } //while
            }
        }

        private void BotonFinal_Click(object sender, EventArgs e)
        {
            //Clasificar tokens, si no hay errores hace el arbol.
            try
            {
                if (identificador_Textbox.Text != "" && numero_Textbox.Text != "" && codigo_Textbox.Text != "")
                {
                    /** Limpiar Area de trabajo **/
                    //Reiniciar Tabla de Tokens, arbol y Campo de Error
                    Tabla_Tokens.Rows.Clear();
                    Arbol.Nodes.Clear(); Arbol.Refresh();
                    Errores_Textbox.Text = "";

                    /** Crear AFNs / AFDs **/
                    //AFN/AFD identificador
                    String identificador_posfija = ConvPosfija(identificador_Textbox.Text); //Posfija del identificador
                    AFN AFN_identificador = new AFN();                                      //Crear AFN
                    AFN_identificador.conviertePosfijaEnAFN(identificador_posfija);         //Inicializar AFN
                    AFD AFD_identificador = new AFD();                                      //Crear AFD
                    AFD_identificador.construyeAFD(AFN_identificador);                      //Inicializar AFD

                    //AFN/AFD numero
                    String numero_posfija = ConvPosfija(numero_Textbox.Text);               //Posfija del numero
                    AFN AFN_numero = new AFN();                                             //Crear AFN
                    AFN_numero.conviertePosfijaEnAFN(numero_posfija);                       //Inicializar AFN
                    AFD AFD_numero = new AFD();                                             //Crear AFD
                    AFD_numero.construyeAFD(AFN_numero);                                    //Inicializar AFD

                    /** Analizador de Tokens TINY (COPIAR Y PEGAR ENTREGA 5) **/
                    //Entrega 5 copiado y pegado.
                    List<String> Programa = AnalizadorLexicoTINY_Entrega5(AFD_identificador, AFD_numero);

                    /** Analizador sintactico TINY (ENTREGA FINAL) **/ 
                    //Si no se marcan errores, procede con el analizador sintactico
                    if(Errores_Textbox.Text == "")          //Si se mantuvo vacio se inicia el analisis sintactico
                    {
                        A_LR LR = CrearLR_Entrega6y7();     //Entrega 6-7 copiado y pegado.
                        AnalizadorSintacticoTINY(LR,Programa);  //Entrega 8 arbol.
                    }
                }
                else
                    MessageBox.Show("Uno de los campos no ha sido rellenado");
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        #endregion
    }
}
