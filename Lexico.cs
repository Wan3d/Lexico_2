
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using System.Linq.Expressions;
using System.Security.Cryptography;
using System.Runtime.InteropServices;

namespace Lexico_2
{
    public class Lexico : Token, IDisposable
    {
        public StreamReader archivo;
        public StreamWriter log;
        public StreamWriter asm;
        public int linea = 1;

        const int F = -1;

        const int E = -2;

        public Lexico()
        {
            log = new StreamWriter("prueba.log");
            asm = new StreamWriter("prueba.asm");
            log.AutoFlush = true;
            asm.AutoFlush = true;
            if (File.Exists("prueba.cpp"))
            {
                archivo = new StreamReader("prueba.cpp");
            }
            else
            {
                throw new Error("El archivo prueba.cpp no existe", log);
            }
        }

        public Lexico(string nombreArchivo)
        {
            string nombreArchivoWithoutExt = Path.GetFileNameWithoutExtension(nombreArchivo);   /* Obtenemos el nombre del archivo sin la extensión para poder crear el .log y .asm */
            if (File.Exists(nombreArchivo))
            {
                log = new StreamWriter(nombreArchivoWithoutExt + ".log");
                asm = new StreamWriter(nombreArchivoWithoutExt + ".asm");
                log.AutoFlush = true;
                asm.AutoFlush = true;
                archivo = new StreamReader(nombreArchivo);
            }
            else if (Path.GetExtension(nombreArchivo) != ".cpp")
            {
                throw new ArgumentException("El archivo debe ser de extensión .cpp");
            }
            else
            {
                throw new FileNotFoundException("La extensión " + Path.GetExtension(nombreArchivo) + " no existe");    /* Defino una excepción que indica que existe un error con el archivo en caso de no ser encontrado */
            }
        }
        public void Dispose()
        {
            archivo.Close();
            log.Close();
            asm.Close();
        }

