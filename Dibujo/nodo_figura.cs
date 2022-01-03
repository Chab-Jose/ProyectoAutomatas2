using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dibujo
{
    class nodo_figura
    {
        //esta clase sirve para todas la figuras
        String tipo = null;//cuadrado,rectangulo, texto ,circulo,
        String color = null;//rojo ,amarillo,verde
        String tamano = null;//mediano ,grande
        String texto = null;//texto 

        public void insertaTipo(String a)
        {
            tipo = a;
        }
        public void insertaColor(String a)
        {
            color = a;
        }
        public void insertaTamano(String a)
        {
            tamano = a;
        }
        public void insertaTexto(String a)
        {
            texto = a;
        }



        public String regresarTipo()
        {
            return tipo;
        }
        public String regresarColor()
        {
            return color;
        }
        public String regresarTamano()
        {
            return tamano;
        }
        public String regresarTexto()
        {
            return texto;
        }



    }
}