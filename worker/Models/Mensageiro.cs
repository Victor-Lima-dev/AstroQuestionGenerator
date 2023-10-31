using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RabbitMQ.Client;
using worker.context;
using RabbitMQ.Client.Events;

namespace worker.Models
{
    public class Mensageiro
    {

        private readonly string _rabbitMqUrl;
        private readonly ConnectionFactory _factory;
        public Mensageiro(string rabbitMqUrl)
        {
            _rabbitMqUrl = rabbitMqUrl;
            _factory = new ConnectionFactory { Uri = new Uri(_rabbitMqUrl) };
        }
        public void Consumir(string _queueName, EventHandler<BasicDeliverEventArgs> action)
        {
    
           var connection = _factory.CreateConnection();


             var channel = connection.CreateModel();


            channel.QueueDeclare(queue: _queueName,
                                 durable: false,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);


            var consumer = new EventingBasicConsumer(channel);

            consumer.Received += action; 
            
    

            channel.BasicConsume(queue: _queueName,
                                    autoAck: true,
                                    consumer: consumer);
        }

    }
}