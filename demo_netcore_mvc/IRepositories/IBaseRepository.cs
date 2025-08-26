namespace demo_netcore_mvc.IRepositories
{
    public interface IBaseRepository<T> where T : class
    {
        public Task<List<T>> GetAllAsync(object requestData);

        public Task<T?> GetByIdAsync(object id);

        public Task InsertAsync(T obj);

        public Task UpdateAsync(T obj);

        public Task DeleteAsync(object id);
    }
}
