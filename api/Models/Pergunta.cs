using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.Models
{
    public class Pergunta : BaseModel
    {
        public Guid RequisicaoId { get; set; }

        public string Conteudo { get; set; }

        public bool Valided { get; set; } = false;

        public List<Resposta> Respostas { get; set; } = new List<Resposta>();

        public List<TAG> TAGs { get; set; } = new List<TAG>();

        public string Explicacao { get; set; } = "";

        public string Erro { get; set; } = "";



        public static string VerificarPergunta(Pergunta pergunta)
        {
            //verificar se todos os campos foram preenchidos

            if (string.IsNullOrEmpty(pergunta.Conteudo))
            {
                return "Não é possível salvar sem que a pergunta tenha um enunciado";
            }

            if (pergunta.Respostas.Count < 2)
            {
                return "Não é possível salvar uma questão com menos de duas respostas";
            }

            if (pergunta.TAGs.Count < 1)
            {
                return "Não é possível salvar uma pergunta sem tags";
            }

            foreach (var resposta in pergunta.Respostas)
            {
                if (string.IsNullOrEmpty(resposta.Conteudo))
                {
                    return "Não é possível salvar uma resposta sem conteúdo";
                }
            }

            var contagemTrue = 0;
            foreach (var resposta in pergunta.Respostas)
            {
                if (resposta.Correta)
                {
                    contagemTrue++;
                }
            }

            if (contagemTrue < 1)
            {
                return "Não é possível salvar uma pergunta sem pelo menos uma resposta correta";
            }

            //só pode ter uma resposta
            if (contagemTrue > 1)
            {
                return "Não é possível salvar uma pergunta com mais de uma resposta correta";
            }

            return string.Empty;
        }

    }
}