        private int automata(char c, int estado)
        {
            int nuevoEstado = estado;
            switch (estado)
            {
                case 0:
                    if (char.IsWhiteSpace(c))
                    {
                        nuevoEstado = 0;
                    }
                    else if (char.IsLetter(c))
                    {
                        nuevoEstado = 1;
                    }
                    else if (char.IsDigit(c))
                    {
                        nuevoEstado = 2;
                    }
                    else if (c == ';')
                    {
                        nuevoEstado = 8;
                    }
                    else if (c == '{')
                    {
                        nuevoEstado = 9;
                    }
                    else if (c == '}')
                    {
                        nuevoEstado = 10;
                    }
                    else if (c == '?')
                    {
                        nuevoEstado = 11;
                    }
                    else if (c == '+')
                    {
                        nuevoEstado = 12;
                    }
                    else if (c == '-')
                    {
                        nuevoEstado = 14;
                    }
                    else if (c == '*' || c == '%')
                    {
                        nuevoEstado = 16;
                    }
                    else if (c == '&')
                    {
                        nuevoEstado = 18;
                    }
                    else if (c == '|')
                    {
                        nuevoEstado = 20;
                    }
                    else if (c == '!')
                    {
                        nuevoEstado = 21;
                    }
                    else if (c == '=')
                    {
                        nuevoEstado = 23;
                    }
                    else if (c == '>')
                    {
                        nuevoEstado = 25;
                    }
                    else if (c == '<')
                    {
                        nuevoEstado = 26;
                    }
                    else if (c == '"')
                    {
                        nuevoEstado = 27;
                    }
                    else if (c == '\'')
                    {
                        nuevoEstado = 29;
                    }
                    else if (c == '#')
                    {
                        nuevoEstado = 32;
                    }
                    else if (c == '/')
                    {
                        nuevoEstado = 34;
                    }
                    else
                    {
                        nuevoEstado = 33;
                    }
                    break;
                case 1:
                    setClasificacion(Tipos.Identificador);
                    if (!char.IsLetterOrDigit(c))
                    {
                        nuevoEstado = F;
                    }
                    break;
                case 2:
                    setClasificacion(Tipos.Numero);
                    if (char.IsDigit(c))
                    {
                        nuevoEstado = 2;
                    }
                    else if (c == '.')
                    {
                        nuevoEstado = 3;
                    }
                    else if (char.ToLower(c) == 'e')
                    {
                        nuevoEstado = 5;
                    }
                    else
                    {
                        nuevoEstado = F;
                    }
                    break;
                case 3:
                    if (char.IsDigit(c))
                    {
                        nuevoEstado = 4;
                    }
                    else
                    {
                        nuevoEstado = E;
                    }
                    break;
                case 4:
                    if (char.IsDigit(c))
                    {
                        nuevoEstado = 4;
                    }
                    else if (char.ToLower(c) == 'e')
                    {
                        nuevoEstado = 5;
                    }
                    else
                    {
                        nuevoEstado = F;
                    }
                    break;
                case 5:
                    if (char.IsDigit(c))
                    {
                        nuevoEstado = 7;
                    }
                    else if (c == '+' || c == '-')
                    {
                        nuevoEstado = 6;
                    }
                    else
                    {
                        nuevoEstado = E;
                    }
                    break;
                case 6:
                    if (char.IsDigit(c))
                    {
                        nuevoEstado = 7;
                    }
                    else
                    {
                        nuevoEstado = E;
                    }
                    break;
                case 7:
                    if (char.IsDigit(c))
                    {
                        nuevoEstado = 7;
                    }
                    else
                    {
                        nuevoEstado = F;
                    }
                    break;
                case 8:
                    setClasificacion(Tipos.FinSentencia);
                    nuevoEstado = F;
                    break;
                case 9:
                    setClasificacion(Tipos.InicioBloque);
                    nuevoEstado = F;
                    break;
                case 10:
                    setClasificacion(Tipos.FinBloque);
                    nuevoEstado = F;
                    break;
                case 11:
                    setClasificacion(Tipos.OperadorTernario);
                    nuevoEstado = F;
                    break;
                case 12:
                    setClasificacion(Tipos.OperadorTermino);
                    if (c == '+' || c == '=')
                    {
                        nuevoEstado = 13;
                    }
                    else
                    {
                        nuevoEstado = F;
                    }
                    break;
                case 13:
                    setClasificacion(Tipos.IncrementoTermino);
                    nuevoEstado = F;
                    break;
                case 14:
                    setClasificacion(Tipos.OperadorTermino);
                    if (c == '-' || c == '=')
                    {
                        nuevoEstado = 13;
                    }
                    else if (c == '>')
                    {
                        nuevoEstado = 15;
                    }
                    else
                    {
                        nuevoEstado = F;
                    }
                    break;
                case 15:
                    setClasificacion(Tipos.Puntero);
                    nuevoEstado = F;
                    break;
                case 16:
                    setClasificacion(Tipos.OperadorFactor);
                    if (c == '=')
                    {
                        nuevoEstado = 17;
                    }
                    else
                    {
                        nuevoEstado = F;
                    }
                    break;
                case 17:
                    setClasificacion(Tipos.IncrementoFactor);
                    nuevoEstado = F;
                    break;
                case 18:
                    setClasificacion(Tipos.Caracter);
                    if (c == '&')
                    {
                        nuevoEstado = 19;
                    }
                    else
                    {
                        nuevoEstado = F;
                    }
                    break;
                case 19:
                    setClasificacion(Tipos.OperadorLogico);
                    nuevoEstado = F;
                    break;
                case 20:
                    setClasificacion(Tipos.Caracter);
                    if (c == '|')
                    {
                        nuevoEstado = 19;
                    }
                    else
                    {
                        nuevoEstado = F;
                    }
                    break;
                case 21:
                    setClasificacion(Tipos.OperadorLogico);
                    if (c == '=')
                    {
                        nuevoEstado = 22;
                    }
                    else
                    {
                        nuevoEstado = F;
                    }
                    break;
                case 22:
                    setClasificacion(Tipos.OperadorRelacional);
                    nuevoEstado = F;
                    break;
                case 23:
                    setClasificacion(Tipos.Asignacion);
                    if (c == '=')
                    {
                        nuevoEstado = 24;
                    }
                    else
                    {
                        nuevoEstado = F;
                    }
                    break;
                case 24:
                    setClasificacion(Tipos.OperadorRelacional);
                    nuevoEstado = F;
                    break;
                case 25:
                    setClasificacion(Tipos.OperadorRelacional);
                    if (c == '=')
                    {
                        nuevoEstado = 24;
                    }
                    else
                    {
                        nuevoEstado = F;
                    }
                    break;
                case 26:
                    setClasificacion(Tipos.OperadorRelacional);
                    if (c == '>' || c == '=')
                    {
                        nuevoEstado = 24;
                    }
                    else
                    {
                        nuevoEstado = F;
                    }
                    break;
                case 27:
                    setClasificacion(Tipos.Cadena);
                    if (c == '"')
                    {
                        nuevoEstado = 28;
                    }
                    else if (archivo.EndOfStream)
                    {
                        nuevoEstado = E;
                    }
                    else
                    {
                        nuevoEstado = 27;
                    }
                    break;
                case 28:
                    nuevoEstado = F;
                    break;
                case 29:
                    setClasificacion(Tipos.Caracter);
                    nuevoEstado = 30;
                    break;
                case 30:
                    if (c == '\'')
                    {
                        nuevoEstado = 31;
                    }
                    else
                    {
                        nuevoEstado = E;
                    }
                    break;
                case 31:
                    nuevoEstado = F;
                    break;
                case 32:
                    setClasificacion(Tipos.Caracter);
                    if (char.IsDigit(c))
                    {
                        nuevoEstado = 32;
                    }
                    else
                    {
                        nuevoEstado = F;
                    }
                    break;
                case 33:
                    setClasificacion(Tipos.Caracter);
                    nuevoEstado = F;
                    break;
                case 34:
                    setClasificacion(Tipos.OperadorFactor);
                    if (c == '=')
                    {
                        nuevoEstado = 17;
                    }
                    else if (c == '*')
                    {
                        nuevoEstado = 36;
                    }
                    else if (c == '/')
                    {
                        nuevoEstado = 35;
                    }
                    else
                    {
                        nuevoEstado = F;
                    }
                    break;
                case 35:
                    if (c == '\n')
                    {
                        nuevoEstado = 0;
                    }
                    else
                    {
                        nuevoEstado = 35;
                    }
                    break;
                case 36:
                    if (c == '*')
                    {
                        nuevoEstado = 37;
                    }
                    else if (archivo.EndOfStream)
                    {
                        nuevoEstado = E;
                    }
                    else
                    {
                        nuevoEstado = 36;
                    }
                    break;
                case 37: //AGREGAR EXCEPCIÓN EN 36 Y 37 SI LLEGA A FIN DE ARCHIVO
                    if (c == '*')
                    {
                        nuevoEstado = 37;
                    }
                    else if (c == '/')
                    {
                        nuevoEstado = 0;
                    }
                    else if (archivo.EndOfStream)
                    {
                        nuevoEstado = E;
                    }
                    else
                    {
                        nuevoEstado = 36;
                    }
                    break;
            }
            return nuevoEstado;
        }
        public void nextToken()
        {
            char transicion;
            string buffer = "";
            int estado = 0;
            while (estado >= 0)
            {
                transicion = (char)archivo.Peek();
                estado = automata(transicion, estado);
                if (estado >= 0)
                {
                    archivo.Read();
                    if (transicion == '\n')
                    {
                        linea++;
                    }
                    if (estado > 0)
                    {
                        buffer += transicion;
                    }
                    else
                    {
                        buffer = "";
                    }
                }
            }
            if (estado == E)
            {
                if (getClasificacion() == Tipos.Cadena)
                {
                    throw new Error("léxico, se esperaba un cierre de cadena", log, linea);
                }
                else if (getClasificacion() == Tipos.Caracter)
                {
                    throw new Error("léxico, se esperaba un cierre de comilla simple", log, linea);
                }
                else if (getClasificacion() == Tipos.Numero)
                {
                    throw new Error("léxico, se esperaba un dígitos", log, linea);
                }
                else
                {
                    throw new Error("léxico, se espera fin de comentario", log, linea);
                }
            }
            if (!finArchivo())
            {
                setContenido(buffer);
                log.WriteLine(getContenido() + " = " + getClasificacion());
            }
        }
        public bool finArchivo()
        {
            return archivo.EndOfStream;
        }

    }
}

