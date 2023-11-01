using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using api.context;
using RabbitMQ.Client;

namespace api.Models
{
    public class Mensageiro
    {
        // URL do servidor RabbitMQ
        private readonly string _rabbitMqUrl;

        private readonly AppDbContext _context;



        // Nome da fila que será usada
        private readonly string _queueName;

        // Construtor que recebe a URL e o nome da fila como parâmetros
        public Mensageiro(string rabbitMqUrl, string queueName, AppDbContext context)
        {
            _rabbitMqUrl = rabbitMqUrl;
            _queueName = queueName;
            _context = context;
        }

        // Método que publica uma mensagem na fila
        public void Publicar(string mensagem)
        {
            // Cria uma fábrica de conexões com a URL
            var factory = new ConnectionFactory { Uri = new Uri(_rabbitMqUrl) };

            // Cria uma conexão com o servidor usando a fábrica
            using var connection = factory.CreateConnection();

            // Cria um canal de comunicação dentro da conexão
            using var channel = connection.CreateModel();

            // Declara uma fila no servidor, se ela não existir
            channel.QueueDeclare(queue: _queueName,
                                 durable: false,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);

            // Converte a mensagem em um array de bytes
            var body = Encoding.UTF8.GetBytes(mensagem);




            // Publica a mensagem na troca vazia ("") com a chave de roteamento igual ao nome da fila
            channel.BasicPublish(exchange: string.Empty,
                                 routingKey: _queueName,
                                 basicProperties: null,
                                 body: body);
        }

    }
}