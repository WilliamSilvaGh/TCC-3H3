﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelpTech.Domain.DTOs.Ocorrencia.Request
{
    public class OcorrenciaAtualizarRequest
    {
        public Guid Id { get; set; }
        public string Descricao { get; set; }
        public string TipoOcorrencia { get; set; }
    }
}