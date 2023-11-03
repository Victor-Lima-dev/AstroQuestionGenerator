using api.context;
using api.Models;
using api.Models.Enums;
using mensageiroConsumir.Services;
using Microsoft.EntityFrameworkCore;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;




string _rabbitMqUrl = "amqps://peelqnnc:gU-p0eAigyVNJNfNPanQHz4onYx-Oe7u@jackal.rmq.cloudamqp.com/peelqnnc";

var url = "amqps://peelqnnc:gU-p0eAigyVNJNfNPanQHz4onYx-Oe7u@jackal.rmq.cloudamqp.com/peelqnnc";

//inicio

string _queueName = "PENDENTE";

var factory = new ConnectionFactory { Uri = new Uri(_rabbitMqUrl) };


using var connection = factory.CreateConnection();


using var channel = connection.CreateModel();


channel.QueueDeclare(queue: _queueName,
                     durable: false,
                     exclusive: false,
                     autoDelete: false,
                     arguments: null);


var consumer = new EventingBasicConsumer(channel);

consumer.Received += async (model, ea) =>
   {
      var body = ea.Body.ToArray();

      var message = Encoding.UTF8.GetString(body);

      Console.WriteLine(" [x] Recebido (PENDENTE) {0}", message);


                   //codigo repetido - refatorar
      var id = Guid.Parse(message);

      var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();

      string mySqlConnectionStr = "Server=206.189.180.236;Port=3306;Database=PerguntasRespostas;Uid=root;Pwd=mauFJcuf5dhRMQrjj-vitinh0l0l;Charset=utf8;";

      optionsBuilder.UseMySql(mySqlConnectionStr, ServerVersion.AutoDetect(mySqlConnectionStr));

      using var _context = new AppDbContext(optionsBuilder.Options);

      var requisicao = await _context.Requisicoes.FirstOrDefaultAsync(x => x.Id == id);
                  //codigo repetido - refatorar


      if (requisicao == null)
      {
         Console.WriteLine(" [x] Requisicao não encontrada {0}", message);
         return;
      }

      requisicao.Status = StatusRequisicao.AguardandoProcessamento;

      await _context.SaveChangesAsync();

      Console.WriteLine(" [x] Alterado para (AGUARDANDO PROCESSAMENTO) {0}", message);

     

      var queueName = "AGUARDANDOPROCESSAMENTO";

      var mensageiro = new Mensageiro(url, queueName, _context);

      mensageiro.Publicar(requisicao.Id.ToString());

   };

channel.BasicConsume(queue: _queueName,
                        autoAck: true,
                        consumer: consumer);

//fim

//Inicio 2

_queueName = "AGUARDANDOPROCESSAMENTO";

var factory2 = new ConnectionFactory { Uri = new Uri(_rabbitMqUrl) };


using var connection2 = factory2.CreateConnection();


using var channel2 = connection2.CreateModel();


channel.QueueDeclare(queue: _queueName,
                     durable: false,
                     exclusive: false,
                     autoDelete: false,
                     arguments: null);


var consumer2 = new EventingBasicConsumer(channel2);

