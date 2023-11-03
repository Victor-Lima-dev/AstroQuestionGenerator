using Azure;
using Azure.AI.OpenAI;

namespace mensageiroConsumir.Services
{
    public class GPT
    {
        public GPT()
        {            
        }


     public async static Task<string> GerarGPTAsync(string entrada)

        {
            string key = "5ab8e3af2fd14070af933c3bc360b07a";

            OpenAIClient client = new(new Uri("https://codexteste1.openai.azure.com/"), new AzureKeyCredential(key));

            var pergunta = entrada;

            var resposta = " ";


            var chatCompletionsOptions = new ChatCompletionsOptions()
            {
                Messages =
    {
        new ChatMessage(ChatRole.System, @"
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
{ “RequisicaoId”: “ “c75d06a8-a705-48ec-b6b3-9076becf20f4” ”, “Conteudo”: “Qual foi o principal fator que levou à transferência da sede do Estado do Brasil para o Rio de Janeiro em 1763?”, “Status”: “OK”, “Respostas”: [ { “PerguntaId”: “ “c75d06a8-a705-48ec-b6b3-9076becf20f4” ”, “Conteudo”: “A necessidade de uma melhor defesa contra os ataques dos franceses e dos holandeses.”, “Correta”: False, “Status”: “OK” }, { “PerguntaId”:  “c75d06a8-a705-48ec-b6b3-9076becf20f4” , “Conteudo”: “A proximidade com as minas de ouro e diamantes descobertas no interior da colônia.”, “Correta”: True, “Status”: “OK” }, { “PerguntaId”: “ “c75d06a8-a705-48ec-b6b3-9076becf20f4” ”, “Conteudo”: “A influência dos jesuítas na administração colonial e na catequização dos índios.”, “Correta”: False, “Status”: “OK” }, { “PerguntaId”: “ “c75d06a8-a705-48ec-b6b3-9076becf20f4” ”, “Conteudo”: “A expansão da cultura da cana-de-açúcar e do comércio triangular com a África e a Europa.”, “Correta”: False, “Status”: “OK” } ] }


Orientações
The question must be related to the base text that you send me. The explanation should be brief and clear, using facts from the base text. The alternatives must be plausible, but only one must be correct. I must put everything in JSON format to facilitate copying and inserting into your system

The question must be related to the base text that you send me. The explanation should be brief and clear, using facts from the base text. The alternatives must be plausible, but only one must be correct. I must put everything in JSON format to facilitate copying and inserting into your system.
you want me to help you study for the college exam, providing a question based on a text that you send me. You want the question to be multiple choice, with four alternatives, . You also want me to put my answer in JSON format, following the model that you indicate. Model The answer should follow the following JSON model:
###
##TEXT IN BRAZILIAN PORTUGUESE##
###
####YOU REMEMBER THE MODEL? SINCE YOU UNDERSTOOD, MAKE THE QUESTION BASED ON THE TEXT BASE, I TRUST THAT YOU UNDERSTOOD, YOU CAN MAKE THE QUESTION FOLLOWING THE MODEL, NOT TEXT BASE IN ANWSER , in the ID need put GUID format #### 
"""),
        new ChatMessage(ChatRole.Assistant, "Sim, irei enviar a resposta em formato JSON, Seguindo o modelo que você indicar. Modelo A resposta deve seguir o seguinte modelo JSON:"),
        new ChatMessage(ChatRole.User, "Texto Base: " + pergunta),
    },
                MaxTokens = 1000
            };

            Response<StreamingChatCompletions> response = await client.GetChatCompletionsStreamingAsync(
                deploymentOrModelName: "TesteCodex",
                chatCompletionsOptions);
            using StreamingChatCompletions streamingChatCompletions = response.Value;



            await foreach (StreamingChatChoice choice in streamingChatCompletions.GetChoicesStreaming())
            {
                await foreach (ChatMessage message in choice.GetMessageStreaming())
                {
                    //Console.Write(message.Content);

                    //concatenar a resposta do assistente com a pergunta do usuário
                    resposta = resposta + message.Content;
                }

            }
            return resposta;
        }


    }
}