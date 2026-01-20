using System.Linq.Expressions;

namespace CvBuilderBack.Repositories;

public interface IUserRepository
{
    public bool TryGetById<T>(int id, out T? data) where T : class;
    
    public T? Get<T>(Expression<Func<T, bool>>? predicate) where T : class;
    
    public List<T> GetAll<T>(Expression<Func<T, bool>>? predicate) where T : class;

    public bool Update<T>(int id, Action<T> func) where T : class;
    
    public bool Add<T>(T? data) where T : class;

    public bool Remove<T>(int id) where T : class;

    public T[] ExecuteStoreProcedure<T>(string storedProcedure, params Tuple<string, object>[] parameters);

    public bool SaveChanges();
}