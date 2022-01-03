using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections;
using com.calitha.goldparser;
using System.IO;


namespace Dibujo
{
    public partial class Form1 : Form
    {
        public MyParser analisis;
        public static String archivo = "";//contiene el texto del archivo
        public static String nombrearchivo = "";//contiene el nombre del archivo abierto
        //direcciones en el panel form1 de los objetos a dibujar
        int x = 0;
        int y = 50;
        

        public Form1()
        {
            InitializeComponent();
            analisis = new MyParser(Application.StartupPath + "\\Dibujo.cgt");
            llenarconstantes();
        }

        private void llenarconstantes()
        {
            Dibujo.Mi_Clase.e1.Add(8);//del enum se tomo 
            Dibujo.Mi_Clase.e2.Add("&");
            Dibujo.Mi_Clase.e1.Add(9);
            Dibujo.Mi_Clase.e2.Add(":");
            Dibujo.Mi_Clase.e1.Add(10);
            Dibujo.Mi_Clase.e2.Add(";");

        }

        private void abrirArchivoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //abrir archivo

            this.Refresh();//para repintar todo el form1
            y = 50;//regresamos y=50 siempre  antes de dibujar

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                System.IO.StreamReader sr = new
                   System.IO.StreamReader(openFileDialog1.FileName, Encoding.Default);//esto es para el archivo tipo unicode
                archivo = sr.ReadToEnd();

                nombrearchivo = openFileDialog1.FileName;
                sr.Close();
                analizarArchivo();
            }

        }

        private void analizarArchivo()
        {
            //despues de abrir el archivo lo pasamaos a la gramatica

            //entonces hay objetos para dibujar
            Dibujo.Mi_Clase.contador = 0;
            //siempre limpiamos la lista 
            Dibujo.Mi_Clase.error.Clear();

            analisis.Parse(archivo);
            int k = Dibujo.Mi_Clase.milista.Count;
            if (k > 0)
            {
                dibujar_objetos_visibles();

                if (Dibujo.Mi_Clase.error.Count > 0)
                    MessageBox.Show("Tiene errores el Archivo");
            }
        }

        private void dibujar_objetos_visibles()
        {
            while (Dibujo.Mi_Clase.milista.Count > 0)
            {
                Dibujo.nodo_figura local = Dibujo.Mi_Clase.milista.First();

                if (local.regresarTipo().ToLower() == "titulo")
                {//esto es texo
                    dibujar_Texto(local.regresarColor(), local.regresarTexto(), local.regresarTamano());
                }
                else
                {//si no es cualquier figura
                    if (local.regresarTipo().ToLower() == "circulo")
                        dibujar_Circulo(local.regresarColor());
                    else if (local.regresarTipo().ToLower() == "rectangulo")
                        dibujar_Rectangulo(local.regresarColor());
                    else if (local.regresarTipo().ToLower() == "cuadrado")
                        dibujar_Cuadrado(local.regresarColor());
                    else if (local.regresarTipo().ToLower() == "triangulo")
                        dibujar_Triangulo(local.regresarColor());
                }

                Dibujo.Mi_Clase.milista.RemoveFirst();
            }

        }

        private void dibujar_Texto(string color, string texto, string tam)
        {
            SolidBrush m = new SolidBrush(regresaColor(color));
            Graphics s = this.CreateGraphics();

            if (tam.ToLower() == "mediano")
            {
                Font letra = new Font("Tahoma", 20, FontStyle.Bold);
                s.DrawString(texto, letra, m, x, y);
                y += 20;
            }
            else if (tam.ToLower() == "grande")
            {
                Font letra = new Font("Tahoma", 30, FontStyle.Bold);
                s.DrawString(texto, letra, m, x, y);
                y += 45;

            }
        }

        private void dibujar_Rectangulo(String a)
        {
            SolidBrush m = new SolidBrush(regresaColor(a));//creamos una borcha
            Graphics s = this.CreateGraphics();//el objeto grafico que contiene la figura que se dibujara
            s.FillRectangle(m, x, y, 150, 50);
            y += 55;        //incrementamos el valor de y q es la variable donde inica la figura en el form
        }

        private void dibujar_Cuadrado(String a)
        {
            SolidBrush m = new SolidBrush(regresaColor(a));
            Graphics s = this.CreateGraphics();
            s.FillRectangle(m, x, y, 100, 100);
            y += 105;
        }



        private void dibujar_Circulo(String a)
        {
            SolidBrush m = new SolidBrush(regresaColor(a));
            Graphics s = this.CreateGraphics();
            s.FillEllipse(m, x, y, 100, 100);
            y += 105;
        }

        private void dibujar_Triangulo(String a)
        {
            SolidBrush m = new SolidBrush(regresaColor(a));
            Graphics s = this.CreateGraphics();
            Point point1 = new Point(x,y );
            Point point2 = new Point(x,y+=100 );
            Point point3 = new Point(200, y);
            Point[] listaPuntos = { point1, point2, point3 };
            s.FillPolygon(m, listaPuntos);
            y += 30;
        }



        private Color regresaColor(string a)
        {
            if ("amarillo" == a.ToLower())
                return System.Drawing.Color.Yellow;
            else if ("verde" == a.ToLower())
                return System.Drawing.Color.Green;
            else if ("rojo" == a.ToLower())
                return System.Drawing.Color.Red;
            else if ("naranja" == a.ToLower())
                return System.Drawing.Color.Orange;
            else if ("azul" == a.ToLower())
                return System.Drawing.Color.Blue;
            else if ("morado" == a.ToLower())
                return System.Drawing.Color.Purple;

            return Color.Black;
        }

        private void erroresToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form2 a = new Form2();
            if (Dibujo.Mi_Clase.error.Count > 0)
            {
                mostrarErrores(a);
            }

            a.Show();
        }

        private void mostrarErrores(Form2 a)
        {
            foreach (Symbol mi in Dibujo.Mi_Clase.error)
            {/*solo agregamos los errores al listview q recibe de parametrs un vector de 5 posiciones de la clase simbol*/

                System.Windows.Forms.ListViewItem lis = new System.Windows.Forms.ListViewItem(mi.regresarError());
                a.listView3.Items.Add(lis);
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}
