namespace worker;
using worker.Models;
using worker.context;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;


public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;

    private readonly Mensageiro _mensageiro;
    private readonly IServiceProvider _serviceProvider;

    private readonly RequestServices _requestServices;

    public Worker(ILogger<Worker> logger, Mensageiro mensageiro, RequestServices requestServices)
    {
        _logger = logger;
        _mensageiro = mensageiro;
        _requestServices = requestServices;
    }


    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        //_mensageiro.Consumir("PENDENTE", _requestServices.ProcessaTextoBaseGPT());

        _mensageiro.Consumir("ProcessarTexto", _requestServices.ProcessaRequisicaoGPT(GPT.GerarGPTAsync));

        _mensageiro.Consumir("ProcessarTAG", _requestServices.ProcessaRequisicaoGPT(GPT.GerarPerguntaTAGAsync));
             
    }
}