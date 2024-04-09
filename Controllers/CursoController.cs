using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[Route("api/[controller]")]
[ApiController]
public class CursoController : ControllerBase
{
    private readonly DataContext context;

    public CursoController(DataContext _context)
    {
        context = _context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Curso>>> Get()
    {
        try
        {
            return Ok(await context.Cursos.Include(p => p.TipoDoCurso).ToListAsync());
        }
        catch
        {
            return BadRequest("Erro ao listar os cursos");
        }
    }

    [HttpPost]
    public async Task<ActionResult> Post([FromBody] Curso item)
    {
        try
        {
            await context.Cursos.AddAsync(item);
            await context.SaveChangesAsync();
            return Ok("Curso salvo com sucesso");
        }
        catch
        {
            return BadRequest("Erro ao salvar o curso informado");
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Curso>> Get([FromRoute] int id)
    {
        try
        {
            if (await context.Cursos.AnyAsync(p => p.Id == id))
                return Ok(await context.Cursos.Include(p => p.TipoDoCurso).FirstOrDefaultAsync(p => p.Id == id));
            else
                return NotFound("O curso informado não foi encontrado");
        }
        catch
        {
            return BadRequest("Erro ao efetuar a busca de curso");
        }
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> Put([FromRoute] int id, [FromBody] Curso model)
    {
        if (id != model.Id)
            return BadRequest("Curso inválido");

        try
        {
            if (!await context.Cursos.AnyAsync(p => p.Id == id))
                return NotFound("Curso inválido");

            context.Cursos.Update(model);
            await context.SaveChangesAsync();
            return Ok("Curso salvo com sucesso");
        }
        catch
        {
            return BadRequest("Erro ao salvar o curso informado");
        }
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete([FromRoute] int id)
    {
        try
        {
            Curso model = await context.Cursos.FindAsync(id);

            if (model == null)
                return NotFound("Curso inválido");

            context.Cursos.Remove(model);
            await context.SaveChangesAsync();
            return Ok("Curso removido com sucesso");
        }
        catch
        {
            return BadRequest("Falha ao remover o curso");
        }
    }
}