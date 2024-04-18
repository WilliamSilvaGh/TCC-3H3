using HelpTech.Domain.Enumerators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace HelpTech.Domain.DTOs.Ocorrencia.Response
{
    public class OcorrenciaObterResponse
    {
        public Guid Id { get; set; }
        public Guid UsuarioId { get; set; }

        //[JsonConverter(typeof(JsonStringEnumConverter))]
        public EnumTipoOcorrencia TipoOcorrencia { get; set; }
        public DateTime DataHora { get; set; }
        public string Descricao { get; set; }
        public string DescricaoResolucao { get; set; }

        //[JsonConverter(typeof(JsonStringEnumConverter))]
        public EnumStatus Status { get; set; }
    }
}
