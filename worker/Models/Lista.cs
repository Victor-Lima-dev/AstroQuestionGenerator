using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace worker.Models
{
    public class Lista : BaseModel
    {
        public string Nome { get; set; }
        public string Descricao { get; set; }
        public List<Pergunta> Perguntas { get; set; } = new List<Pergunta>();

        //construtor

    }
}