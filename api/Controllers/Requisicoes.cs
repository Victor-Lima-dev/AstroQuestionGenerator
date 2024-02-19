using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.context;
using api.Models;
using api.Models.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace api.Controllers
{
    [ApiController]
    [Authorize]
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

            // var perguntas = await _context.Perguntas.ToListAsync();

          var perguntas = await _context.Perguntas
    .Include(p => p.Respostas)
    .Select(p => new
    {
        Id = p.Id,
        RequisicaoId = p.RequisicaoId,
        Conteudo = p.Conteudo,
        Respostas = p.Respostas.Select(r => new
        {
            Id = r.Id,
            Conteudo = r.Conteudo,
            Correta = r.Correta
        }),
        Tags = p.TAGs.Select(t => t.Texto)  // Incluir apenas os nomes das TAGs
    })
    .ToListAsync();


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
        public async Task<IActionResult> PerguntasPorTags([FromForm] Guid tagId)
        {
            //verificar se a tag existe

            var tagExistente = await _context.TAGs.FirstOrDefaultAsync(x => x.Id == tagId);

            if (tagExistente == null)
            {
                Console.WriteLine(tagId);
                return NotFound("Tag não encontrada");
            }
                  
          var perguntas = await _context.Perguntas
            .Include(p => p.Respostas)
            .Select(p => new
            {
            Id = p.Id,
            RequisicaoId = p.RequisicaoId,
            Conteudo = p.Conteudo,
            Respostas = p.Respostas.Select(r => new
            {
                Id = r.Id,
                Conteudo = r.Conteudo,
                Correta = r.Correta
            }),
            Tags = p.TAGs.Select(t => t.Texto)  
            }).Where(p => p.Tags.Any(t => t == tagExistente.Texto))
            .ToListAsync();

            return Ok(perguntas);
          
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

        [HttpGet("TAGsSemelhantes")]
        public async Task<IActionResult> TAGsSemelhantes(string texto)
        {
            var tags = await _context.TAGs.ToListAsync();

            var tagsSemelhantes = tags.Where(t => t.Texto.Contains(texto)).ToList();

            tagsSemelhantes = TAG.RemoverTagsDuplicadas(tagsSemelhantes);

            return Ok(tagsSemelhantes);
        }


        [HttpGet("TAGsRelacionadas")]
        public async Task<IActionResult> TAGsRelacionadas(Guid tagId)
        {
            //procurar uma lista de perguntas que contem a tag
            //verificar se a tag existe

            var tagExistente = await _context.TAGs.FirstOrDefaultAsync(x => x.Id == tagId);

            if (tagExistente == null)
            {
                Console.WriteLine(tagId);
                return NotFound("Tag não encontrada");
            }    
                    
          var perguntas = await _context.Perguntas
            .Include(p => p.Respostas)
            .Select(p => new
            {
            Id = p.Id,
            RequisicaoId = p.RequisicaoId,
            Conteudo = p.Conteudo,
            Respostas = p.Respostas.Select(r => new
            {
                Id = r.Id,
                Conteudo = r.Conteudo,
                Correta = r.Correta
            }),
            Tags = p.TAGs.Select(t => t.Texto)  
            }).Where(p => p.Tags.Any(t => t == tagExistente.Texto))
            .ToListAsync();

            //agora que temos a lista de perguntas, vamos todas as tags que estão relacionadas a essas perguntas

            var tagsRelacionadas = new List<TAG>();

            foreach (var pergunta in perguntas)
            {
                var tags = await _context.TAGs.Where(t => pergunta.Tags.Any(tag => tag == t.Texto)).ToListAsync();

                tagsRelacionadas.AddRange(tags);
            }

            //agora vamos remover as tags que repetidas na lista

          tagsRelacionadas = TAG.RemoverTagsDuplicadas(tagsRelacionadas);



            

            return Ok(tagsRelacionadas);
        }


        [HttpGet("RetornarTAGs")]
        public async Task<IActionResult> RetornarTAGs()
        {
            var tags = await _context.TAGs.ToListAsync();

            tags = TAG.RemoverTagsDuplicadas(tags);

            return Ok(tags);
        }

        [HttpGet("RetornarTAGsPaginacao")]
        public async Task<IActionResult> RetornarTAGsPaginacao(int quantidade)
        {
          //criar paginacao

            var tags = await _context.TAGs.Take(quantidade).ToListAsync();

            //verificar se existem tags iguais, por exemplo Quimica e quimica, só quero uma delas

            tags = TAG.RemoverTagsDuplicadas(tags);
            

           



            return Ok(tags);
        }

        [HttpGet("ProcurarTAG")]
        public async Task<IActionResult> ProcurarTAG(string texto)
        {
            //pesquisar todas as tags que contem o texto

            var tags = await _context.TAGs.Where(t => t.Texto.Contains(texto)).ToListAsync();

            tags = TAG.RemoverTagsDuplicadas(tags);

            return Ok(tags);
        }

        [HttpGet("ProcurarQuestao")]
        public async Task<IActionResult> ProcurarQuestao(string texto)
        {
            //pesquisar todas as perguntas que contem o texto

           var perguntas = await _context.Perguntas
    .Where(t => t.Conteudo.Contains(texto))
    .Include(p => p.Respostas)
    .Select(p => new
    {
        Id = p.Id,
        RequisicaoId = p.RequisicaoId,
        Conteudo = p.Conteudo,
        Respostas = p.Respostas.Select(r => new
        {
            Id = r.Id,
            Conteudo = r.Conteudo,
            Correta = r.Correta
        }),
        Tags = p.TAGs.Select(t => t.Texto)  // Incluir apenas os nomes das TAGs
    })
    .ToListAsync();

            return Ok(perguntas);
        }

        [HttpPost("ResponderPergunta")]
        public async Task<IActionResult>ResponderPergunta ([FromForm] Guid IdPergunta,[FromForm] Guid IdResposta)
        {
    
            var pergunta = await _context.Perguntas.FirstOrDefaultAsync(x => x.Id == IdPergunta);
            var resposta = await _context.Respostas.FirstOrDefaultAsync(x => x.Id == IdResposta);

            if (pergunta == null)
            {

                return NotFound("Pergunta não encontrada");
            }

            if (resposta == null)
            {
       
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