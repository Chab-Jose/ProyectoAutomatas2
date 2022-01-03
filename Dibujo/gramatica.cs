
using System;
using System.IO;
using System.Runtime.Serialization;
using com.calitha.goldparser.lalr;
using com.calitha.commons;
using com.calitha.goldparser;
using System.Windows.Forms;
using System.Linq;

namespace com.calitha.goldparser
{

    [Serializable()]
    public class SymbolException : System.Exception
    {
        public SymbolException(string message) : base(message)
        {
        }

        public SymbolException(string message,
            Exception inner) : base(message, inner)
        {
        }

        protected SymbolException(SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }

    }

    [Serializable()]
    public class RuleException : System.Exception
    {

        public RuleException(string message) : base(message)
        {
        }

        public RuleException(string message,
                             Exception inner) : base(message, inner)
        {
        }

        protected RuleException(SerializationInfo info,
                                StreamingContext context) : base(info, context)
        {
        }

    }

    enum SymbolConstants : int
    {
        SYMBOL_EOF          =  0, // (EOF)
        SYMBOL_ERROR        =  1, // (Error)
        SYMBOL_COMMENT      =  2, // Comment
        SYMBOL_NEWLINE      =  3, // NewLine
        SYMBOL_WHITESPACE   =  4, // Whitespace
        SYMBOL_DIV          =  5, // '/'
        SYMBOL_LTMINUSMINUS =  6, // '<--'
        SYMBOL_MINUSMINUSGT =  7, // '-->'
        SYMBOL_AMP          =  8, // '&'
        SYMBOL_COLON        =  9, // ':'
        SYMBOL_SEMI         = 10, // ';'
        SYMBOL_AMARILLO     = 11, // amarillo
        SYMBOL_AZUL         = 12, // azul
        SYMBOL_CIRCULO      = 13, // circulo
        SYMBOL_CUADRADO     = 14, // cuadrado
        SYMBOL_FIGURA       = 15, // Figura
        SYMBOL_GRANDE       = 16, // grande
        SYMBOL_LETRA        = 17, // letra
        SYMBOL_MEDIANO      = 18, // mediano
        SYMBOL_MORADO       = 19, // morado
        SYMBOL_NARANJA      = 20, // naranja
        SYMBOL_NUMERO       = 21, // numero
        SYMBOL_RECTANGULO   = 22, // rectangulo
        SYMBOL_ROJO         = 23, // rojo
        SYMBOL_TILDE        = 24, // tilde
        SYMBOL_TITULO       = 25, // Titulo
        SYMBOL_TRIANGULO    = 26, // triangulo
        SYMBOL_VERDE        = 27, // verde
        SYMBOL_COLOR        = 28, // <Color>
        SYMBOL_FIGURA2      = 29, // <Figura>
        SYMBOL_INICIO       = 30, // <Inicio>
        SYMBOL_PALABRA      = 31, // <Palabra>
        SYMBOL_SIZE         = 32, // <Size>
        SYMBOL_TIPO         = 33  // <Tipo>
    };

    enum RuleConstants : int
    {
        RULE_INICIO                                 =  0, // <Inicio> ::= <Figura>
        RULE_INICIO2                                =  1, // <Inicio> ::= <Inicio> <Figura>
        RULE_FIGURA_FIGURA_COLON_COLON_AMP_SEMI     =  2, // <Figura> ::= Figura ':' ':' <Tipo> '&' <Color> ';'
        RULE_FIGURA_TITULO_COLON_COLON_AMP_AMP_SEMI =  3, // <Figura> ::= Titulo ':' ':' <Palabra> '&' <Color> '&' <Size> ';'
        RULE_PALABRA_LETRA                          =  4, // <Palabra> ::= letra
        RULE_PALABRA_NUMERO                         =  5, // <Palabra> ::= numero
        RULE_PALABRA_TILDE                          =  6, // <Palabra> ::= tilde
        RULE_PALABRA_LETRA2                         =  7, // <Palabra> ::= <Palabra> letra
        RULE_PALABRA_NUMERO2                        =  8, // <Palabra> ::= <Palabra> numero
        RULE_PALABRA_TILDE2                         =  9, // <Palabra> ::= <Palabra> tilde
        RULE_COLOR_ROJO                             = 10, // <Color> ::= rojo
        RULE_COLOR_AMARILLO                         = 11, // <Color> ::= amarillo
        RULE_COLOR_VERDE                            = 12, // <Color> ::= verde
        RULE_COLOR_NARANJA                          = 13, // <Color> ::= naranja
        RULE_COLOR_AZUL                             = 14, // <Color> ::= azul
        RULE_COLOR_MORADO                           = 15, // <Color> ::= morado
        RULE_TIPO_CUADRADO                          = 16, // <Tipo> ::= cuadrado
        RULE_TIPO_RECTANGULO                        = 17, // <Tipo> ::= rectangulo
        RULE_TIPO_CIRCULO                           = 18, // <Tipo> ::= circulo
        RULE_TIPO_TRIANGULO                         = 19, // <Tipo> ::= triangulo
        RULE_SIZE_GRANDE                            = 20, // <Size> ::= grande
        RULE_SIZE_MEDIANO                           = 21  // <Size> ::= mediano
    };

