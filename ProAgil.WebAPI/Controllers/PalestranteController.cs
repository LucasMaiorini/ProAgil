using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProAgil.Domain;
using ProAgil.Repository;

namespace ProAgil.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PalestranteController : ControllerBase
    {
        private readonly IProAgilRepository _repo;

        public PalestranteController(IProAgilRepository repo)
        {
            this._repo = repo;
        }

        // GET api/palestrante
        [HttpGet]
        public async Task<ActionResult> Get()
        {
            try
            {
                var results = await _repo.GetAllPalestrantesAsync(true);
                return Ok(results);
            }
            catch
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, "Banco de Dados Falhou");

            }
        }

        // GET api/palestrante/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            try
            {
                var results = await _repo.GetPalestranteAsyncById(id, true);
                return Ok(results);
            }
            catch (System.Exception)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, "Banco de Dados Falhou");
            }
        }

        // GET api/palestrante/getByNome/nome
        [HttpGet("getByNome/{nome}")]
        public async Task<ActionResult> Get(string nome)
        {
            try
            {
                var result = await _repo.GetAllPalestrantesAsyncByNome(nome, true);
                return Ok(result);
            }
            catch
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, "Banco de Dados Falhou");

            }
        }


        [HttpPost]
        public async Task<ActionResult> Post(Palestrante model)
        {
            try
            {
                _repo.Add(model);
                if (await _repo.SaveChangesAsync())
                {
                    return Created($"/api/palestrante/{model.Id}", model);

                }
            }
            catch
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, "Banco de Dados Falhou");
            }
            return BadRequest();
        }

        [HttpPut]
        public async Task<ActionResult> Put(Palestrante model)
        {
            try
            {
                if (await _repo.GetEventoAsyncById(model.Id) == null)
                {
                    return NotFound();
                }
                _repo.Update(model);
                if (await _repo.SaveChangesAsync())
                {
                    return Created($"/api/palestrante/{model.Id}", model);
                }
            }
            catch
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, "Banco de Dados Falhou");
            }
            return BadRequest();
        }

        [HttpDelete]
        public async Task<ActionResult> Delete(Palestrante model)
        {
            try
            {
                if (await _repo.GetEventoAsyncById(model.Id) == null)
                {
                    return NotFound();
                }
                _repo.Delete(model);
                if (await _repo.SaveChangesAsync())
                {
                    return Ok();
                }
            }
            catch
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, "Banco de Dados Falhou");
            }
            return BadRequest();
        }
    }
}