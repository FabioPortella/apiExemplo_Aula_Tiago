using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;

[Route("api/[controller]")]
[Authorize]
[ResponseCache(NoStore = true, Duration = 0, Location = ResponseCacheLocation.None)]
[ApiController]
public class UsuarioController : ControllerBase
{
    private readonly DataContext context;

    public UsuarioController(DataContext Context)
    {
        context = Context;
    }

    [NonAction]
    private static string Hash(string password)
    {
        HashAlgorithm hasher = HashAlgorithm.Create(HashAlgorithmName.SHA512.Name);
        byte[] stringBytes = Encoding.ASCII.GetBytes(password);
        byte[] byteArray = hasher.ComputeHash(stringBytes);

        StringBuilder stringBuilder = new StringBuilder();
        foreach (byte b in byteArray)
        {
            stringBuilder.AppendFormat("{0:x2}", b);
        }

        return stringBuilder.ToString();
    }

    [NonAction]
    private static string ObterSenha(Usuario usuario)
    {
        if (usuario == null || usuario.Senha == null || usuario.Senha.Trim() == "")
            throw new Exception();

        string retorno = usuario.Senha;

        retorno = "djfs0dj87h78" + retorno;
        retorno = Hash(retorno);
        retorno = retorno + "87sdfhns78dfh8";
        retorno = Hash(retorno);

        return retorno;
    }
    [AllowAnonymous]
    [HttpPost]
    public async Task<ActionResult> Post([FromBody] Usuario model)
    {
        try
        {
            if (await context.Usuarios.AnyAsync(p => p.Email == model.Email))
                return BadRequest("Já existe usuário com o e-mail informado");

            model.Senha = ObterSenha(model);
            await context.Usuarios.AddAsync(model);
            await context.SaveChangesAsync();
            return Ok("Usuário salvo com sucesso");
        }
        catch
        {
            return BadRequest("Falha ao inserir o usuário informado");
        }
    }

    [AllowAnonymous]
    [HttpPost("autenticar")]
    public ActionResult Autenticar([FromBody] Usuario usuario)
    {
        try
        {
            usuario.Senha = ObterSenha(usuario);
            Usuario autenticado = context.Usuarios.FirstOrDefault(p => p.Email == usuario.Email && p.Senha == usuario.Senha);

            if (autenticado == null)
                return BadRequest("Usuário inválido");

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetValue("Secret", ""));

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                new Claim(ClaimTypes.Sid, autenticado.Id.ToString()),
                new Claim(ClaimTypes.Name, autenticado.Nome),
                new Claim(ClaimTypes.Email, autenticado.Email)
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha512Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            usuario.Token = tokenHandler.WriteToken(token);

            return Ok(usuario.Token);
        }
        catch
        {
            return BadRequest();
        }
    }
}