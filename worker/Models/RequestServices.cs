using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using worker.context;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using Microsoft.EntityFrameworkCore;
using worker.Models.Enums;
using System.Text.Json;

namespace worker.Models
{
    public class RequestServices
    {
        private readonly IServiceProvider _serviceProvider;

        public RequestServices(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public EventHandler<BasicDeliverEventArgs> ProcessaTextoBaseGPT()
        {
            return async (model, ea) =>
            {
                using var scope = _serviceProvider.CreateScope();
                var services = scope.ServiceProvider;
                using var _context =
                    services.GetService<AppDbContext>()
                    ?? throw new Exception();

                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                var id = Guid.Parse(message);

                var requisicao = await _context.Requisicoes.FirstOrDefaultAsync(x => x.Id == id);

                if (requisicao == null)
                {
                    Console.WriteLine("Requisição não encontrada");
                    return;
                }

                requisicao.Status = StatusRequisicao.AguardandoProcessamento;

                await _context.SaveChangesAsync();

                var textoBase = await _context.TextosBase.FirstOrDefaultAsync(x => x.RequisicaoId == requisicao.Id);

                if (textoBase == null)
                {
                    Console.WriteLine("Texto base não encontrado");

                    requisicao.Status = StatusRequisicao.FalhaGenerica;
                    requisicao.DataFim = DateTime.Now;
                    await _context.SaveChangesAsync();

                    return;
                }

                var texto = textoBase.Texto;

                //aqui vai ter um metodo de validação do texto base

                requisicao.Status = StatusRequisicao.Processando;

                await _context.SaveChangesAsync();

                requisicao.Status = StatusRequisicao.AguardandoPerguntasRespostas;

                await _context.SaveChangesAsync();

                var pergunta = await GPT.GerarGPTAsync(texto);

                try
                {
                    var perguntaDeserializada = JsonSerializer.Deserialize<Pergunta>(pergunta);

                    if (perguntaDeserializada == null)
                    {
                        requisicao.Status = StatusRequisicao.FalhaPerguntasRespostas;
                        requisicao.DataFim = DateTime.Now;
                        await _context.SaveChangesAsync();

                        return;
                    }

                    perguntaDeserializada.Id = Guid.NewGuid();
                    perguntaDeserializada.RequisicaoId = requisicao.Id;

                    foreach (var resposta in perguntaDeserializada.Respostas)
                    {
                        resposta.PerguntaId = perguntaDeserializada.Id;
                        resposta.Id = Guid.NewGuid();
                    }

                    _context.Perguntas.Add(perguntaDeserializada);
                    requisicao.Status = StatusRequisicao.Pronto;
                    requisicao.DataFim = DateTime.Now;
                    await _context.SaveChangesAsync();

                }
                catch (Exception)
                {
                    requisicao.Status = StatusRequisicao.FalhaGenerica;
                    requisicao.DataFim = DateTime.Now;
                    await _context.SaveChangesAsync();
                    return;
                }

            };
        }
        public EventHandler<BasicDeliverEventArgs> ProcessaRequisicaoGPT(Func<string, Task<string>> func)
        {
            return async (model, ea) =>
            {
                using var scope = _serviceProvider.CreateScope();
                var services = scope.ServiceProvider;
                using var _context =
                    services.GetService<AppDbContext>()
                    ?? throw new Exception();

                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                var id = Guid.Parse(message);

                var requisicao = await _context.Requisicoes.FirstOrDefaultAsync(x => x.Id == id);

                if (requisicao == null)
                {
                    Console.WriteLine("Requisição não encontrada");
                    return;
                }

                requisicao.Status = StatusRequisicao.AguardandoProcessamento;

                await _context.SaveChangesAsync();

                var textoBase = await _context.TextosBase.FirstOrDefaultAsync(x => x.RequisicaoId == requisicao.Id);

                if (textoBase == null)
                {
                    Console.WriteLine("Texto base não encontrado");

                    requisicao.Status = StatusRequisicao.FalhaGenerica;
                    requisicao.DataFim = DateTime.Now;
                    await _context.SaveChangesAsync();

                    return;
                }

                var texto = textoBase.Texto;

                //aqui vai ter um metodo de validação do texto base

                requisicao.Status = StatusRequisicao.Processando;

                await _context.SaveChangesAsync();

                requisicao.Status = StatusRequisicao.AguardandoPerguntasRespostas;

                await _context.SaveChangesAsync();


                var pergunta = await func(texto);

                try
                {
                    var perguntaDeserializada = JsonSerializer.Deserialize<Pergunta>(pergunta);

                    if (perguntaDeserializada == null)
                    {
                        requisicao.Status = StatusRequisicao.FalhaPerguntasRespostas;
                        requisicao.DataFim = DateTime.Now;
                        await _context.SaveChangesAsync();

                        return;
                    }

                    perguntaDeserializada.Id = Guid.NewGuid();
                    perguntaDeserializada.RequisicaoId = requisicao.Id;

                    foreach (var resposta in perguntaDeserializada.Respostas)
                    {
                        resposta.PerguntaId = perguntaDeserializada.Id;
                        resposta.Id = Guid.NewGuid();
                    }

                    _context.Perguntas.Add(perguntaDeserializada);
                    requisicao.Status = StatusRequisicao.Pronto;
                    requisicao.DataFim = DateTime.Now;
                    await _context.SaveChangesAsync();

                }
                catch (Exception)
                {
                    requisicao.Status = StatusRequisicao.FalhaGenerica;
                    requisicao.DataFim = DateTime.Now;
                    await _context.SaveChangesAsync();
                    return;
                }

            };
        }


    }
}