using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using worker.Models.Enums;

namespace worker.Models
{
    public sealed class Requisicao : BaseModel
    {
        public StatusRequisicao Status { get; set; }

        public DateTime DataInicio { get; set; }

        public DateTime DataFim { get; set; }

        public bool Valided { get; set; } = false;
    }
}