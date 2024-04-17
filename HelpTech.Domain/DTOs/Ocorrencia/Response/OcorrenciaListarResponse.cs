using HelpTech.Domain.Enumerators;

namespace HelpTech.Domain.DTOs.Ocorrencia.Response
{
    public class OcorrenciaListarResponse
    {
        public Guid Id { get; set; }
        public string UsuarioNome { get; set; }
        public EnumTipoOcorrencia TipoOcorrencia { get; set; }
        public DateTime DataHora { get; set; }
        public string Descricao { get; set; }
        public EnumStatus Status {  get; set; }
    }
}
