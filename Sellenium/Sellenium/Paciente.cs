using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sellenium
{
    public class Paciente
    {
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public DateTime FechaCreacion { get; set; }
        public string Telefono { get; set; }
        public bool Escaneado { get; set; }

        public int index { get; set; }

    }
}
