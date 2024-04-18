using HelpTech.Domain.Enumerators;
using System.Text.Json.Serialization;

namespace HelpTech.Domain.DTOs.Ocorrencia.Request
{
    public class OcorrenciaAdicionarRequest
    {
        public string Descricao { get; set; }

        //[JsonConverter(typeof(JsonStringEnumConverter))]
        public EnumTipoOcorrencia TipoOcorrencia { get; set; }
    }
}