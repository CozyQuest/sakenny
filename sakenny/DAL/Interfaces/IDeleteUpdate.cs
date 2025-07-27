namespace sakenny.DAL.Interfaces
{
    public interface IDeleteUpdate<T> : IBaseRepository<T> where T : class
    {
        void DeleteAsync(T entity);
        void UpdateAsync(T entity);
        void SoftDeleteAsync(T entity);
    }
}
