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

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.EnableAnnotations();
});

builder.Services.AddDbContext<HelpTechContext>();
builder.Services.AddControllers().AddJsonOptions(options => options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

builder.Services.ConfigureAuthentication();
builder.Services.ConfigureAuthenticateSwagger();

var app = builder.Build();

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
//    app.UseSwagger();
//    app.UseSwaggerUI();
//}

app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthentication();
app.UseAuthorization();
app.UseHttpsRedirection();




#region Endpoints de Ocorrencia
app.MapGet("/ocorrencia", (HelpTechContext context) =>
{
    var ocorrencias = context.OcorrenciaSet.Select(ocorrencia => new OcorrenciaListarResponse
    {
        Id = ocorrencia.Id,
        Descricao = ocorrencia.Descricao,
        TipoOcorrencia = ocorrencia.TipoOcorrencia
    });

    return Results.Ok(ocorrencias);
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
        Descricao = ocorrencia.Descricao,
        TipoOcorrencia = ocorrencia.TipoOcorrencia
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

app.MapPost("/ocorrencia", (HelpTechContext context, OcorrenciaAdicionarRequest ocorrenciaAdicionarRequest) =>
{
    try
    {
        var ocorrencia = new Ocorrencia(ocorrenciaAdicionarRequest.Descricao, ocorrenciaAdicionarRequest.TipoOcorrencia);

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

app.MapPut("/ocorrencia", (HelpTechContext context, OcorrenciaAtualizarRequest ocorrenciaAtualizarRequest) =>
{
    try
    {
        var ocorrencia = context.OcorrenciaSet.Find(ocorrenciaAtualizarRequest.Id);
        if (ocorrencia is null)
            return Results.BadRequest("Ocorrencia não Localizada.");

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

app.MapDelete("/ocorrencia/{ocorrenciaId}", (HelpTechContext context, Guid ocorrenciaId) =>
{
    try
    {
        var ocorrencia = context.OcorrenciaSet.Find(ocorrenciaId);
        if (ocorrencia is null)
            return Results.BadRequest("Ocorrencia não Localizada.");

        context.OcorrenciaSet.Remove(ocorrencia);
        context.SaveChanges();

        return Results.Ok("Ocorrencia Removida com Sucesso.");
    }
    catch (Exception ex)
    {
        return Results.BadRequest(ex.InnerException?.Message ?? ex.Message);
    }
})
    .WithOpenApi(operation =>
    {
        operation.Description = "Endpoint para Excluir uma Ocorrencia";
        operation.Summary = "Excluir Ocorrencia";
        operation.Parameters[0].Description = "Id da Ocorrencia";
        return operation;
    })
    .WithTags("Ocorrencias");
////.RequireAuthorization();

#endregion

#region Endpoints de Usu�rios

app.MapGet("/usuario", (HelpTechContext context) =>
{
    var usuarios = context.UsuariosSet.Select(usuario => new UsuarioListarResponse
    {
        Id = usuario.Id,
        Nome = usuario.Nome
    });

    return Results.Ok(usuarios);
})
    .WithOpenApi(operation =>
    {
        operation.Description = "Endpoint para obter todos os usu�rios cadastrados";
        operation.Summary = "Listar todos os Usu�rios";
        return operation;
    })
    .WithTags("Usu�rios");
    //.RequireAuthorization();

app.MapGet("/usuario/{usuarioId}", (HelpTechContext context, Guid usuarioId) =>
{
    var usuario = context.UsuariosSet.Find(usuarioId);
    if (usuario is null)
        return Results.BadRequest("Usu�rio n�o Localizado.");

    var usuarioDto = new UsuarioObterResponse
    {
        Id = usuario.Id,
        Nome = usuario.Nome,
        EmailLogin = usuario.EmailLogin
    };

    return Results.Ok(usuarioDto);
})
    .WithOpenApi(operation =>
    {
        operation.Description = "Endpoint para obter um usu�rio com base no ID informado";
        operation.Summary = "Obter um Usu�rio";
        operation.Parameters[0].Description = "Id do Usu�rio";
        return operation;
    })
    .WithTags("Usu�rios");
    //.RequireAuthorization();

app.MapPost("/usuario", (HelpTechContext context, UsuarioAdicionarRequest usuarioAdicionarRequest) =>
{
    try
    {
        if (usuarioAdicionarRequest.EmailLogin != usuarioAdicionarRequest.EmailLoginConfirmacao)
            return Results.BadRequest("Email de Login n�o Confere.");

        if (usuarioAdicionarRequest.Senha != usuarioAdicionarRequest.SenhaConfirmacao)
            return Results.BadRequest("Senha n�o Confere.");

        if (context.UsuariosSet.Any(p => p.EmailLogin == usuarioAdicionarRequest.EmailLogin))
            return Results.BadRequest("Email j� utilizado para Login em outro Usu�rio.");

        var usuario = new Usuario(
            usuarioAdicionarRequest.Nome,
            usuarioAdicionarRequest.EmailLogin,
            usuarioAdicionarRequest.Senha.EncryptPassword());

        context.UsuariosSet.Add(usuario);
        context.SaveChanges();

        return Results.Created("Created", $"Usu�rio Registrado com Sucesso. {usuario.Id}");
    }
    catch (Exception ex)
    {
        return Results.BadRequest(ex.InnerException?.Message ?? ex.Message);
    }
})
    .WithOpenApi(operation =>
    {
        operation.Description = "Endpoint para Cadastrar um Usu�rio";
        operation.Summary = "Novo Usu�rio";
        return operation;
    })
    .WithTags("Usu�rios");
    //.RequireAuthorization();

app.MapPut("/usuario/alterar-senha", (HelpTechContext context, UsuarioAtualizarRequest usuarioAtualizarRequest) =>
{
    try
    {
        var usuario = context.UsuariosSet.Find(usuarioAtualizarRequest.Id);
        if (usuario is null)
            return Results.BadRequest("Usu�rio n�o Localizado");

        if (usuarioAtualizarRequest.SenhaAtual.EncryptPassword() == usuario.Senha)
        {
            usuario.AlterarSenha(usuarioAtualizarRequest.SenhaNova.EncryptPassword());
            context.UsuariosSet.Update(usuario);
            context.SaveChanges();

            return Results.Ok("Senha Alterada com Sucesso.");
        }

        return Results.BadRequest("Ocorreu um Problema ao Alterar a Senha do Usu�rio.");
    }
    catch (Exception ex)
    {
        return Results.BadRequest(ex.InnerException?.Message ?? ex.Message);
    }
})
    .WithOpenApi(operation =>
    {
        operation.Description = "Endpoint para Alterar a Senha do Usu�rio";
        operation.Summary = "Alterar Senha";
        return operation;
    })
    .WithTags("Usu�rios");
//.RequireAuthorization();

#endregion


#region Autenticação

app.MapPost("/autenticar", (HelpTechContext context, UsuarioAutenticarRequest usuarioAutenticarRequest) =>
{
    var usuario = context.UsuariosSet.FirstOrDefault(p => p.EmailLogin == usuarioAutenticarRequest.EmailLogin && p.Senha == usuarioAutenticarRequest.Senha.EncryptPassword());
    if (usuario is null)
        return Results.BadRequest("N�o foi Poss�vel Efetuar o Login.");

    var claims = new[]
    {
            new Claim("UsuarioId", usuario.Id.ToString()),
            new Claim(ClaimTypes.Name, usuario.Nome)
        };

    //Recebe uma inst�ncia da Classe SymmetricSecurityKey
    //armazenando a chave de criptografia usada na cria��o do Token
    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("{ccdc511d-23f0-4a30-995e-ebc3658e901d}"));

    //Recebe um objeto do tipo SigninCredentials contendo a chave de
    //criptografia e o algoritimo de seguran�a empregados na gera��o
    //de assinaturas digitais para tokens
    var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

    var token = new JwtSecurityToken(
        issuer: "etec.pwIII",
        audience: "etec.pwIII",
        claims: claims,
        expires: DateTime.Now.AddDays(1),
        signingCredentials: creds
    );

    return Results.Ok(new
    {
        UsuarioId = usuario.Id,
        UsuarioNome = usuario.Nome,
        AccessToken = new JwtSecurityTokenHandler().WriteToken(token)
    });
})
    .WithOpenApi(operation =>
    {
        operation.Description = "Endpoint para Autenticar um Usuario na API";
        operation.Summary = "Autenticar Usuario";
        return operation;
    })
    .WithTags("Segurança");

#endregion

app.MapControllers();

app.Run();
