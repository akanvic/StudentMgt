using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using static Dapper.SqlMapper;

namespace StudentMgt.Repo.Interface
{
    public interface IDPGenericRepository<TEntity> : IGenericRepository<TEntity> where TEntity : class
    {
        IEnumerable<TEntity> GetData(object filter);
        Task<IEnumerable<TEntity>> GetDataAsync(object filter);
        Task<int> ExecuteAsyncSp(string sql, CommandType commandType, object parameters = null);
        Task<TEntity> ExecuteScalarAsyncSp(string sql, CommandType commandType, object parameters = null);
        Task<IEnumerable<T>> QueryAsyncWithTypeSp<T>(string sql, CommandType commandType, object parameters = null);
        Task<IEnumerable<TEntity>> QueryAsyncSp(string sql, CommandType commandType, object parameters = null);
        Task<TEntity> QueryFirstOrDefaultAsyncSp(string sql, CommandType commandType, object parameters = null);
        Task<Tuple<IEnumerable<T1>, IEnumerable<T2>>> GetMultiple<T1, T2>(string sql, object parameters,
                                            Func<GridReader, IEnumerable<T1>> func1,
                                            Func<GridReader, IEnumerable<T2>> func2);
        Task<Tuple<IEnumerable<T1>>> GetSingleTableList<T1>(string sql, object parameters,
                                            Func<GridReader, IEnumerable<T1>> func1);
        Task<Tuple<IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>>> GetMultiple<T1, T2, T3>(string sql, object parameters,
                                            Func<GridReader, IEnumerable<T1>> func1,
                                            Func<GridReader, IEnumerable<T2>> func2,
                                            Func<GridReader, IEnumerable<T3>> func3);
        Task<Tuple<IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>, IEnumerable<T4>>> GetMultiple<T1, T2, T3, T4>(string sql, object parameters,
                                            Func<GridReader, IEnumerable<T1>> func1,
                                            Func<GridReader, IEnumerable<T2>> func2,
                                            Func<GridReader, IEnumerable<T3>> func3,
                                            Func<GridReader, IEnumerable<T4>> func4);

    }
}