    public class MyParser
    {
        private LALRParser parser;

        public MyParser(string filename)
        {
            FileStream stream = new FileStream(filename,
                                               FileMode.Open,
                                               FileAccess.Read,
                                               FileShare.Read);
            Init(stream);
            stream.Close();
        }

        public MyParser(string baseName, string resourceName)
        {
            byte[] buffer = ResourceUtil.GetByteArrayResource(
                System.Reflection.Assembly.GetExecutingAssembly(),
                baseName,
                resourceName);
            MemoryStream stream = new MemoryStream(buffer);
            Init(stream);
            stream.Close();
        }

        public MyParser(Stream stream)
        {
            Init(stream);
        }

        private void Init(Stream stream)
        {
            CGTReader reader = new CGTReader(stream);
            parser = reader.CreateNewParser();
            parser.TrimReductions = false;
            parser.StoreTokens = LALRParser.StoreTokensMode.NoUserObject;

            parser.OnReduce += new LALRParser.ReduceHandler(ReduceEvent);
            parser.OnTokenRead += new LALRParser.TokenReadHandler(TokenReadEvent);
            parser.OnAccept += new LALRParser.AcceptHandler(AcceptEvent);
            parser.OnTokenError += new LALRParser.TokenErrorHandler(TokenErrorEvent);
            parser.OnParseError += new LALRParser.ParseErrorHandler(ParseErrorEvent);
        }

        public void Parse(string source)
        {
            parser.Parse(source);

        }

        private void TokenReadEvent(LALRParser parser, TokenReadEventArgs args)
        {
            try
            {
                args.Token.UserObject = CreateObject(args.Token);
            }
            catch (Exception e)
            {
                args.Continue = false;
                //todo: Report message to UI?
            }
        }

