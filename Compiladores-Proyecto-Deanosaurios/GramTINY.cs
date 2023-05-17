using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Compiladores_Proyecto_Deanosaurios
{
    internal class GramTINY
    {
        public Dictionary<String, String> Gramatica, Siguientes;
        public List<String> Terminales, NoTerminales;
        public String Produccion;
        public A_LR Afd_lr;

        public GramTINY()
        {
            this.Gramatica = new Dictionary<string, string> 
            {
                {"programa" , "secuencia-sent" },
                {"secuencia-sent" , "secuencia-sent ; sentencia|sentencia" },
                {"sentencia" , "sent-if|sent-repeat|sent-assign|sent-read|sent-write"},
                {"sent-if" , "if exp then secuencia-sent end|if exp then secuencia-sent else secuencia-sent end"},
                {"sent-repeat" , "repeat secuencia-sent until exp"},
                {"sent-assign" , "identificador := exp"},
                {"sent-read" , "read identificador"},
                {"sent-write" , "write exp"},
                {"exp" , "exp-simple op-comp exp-simple|exp-simple"},
                {"op-comp" , "<|>|=" },
                {"exp-simple" , "exp-simple opsuma term|term"},
                {"opsuma" , "+|-"},
                {"term" , "term opmult factor|factor"},
                {"opmult" , "*|/"},
                {"factor" , "( exp )|numero|identificador"},
            };

            this.Terminales = new List<String>{ ";" , "if" , "then" , "end" , "else" , "repeat" , "until" , "identificador" , ":=" , "read" , "write" , "<" , ">" , "=" , "+" , "-" , "*" , "/" , "(" , ")" , "numero" };
            this.NoTerminales = new List<String> { "programa" , "secuencia-sent" , "sentencia" , "sent-if" , "sent-repeat" , "sent-assign" , "sent-read" , "sent-write" , "exp" , "op-comp" , "exp-simple" , "opsuma" , "term" , "opmult" , "factor" };

            this.Produccion = ". programa";

                //(Luis)
            this.Siguientes = new Dictionary<string, string>
            {
                {"programa" , "$" },
                {"secuencia-sent" , "; end else until $" },
                {"sentencia" , "; end else until $" },
                {"sent-if" , "; end else until $"},
                {"sent-repeat" , "; end else until $"},
                {"sent-assign" , "; end else until $"},
                {"sent-read" , "; end else until $"},
                {"sent-write" , "; end else until $"},
                {"exp" , "; end else until $ then )"},
                {"op-comp" , "( numero identificador" },
                {"exp-simple" , "; end else until $ then ) < > = + -"},
                {"opsuma" , "( numero identificador"},
                {"term" , "; end else until $ then ) < > = + - * /"},
                {"opmult" , "( numero identificador"},
                {"factor" , "; end else until $ then ) < > = + - * /"},
            };

            this.Afd_lr = new A_LR(this.Gramatica, this.Terminales, this.NoTerminales, this.Produccion);

        }
    }
}
