using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dibujo
{
    class Mi_Clase
    {
        static public LinkedList<nodo_figura> milista = new LinkedList<nodo_figura>();//funciona como una cola de objetos a dibujar
        static public int contador = 0;
        static public LinkedList<Symbol> error = new LinkedList<Symbol>();//funciona como una cola de objetos de errres
        

        /*estas se usaran para insetar los errores sintaticos observar q los simbolos & ; : diferecian
         * de su comenatio y su declaracion en el enum con symbol.... para ese cambio nos servira una guarda el caracter como tal y
         * el otro el sub indice se podria decie del enum el valor constante*/

        static public List<int> e1 = new List<int>();
        static public List<String> e2 = new List<String>();
    }
}
