using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lexico_2
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                using (Lexico lexico = new Lexico())
                {
                    while (!lexico.finArchivo())
                    {
                        lexico.nextToken();
                    }
                    lexico.log.WriteLine("\n-----------------------------------\n");
                    lexico.log.WriteLine("Líneas del archivo: " + lexico.linea);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: " + e.Message);
            }
        }
    }
}