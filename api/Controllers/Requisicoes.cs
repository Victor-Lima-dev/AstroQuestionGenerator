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
        public async Task<IActionResult> IniciarRequisicao([FromForm] string textoEntrada)
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

        [HttpGet("ConsultarTAGs")]
        public async Task<IActionResult> ConsultarTAGs()
        {
            var tags = await _context.TAGs.Include(t => t.Perguntas).ToListAsync();
            return Ok(tags);
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
        public async Task<IActionResult> ConsultarRequisicao([FromQuery] Guid id)
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
            var tags = await _context.TAGs.ToListAsync();

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

                        var pergunta = await _context.Perguntas
                            .Include(p => p.Respostas).Include(p => p.TAGs)
                            .FirstOrDefaultAsync(x => x.RequisicaoId == id);

                        return Ok(pergunta);

                    default: return NotFound("Requisição não encontrada");
                }
            }

            return NotFound("Requisição não encontrada");
        }

        [HttpPost("ReenviarRequisicao")]
        public async Task<IActionResult> ReenviarRequisicao(Guid id)
        {
            var requisicao = await _context.Requisicoes.FirstOrDefaultAsync(x => x.Id == id);

            if (requisicao == null)
            {
                return NotFound("Requisição não encontrada");
            }

            if (requisicao.Status == StatusRequisicao.FalhaGenerica || requisicao.Status == StatusRequisicao.FalhaPerguntasRespostas)
            {

                var queueName = "ProcessarTexto";
                var mensageiro = new Mensageiro(_url, queueName, _context);
                requisicao.Status = StatusRequisicao.Pendente;
                _context.Requisicoes.Update(requisicao);
                mensageiro.Publicar(requisicao.Id.ToString());

                return Ok(requisicao.Id);
            }

            else
            {
                return BadRequest("Requisição não pode ser reenviada, ela já esta na fila ou foi concluida com sucesso");
            }


        }

        [HttpPost("PerguntasPorTags")]
        public async Task<IActionResult> PerguntasPorTags(List<TAG> tags)
        {
            var listaPerguntas = new List<Pergunta>();

            foreach (var tag in tags)
            {
                var perguntas = await _context.Perguntas.
                    Include(p => p.TAGs).
                    Where(x => x.TAGs.Any(y => y.Texto == tag.Texto)).
                    ToListAsync();

                listaPerguntas.AddRange(perguntas);
            }

            return Ok(listaPerguntas);
        }
        [HttpPost("PerguntasPorTagsEstrito")]
        public async Task<IActionResult> PerguntasPorTagsEstrito(List<TAG> tags)
        {
            var listaPerguntas = new List<Pergunta>();

            var tagTextos = tags.Select(t => t.Texto).ToList();

            foreach (var tag in tags)
            {
                var perguntas = await _context.Perguntas
                    .Include(p => p.TAGs)
                    .Where(p => tagTextos.All(tagTexto => p.TAGs.Any(t => t.Texto == tagTexto)))
                    .ToListAsync();

                listaPerguntas.AddRange(perguntas);
            }

            return Ok();
        }


        [HttpPost("ResponderPergunta")]
        public async Task<IActionResult>ResponderPergunta ([FromForm] Guid IdPergunta,[FromForm] Guid IdResposta)
        {
    
            var pergunta = await _context.Perguntas.FirstOrDefaultAsync(x => x.Id == IdPergunta);
            var resposta = await _context.Respostas.FirstOrDefaultAsync(x => x.Id == IdResposta);

            if (pergunta == null)
            {
                Console.WriteLine(pergunta);
                return NotFound("Pergunta não encontrada");
            }

            if (resposta == null)
            {
                Console.WriteLine(resposta);
                return NotFound("Resposta não encontrada");
            }

            //verificar se a pergunta e a resposta sao correspondentes

            if (!pergunta.Respostas.Any(x => x.Id == resposta.Id))
            {
                return BadRequest("A resposta não corresponde a pergunta");
            }

            var retorno = resposta.Correta;



            return Ok(retorno);
        }
    }
}