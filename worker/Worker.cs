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
        string FilaTexto = "ProcessarTexto";

        string complementoTexto = "Texto base or TAGS: ";

        string novoPrompt = @"
""you want me to help you study for the college exam, providing a question based on a text that you send me or tags. You want the question to be multiple choice, with four alternatives,You also want me to put my answer in JSON format, following the model that you indicate. Model The answer should follow the following JSON model
#TEXT IN BRAZILIAN PORTUGUESE#
{
  ""RequisicaoId"": """",
  ""TAGs"": [{""Texto"": ""},
            {""Texto"": ""}],
  ""Conteudo"": """",
  ""Explicacao"": """",
  ""Status"": """",
  ""Respostas"": [
    {
      ""PerguntaId"": """",
      ""Conteudo"": """",
      ""Correta"": """",
      ""Status"": """",
      ""Erro"":""""
    },
    {
      ""PerguntaId"": """",
      ""Conteudo"": """",
      ""Correta"": """",
      ""Status"": """",
      ""Erro"":""""
    },
    {
      ""PerguntaId"": """",
      ""Conteudo"": """",
      ""Correta"": """",
      ""Status"": """",
      ""Erro"":""""
    },
    {
      ""PerguntaId"": """",
      ""Conteudo"": """",
      ""Correta"": """",
      ""Status"": """",
      ""Erro"":""""
    },
    {
      ""PerguntaId"": """",
      ""Conteudo"": """",
      ""Correta"": """",
      ""Status"": """",
      ""Erro"":""""
    }
  ]
}
##EXAMPLE##
User:
Texto base: Na década de 1530, a Coroa Portuguesa implementou uma política de colonização para a terra recém-descoberta que se organizou por meio da distribuição de capitanias hereditárias a membros da nobreza, porém, esse sistema malogrou, uma vez que somente as capitanias de Pernambuco e São Vicente prosperaram. Em 1548, é criado o Estado do Brasil, com consequente instalação de um governo-geral e, no ano seguinte, é fundada a primeira sede colonial, Salvador. 
Or
User: Tags: Historia do Brasil, Colonização Brasil
Assistant: 
```
{
  ""RequisicaoId"": ""c75d06a8-a705-48ec-b6b3-9076becf20f4"",
  ""TAGs"": [
    {""Texto"": ""História""},
    {""Texto"": ""História do Brasil""}
  ],
  ""Conteudo"": ""Qual foi o principal fator que levou à transferência da sede do Estado do Brasil para o Rio de Janeiro em 1763?"",
  ""Explicacao"": ""A transferência da capital da colônia portuguesa de Salvador para o Rio de Janeiro foi estratégica para proteger as riquezas minerais e garantir uma melhor defesa do território."",
  ""Status"": ""OK"",
  ""Respostas"": [
    {
      ""PerguntaId"": ""c75d06a8-a705-48ec-b6b3-9076becf20f4"",
      ""Conteudo"": ""A necessidade de uma melhor defesa contra os ataques dos franceses e dos holandeses."",
      ""Correta"": false,
      ""Status"": ""OK"",
      ""Erro"": ""A mudança foi mais influenciada pelas questões econômicas relacionadas à mineração do que por ameaças militares específicas.""
    },
    {
      ""PerguntaId"": ""c75d06a8-a705-48ec-b6b3-9076becf20f4"",
      ""Conteudo"": ""A proximidade com as minas de ouro e diamantes descobertas no interior da colônia."",
      ""Correta"": true,
      ""Status"": ""OK"",
      ""Erro"": ""Essa é a questao Correta""
    },
    {
      ""PerguntaId"": ""c75d06a8-a705-48ec-b6b3-9076becf20f4"",
      ""Conteudo"": ""A influência dos jesuítas na administração colonial e na catequização dos índios."",
      ""Correta"": false,
      ""Status"": ""OK"",
      ""Erro"": ""A influência dos jesuítas estava mais atrelada às questões de educação e evangelização do que às decisões administrativas de tal magnitude.""
    },
    {
      ""PerguntaId"": ""c75d06a8-a705-48ec-b6b3-9076becf20f4"",
      ""Conteudo"": ""A expansão da cultura da cana-de-açúcar e do comércio triangular com a África e a Europa."",
      ""Correta"": false,
      ""Status"": ""OK"",
      ""Erro"": ""Embora a cana-de-açúcar fosse importante para a economia colonial, este não foi o fator principal para a transferência da sede do governo.""
    }
  ]
}
```Orientações
The question must be related to the base text that you send me. The explanation should be brief and clear, using facts from the base texto or tags. The alternatives must be plausible, but only one must be correct. I must put everything in JSON format to facilitate copying and inserting into your system. Generate Tags
###
##EXPLAIN THE ANSER CORRECT AND MAKES EXPLICATIONS ABOUT ERROR CONTAINER IN THE OUTERS ANSERS

##TEXT IN BRAZILIAN PORTUGUESE##
###
####YOU REMEMBER THE MODEL? SINCE YOU UNDERSTOOD, MAKE THE QUESTION BASED ON THE TEXT BASE or tags, I TRUST THAT YOU UNDERSTOOD, YOU CAN MAKE THE QUESTION FOLLOWING THE MODEL, NOT TEXT BASE IN ANWSER , in the ID need put GUID format, Generate Tags, APENAS RESPONDA EM JSON#### ATENTION PERGUNTA HAVE A EXPLICACAO PROPRIEDADE E RESPOSTAS HAVE A ERRO PROPRIEDADE#### ###POR FAVOR QUANDO A RESPOSTA ESTIVER CORRETA PREENCHA A PROPRIEDADE ERRO COM 'ESTA CORRETA' E QUANDO A RESPOSTA ESTIVER ERRADA PREENCHA A PROPRIEDADE ERRO COM O MOTIVO DA RESPOSTA ESTAR ERRADA###

##ATENÇÂO, QUANDO FOR CRIAR O MOTIVO DA RESPOSTA ESTAR ERRADA, CRIE UM MOTIVO QUE SEJA PLAUSIVEL, EXPLIQUE PORQUE ELA NAO RESPONDE A PERGUNTA, SEJA CLARO E EXPLIQUE BEM 
"""
;
      _mensageiro.Consumir(FilaTexto, _requestServices.ProcessaRequisicaoGPT2(novoPrompt, complementoTexto));
            
    }
}