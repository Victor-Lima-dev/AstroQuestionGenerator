using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.context;
using api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace api.Controllers
{
    [ApiController]

    [Authorize]
    
    [Route("api/[controller]")]
    public class Listas : ControllerBase
    {


        private readonly AppDbContext _context;

        public Listas(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost("ReceberLista")]
        public ActionResult ReceberLista([FromForm] List<string> lista, [FromForm] string nome, [FromForm] string descricao)
        {
            var listaSeparada = lista[0].Split(",").ToList();
           
           //a lista de strings contem uma lista de ids das perguntas, preciso pegar esses ids e buscar as perguntas no banco de dados
           // converter o id para GUID e buscar no banco de dados

         var perguntasRecebidas = new List<Pergunta>();     

            foreach (var item in listaSeparada)
            {
                //a string esta vindo entre colchetes [] e aspas "", preciso remover para depois converter para GUID

                var id = item.Replace("[", "").Replace("]", "").Replace("\"", "");

                var idConvertido = Guid.Parse(id);

                var pergunta = _context.Perguntas.Where(x => x.Id == idConvertido).FirstOrDefault();

                //verificar se a pergunta existe

                if(pergunta == null)
                {
                    return BadRequest("O id da pergunta não existe");
                }

                perguntasRecebidas.Add(pergunta);
        
            }

        var listaRecebida = new Lista{
            Nome = nome,
            Descricao = descricao,
            Perguntas = perguntasRecebidas
        };
        
        _context.Listas.Add(listaRecebida);
        _context.SaveChanges();


            return Ok("Lista criada com sucesso" + listaRecebida.Id);
        }

        [HttpGet("RetornarTodasListas")]
        public ActionResult RetornarTodasListas()
        {
            var listas = _context.Listas
                .Include(x => x.Perguntas)
                    .ThenInclude(p => p.TAGs)
                .Include(x => x.Perguntas)
                    .ThenInclude(p => p.Respostas) // Include the responses
                .ToList();

            return Ok(listas);
        }

        [HttpGet("RetornarLista/{id}")]
        public ActionResult RetornarLista(Guid id)
        {
            var lista = _context.Listas.Where(x => x.Id == id).Include(x => x.Perguntas).FirstOrDefault();
            return Ok(lista);
        }


        [HttpGet("ProcurarLista")]
        public ActionResult ProcurarLista(string nome)
        {
            var lista = _context.Listas
                .Where(x => x.Nome.Contains(nome))
                .Include(x => x.Perguntas)
                    .ThenInclude(p => p.TAGs)
                .Include(x => x.Perguntas)
                    .ThenInclude(p => p.Respostas)
                .ToList();

            return Ok(lista);
        }

        [HttpDelete("DeletarLista")]
        public ActionResult DeletarLista(Guid id)
        {
            var lista = _context.Listas.Include(x => x.Perguntas).FirstOrDefault(x => x.Id == id);

            if (lista == null)
            {
                return BadRequest("Lista não encontrada");
            }

            // Remover as perguntas da lista sem remover as perguntas do banco de dados
            lista.Perguntas.Clear();

            _context.Listas.Remove(lista);
            _context.SaveChanges();

            return Ok("Lista deletada com sucesso");
        }

        [HttpPut("AtualizarPerguntasDaLista")]
        public ActionResult AtualizarPerguntasDaLista([FromForm] string id, [FromForm] List<string> lista)
        {
            var listaSeparada = lista[0].Split(",").ToList();

        

            var idC = id.Replace("[", "").Replace("]", "").Replace("\"", "");

            var novoId = Guid.Parse(idC);

            //converter o id para GUID e buscar no banco de dados

            var listaAtualizada = _context.Listas.Include(x => x.Perguntas).FirstOrDefault(x => x.Id == novoId);

        

            if (listaAtualizada == null)
            {
                return BadRequest("Lista não encontrada");
            }

            // Remover as perguntas da lista sem remover as perguntas do banco de dados
            listaAtualizada.Perguntas.Clear();

            foreach (var item in listaSeparada)
            {
                Console.WriteLine(item);
                //a string esta vindo entre colchetes [] e aspas "", preciso remover para depois converter para GUID

                var itemPergunta = item.Replace("[", "").Replace("]", "").Replace("\"", "");

                var idConvertido = Guid.Parse(itemPergunta);

                var pergunta = _context.Perguntas.Where(x => x.Id == idConvertido).FirstOrDefault();

                //verificar se a pergunta existe

                if (pergunta == null)
                {
                    return BadRequest("O id da pergunta não existe");
                }

                listaAtualizada.Perguntas.Add(pergunta);

            }

            _context.Listas.Update(listaAtualizada);
            _context.SaveChanges();

            return Ok("Lista atualizada com sucesso");
        }

    }
}