using HelpTech.Domain.Enumerators;

namespace HelpTech.Domain.DTOs.Ocorrencia.Request
{
    public class OcorrenciaAdicionarRequest
    {
        public string Descricao { get; set; }

        public EnumTipoOcorrencia TipoOcorrencia { get; set; }
    }
}