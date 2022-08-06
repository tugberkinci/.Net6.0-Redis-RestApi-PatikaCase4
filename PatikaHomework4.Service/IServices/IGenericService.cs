namespace PatikaHomework4.Service.IServices
{
    public interface IGenericService <T> where T : class
    {
        Task<IEnumerable<T>> GetAll();
        Task<T> GetById(int id);
        Task<T> Add(T entity);
        Task<T> Update(T entity);
        Task<String> Delete(int id);
    }
}
