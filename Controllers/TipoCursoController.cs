using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[Route("api/[controller]")]
[ApiController]
public class TipoCursoController : ControllerBase
{

    private readonly DataContext context;

    public TipoCursoController(DataContext _context)
    {
        context = _context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<TipoCurso>>> Get() {
        try {
            List<TipoCurso> retorno = await context.TipoCursos.ToListAsync();
            return Ok(retorno);
        }
        catch {
            return BadRequest("Erro ao listar os tipos de curso");
        }        
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<TipoCurso>> Get([FromRoute] int id) {
        try
        {
            if (await context.TipoCursos.AnyAsync(p => p.Id == id))
                return Ok(await context.TipoCursos.FindAsync(id));
            else
                return NotFound("O tipo de curso informado não foi encontrado");
        }
        catch
        {
            return BadRequest("Erro ao efetuar a busca de tipo de curso");
        }
    }

    [HttpPost]
    public async Task<ActionResult> Post([FromBody]TipoCurso model) {
        try{
            await context.TipoCursos.AddAsync(model);
            await context.SaveChangesAsync(); 
            return Ok("Tipo de curso salvo com sucesso");
        }
        catch{
            return BadRequest("Erro ao salvar o tipo de curso informado");
        }        
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> Put([FromRoute] int id, [FromBody] TipoCurso model)
    {
        if (id != model.Id) //se é diferente da rota, erro
            return BadRequest("Tipo de curso inválido");

        try
        {
            //se não existe, erro, senão cria um novo tipo de curso
            if (!await context.TipoCursos.AnyAsync(p => p.Id == id))
                return NotFound("Tipo de curso inválido");

            context.TipoCursos.Update(model);  // não tem async
            await context.SaveChangesAsync();
            return Ok("Tipo de curso salvo com sucesso");
        }
        catch
        {
            return BadRequest("Erro ao salvar o tipo de curso informado");
        }
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete([FromRoute] int id)
    {
        try
        {
            TipoCurso? model = await context.TipoCursos.FindAsync(id);

            if (model == null)
                return NotFound("Tipo de curso inválido");

            context.TipoCursos.Remove(model);
            await context.SaveChangesAsync();
            return Ok("Tipo de curso removido com sucesso");
        }
        catch
        {
            return BadRequest("Falha ao remover o tipo de curso");
        }
    }

    [HttpGet("pesquisaNome/{nome}")]
    public async Task<ActionResult<IEnumerable<TipoCurso>>> Get([FromRoute] string nome)
    {
        try
        {
            List<TipoCurso> resultado = await context.TipoCursos.Where(p => p.Nome == nome).ToListAsync();
            return Ok(resultado);
        }
        catch
        {
            return BadRequest("Falha ao buscar um tipo de curso");
        }
    }

    [HttpGet("pesquisaNomeSemelhante/{nome}")]
    public async Task<ActionResult<IEnumerable<TipoCurso>>> PesquisaNomeSemelhante([FromRoute] string nome)
    {
        try
        {
            List<TipoCurso> resultado = await context.TipoCursos.
            Where(p => p.Nome.Contains(nome)).ToListAsync();
            return Ok(resultado);
        }
        catch
        {
            return BadRequest("Falha ao buscar um tipo de curso");
        }
    }

    [Route("pesquisa")]
    [HttpPost]
    public async Task<ActionResult<IEnumerable<TipoCurso>>> Pesquisa([FromBody] object item)
    {
        try
        {
            TipoCurso model = JsonSerializer.Deserialize<TipoCurso>(item.ToString());

            List<TipoCurso> resultado = await context.TipoCursos
                .WhereIf(model.Nome != null, p => p.Nome == model.Nome)
                .WhereIf(model.Descricao != null, p => p.Descricao == model.Descricao).ToListAsync();

            return Ok(resultado);
        }
        catch
        {
            return BadRequest();
        }
    }
}