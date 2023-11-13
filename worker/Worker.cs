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

        // _mensageiro.Consumir("ProcessarTexto", _requestServices.ProcessaRequisicaoGPT(GPT.GerarGPTAsync));

        //_mensageiro.Consumir("ProcessarTAG", _requestServices.ProcessaRequisicaoGPT(GPT.GerarPerguntaTAGAsync));


        string FilaTAG = "ProcessarTAG";

        string GerarPerguntaTAG =     @"
  ""you want me to help you study for the college exam, providing a question based on a TAGS that you send me. You want the question to be multiple choice, with four alternatives,You also want me to put my answer in JSON format, following the model that you indicate. Model The answer should follow the following JSON model
DON'T ADD ANY EXTRA INFORMATION OUTSIDE OF THE JSON, THE ANSWER SHOULD JUST BE THE JSON
#TEXT IN BRAZILIAN PORTUGUESE#
{
  {
  ""RequisicaoId"": """",
  ""Conteudo"": """",
  ""Status"": """",
  ""Respostas"": [
    {
      ""PerguntaId"": """",
      ""Conteudo"": """",
      ""Correta"": """",
      ""Status"": """"
    },
    {
      ""PerguntaId"": """",
      ""Conteudo"": """",
      ""Correta"": """",
      ""Status"": """"
    },
    {
      ""PerguntaId"": """",
      ""Conteudo"": """",
      ""Correta"": """",
      ""Status"": """"
    },
    {
      ""PerguntaId"": """",
      ""Conteudo"": """",
      ""Correta"": """",
      ""Status"": """"
    },
    {
      ""PerguntaId"": """",
      ""Conteudo"": """",
      ""Correta"": """",
      ""Status"": """"
    }
  ]
}
##EXAMPLE##
User:
TAGS: HISTORIADOBRASIL, HISTORIA, PERIODOCOLONIALBRASIL
Assistant: 
{ ""RequisicaoId"": ""c75d06a8-a705-48ec-b6b3-9076becf20f4"", ""Conteudo"": ""Qual foi o principal fator que levou à transferência da sede do Estado do Brasil para o Rio de Janeiro em 1763?"", ""Status"": ""OK"", ""Respostas"": [ { ""PerguntaId"": ""c75d06a8-a705-48ec-b6b3-9076becf20f4"", ""Conteudo"": ""A necessidade de uma melhor defesa contra os ataques dos franceses e dos holandeses."", ""Correta"": false, ""Status"": ""OK"" }, { ""PerguntaId"":  ""c75d06a8-a705-48ec-b6b3-9076becf20f4"" , ""Conteudo"": ""A proximidade com as minas de ouro e diamantes descobertas no interior da colônia."", ""Correta"": true, ""Status"": ""OK"" }, { ""PerguntaId"": ""c75d06a8-a705-48ec-b6b3-9076becf20f4"", ""Conteudo"": ""A influência dos jesuítas na administração colonial e na catequização dos índios."", ""Correta"": false, ""Status"": ""OK"" }, { ""PerguntaId"": ""c75d06a8-a705-48ec-b6b3-9076becf20f4"", ""Conteudo"": ""A expansão da cultura da cana-de-açúcar e do comércio triangular com a África e a Europa."", ""Correta"": false, ""Status"": ""OK"" } ] }


Orientações
The question must be related to the base TAGS that you send me. TAGS. The alternatives must be plausible, but only one must be correct. I must put everything in JSON format to facilitate copying and inserting into your system

The question must be related to TAGS text that you send me alternatives must be plausible, but only one must be correct. I must put everything in JSON format to facilitate copying and inserting into your system.
you want me to help you study for the college exam, providing a question based on a TAGS  hat you send me. You want the question to be multiple choice, with four alternatives, . You also want me to put my answer in JSON format, following the model that you indicate. Model The answer should follow the following JSON model:
###
##TEXT IN BRAZILIAN PORTUGUESE##
###
####YOU REMEMBER THE MODEL? SINCE YOU UNDERSTOOD, MAKE THE QUESTION BASED ON THE TAGS, I TRUST THAT YOU UNDERSTOOD, YOU CAN MAKE THE QUESTION FOLLOWING THE MODEL, NOT TEXT BASE IN ANWSER , in the ID need put GUID format DON'T ADD ANY EXTRA INFORMATION OUTSIDE OF THE JSON, THE ANSWER SHOULD JUST BE THE JSON

PLEASE REMEMBER NOT TO ADD ANY REMARKS (OBS)
NOT ADD ANY OBS
#### 
";

        string complementoTAG = "TAGS: ";

        _mensageiro.Consumir(FilaTAG, _requestServices.ProcessaRequisicaoGPT2(GerarPerguntaTAG, complementoTAG));

        string FilaTexto = "ProcessarTexto";

        string GerarPerguntaTexto = @"
""you want me to help you study for the college exam, providing a question based on a text that you send me. You want the question to be multiple choice, with four alternatives,You also want me to put my answer in JSON format, following the model that you indicate. Model The answer should follow the following JSON model
#TEXT IN BRAZILIAN PORTUGUESE#
{
{
  ""RequisicaoId"": """",
  ""Conteudo"": """",
  ""Status"": """",
  ""Respostas"": [
    {
      ""PerguntaId"": """",
      ""Conteudo"": """",
      ""Correta"": """",
      ""Status"": """"
    },
    {
      ""PerguntaId"": """",
      ""Conteudo"": """",
      ""Correta"": """",
      ""Status"": """"
    },
    {
      ""PerguntaId"": """",
      ""Conteudo"": """",
      ""Correta"": """",
      ""Status"": """"
    },
    {
      ""PerguntaId"": """",
      ""Conteudo"": """",
      ""Correta"": """",
      ""Status"": """"
    },
    {
      ""PerguntaId"": """",
      ""Conteudo"": """",
      ""Correta"": """",
      ""Status"": """"
    }
  ]
}
##EXAMPLE##
User:
Texto base: Na década de 1530, a Coroa Portuguesa implementou uma política de colonização para a terra recém-descoberta que se organizou por meio da distribuição de capitanias hereditárias a membros da nobreza, porém, esse sistema malogrou, uma vez que somente as capitanias de Pernambuco e São Vicente prosperaram. Em 1548, é criado o Estado do Brasil, com consequente instalação de um governo-geral e, no ano seguinte, é fundada a primeira sede colonial, Salvador. A economia da colônia, iniciada com o extrativismo do pau-brasil e as trocas entre os colonos e os índios, gradualmente passou a ser dominada pelo cultivo da cana-de-açúcar com o uso de mão de obra escrava, inicialmente indígena e, depois, africana.[3] No fim do século XVII, foram descobertas, através das bandeiras, importantes jazidas de ouro no interior do Brasil que foram determinantes para o seu povoamento e que pontuam o início do chamado ciclo do ouro, período que marca a ascensão da Capitania de Minas Gerais, desmembrada da Capitania de São Paulo e Minas de Ouro, na economia colonial. Em 1763, a sede do Estado do Brasil foi transferida para o Rio de Janeiro.[4]

Assistant: 
{ ""RequisicaoId"": ""c75d06a8-a705-48ec-b6b3-9076becf20f4"", ""Conteudo"": ""Qual foi o principal fator que levou à transferência da sede do Estado do Brasil para o Rio de Janeiro em 1763?"", ""Status"": ""OK"", ""Respostas"": [ { ""PerguntaId"": ""c75d06a8-a705-48ec-b6b3-9076becf20f4"", ""Conteudo"": ""A necessidade de uma melhor defesa contra os ataques dos franceses e dos holandeses."", ""Correta"": false, ""Status"": ""OK"" }, { ""PerguntaId"":  ""c75d06a8-a705-48ec-b6b3-9076becf20f4"" , ""Conteudo"": ""A proximidade com as minas de ouro e diamantes descobertas no interior da colônia."", ""Correta"": true, ""Status"": ""OK"" }, { ""PerguntaId"": ""c75d06a8-a705-48ec-b6b3-9076becf20f4"", ""Conteudo"": ""A influência dos jesuítas na administração colonial e na catequização dos índios."", ""Correta"": false, ""Status"": ""OK"" }, { ""PerguntaId"": ""c75d06a8-a705-48ec-b6b3-9076becf20f4"", ""Conteudo"": ""A expansão da cultura da cana-de-açúcar e do comércio triangular com a África e a Europa."", ""Correta"": false, ""Status"": ""OK"" } ] }


Orientações
The question must be related to the base text that you send me. The explanation should be brief and clear, using facts from the base text. The alternatives must be plausible, but only one must be correct. I must put everything in JSON format to facilitate copying and inserting into your system

The question must be related to the base text that you send me. The explanation should be brief and clear, using facts from the base text. The alternatives must be plausible, but only one must be correct. I must put everything in JSON format to facilitate copying and inserting into your system.
you want me to help you study for the college exam, providing a question based on a text that you send me. You want the question to be multiple choice, with four alternatives, . You also want me to put my answer in JSON format, following the model that you indicate. Model The answer should follow the following JSON model:
###
##TEXT IN BRAZILIAN PORTUGUESE##
###
####YOU REMEMBER THE MODEL? SINCE YOU UNDERSTOOD, MAKE THE QUESTION BASED ON THE TEXT BASE, I TRUST THAT YOU UNDERSTOOD, YOU CAN MAKE THE QUESTION FOLLOWING THE MODEL, NOT TEXT BASE IN ANWSER , in the ID need put GUID format #### 
""";

        string complementoTexto = "Texto base or TAGS: ";

        string novoPrompt = @"
""you want me to help you study for the college exam, providing a question based on a text that you send me or tags. You want the question to be multiple choice, with four alternatives,You also want me to put my answer in JSON format, following the model that you indicate. Model The answer should follow the following JSON model
#TEXT IN BRAZILIAN PORTUGUESE#
{
{
  ""RequisicaoId"": """",
“”TAGs: [{“”Texto””: },
{“”Texto””: }],

  ""Conteudo"": """",
  ""Status"": """",
  ""Respostas"": [
    {
      ""PerguntaId"": """",
      ""Conteudo"": """",
      ""Correta"": """",
      ""Status"": """"
    },
    {
      ""PerguntaId"": """",
      ""Conteudo"": """",
      ""Correta"": """",
      ""Status"": """"
    },
    {
      ""PerguntaId"": """",
      ""Conteudo"": """",
      ""Correta"": """",
      ""Status"": """"
    },
    {
      ""PerguntaId"": """",
      ""Conteudo"": """",
      ""Correta"": """",
      ""Status"": """"
    },
    {
      ""PerguntaId"": """",
      ""Conteudo"": """",
      ""Correta"": """",
      ""Status"": """"
    }
  ]
}
##EXAMPLE##
User:
Texto base: Na década de 1530, a Coroa Portuguesa implementou uma política de colonização para a terra recém-descoberta que se organizou por meio da distribuição de capitanias hereditárias a membros da nobreza, porém, esse sistema malogrou, uma vez que somente as capitanias de Pernambuco e São Vicente prosperaram. Em 1548, é criado o Estado do Brasil, com consequente instalação de um governo-geral e, no ano seguinte, é fundada a primeira sede colonial, Salvador. 
Or
User: Tags: Historia do Brasil, Colonização Brasil
Assistant: 
{ ""RequisicaoId"": ""c75d06a8-a705-48ec-b6b3-9076becf20f4"",
“”TAGs: [{“”Texto””: História},
{“”Texto””: História Brasil}],
 ""Conteudo"": ""Qual foi o principal fator que levou à transferência da sede do Estado do Brasil para o Rio de Janeiro em 1763?"", ""Status"": ""OK"", 
""Respostas"": [ { ""PerguntaId"": ""c75d06a8-a705-48ec-b6b3-9076becf20f4"", ""Conteudo"": ""A necessidade de uma melhor defesa contra os ataques dos franceses e dos holandeses."", ""Correta"": false, ""Status"": ""OK"" }, { ""PerguntaId"":  ""c75d06a8-a705-48ec-b6b3-9076becf20f4"" , ""Conteudo"": ""A proximidade com as minas de ouro e diamantes descobertas no interior da colônia."", ""Correta"": true, ""Status"": ""OK"" }, { ""PerguntaId"": ""c75d06a8-a705-48ec-b6b3-9076becf20f4"", ""Conteudo"": ""A influência dos jesuítas na administração colonial e na catequização dos índios."", ""Correta"": false, ""Status"": ""OK"" }, { ""PerguntaId"": ""c75d06a8-a705-48ec-b6b3-9076becf20f4"", ""Conteudo"": ""A expansão da cultura da cana-de-açúcar e do comércio triangular com a África e a Europa."", ""Correta"": false, ""Status"": ""OK"" } ] }
Orientações
The question must be related to the base text that you send me. The explanation should be brief and clear, using facts from the base texto or tags. The alternatives must be plausible, but only one must be correct. I must put everything in JSON format to facilitate copying and inserting into your system. Generate Tags
###
##TEXT IN BRAZILIAN PORTUGUESE##
###
####YOU REMEMBER THE MODEL? SINCE YOU UNDERSTOOD, MAKE THE QUESTION BASED ON THE TEXT BASE or tags, I TRUST THAT YOU UNDERSTOOD, YOU CAN MAKE THE QUESTION FOLLOWING THE MODEL, NOT TEXT BASE IN ANWSER , in the ID need put GUID format, Generate Tags, NÃO PRECISA DE EXPLICAÇÃO, APENAS RESPONDA EM JSON#### 
"""
;

       // _mensageiro.Consumir(FilaTexto, _requestServices.ProcessaRequisicaoGPT2(GerarPerguntaTexto, complementoTexto));

        _mensageiro.Consumir(FilaTexto, _requestServices.ProcessaRequisicaoGPT2(novoPrompt, complementoTexto));
             
    }
}