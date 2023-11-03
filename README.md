# Astro Question Generator

## Sobre o projeto

Este é um projeto que utiliza a api da open ai para gerar perguntas e respostas sobre temas de estudo. O objetivo é facilitar o aprendizado de conteúdos teóricos por meio de atividades práticas.

## Como funciona

O projeto consiste em uma API que recebe como parâmetro os temas que o usuário quer estudar e retorna um conjunto de perguntas e respostas no formato json. Para gerar as perguntas e respostas, o projeto usa um prompt que foi criado a partir do Chat GPT, um site que permite interagir com o GPT, um modelo de inteligência artificial capaz de gerar textos a partir de informações fornecidas.

## Exemplo de uso

Suponha que o usuário queira estudar sobre a história do Brasil. Ele pode enviar o seguinte parâmetro para a API:

```
 [
    {
        "Texto" : "historia do brasil",
    
    },
    {
        "Texto" : "familia real no brasil"
    }
  ]
  ```
A API irá retornar um json com uma pergunta sobre esses temas, por exemplo:

```
{ 
  ""RequisicaoId"": ""c75d06a8-a705-48ec-b6b3-9076becf20f4"",
 ""Conteudo"": ""Qual foi o principal 
 fator que levou à transferência da sede do Estado do Brasil 
 para o Rio de Janeiro em 1763?"",
  ""Status"": ""OK"", 
  ""Respostas"": [ 

    { ""PerguntaId"": ""c75d06a8-a705-48ec-b6b3-9076becf20f4"",
      ""Conteudo"": ""A necessidade de uma melhor defesa 
                      contra os ataques
                       dos franceses e dos holandeses."",
      ""Correta"": false, 
      ""Status"": ""OK"" },

    { ""PerguntaId"":  ""c75d06a8-a705-48ec-b6b3-9076becf20f4"" ,
      ""Conteudo"": ""A proximidade com as minas de ouro e   
                      diamantes 
                      descobertas no interior da colônia."", 
      ""Correta"": true,
      ""Status"": ""OK"" },

    { ""PerguntaId"": ""c75d06a8-a705-48ec-b6b3-9076becf20f4"",
      ""Conteudo"": ""A influência dos 
                      jesuítas na administração colonial 
                      e na catequização dos índios."",
      ""Correta"": false, 
      ""Status"": ""OK"" },

    { ""PerguntaId"": ""c75d06a8-a705-48ec-b6b3-9076becf20f4"", 
      ""Conteudo"": ""A expansão da cultura da 
                      cana-de-açúcar e do comércio
                      triangular com  a África e a Europa."", 
      ""Correta"": false, 
      ""Status"": ""OK"" } 
        ] 
        }
```
## Documentação da API


#### Inicio do Processo para gerar a Pergunta, usando TAGs

```http
  POST api/RequisicoesTAG/IniciarRequisicaoTAG 

```

| Parâmetro   | Tipo       | Descrição                                   |
| :---------- | :--------- | :------------------------------------------ |
| `TAGs`      | `List<TAGs>` | **Obrigatório** |

#### Exemplo de Formato esperado
```
content-Type: application/json
  [
    {
        "Texto" : "c#"
    },
    {
        "Texto" : "websockets"
    }
  ]

```

##### Retorna o ID da Requisição Criada (formato GUID)
```
Id: "xxx-xxxx-xxxxx-xxxx-xxx"
```

#### Consultar Requisição

```http
  GET api/Requisicoes/ConsultarRequisicao?id={{id}}

```
| Parâmetro   | Tipo       | Descrição                                   |
| :---------- | :--------- | :------------------------------------------ |
| `Id`      | `Guid` | **Obrigatório**: ID da Requisição |

#### Consultar Pergunta

```http
  GET api/Requisicoes/ConsultarPergunta?id={{id}}
```
| Parâmetro   | Tipo       | Descrição                                   |
| :---------- | :--------- | :------------------------------------------ |
| `Id`      | `Guid` | **Obrigatório**: ID da Requisição |

## Stack utilizada

**Back-end:** .NET 7, MySQL, RabbitMQ, ASP.NET