/*

Expresión Regular: Método Formal que a través de una secuencia de caracteres que define un PATRÓN de búsqueda

a) Reglas BNF 
b) Reglas BNF extendidas
c) Operaciones aplicadas al lenguaje

----------------------------------------------------------------

OAL

1. Concatenación simple (·)
2. Concatenación exponencial (Exponente) 
3. Cerradura de Kleene (*)
4. Cerradura positiva (+)
5. Cerradura Epsilon (?)
6. Operador OR (|)
7. Paréntesis ( y )

L = {A, B, C, D, E, ... , Z | a, b, c, d, e, ... , z}

D = {0, 1, 2, 3, 4, 5, 6, 7, 8, 9}

1. L.D
    LD
    >=

2. L^3 = LLL
    L^3D^2 = LLLDD
    D^5 = DDDDD
    =^2 = ==

3. L* = Cero o más letras
    D* = Cero o más dígitos

4. L+ = Una o más letras
    D+ = Una o más dígitos

5. L? = Cero o una letra (la letra es optativa-opcional)

6. L | D = Letra o dígito
    + | - = más o menos

7. (L D) L? (Letra seguido de un dígito y al final una letra opcional)

Producción gramátical

Clasificación del Token -> Expresión regular

Identificador -> L (L | D)*

Número -> D+ (.D+)? (E(+|-)? D+)?
FinSentencia -> ;
InicioBloque -> {
FinBloque -> }
OperadorTernario -> ?

Puntero -> ->

OperadorTermino -> + | -
IncrementoTermino -> ++ | += | -- | -=

Término+ -> + (+ | =)?
Término- -> - (- | = | >)?

OperadorFactor -> * | / | %
IncrementoFactor -> *= | /= | %=

Factor -> * | / | % (=)?

OperadorLogico -> && | || | !

NotOpRel -> ! (=)?

Asignación -> =

AsgOpRel -> = (=)?

OperadorRelacional -> > (=)? | < (> | =)? | == | !=

Cadena -> "c*"
Carácter -> 'c' | #D* | Lamda

----------------------------------------------------------------

Autómata: Modelo matemático que representa una expresión regular a través de un GRAFO, 
para una maquina de estado finito, para una máquina de estado finito que consiste en 
un conjunto de estados bien definidos:

- Un estado inicial 
- Un alfabeto de entrada 
- Una función de transición 

*/