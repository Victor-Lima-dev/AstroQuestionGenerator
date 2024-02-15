using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using api.context;
using api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace api.Controllers
{
    [ApiController]
    [Authorize]

    [Route("api/[controller]")]
    public class Perguntas : ControllerBase
    {
        private readonly AppDbContext _context;
        
        public Perguntas(AppDbContext context)
        {
            _context = context;
        }

        [HttpDelete("Deletar")]
        public ActionResult Deletar([FromForm] string id)
        {
            Console.WriteLine(id);
             var idRecebido = id.Replace("[", "").Replace("]", "").Replace("\"", "");

            var idConvertido = Guid.Parse(idRecebido);


            var pergunta = _context.Perguntas.Where(x => x.Id == idConvertido).FirstOrDefault();

            if(pergunta == null)
            {
                Console.WriteLine("Pergunta não existe");
                return BadRequest("A pergunta não existe");
            }

            _context.Perguntas.Remove(pergunta);
            _context.SaveChanges();

            return Ok("Pergunta deletada com sucesso");
        }

        [HttpPut("Editar")]
        public ActionResult Editar([FromBody] Pergunta pergunta)
        {
            var perguntaEditada = _context.Perguntas.Where(x => x.Id == pergunta.Id).FirstOrDefault();

            if(perguntaEditada == null)
            {
                Console.WriteLine(pergunta.Id);
                return BadRequest("Pergunta não existe");
            }

        //     public Guid RequisicaoId { get; set; }


        // public string Conteudo { get; set; }
        
            perguntaEditada.Conteudo = pergunta.Conteudo;


        // public List<Resposta> Respostas { get; set; } = new List<Resposta>();

            perguntaEditada.Respostas = pergunta.Respostas;

        // public List<TAG> TAGs { get; set; } = new List<TAG>();

            _context.SaveChanges();

            return Ok("Pergunta editada com sucesso");
        }

   
   
    }
}