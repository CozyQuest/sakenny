using sakenny.DAL;
using sakenny.DAL.Interfaces;

namespace sakenny.DAL.Repository
{
    public class DeleteUpdateRepository<T> : BaseRepository<T>, IDeleteUpdate<T> where T : class, ISoftDeletable
    {
        private readonly ApplicationDBContext _context;
        public DeleteUpdateRepository(ApplicationDBContext context) : base(context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }
        public void DeleteAsync(T entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }
            _context.Set<T>().Remove(entity);
        }

        public void UpdateAsync(T entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }
            _context.Set<T>().Update(entity);
        }
        public void SoftDeleteAsync(T entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            entity.IsDeleted = true;
            _context.Set<T>().Update(entity);
        }
    }
}