        private Object CreateObject(TerminalToken token)
        {
            switch (token.Symbol.Id)
            {
                case (int)SymbolConstants.SYMBOL_EOF:
                    //(EOF)
                    //todo: Create a new object that corresponds to the symbol
                    return null;

                case (int)SymbolConstants.SYMBOL_ERROR:
                    //(Error)
                    //todo: Create a new object that corresponds to the symbol
                    return null;

                case (int)SymbolConstants.SYMBOL_COMMENT:
                    //Comment
                    //todo: Create a new object that corresponds to the symbol
                    return null;

                case (int)SymbolConstants.SYMBOL_NEWLINE:
                    //NewLine
                    //todo: Create a new object that corresponds to the symbol
                    return null;

                case (int)SymbolConstants.SYMBOL_WHITESPACE:
                    //Whitespace
                    //todo: Create a new object that corresponds to the symbol
                    return null;

                case (int)SymbolConstants.SYMBOL_DIV:
                    //'/'
                    //todo: Create a new object that corresponds to the symbol
                    return null;

                case (int)SymbolConstants.SYMBOL_LTMINUSMINUS:
                    //'<--'
                    //todo: Create a new object that corresponds to the symbol
                    return null;

                case (int)SymbolConstants.SYMBOL_MINUSMINUSGT:
                    //'-->'
                    //todo: Create a new object that corresponds to the symbol
                    return null;

                case (int)SymbolConstants.SYMBOL_AMP:
                    //'&'
                    //todo: Create a new object that corresponds to the symbol
                    return null;

                case (int)SymbolConstants.SYMBOL_COLON:
                    //':'
                    //todo: Create a new object that corresponds to the symbol
                    return null;

                case (int)SymbolConstants.SYMBOL_SEMI:
                    //';'
                    //todo: Create a new object that corresponds to the symbol
                    return null;

                case (int)SymbolConstants.SYMBOL_AMARILLO:
                    //amarillo
                    //todo: Create a new object that corresponds to the symbol
                    return token.Text;

                case (int)SymbolConstants.SYMBOL_AZUL:
                    //azul
                    //todo: Create a new object that corresponds to the symbol
                    return token.Text;

                case (int)SymbolConstants.SYMBOL_CIRCULO:
                    //circulo
                    //todo: Create a new object that corresponds to the symbol
                    return token.Text;

                case (int)SymbolConstants.SYMBOL_CUADRADO:
                    //cuadrado
                    //todo: Create a new object that corresponds to the symbol
                    return token.Text;

                case (int)SymbolConstants.SYMBOL_FIGURA:
                    //Figura
                    //todo: Create a new object that corresponds to the symbol
                    return null;

                case (int)SymbolConstants.SYMBOL_GRANDE:
                    //grande
                    //todo: Create a new object that corresponds to the symbol
                    return token.Text;

                case (int)SymbolConstants.SYMBOL_LETRA:
                    //letra
                    //todo: Create a new object that corresponds to the symbol
                    return token.Text;

                case (int)SymbolConstants.SYMBOL_MEDIANO:
                    //mediano
                    //todo: Create a new object that corresponds to the symbol
                    return token.Text;

                case (int)SymbolConstants.SYMBOL_MORADO:
                    //morado
                    //todo: Create a new object that corresponds to the symbol
                    return token.Text;

                case (int)SymbolConstants.SYMBOL_NARANJA:
                    //naranja
                    //todo: Create a new object that corresponds to the symbol
                    return token.Text;

                case (int)SymbolConstants.SYMBOL_NUMERO:
                    //numero
                    //todo: Create a new object that corresponds to the symbol
                    return token.Text;

                case (int)SymbolConstants.SYMBOL_RECTANGULO:
                    //rectangulo
                    //todo: Create a new object that corresponds to the symbol
                    return token.Text;

                case (int)SymbolConstants.SYMBOL_ROJO:
                    //rojo
                    //todo: Create a new object that corresponds to the symbol
                    return token.Text;

                case (int)SymbolConstants.SYMBOL_TILDE:
                    //tilde
                    //todo: Create a new object that corresponds to the symbol
                    return token.Text;

                case (int)SymbolConstants.SYMBOL_TITULO:
                    //Titulo
                    //todo: Create a new object that corresponds to the symbol
                    return null;

                case (int)SymbolConstants.SYMBOL_TRIANGULO:
                    //triangulo
                    //todo: Create a new object that corresponds to the symbol
                    return token.Text;

                case (int)SymbolConstants.SYMBOL_VERDE:
                    //verde
                    //todo: Create a new object that corresponds to the symbol
                    return token.Text;

                case (int)SymbolConstants.SYMBOL_COLOR:
                    //<Color>
                    //todo: Create a new object that corresponds to the symbol
                    return null;

                case (int)SymbolConstants.SYMBOL_FIGURA2:
                    //<Figura>
                    //todo: Create a new object that corresponds to the symbol
                    return null;

                case (int)SymbolConstants.SYMBOL_INICIO:
                    //<Inicio>
                    //todo: Create a new object that corresponds to the symbol
                    return null;

                case (int)SymbolConstants.SYMBOL_PALABRA:
                    //<Palabra>
                    //todo: Create a new object that corresponds to the symbol
                    return null;

                case (int)SymbolConstants.SYMBOL_SIZE:
                    //<Size>
                    //todo: Create a new object that corresponds to the symbol
                    return null;

                case (int)SymbolConstants.SYMBOL_TIPO:
                    //<Tipo>
                    //todo: Create a new object that corresponds to the symbol
                    return null;

            }
            throw new SymbolException("Unknown symbol");
        }

        private void ReduceEvent(LALRParser parser, ReduceEventArgs args)
        {
            try
            {
                args.Token.UserObject = CreateObject(args.Token);
            }
            catch (Exception e)
            {
                args.Continue = false;
                //todo: Report message to UI?
            }
        }

