using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace worker.Models
{
    public sealed class TextoBase : BaseModel
    {
        public Guid RequisicaoId { get; set; }
        public string Texto { get; set; }
    }
}