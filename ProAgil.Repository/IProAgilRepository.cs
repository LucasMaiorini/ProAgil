using System.Threading.Tasks;
using ProAgil.Domain;

namespace ProAgil.Repository
{
    public interface IProAgilRepository
    {
        void Add<T>(T entity) where T : class;
        void Update<T>(T entity) where T : class;
        void Delete<T>(T entity) where T : class;
        Task<bool> SaveChangesAsync();

        //Evento
        Task<Evento[]> GetAllEventoAsync(bool includePalestrantes = false);
        Task<Evento> GetEventoAsyncById(int Id, bool includePalestrantes = false);
        Task<Evento[]> GetAllEventoAsyncByTema(string filtro, bool includePalestrantes = false);

        //Palestrante
        Task<Palestrante[]> GetAllPalestrantesAsync(bool includeEventos = false);
        Task<Palestrante> GetPalestranteAsyncById(int palestranteId, bool includeEventos = false);
        Task<Palestrante[]> GetAllPalestrantesAsyncByNome(string nome, bool includeEventos = false);
    }
}