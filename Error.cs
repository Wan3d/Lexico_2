using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;

namespace Lexico_2
{
    public class Error : Exception
    {
        public Error(string message, StreamWriter log) : base(message)
        {
            log.WriteLine("Error: " + message);
        }
        public Error(string message, StreamWriter log, int linea) : base(message + " en la linea " + linea)
        {
            log.WriteLine("Error: " + message + " en la linea {" + linea + "}");
        }
    }
}