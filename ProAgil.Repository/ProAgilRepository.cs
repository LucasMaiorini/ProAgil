using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ProAgil.Domain;

namespace ProAgil.Repository
{
    public class ProAgilRepository : IProAgilRepository
    {
        private readonly ProAgilContext _context;

        public ProAgilRepository(ProAgilContext context)
        {
            _context = context;
            _context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
        }

        #region COMUM
        public void Add<T>(T entity) where T : class
        {
            _context.Add(entity);
        }

        public void Update<T>(T entity) where T : class
        {
            _context.Update(entity);
        }

        public void Delete<T>(T entity) where T : class
        {
            _context.Remove(entity);
        }

        public async Task<bool> SaveChangesAsync()
        {
            return (await _context.SaveChangesAsync()) > 0;
        }

        #endregion

        #region  EVENTO
        public async Task<Evento[]> GetAllEventoAsync(bool includePalestrantes = false)
        {
            IQueryable<Evento> query = _context.Eventos.Include(e => e.Lotes).Include(e => e.RedesSociais);

            if (includePalestrantes)
            {
                query = query.Include(e => e.PalestranteEventos).ThenInclude(p => p.Palestrante);
            }
            query = query.OrderByDescending(e => e.DataEvento);
            return await query.ToArrayAsync();
        }

        public async Task<Evento> GetEventoAsyncById(int Id, bool includePalestrantes = false)
        {
            IQueryable<Evento> query = _context.Eventos.Where(e => e.Id == Id)
            .Include(e => e.Lotes).Include(e => e.RedesSociais);

            if (includePalestrantes)
            {
                query = query.Include(e => e.PalestranteEventos).ThenInclude(p => p.Palestrante);
            }
            query = query.OrderByDescending(e => e.DataEvento);
            return await query.FirstOrDefaultAsync();
        }

        public async Task<Evento[]> GetAllEventoAsyncByTema(string filtro, bool includePalestrantes)
        {
            IQueryable<Evento> query = _context.Eventos.Where(e => e.Tema.ToLower().Contains(filtro.ToLower()))
            .Include(e => e.Lotes).Include(e => e.RedesSociais);

            if (includePalestrantes)
            {
                query = query.Include(e => e.PalestranteEventos).ThenInclude(p => p.Palestrante);
            }
            // query = query.OrderByDescending(e => e.DataEvento).Where(e => e.Tema.Contains(filtro));
            query = query.OrderByDescending(e => e.DataEvento);
            return await query.ToArrayAsync();
        }

        #endregion

        #region PALESTRANTE

        public async Task<Palestrante> GetPalestranteAsyncById(int PalestranteId, bool includeEventos = false)
        {
            IQueryable<Palestrante> query = _context.Palestrantes.Include(p => p.RedesSociais);

            if (includeEventos)
            {
                query = query.Include(p => p.PalestranteEventos).ThenInclude(e => e.Evento);
            }
            query = query.OrderBy(p => p.Nome).Where(p => p.Id == PalestranteId);
            return await query.FirstOrDefaultAsync();
        }

        public async Task<Palestrante[]> GetAllPalestrantesAsyncByNome(string nome, bool includeEventos = false)
        {
            IQueryable<Palestrante> query = _context.Palestrantes.Where(p => p.Nome.ToLower().Contains(nome.ToLower()))
            .Include(p => p.RedesSociais);

            if (includeEventos)
            {
                query = query.Include(p => p.PalestranteEventos).ThenInclude(e => e.Evento);
            }
            query = query.OrderBy(p => p.Nome);
            return await query.ToArrayAsync();
        }

        public async Task<Palestrante[]> GetAllPalestrantesAsync(bool includeEventos = false)
        {
            IQueryable<Palestrante> query = _context.Palestrantes.Include(p => p.RedesSociais);
            if (includeEventos)
            {
                query = query.Include(p => p.PalestranteEventos).ThenInclude(e => e.Evento);
            }
            query = query.OrderBy(p => p.Nome);
            return await query.ToArrayAsync();
        }
        #endregion

    }
}