using HelpTech.Domain.Enumerators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelpTech.Domain.DTOs.Ocorrencia.Response
{
    public class OcorrenciaObterResponse
    {
        public Guid Id { get; set; }
        public Guid UsuarioId { get; set; }
        public EnumTipoOcorrencia TipoOcorrencia { get; set; }
        public DateTime DataHora { get; set; }
        public string Descricao { get; set; }
        public Enum Status { get; set; }
    }
}
