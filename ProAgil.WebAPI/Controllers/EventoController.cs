using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProAgil.Domain;
using ProAgil.Repository;
using ProAgil.WebAPI.Dtos;

namespace ProAgil.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EventoController : ControllerBase
    {
        private readonly IProAgilRepository _repo;
        private readonly IMapper _mapper;

        public EventoController(IProAgilRepository repo, IMapper mapper)
        {
            this._repo = repo;
            this._mapper = mapper;
        }

        // GET api/evento
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            try
            {
                var eventos = await _repo.GetAllEventoAsync(true);
                var results = _mapper.Map<EventoDto[]>(eventos);
                return Ok(results);
            }
            catch (System.Exception)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, "Banco de Dados Falhou");
            }
        }

        // GET api/evento/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            try
            {
                var evento = await _repo.GetEventoAsyncById(id, true);
                var result = this._mapper.Map<EventoDto>(evento);
                return Ok(result);
            }
            catch (System.Exception)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, "Banco de Dados Falhou");
            }
        }

        // GET api/evento/getByTema/tema
        [HttpGet("getByTema/{tema}")]
        public async Task<IActionResult> Get(string tema)
        {
            try
            {
                var eventos = await _repo.GetAllEventoAsyncByTema(tema, true);
                var results = this._mapper.Map<EventoDto[]>(eventos);
                return Ok(results);
            }
            catch (System.Exception)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, "Banco de Dados Falhou");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Post(EventoDto model)
        {
            try
            {
                var evento = this._mapper.Map<Evento>(model);

                _repo.Add(evento);
                if (await _repo.SaveChangesAsync())
                {
                    return Created($"/api/evento/{model.Id}", this._mapper.Map<EventoDto>(evento));

                }
            }
            catch (System.Exception)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, "Banco de Dados Falhou");
            }
            return BadRequest();
        }

        // Used to upload image file
        [HttpPost("upload")]
        public async Task<IActionResult> Upload()
        {
            try
            {
                //it's always an array, that's the reason the position is 0.
                var file = Request.Form.Files[0];
                //Merges the two arguments to name the variable folderName.
                var folderName = Path.Combine("Resources", "Images");
                //Gets the current directory and merges with the variable folderName.
                var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);

                if (file.Length > 0)
                {
                    //finds out the file's name
                    var fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName;
                    //Path + Name that the picture will be saved. Replaces double quotes by empty space
                    var fullPath = Path.Combine(pathToSave, fileName.Replace("\"", " ").Trim());

                    using (var stream = new FileStream(fullPath, FileMode.Create))
                    {
                        file.CopyTo(stream);
                    }
                }
                return Ok();
            }
            catch (System.Exception ex)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, $"Banco de Dados Falhou { ex.Message}");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, EventoDto model)
        {
            try
            {
                var evento = await _repo.GetEventoAsyncById(id);
                if (evento == null)
                {
                    return NotFound();
                }

                // DELETE EVENTO & REDE SOCIAL IF NECESSARY

                // Creates the lists which will store the respectives datas ids.
                var idLotesList = new List<int>();
                var idRedesSociaisList = new List<int>();

                // For each item in the given entity (Lotes or RedesSociais) passed through model,
                //we add it to the newly created lists.
                model.Lotes.ForEach(item => idLotesList.Add(item.Id));
                model.RedesSociais.ForEach(item => idRedesSociaisList.Add(item.Id));

                /*
                Gets all the data in the given entity which doesn't match the model
                (passed through parameter in this function).
                */
                var lotes = evento.Lotes.Where(
                                   lote => !idLotesList.Contains(lote.Id)
                    ).ToArray();

                var redesSociais = evento.RedesSociais.Where(
                    rede => !idRedesSociaisList.Contains(rede.Id)
                            ).ToArray();

                // If there is some data wich is not present in the new update, it must be deleted.
                if (lotes.Length > 0) _repo.DeleteRange(lotes);
                if (redesSociais.Length > 0) _repo.DeleteRange(redesSociais);
                //END OF THE DELETE EVENTO & REDE SOCIAL SECTION

                //Changes the 'evento' according to 'model'
                this._mapper.Map(model, evento);

                _repo.Update(evento);
                if (await _repo.SaveChangesAsync())
                {
                    //Returns the 'evento' mapped according to 'EventoDto'
                    return Created($"/api/evento/{model.Id}", this._mapper.Map<EventoDto>(evento));

                }
            }
            catch (System.Exception)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, "Banco de Dados Falhou");
            }
            return BadRequest();
        }

        [HttpDelete("{Eventoid}")]
        public async Task<IActionResult> Delete(int Eventoid)
        {
            try
            {
                var evento = await _repo.GetEventoAsyncById(Eventoid);
                if (evento == null) { return NotFound(); }
                _repo.Delete(evento);
                if (await _repo.SaveChangesAsync())
                {
                    return Ok();
                }
            }
            catch (System.Exception)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, "Banco de Dados Falhou");
            }
            return BadRequest();
        }

    }
}