consumer2.Received += async (model, ea) =>
   {
      var body = ea.Body.ToArray();
      var message = Encoding.UTF8.GetString(body);

      Console.WriteLine(" [x] Recebido (AGUARDANDOPROCESSAMENTO) {0}", message);


                     //codigo repetido - refatorar
      var id = Guid.Parse(message);

      var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();

      string mySqlConnectionStr = "Server=206.189.180.236;Port=3306;Database=PerguntasRespostas;Uid=root;Pwd=mauFJcuf5dhRMQrjj-vitinh0l0l;Charset=utf8;";

      optionsBuilder.UseMySql(mySqlConnectionStr, ServerVersion.AutoDetect(mySqlConnectionStr));

      using var _context = new AppDbContext(optionsBuilder.Options);

      var requisicao = await _context.Requisicoes.FirstOrDefaultAsync(x => x.Id == id);
                     //fim codigo repetido

      
     try
     { 
      if (requisicao == null)
      {
         Console.WriteLine(" [x] Requisicao não encontrada {0}", message);
         return;
      }
      requisicao.Status = StatusRequisicao.Processando;
      await _context.SaveChangesAsync();
      Console.WriteLine(" [x] Alterado para (Processando) {0}", message);

      string queueName = "PROCESSANDO";
      var mensageiro = new Mensageiro(url, queueName, _context);
      mensageiro.Publicar(requisicao.Id.ToString());
      }
      catch(Exception)
      {
         Console.WriteLine(" [x] Erro ao alterar para (Processando) {0}", message);
         requisicao.Status = StatusRequisicao.FalhaProcessamento;
         await _context.SaveChangesAsync();
         return;
      }
   };

channel2.BasicConsume(queue: _queueName,
                        autoAck: true,
                        consumer: consumer2);


//fila 3

//Inicio 2

_queueName = "PROCESSANDO";

var factory3 = new ConnectionFactory { Uri = new Uri(_rabbitMqUrl) };

// Cria uma conexão com o servidor usando a fábrica
using var connection3 = factory3.CreateConnection();

// Cria um canal de comunicação dentro da conexão
using var channel3 = connection3.CreateModel();

// Declara uma fila no servidor, se ela não existir
channel.QueueDeclare(queue: _queueName,
                     durable: false,
                     exclusive: false,
                     autoDelete: false,
                     arguments: null);

// Cria um consumidor de eventos para receber as mensagens
var consumer3 = new EventingBasicConsumer(channel3);

consumer.Received += async (model, ea) =>
   {
      var body = ea.Body.ToArray();
      var message = Encoding.UTF8.GetString(body);

      Console.WriteLine(" [x] Recebido (PROCESSANDO) {0}", message);

      var id = Guid.Parse(message);

      var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();

      string mySqlConnectionStr = "Server=206.189.180.236;Port=3306;Database=PerguntasRespostas;Uid=root;Pwd=mauFJcuf5dhRMQrjj-vitinh0l0l;Charset=utf8;";

      optionsBuilder.UseMySql(mySqlConnectionStr, ServerVersion.AutoDetect(mySqlConnectionStr));

      using var _context = new AppDbContext(optionsBuilder.Options);

      var requisicao = await _context.Requisicoes.FirstOrDefaultAsync(x => x.Id == id);

      if (requisicao == null)
      {
         Console.WriteLine(" [x] Requisicao não encontrada {0}", message);
         return;
      }

      var textoBase = await _context.TextosBase.FirstOrDefaultAsync(x => x.RequisicaoId == requisicao.Id);
      
      if (textoBase == null)
      {
         Console.WriteLine(" [x] Texto base não encontrado {0}", message);
         return;
      }

      
      var texto = textoBase.Texto;

      requisicao.Status = StatusRequisicao.AguardandoPerguntasRespostas;

      var pergunta = await GPT.GerarGPTAsync(texto);


      await _context.SaveChangesAsync();

      try
      {
         var perguntaDeserializada = JsonSerializer.Deserialize<Pergunta>(pergunta);

         Console.WriteLine(" [x] Pergunta deserializada {0}", message);

         if (perguntaDeserializada == null)
         {
            Console.WriteLine(" [x] Pergunta não deserializada {0}", message);
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
         Console.WriteLine(" [x] Erro ao deserializar pergunta {0}", message);
         requisicao.Status = StatusRequisicao.FalhaPerguntasRespostas;
         requisicao.DataFim = DateTime.Now;
         await _context.SaveChangesAsync();
         return;
      }



   };

channel3.BasicConsume(queue: _queueName,
                        autoAck: true,
                        consumer: consumer3);


Console.WriteLine(" Pressione [enter] para sair.");
Console.ReadLine();

