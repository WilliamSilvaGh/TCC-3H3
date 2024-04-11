using HelpTech.Domain.Entities.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelpTech.Domain.Entities
{
    public class Ocorrencia : EntityBase
    {
        public string Descricao { get; set; }

        public string TipoOcorrencia { get; set; }

        protected Ocorrencia() {}

        public Ocorrencia( string descricao, string tipoOcorrencia)
        {
            Id = Guid.NewGuid();
            Descricao = descricao;
            TipoOcorrencia = tipoOcorrencia;

        }

        public void Atualizar(string descricao, string tipoOcorrencia)
        {
            Descricao = descricao;
            TipoOcorrencia = tipoOcorrencia;
        }
    }
}
