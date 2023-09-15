using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    internal class Registro
    {
        private string numPlaza;
        private string numCarril;
        private string fecha;
        private string hora;

        public Registro() { }

        public Registro(string numPlaza, string numCarril, string fecha, string hora)
        {
            this.numPlaza = numPlaza;
            this.numCarril = numCarril;
            this.fecha = fecha;
            this.hora = hora;
        }

        public string getNumPlaza() { return numPlaza; }

        public string getNumCarril() { return numCarril; }

        public string getFecha() { return fecha; }

        public string getHora() {  return hora; }

        public void setNumPlaza(string numPlaza) { this.numPlaza=numPlaza; }

        public void setNumCarril(string numCarril) { this.numCarril=numCarril; }

        public void setFecha(string fecha) { this.fecha = fecha; }

        public void setHora(string hora) { this.hora = hora; }
    }
}
