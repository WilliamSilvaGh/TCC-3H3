using HelpTech.Data.Context;
using HelpTech.CrossCutting;
using System.Text.Json.Serialization;
using HelpTech.Domain.DTOs.Ocorrencia.Response;
using HelpTech.Domain.DTOs.Ocorrencia.Request;
using HelpTech.Domain.Entities;
using HelpTech.Domain.DTOs.Usuario.Request;
using HelpTech.Domain.DTOs.Usuario.Response;
using HelpTech.Domain.Extensions;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.IdentityModel.Tokens.Jwt;

var builder = WebApplication.CreateBuilder(args);
Guid usuarioLogadoId;
bool usuarioLogadoEhAdmin;

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.EnableAnnotations();
});

builder.Services.AddDbContext<HelpTechContext>();
//builder.Services.AddControllers().AddJsonOptions(options => options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

builder.Services.ConfigureAuthentication();
builder.Services.ConfigureAuthenticateSwagger();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthentication();
app.UseAuthorization();
app.UseHttpsRedirection();

#region Endpoints de Ocorrencia
app.MapGet("/ocorrencia/listar", (HelpTechContext context, ClaimsPrincipal user) =>
{
    try
    {
        SetarDadosToken(user);

        if (usuarioLogadoEhAdmin)
        {
            var ocorrenciasAdmin = context.OcorrenciaSet.Select(ocorrencia => new OcorrenciaListarResponse
            {
                Id = ocorrencia.Id,
                UsuarioNome = ocorrencia.Usuario.Nome,
                Descricao = ocorrencia.Descricao,
                DescricaoResolucao = ocorrencia.DescricaoResolucao,
                TipoOcorrencia = ocorrencia.TipoOcorrencia,
                Status = ocorrencia.Status
            });

            return Results.Ok(ocorrenciasAdmin);
        }

        var ocorrencias = context.OcorrenciaSet.Where(p => p.UsuarioId == usuarioLogadoId).Select(ocorrencia => new OcorrenciaListarResponse
        {
            Id = ocorrencia.Id,
            UsuarioNome = ocorrencia.Usuario.Nome,
            Descricao = ocorrencia.Descricao,
            DescricaoResolucao = ocorrencia.DescricaoResolucao,
            TipoOcorrencia = ocorrencia.TipoOcorrencia,
            Status = ocorrencia.Status
        }
        );

        return Results.Ok(ocorrencias);

    }
    catch
    {
        return Results.BadRequest("Ocorreu um problema! Verifique se está logado!");
    }

})
.WithOpenApi(operation =>
{
    operation.Description = "Endpoint para obter todas as ocorrencias cadastradas";
    operation.Summary = "Listar todas as Ocorrencias";
    return operation;
})
.WithTags("Ocorrencias");

////.RequireAuthorization();

app.MapGet("/ocorrencia/{ocorrenciaId}", (HelpTechContext context, Guid ocorrenciaId) =>
{
    var ocorrencia = context.OcorrenciaSet.Find(ocorrenciaId);
    if (ocorrencia is null)
        return Results.BadRequest("Ocorrencia não Localizada.");

    var ocorrenciaDto = new OcorrenciaObterResponse
    {
        Id = ocorrencia.Id,
        UsuarioId = ocorrencia.UsuarioId,
        Descricao = ocorrencia.Descricao,
        DescricaoResolucao = ocorrencia.DescricaoResolucao,
        TipoOcorrencia = ocorrencia.TipoOcorrencia,
        DataHora = ocorrencia.DataHora,
        Status = ocorrencia.Status
    };

    return Results.Ok(ocorrenciaDto);
})
    .WithOpenApi(operation =>
    {
        operation.Description = "Endpoint para obter uma Ocorrencia com base no ID informado";
        operation.Summary = "Obter uma Ocorrencia";
        operation.Parameters[0].Description = "Id da Ocorrencia";
        return operation;
    })
    .WithTags("Ocorrencias");
