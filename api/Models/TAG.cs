using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.Models
{
    public class TAG : BaseModel
    {
        public string Texto { get; set; }

        public List<Pergunta> Perguntas { get; set; } = new List<Pergunta>();


        public static string ConcatenarTags(List<TAG> tags)
        {
            var tagsNormalizadas = NormalizarTAG(tags);

            var textos = tagsNormalizadas.Select(x => x.Texto).ToList();

            var texto = string.Join(",", textos);

            return texto;
        }

        public static List<TAG> NormalizarTAG(List<TAG> tags)
        {
            var tagsNormalizadas = new List<TAG>();

            foreach (var tag in tags)
            {
                var tagNormalizada = new TAG
                {
                    Texto = tag.Texto.ToLower().Trim()
                };

                tagsNormalizadas.Add(tagNormalizada);
            }

            return tagsNormalizadas;
        }

    }
}