using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.context;
using api.Models;
using api.Models.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class Requisicoes : ControllerBase
    {
        private readonly AppDbContext _context;

        private readonly string _url = "amqps://peelqnnc:gU-p0eAigyVNJNfNPanQHz4onYx-Oe7u@jackal.rmq.cloudamqp.com/peelqnnc";

        public Requisicoes(AppDbContext context)
        {
            _context = context;
        }


        [HttpPost("IniciarRequisicao")]
        public async Task<IActionResult> IniciarRequisicao(string textoEntrada)
        {
            var requisicao = new Requisicao
            {
                DataInicio = DateTime.Now,
                Status = StatusRequisicao.Pendente,
                Id = Guid.NewGuid()
            };


            var textoBase = new TextoBase
            {
                Texto = textoEntrada,
                RequisicaoId = requisicao.Id
            };

            _context.Requisicoes.Add(requisicao);
            _context.TextosBase.Add(textoBase);
            await _context.SaveChangesAsync();

            var queueName = "ProcessarTexto";

            var mensageiro = new Mensageiro(_url, queueName, _context);

            mensageiro.Publicar(requisicao.Id.ToString());


            return Ok(requisicao.Id);
        }


        [HttpPost("IniciarRequisicaoTAG")]
        public async Task<IActionResult> IniciarRequisicaoTAG(string textoEntrada)
        {
            var requisicao = new Requisicao
            {
                DataInicio = DateTime.Now,
                Status = StatusRequisicao.Pendente,
                Id = Guid.NewGuid()
            };
            var textoBase = new TextoBase
            {
                Texto = textoEntrada,
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

        [HttpGet("ConsultarRequisicoes")]
        public async Task<IActionResult> ConsultarRequisicoes()
        {
            var requisicoes = await _context.Requisicoes.ToListAsync();

            return Ok(requisicoes);
        }

        [HttpGet("ConsultarTextosBases")]
        public async Task<IActionResult> ConsultarTextosBases()
        {
            var textosBases = await _context.TextosBase.ToListAsync();

            return Ok(textosBases);
        }

        [HttpGet("ConsultarRequisicao")]
        public async Task<IActionResult> ConsultarRequisicao(Guid id)
        {
            var requisicao = await _context.Requisicoes.FirstOrDefaultAsync(x => x.Id == id);

            if (requisicao == null)
            {
                return NotFound();
            }

            return Ok(requisicao);
        }

        [HttpGet("ConsultarPerguntas")]
        public async Task<IActionResult> ConsultarPerguntas()
        {
            var respostas = await _context.Respostas.ToListAsync();
            var perguntas = await _context.Perguntas.ToListAsync();

            return Ok(perguntas);
        }

        [HttpGet("ConsultarPergunta")]
        public async Task<IActionResult> ConsultarPergunta(Guid id)
        {
            var requisicao = await _context.Requisicoes.FirstOrDefaultAsync(x => x.Id == id);
            if (requisicao != null)
            {
                switch (requisicao)
                {
                    case Requisicao r when r.Status == StatusRequisicao.Pendente:
                        return Ok("Requisição pendente");

                    case Requisicao r when r.Status == StatusRequisicao.Processando:
                        return Ok("Requisição processando");

                    case Requisicao r when r.Status == StatusRequisicao.AguardandoPerguntasRespostas:
                        return Ok("Requisição foi enviada ao GPT, a pergunta esta sendo gerada");

                    case Requisicao r when r.Status == StatusRequisicao.FalhaPerguntasRespostas:
                        return Ok("A requisição falhou, a resposta do GPT não retornou no formato adequado");

                    case Requisicao r when r.Status == StatusRequisicao.FalhaGenerica:
                        return Ok("A requisição falhou, houve alguma falha no processo");

                    case Requisicao r when r.Status == StatusRequisicao.Pronto:

                        var pergunta = await _context.Perguntas.FirstOrDefaultAsync(x => x.RequisicaoId == id);

                        return Ok(pergunta);

                    default: return NotFound("Requisição não encontrada");
                }
            }

            return NotFound("Requisição não encontrada");
        }

        [HttpPost("ReenviarRequisicao")]
        public async Task<IActionResult> ReenviarRequisicao(Guid id, bool textoNormal)
        {
            var requisicao = await _context.Requisicoes.FirstOrDefaultAsync(x => x.Id == id);

            if (requisicao == null)
            {
                return NotFound("Requisição não encontrada");
            }

            if (requisicao.Status == StatusRequisicao.FalhaGenerica || requisicao.Status == StatusRequisicao.FalhaPerguntasRespostas)
            {
                if (textoNormal)
                {
                    var queueName = "ProcessarTexto";
                    var mensageiro = new Mensageiro(_url, queueName, _context);
                    requisicao.Status = StatusRequisicao.Pendente;                    
                    _context.Requisicoes.Update(requisicao);
                    mensageiro.Publicar(requisicao.Id.ToString());
                }

                else
                {
                    var queueName = "ProcessarTAG";
                    var mensageiro = new Mensageiro(_url, queueName, _context);
                    requisicao.Status = StatusRequisicao.Pendente;
                    _context.Requisicoes.Update(requisicao);
                    mensageiro.Publicar(requisicao.Id.ToString());
                }

                return Ok(requisicao.Id);
            }

            else
            {
                return BadRequest("Requisição não pode ser reenviada, ela já esta na fila ou foi concluida com sucesso");
            }


        }
    
    }
}