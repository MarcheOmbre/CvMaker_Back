using System.Linq.Expressions;
using CvBuilderBack.Contexts;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace CvBuilderBack.Repositories;

public class UserRepositoryEntityFramework(IConfiguration configuration) : IUserRepository
{
    private readonly DataContextEntityFramework context = new(configuration);

    public bool TryGetById<T>(int id, out T? data) where T : class
    {
        var dataSet = context.Set<T>();
        data = dataSet.Find(id);
        
        return data is not null;
    }

    public T? Get<T>(Expression<Func<T, bool>>? predicate) where T : class
    {
        var dbSet = context.Set<T>();
        return predicate == null ? throw new ArgumentNullException(nameof(predicate)) : dbSet.FirstOrDefault(predicate);
    }

    public List<T> GetAll<T>(Expression<Func<T, bool>>? predicate) where T : class
    {
        var dbSet = context.Set<T>();
        return predicate == null ? dbSet.ToList() : dbSet.Where(predicate).ToList();
    }

    public bool Update<T1, T2>(int id,  Func<T2, T2?> func) where T1 : class, ICopyable<T2>
    {
        var dataSet = context.Set<T1>();
        var foundData = dataSet.Find(id);
        
        if (foundData == null)
            return false;
        
        var modifiedData = func(foundData.CopyTo());
        if(modifiedData == null)
            return false;
        
        foundData.CopyFrom(modifiedData);
        return context.Update(foundData).State == EntityState.Modified;
    }

    public bool Add<T>(T? data) where T : class
    {
        if (data is null)
            return false;

        return context.Add(data).State == EntityState.Added;
    }

    public bool Remove<T>(int id) where T : class
    {
        var dataSet = context.Set<T>();
        var data = dataSet.Find(id);
        
        return data != null && context.Remove(data).State == EntityState.Deleted;
    }

    public T[] ExecuteStoreProcedure<T>(string storedProcedure, Tuple<string, object>[] parameters)
    {
        var queryString = storedProcedure;
        var sqlParameters = new object[parameters.Length];

        for (var i = 0; i < parameters.Length; i++)
        {
            if (i > 0)
                queryString += ",";
         
            queryString += " @" + parameters[i].Item1;
            sqlParameters[i] = new SqlParameter("@" + parameters[i].Item1, parameters[i].Item2);
        }
        
        return context.Database.SqlQueryRaw<T>(queryString, sqlParameters).ToArray();
    }
    
    public bool SaveChanges() => context.SaveChanges() > 0;
}