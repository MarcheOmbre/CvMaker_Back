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
        if(!dbSet.Any())
            return [];
        
        return predicate == null ? dbSet.ToList() : dbSet.Where(predicate).ToList();
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

    public string[] GetModifiedColumns<T>(int id, T data)
    {
        if(data is null)
            return [];
        
        var dbEntityEntry = context.Entry(data);
        return dbEntityEntry.Properties.Where(p => p.IsModified).Select(p => p.Metadata.Name).ToArray();
    }

    public bool SaveChanges()
    {
        try
        {
            context.SaveChanges();
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }
}