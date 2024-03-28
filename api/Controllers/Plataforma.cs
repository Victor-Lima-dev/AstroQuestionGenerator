using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.context;
using api.Models;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class Plataforma : ControllerBase
    {
        private readonly AppDbContext _context;
        
        public Plataforma(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost("CriarQuestao")]
        public async Task<IActionResult> CriarQuestao(Pergunta pergunta)
        {
            //gerar um guid para a pergunta Id

            pergunta.Id = Guid.NewGuid();
            pergunta.RequisicaoId = Guid.NewGuid();

            //loop para colocar o id da pergunta nas respostas e criar um id para cada item

            foreach (var resposta in pergunta.Respostas)
            {
                resposta.Id = Guid.NewGuid();
                resposta.PerguntaId = pergunta.Id;
            }

            //loop para colocar o id da pergunta nas tags e criar um id para cada item

            foreach (var tag in pergunta.TAGs)
            {
                tag.Id = Guid.NewGuid();
                //loop para colocar o id da tag nas perguntas
                tag.Perguntas.Add(pergunta);
            }

            var verificarPergunta = Pergunta.VerificarPergunta(pergunta);

            if (!string.IsNullOrEmpty(verificarPergunta))
            {
                return BadRequest(verificarPergunta);
            }



            _context.Perguntas.Add(pergunta);

            await _context.SaveChangesAsync();

            return Ok(pergunta);
        }

        [HttpGet("ResponderQuestao")]
        public async Task<IActionResult> ResponderQuestao (string IdPergunta, string IdResposta)
        {
            //converter para guid os dois Ids

            Guid guidIdPergunta = Guid.Parse(IdPergunta);
            Guid guidIdResposta = Guid.Parse(IdResposta);

            //verificar se existe a pergunta com esse id

            var pergunta = _context.Perguntas.FirstOrDefault(p => p.Id == guidIdPergunta);

            if (pergunta == null)
            {
                return NotFound("Pergunta não encontrada");
            }

            //verificar se a resposta existe

            var resposta = _context.Respostas.FirstOrDefault(r => r.Id == guidIdResposta);

            if (resposta == null)
            {
                return NotFound("Resposta não encontrada");
            }

            //verificar se dentro da pergunta essa é uma das respostas correta

            if (resposta.Correta)
            {
                return Ok("Resposta correta");
            }

            else
            {
                return Ok("Resposta incorreta");
            }

        }

        [HttpGet("ListarQuestoes")]
        public async Task<IActionResult> ListarQuestoes()
        {
           // var perguntas = _context.Perguntas.ToList();

            //var perguntas = _context.Perguntas.Include(p => p.Respostas).Include(p => p.TAGs).ToList();

            var perguntas = _context.Perguntas.Select(p => new
            {
                p.Id,
                p.Conteudo,
                p.Respostas,
                p.TAGs
            }).ToList();


            return Ok(perguntas);
        }  

        //listar todas as tags

        [HttpGet("ListarTags")]
        public async Task<IActionResult> ListarTags()
        {
            var tags = _context.TAGs.ToList();

            return Ok(tags);
        }


    }
}