////.RequireAuthorization();

app.MapPost("/ocorrencia/adicionar", (HelpTechContext context, ClaimsPrincipal user, OcorrenciaAdicionarRequest ocorrenciaAdicionarRequest) =>
{
if (user?.Identity?.IsAuthenticated != true)
        return Results.BadRequest("Logue antes de adicionar uma ocorrência");

    try
    {
        SetarDadosToken(user);


        var ocorrencia = new Ocorrencia(
            ocorrenciaAdicionarRequest.Descricao,
            ocorrenciaAdicionarRequest.TipoOcorrencia,
            usuarioLogadoId);
            
        context.OcorrenciaSet.Add(ocorrencia);
        context.SaveChanges();

        return Results.Created("Created", $"Ocorrencia Registrada com Sucesso. {ocorrencia.Id}");
    }
    catch (Exception ex)
    {
        return Results.BadRequest(ex.InnerException?.Message ?? ex.Message);
    }
})
    .WithOpenApi(operation =>
    {
        operation.Description = "Endpoint para Cadastrar uma Ocorrencia";
        operation.Summary = "Nova Ocorrencia";
        return operation;
    })
    .WithTags("Ocorrencias");
////.RequireAuthorization();

app.MapPut("/ocorrencia/atualizar", (HelpTechContext context, ClaimsPrincipal user, OcorrenciaAtualizarRequest ocorrenciaAtualizarRequest) =>
{
    try
    {
        SetarDadosToken(user);

        var ocorrencia = context.OcorrenciaSet.Find(ocorrenciaAtualizarRequest.Id);
        if (ocorrencia is null)
            return Results.BadRequest("Ocorrencia não Localizada.");

        if (usuarioLogadoEhAdmin)
        {
        ocorrencia.Atualizar(ocorrenciaAtualizarRequest.Descricao, ocorrenciaAtualizarRequest.TipoOcorrencia, ocorrenciaAtualizarRequest.Status);
        context.OcorrenciaSet.Update(ocorrencia);
        context.SaveChanges();
        }

        ocorrencia.Atualizar(ocorrenciaAtualizarRequest.Descricao, ocorrenciaAtualizarRequest.TipoOcorrencia);
        context.OcorrenciaSet.Update(ocorrencia);
        context.SaveChanges();

        return Results.Ok("Ocorrencia Atualizada com Sucesso.");
    }
    catch (Exception ex)
    {
        return Results.BadRequest(ex.InnerException?.Message ?? ex.Message);
    }
})
    .WithOpenApi(operation =>
    {
        operation.Description = "Endpoint para Atualizar uma Ocorrencia";
        operation.Summary = "Atualizar Ocorrencia";
        return operation;
    })
    .WithTags("Ocorrencias");
////.RequireAuthorization();

app.MapPut("/ocorrencia/encerrar", (HelpTechContext context, ClaimsPrincipal user, OcorrenciaEncerrarRequest ocorrenciaEncerrarRequest) =>
{
    try
    {
        SetarDadosToken(user);
        if (usuarioLogadoEhAdmin)
        {
            var ocorrencia = context.OcorrenciaSet.Find(ocorrenciaEncerrarRequest.Id);
            if (ocorrencia is null)
                return Results.BadRequest("Ocorrencia não Localizada.");

            ocorrencia.Encerrar(ocorrenciaEncerrarRequest.DescricaoResolucao, usuarioLogadoId);
            context.OcorrenciaSet.Update(ocorrencia);
            context.SaveChanges();

            return Results.Ok("Ocorrencia Encerrada com Sucesso.");
        }
        return Results.BadRequest("Usuário não permitido");
    }
    catch (Exception ex)
    {
        return Results.BadRequest(ex.InnerException?.Message ?? ex.Message);
    }
})
    .WithOpenApi(operation =>
    {
        operation.Description = "Endpoint para Encerrar uma Ocorrencia";
        operation.Summary = "Encerrar Ocorrencia";
        return operation;
    })
    .WithTags("Ocorrencias");