        public static Object CreateObject(NonterminalToken token)
        {
            switch (token.Rule.Id)
            {
                case (int)RuleConstants.RULE_INICIO:
                    //<Inicio> ::= <Figura>
                    //todo: Create a new object using the stored user objects.
                    return null;

                case (int)RuleConstants.RULE_INICIO2:
                    //<Inicio> ::= <Inicio> <Figura>
                    //todo: Create a new object using the stored user objects.
                    return null;

                case (int)RuleConstants.RULE_FIGURA_FIGURA_COLON_COLON_AMP_SEMI:
                    {
                        //<Figura> ::= Figura ':' ':' <Tipo> '&' <Color> ';'
                        Dibujo.nodo_figura nuevo = new Dibujo.nodo_figura();
                        nuevo.insertaTipo(token.Tokens[3].UserObject.ToString());
                        nuevo.insertaColor(token.Tokens[5].UserObject.ToString());
                        insertarFigura_lista(nuevo);
                        return null;
                    }
                case (int)RuleConstants.RULE_FIGURA_TITULO_COLON_COLON_AMP_AMP_SEMI:
                    {
                        //<Figura> ::= Titulo ':' ':' <Palabra> '&' <Color> '&' <Size> ';'
                        Dibujo.nodo_figura nuevo = new Dibujo.nodo_figura();
                        nuevo.insertaTipo("Titulo");
                        nuevo.insertaTexto(token.Tokens[3].UserObject.ToString());
                        nuevo.insertaColor(token.Tokens[5].UserObject.ToString());
                        nuevo.insertaTamano(token.Tokens[7].UserObject.ToString());
                        insertarFigura_lista(nuevo);
                        return null;
                    }
                case (int)RuleConstants.RULE_PALABRA_LETRA:
                    //<Palabra> ::= letra
                    //todo: Create a new object using the stored user objects.
                    return token.Tokens[0].UserObject.ToString();

                case (int)RuleConstants.RULE_PALABRA_NUMERO:
                    //<Palabra> ::= numero
                    //todo: Create a new object using the stored user objects.
                    return token.Tokens[0].UserObject.ToString();

                case (int)RuleConstants.RULE_PALABRA_TILDE:
                    //<Palabra> ::= tilde
                    //todo: Create a new object using the stored user objects.
                    return token.Tokens[0].UserObject.ToString();

                case (int)RuleConstants.RULE_PALABRA_LETRA2:
                    //<Palabra> ::= <Palabra> letra
                    //todo: Create a new object using the stored user objects.
                    return token.Tokens[0].UserObject.ToString() + " " + token.Tokens[1].UserObject.ToString();

                case (int)RuleConstants.RULE_PALABRA_NUMERO2:
                    //<Palabra> ::= <Palabra> numero
                    //todo: Create a new object using the stored user objects.
                    return token.Tokens[0].UserObject.ToString() + " " + token.Tokens[1].UserObject.ToString();

                case (int)RuleConstants.RULE_PALABRA_TILDE2:
                    //<Palabra> ::= <Palabra> tilde
                    //todo: Create a new object using the stored user objects.
                    return token.Tokens[0].UserObject.ToString() + " " + token.Tokens[1].UserObject.ToString();

                case (int)RuleConstants.RULE_COLOR_ROJO:
                    //<Color> ::= rojo
                    //todo: Create a new object using the stored user objects.
                    return token.Tokens[0].UserObject;

                case (int)RuleConstants.RULE_COLOR_AMARILLO:
                    //<Color> ::= amarillo
                    //todo: Create a new object using the stored user objects.
                    return token.Tokens[0].UserObject;

                case (int)RuleConstants.RULE_COLOR_VERDE:
                    //<Color> ::= verde
                    //todo: Create a new object using the stored user objects.
                    return token.Tokens[0].UserObject;

                case (int)RuleConstants.RULE_COLOR_NARANJA:
                    //<Color> ::= naranja
                    //todo: Create a new object using the stored user objects.
                    return token.Tokens[0].UserObject;

                case (int)RuleConstants.RULE_COLOR_AZUL:
                    //<Color> ::= azul
                    //todo: Create a new object using the stored user objects.
                    return token.Tokens[0].UserObject;

                case (int)RuleConstants.RULE_COLOR_MORADO:
                    //<Color> ::= morado
                    //todo: Create a new object using the stored user objects.
                    return token.Tokens[0].UserObject;

                case (int)RuleConstants.RULE_TIPO_CUADRADO:
                    //<Tipo> ::= cuadrado
                    //todo: Create a new object using the stored user objects.
                    return token.Tokens[0].UserObject;

                case (int)RuleConstants.RULE_TIPO_RECTANGULO:
                    //<Tipo> ::= rectangulo
                    //todo: Create a new object using the stored user objects.
                    return token.Tokens[0].UserObject;

                case (int)RuleConstants.RULE_TIPO_CIRCULO:
                    //<Tipo> ::= circulo
                    //todo: Create a new object using the stored user objects.
                    return token.Tokens[0].UserObject;

                case (int)RuleConstants.RULE_TIPO_TRIANGULO:
                    //<Tipo> ::= triangulo
                    //todo: Create a new object using the stored user objects.
                    return token.Tokens[0].UserObject;

                case (int)RuleConstants.RULE_SIZE_GRANDE:
                    //<Size> ::= grande
                    //todo: Create a new object using the stored user objects.
                    return token.Tokens[0].UserObject;

                case (int)RuleConstants.RULE_SIZE_MEDIANO:
                    //<Size> ::= mediano
                    //todo: Create a new object using the stored user objects.
                    return token.Tokens[0].UserObject;

            }
            throw new RuleException("Unknown rule");
        }

