﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime;
using System.Runtime.ExceptionServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Compiladores_Proyecto_Deanosaurios
{
    internal class AFD
    {
        public List<DEstado> dEstados = new List<DEstado>();
        public List<string> alfabeto = new List<string>();
        public int cont = 0;

        public void construyeAFD(AFN afn)
        {
            cont = 0;
            dEstados.Clear();
            alfabeto = afn.alfabeto;
            int contAux = 0;
            DEstado U;
            List<EDO> listaux = new List<EDO>();
            listaux.Add(afn.estados[0]);
            dEstados.Add(cerraduraEpsilon(listaux));
            cont++;
            while (hayDestadosSinVisitar())
            {
                dEstados[contAux].visited = true;
                for(int s = 0; s < alfabeto.Count(); s++)
                {
                    U = cerraduraEpsilon(mover(dEstados[contAux], alfabeto[s]));
                    if (!estaEnDestados(U) && U.estados.Count() != 0)
                    {
                        dEstados[contAux].transiciones.Add(new TransicionDestado(U, alfabeto[s]));
                        dEstados.Add(U);
                        cont++;
                    }
                    else
                    {
                        if (estaEnDestados(U))
                        {
                            for(int i = 0; i < dEstados.Count(); i++)
                            {
                                if (compararDestados(dEstados[i], U))
                                {
                                    dEstados[contAux].transiciones.Add((new TransicionDestado(dEstados[i], alfabeto[s])));
                                }
                            }
                        }
                    }
                }
                contAux++;
            }
            foreach(DEstado d in dEstados)
            {
                foreach(EDO e in d.estados)
                {
                    if(e == afn.estados[afn.estados.Count() - 1])
                    {
                        d.setEDOaceptacion(true);
                    }
                }
            }
            //mostrar el resultado del afd
        }

        public bool estaEnDestados(DEstado U)
        {
            bool res = false;
            for(int i = 0; i < dEstados.Count() && res == false; i++)
            {
                if (dEstados[i].estados.Count() == U.estados.Count())
                {
                    bool band = true;
                    for(int j = 0; j < U.estados.Count() && band; j++)
                    {
                        if (!dEstados[i].estados.Contains(U.estados[j]))
                        {
                            band = false;
                        }
                    }
                    if(band)
                    {
                        res = true;
                        Console.WriteLine("El estado " + U.name + " es igual en sus estados que el estado " + dEstados[i].name);
                    }
                }
            }
            return res;
        }

        public bool compararDestados(DEstado d1,DEstado d2)
        {
            bool res = false;
            if (d1.estados.Count() == d2.estados.Count())
            {
                bool band = true;
                for (int j = 0; j < d1.estados.Count() && band; j++)
                {
                    if (!d1.estados.Contains(d2.estados[j]))
                    {
                        band = false;
                    }
                }
                if (band)
                {
                    res = true;
                }
            }
            return (res);
        }
        public DEstado cerraduraEpsilon(List<EDO> e)
        {
            List<EDO> res = new List<EDO>();
            List<EDO> aux = new List<EDO>();
            foreach (EDO edo in e)
            {
                aux = dfsEpsilon(edo);
                foreach(EDO eaux in aux)
                {
                    if(!res.Contains(eaux))
                    {
                        res.Add(eaux);
                    }
                }
            }
            DEstado d = new DEstado(res, cont.ToString());
            return d;
        }

        public List<EDO> dfsEpsilon(EDO e)
        {
            List<EDO> res = new List<EDO>();
            res.Add(e);
            List<EDO> aux = new List<EDO>();
            for (int i = 0; i < e.transiciones.Count();i++)
            {
                if (e.transiciones[i].valor == "")
                {
                    aux = dfsEpsilon(e.transiciones[i].destino);
                    foreach (EDO d in aux)
                    {
                        res.Add(d);
                    }
                }
            }
            return res;
        }

        public List<EDO> mover(DEstado A, string a)
        {
            List<EDO> res = new List<EDO>();
            foreach (EDO d in A.estados)
            {
                foreach(Transicion t in d.transiciones)
                {
                    if(t.valor == a)
                    {
                        res.Add(t.destino);
                    }
                }
            }
            return res;
        }

        public bool hayDestadosSinVisitar()
        {
            bool res = false;
            foreach(DEstado des in dEstados)
            {
                if(des.visited == false)
                {
                    res = true;
                    return res;
                }
            }
            return res;
        }
        public bool lexemaValido(string s, DEstado d)
        {
            bool res = false;
            if (d.estadoAceptacion && s == "")
            {
                return true;
            }
            else
            {
                string auxS = "";
                if (s.Count()>1)
                {
                    for (int i = 1; i < s.Count(); i++)
                    {
                        auxS += s[i].ToString();
                    }
                }
                foreach (TransicionDestado t in d.transiciones)
                {
                    if (s[0].ToString() == t.valor)
                    {
                        if (lexemaValido(auxS, t.destino))
                        {
                            return true;
                        }
                    }
                }
            }
            return res;
        }

        
    }
}