////.RequireAuthorization();

app.MapPut("/ocorrencia/iniciar-atendimento", (HelpTechContext context, ClaimsPrincipal user, OcorrenciaIniciarAtendimentoRequest ocorrenciaIniciarAtendimentoRequest) =>
{
    try
    {
        SetarDadosToken(user);

        if (usuarioLogadoEhAdmin)
        {
            var ocorrencia = context.OcorrenciaSet.Find(ocorrenciaIniciarAtendimentoRequest.Id);
        if (ocorrencia is null)
            return Results.BadRequest("Ocorrencia não Localizada.");

            ocorrencia.IniciarAtendimento();
            context.OcorrenciaSet.Update(ocorrencia);
            context.SaveChanges();

            return Results.Ok("Ocorrencia Iniciada com Sucesso.");
        }

        return Results.BadRequest("Usuário não permitido!");
        
    }
    catch (Exception ex)
    {
        return Results.BadRequest(ex.InnerException?.Message ?? ex.Message);
    }
})
    .WithOpenApi(operation =>
    {
        operation.Description = "Endpoint para Iniciar Atendimento a uma Ocorrencia";
        operation.Summary = "Iniciar uma Ocorrencia";
        return operation;
    })
    .WithTags("Ocorrencias");
////.RequireAuthorization();

#endregion

#region Endpoints de Usuários

app.MapGet("/usuario/listar", (HelpTechContext context) =>
{
    var usuarios = context.UsuarioSet.Select(usuario => new UsuarioListarResponse
    {
        Id = usuario.Id,
        Nome = usuario.Nome,
        EhAdmin = usuario.EhAdmin
    });

    return Results.Ok(usuarios);
})
    .WithOpenApi(operation =>
    {
        operation.Description = "Endpoint para obter todos os usuários cadastrados";
        operation.Summary = "Listar todos os Usuários";
        return operation;
    })
    .WithTags("Usuários");
    //.RequireAuthorization();

app.MapGet("/usuario/{usuarioId}", (HelpTechContext context, Guid usuarioId) =>
{
    var usuario = context.UsuarioSet.Find(usuarioId);
    if (usuario is null)
        return Results.BadRequest("Usuário não Localizado.");

    var usuarioDto = new UsuarioObterResponse
    {
        Id = usuario.Id,
        Nome = usuario.Nome,
        EmailLogin = usuario.EmailLogin,
        EhAdmin = usuario.EhAdmin
    };

    return Results.Ok(usuarioDto);
})
    .WithOpenApi(operation =>
    {
        operation.Description = "Endpoint para obter um usuário com base no ID informado";
        operation.Summary = "Obter um Usuário";
        operation.Parameters[0].Description = "Id do Usuário";
        return operation;
    })
    .WithTags("Usuários");
    //.RequireAuthorization();

app.MapPost("/usuario/adicionar", (HelpTechContext context, UsuarioAdicionarRequest usuarioAdicionarRequest) =>
{
    try
    {
        if (usuarioAdicionarRequest.EmailLogin != usuarioAdicionarRequest.EmailLoginConfirmacao)
            return Results.BadRequest("Email de Login não Confere.");

        if (usuarioAdicionarRequest.Senha != usuarioAdicionarRequest.SenhaConfirmacao)
            return Results.BadRequest("Senha não Confere.");

        if (context.UsuarioSet.Any(p => p.EmailLogin == usuarioAdicionarRequest.EmailLogin))
            return Results.BadRequest("Email já utilizado para Login em outro Usuário.");

        var usuario = new Usuario(
            usuarioAdicionarRequest.Nome,
            usuarioAdicionarRequest.EmailLogin,
            usuarioAdicionarRequest.Senha.EncryptPassword(),
            usuarioAdicionarRequest.EhAdmin);

        context.UsuarioSet.Add(usuario);
        context.SaveChanges();

        return Results.Created("Created", $"Usuário Registrado com Sucesso. {usuario.Id}");
    }
    catch (Exception ex)
    {
        return Results.BadRequest(ex.InnerException?.Message ?? ex.Message);
    }
})
    .WithOpenApi(operation =>
    {
        operation.Description = "Endpoint para Cadastrar um Usuário";
        operation.Summary = "Novo Usuário";
        return operation;
    })
    .WithTags("Usuários");
    //.RequireAuthorization();

