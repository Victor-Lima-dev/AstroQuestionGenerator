using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.context;
using api.Models;
using api.Models.Enums;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RequisicoesTAG : ControllerBase
    {
        private readonly AppDbContext _context;

        private readonly string _url = "amqps://peelqnnc:gU-p0eAigyVNJNfNPanQHz4onYx-Oe7u@jackal.rmq.cloudamqp.com/peelqnnc";

        public RequisicoesTAG(AppDbContext context)
        {
            _context = context;
        }


        [HttpPost("IniciarRequisicaoTAG")]
        public async Task<IActionResult> IniciarRequisicaoTAG(List<TAG> tags)
        {
            var entrada = TAG.ConcatenarTags(tags);

            var requisicao = new Requisicao
            {
                DataInicio = DateTime.Now,
                Status = StatusRequisicao.Pendente,
                Id = Guid.NewGuid()
            };

            var textoBase = new TextoBase
            {
                Texto = entrada,
                RequisicaoId = requisicao.Id
            };

            _context.Requisicoes.Add(requisicao);
            _context.TextosBase.Add(textoBase);
            await _context.SaveChangesAsync();

            var queueName = "ProcessarTAG";

            var mensageiro = new Mensageiro(_url, queueName, _context);

            mensageiro.Publicar(requisicao.Id.ToString());

            return Ok(requisicao.Id);


        }


    }
}