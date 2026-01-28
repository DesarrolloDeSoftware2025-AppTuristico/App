using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TurisTrack.DestinosTuristicos
{
    public class SaveResultDto
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public Guid? IdInterno { get; set; }
        public int IdApi { get; set; }
    }
}