        private static void insertarFigura_lista(Dibujo.nodo_figura nuevo)
        {

            Dibujo.Mi_Clase.milista.AddLast(nuevo);
        }

        private void AcceptEvent(LALRParser parser, AcceptEventArgs args)
        {
            //cuando finaliza de analizar
            MessageBox.Show("Analisis del documento finalizado");
        }

        private void TokenErrorEvent(LALRParser parser, TokenErrorEventArgs args)
        {
            //string message = "Token error with input: '"+args.Token.ToString()+" ";
            //todo: Report message to UI?

            String a = Dibujo.Form1.nombrearchivo;
            String b = "Lexico";
            String c = args.Token.Location.LineNr.ToString();
            String d = args.Token.Location.ColumnNr.ToString();
            String e = args.Token.ToString();

            Dibujo.Symbol m = new Dibujo.Symbol(a, b, c, d, e);
            Dibujo.Mi_Clase.error.AddLast(m);

            Dibujo.Mi_Clase.contador++;
            if(Dibujo.Mi_Clase.contador > 30)
            {
                args.Continue = false;
            }
            else
            {
                args.Continue = true;
            }
        }

        private void ParseErrorEvent(LALRParser parser, ParseErrorEventArgs args)
        {
            //string message = "Parse error caused by token: '"+args.UnexpectedToken.ToString()+"'";
            //todo: Report message to UI?

            int c = args.UnexpectedToken.Location.LineNr;
            int d = args.UnexpectedToken.Location.ColumnNr;

            String a = Dibujo.Form1.nombrearchivo;
            String b = "Sintactico";
            String c1 = c.ToString();
            String d1 = d.ToString();
            String e = "la palabra incorrecta es: " + args.UnexpectedToken.ToString();

            Dibujo.Mi_Clase.contador++;
            if (Dibujo.Mi_Clase.contador > 30)
            {
                args.Continue = ContinueMode.Stop;
            }
            else
            {
                String u = args.UnexpectedToken.ToString();
                String df = args.ExpectedTokens.ToString();

                String[] g = df.Split(' ');

                String tokensiguiente_nombre;
                tokensiguiente_nombre = g[0];

                int idtoken = 0;

                for(int i = 0; i < 27; i++)
                {
                    String z = Enum.GetName(typeof(SymbolConstants), i);
                    String t = z.Replace("SIMBOL_", "");
                    t = t.ToLower();
                    if (t == tokensiguiente_nombre)
                    {
                        idtoken = i;
                        break;
                    }
                }

                if(idtoken == 0)
                {
                    for (int vv = 0; vv < 3; vv++)
                    {
                        String k = Dibujo.Mi_Clase.e2.ElementAt(vv);
                        if (k == tokensiguiente_nombre)
                        {
                            idtoken = Dibujo.Mi_Clase.e1.ElementAt(vv);
                            break;
                        }
                    }
                }


                Dibujo.Symbol m = new Dibujo.Symbol(a, b, c1, d1, e + "se esperaba " + tokensiguiente_nombre);
                Dibujo.Mi_Clase.error.AddLast(m);

                Location nuevalocation = new Location(0, 0, 0);

                args.NextToken = new TerminalToken(new SymbolTerminal(idtoken, tokensiguiente_nombre), "", nuevalocation);
                args.Continue = ContinueMode.Insert;


            }
        }


    }
}
