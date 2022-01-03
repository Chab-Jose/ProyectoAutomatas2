using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dibujo
{
    class Symbol
    {
        String [] error=new string[5];
        /*sustituidos en el vector en ese orden
         * String nombreArchivo=null;
        String Tipo=null;
        String Linea=null;
        String Columna=null;
        String token = null;
        */

        public Symbol(String a,String b,String c,String d,String e)
        {
            error[0] = a;
            error[1]=b;
            error[2] = c;
            error[3] = d;
            error[4] = e;
        }


        internal string[] regresarError()
        {
            return error;
        }

    }
}
