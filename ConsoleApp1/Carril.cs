using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    internal class Carril
    {
        private string Nombre;
        private string IP;
        public Carril() {
            this.Nombre = string.Empty;
            this.IP = string.Empty;
        }

        public Carril(string nombre, string iP)
        {
            Nombre = nombre;
            IP = iP;
        }

        public string getNombre() { return Nombre; }

        public string getIP() { return IP; }

        public void setNombre(string nombre) { this.Nombre = nombre; }

        public void setIP(string ip) {  this.IP = ip; }
       
    }
}