app.MapPut("/usuario/alterar-senha", (HelpTechContext context, UsuarioAtualizarRequest usuarioAtualizarRequest) =>
{
    try
    {
        var usuario = context.UsuarioSet.Find(usuarioAtualizarRequest.UsuarioId);
        if (usuario is null)
            return Results.BadRequest("Usuário não Localizado.");

        if (usuarioAtualizarRequest.SenhaAtual.EncryptPassword() == usuario.Senha)
        {
            usuario.AlterarSenha(usuarioAtualizarRequest.SenhaNova.EncryptPassword());
            context.UsuarioSet.Update(usuario);
            context.SaveChanges();

            return Results.Ok("Senha Alterada com Sucesso.");
        }

        return Results.BadRequest("Ocorreu um Problema ao Alterar a Senha do Usuário.");
    }
    catch (Exception ex)
    {
        return Results.BadRequest(ex.InnerException?.Message ?? ex.Message);
    }
})
    .WithOpenApi(operation =>
    {
        operation.Description = "Endpoint para Alterar a Senha do Usuário";
        operation.Summary = "Alterar Senha";
        return operation;
    })
    .WithTags("Usuários");
//.RequireAuthorization();

#endregion

#region Autenticação

app.MapPost("/autenticar", (HelpTechContext context, UsuarioAutenticarRequest usuarioAutenticarRequest) =>
{
    var usuario = context.UsuarioSet.FirstOrDefault(p => p.EmailLogin == usuarioAutenticarRequest.EmailLogin && p.Senha == usuarioAutenticarRequest.Senha.EncryptPassword());
    if (usuario is null)
        return Results.BadRequest("Não foi Possível Efetuar o Login.");

    var claims = new[]
    {
            new Claim("UsuarioId", usuario.Id.ToString()),
            new Claim(ClaimTypes.Name, usuario.Nome),
            new Claim("EhAdmin", usuario.EhAdmin ? "S" : "N")
        };

    //Recebe uma instância da Classe SymmetricSecurityKey
    //armazenando a chave de criptografia usada na criação do Token
    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("{469e8343-8fa6-42b9-9553-2f6e182c21fa}"));

    //Recebe um objeto do tipo SigninCredentials contendo a chave de
    //criptografia e o algoritimo de seguran�a empregados na gera��o
    //de assinaturas digitais para tokens
    var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

    var token = new JwtSecurityToken(
        issuer: "help.tech",
        audience: "help.tech",
        claims: claims,
        expires: DateTime.Now.AddDays(1),
        signingCredentials: creds
    );

    return Results.Ok(new UsuarioTokenResponse
    {
        UsuarioId = usuario.Id,
        UsuarioNome = usuario.Nome,
        AccessToken = new JwtSecurityTokenHandler().WriteToken(token)
    });
})
    .WithOpenApi(operation =>
    {
        operation.Description = "Endpoint para Autenticar um Usuário na API";
        operation.Summary = "Autenticar Usuário";
        return operation;
    })
    .WithTags("Segurança");

#endregion

app.MapControllers();

app.Run();

#region Métodos de Apoio

void SetarDadosToken(ClaimsPrincipal user)
{
    usuarioLogadoId = Guid.Parse(user.Identities.First().Claims.ToList()[0].Value);
    usuarioLogadoEhAdmin = user.Identities.First().Claims.ToList()[2].Value == "S";
}

#endregion
