using StudentMgt.Repo.Helper;
using StudentMgt.Repo.Infrastructure;
using StudentMgt.Repo.Interface;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using static Dapper.SqlMapper;

namespace StudentMgt.Repo.Implementation
{
    public class GenericRepository<TEntity> : IDPGenericRepository<TEntity> where TEntity : class
    {
        private readonly IDbConnection conn;

        /// <summary>
        /// Manager Qry Constructor.
        /// </summary>
        public IParameter<TEntity> partsQryGenerator { private get; set; }
        /// <summary>
        /// Manager to worker with Identified Fields
        /// </summary>
        public IIDentityInspector<TEntity> identityInspector { private get; set; }

        /// <summary>
        /// Idetenfier parameter (@) to SqlServer (:) to Oracle
        /// </summary>
        public char ParameterIdentified { get; set; }


        protected string qrySelect { get; set; }
        protected string qryInsert { get; set; }



        private string identityField;

        public Task FindAsync()
        {
            throw new NotImplementedException();
        }


        /// <summary>
        /// Create a GenericRepository for Dapper
        /// </summary>
        /// <param name="conn">Connection</param>
        /// <param name="parameterIdentified">Idetenfier parameter (@) to SqlServer (:) to Oracle</param>
        public GenericRepository(IConnectionFactory conn)
        {
            var con = conn.GetConnection;
            if (con == null) throw new ArgumentNullException(nameof(con), $"The parameter {nameof(con)} can't be null");

            this.conn = con;
            ParameterIdentified = '@';
            partsQryGenerator = new Parameter<TEntity>(ParameterIdentified);
            identityInspector = new IDentityInspector<TEntity>(con);
        }

        #region Creates Qries

        protected virtual void CreateSelectQry()
        {
            if (string.IsNullOrWhiteSpace(qrySelect))
            {
                qrySelect = partsQryGenerator.GenerateSelect();
            }
        }

        protected virtual void CreateInsertQry()
        {
            if (string.IsNullOrWhiteSpace(qryInsert))
            {
                identityField = identityInspector.GetColumnsIdentityForType();

                qryInsert = partsQryGenerator.GeneratePartInsert(identityField);
            }
        }

        #endregion


        /// <summary>
        /// Get all entities
        /// </summary>
        /// <returns>All entities</returns>
        #region All/Async

        public IEnumerable<TEntity> All()
        {
            CreateSelectQry();

            IEnumerable<TEntity> result = conn.Query<TEntity>(qrySelect);

            return result;

        }

        /// <summary>
        /// Get Async all entities
        /// </summary>
        /// <returns>All entities</returns>
        public Task<IEnumerable<TEntity>> AllAsync()
        {
            return Task.Run(() =>
            {
                return All();
            });
        }



        #endregion

        #region GetData/Async

        /// <summary>
        /// Get Entities in DB from qry with object parameters
        /// </summary>
        /// <param name="filter"></param>
        /// <returns>Entities for this filter</returns>
        public IEnumerable<TEntity> GetData(string qry, object parameters)
        {
            ParameterValidator.ValidateString(qry, nameof(qry));
            ParameterValidator.ValidateObject(parameters, nameof(parameters));

            var result = conn.Query<TEntity>(qry, parameters);

            return result;
        }

        /// <summary>
        /// Get async Entities in DB from qry with object parameters
        /// </summary>
        /// <param name="filter"></param>
        /// <returns>Entities for this filter</returns>
        public Task<IEnumerable<TEntity>> GetDataAsync(string qry, object parameters)
        {
            ParameterValidator.ValidateString(qry, nameof(qry));
            ParameterValidator.ValidateObject(parameters, nameof(parameters));

            var result = conn.QueryAsync<TEntity>(qry, parameters);

            return result;
        }

        /// <summary>
        /// Get Entities in DB from a object filter (equals property with value) 
        /// Example:  new { Name = "Peter", Age = 18 } --> Select * ... Where Name = 'Peter' and Age = 18
        /// </summary>
        /// <param name="filter"></param>
        /// <returns>Entities for this filter</returns>
        public IEnumerable<TEntity> GetData(object filter)
        {
            ParameterValidator.ValidateObject(filter, nameof(filter));

            var selectQry = partsQryGenerator.GenerateSelect(filter);

            var result = conn.Query<TEntity>(selectQry, filter);

            return result;
        }

        /// <summary>
        /// Get async Entities in DB from a object filter (equals property with value) (DP)
        /// Example:  new { Name = "Peter", Age = 18 } --> Select * ... Where Name = 'Peter' and Age = 18
        /// </summary>
        /// <param name="filter"></param>
        /// <returns>Task with Entities for this filter</returns>
        public Task<IEnumerable<TEntity>> GetDataAsync(object filter)
        {
            return Task.Run(() =>
            {
                return GetData(filter);
            });
        }

        #endregion

        #region Find/Async

        /// <summary>
        /// Find entity in DB for PK
        /// </summary>
        /// <param name="pksFields">Object with pk properties</param>
        /// <returns>Entity for pk values</returns>
        public TEntity Find(object pksFields)
        {
            ParameterValidator.ValidateObject(pksFields, nameof(pksFields));

            var selectQry = partsQryGenerator.GenerateSelect(pksFields);

            var result = conn.Query<TEntity>(selectQry, pksFields).FirstOrDefault();

            return result;
        }

        public IEnumerable<TEntity> GetAll(object pksFields)
        {
            ParameterValidator.ValidateObject(pksFields, nameof(pksFields));

            var selectQry = partsQryGenerator.GenerateSelect(pksFields);

            var result = conn.Query<TEntity>(selectQry, pksFields).ToList();

            return result;
        }
        public async Task<IEnumerable<TEntity>> GetAllAsync(object pksFields)
        {
            return await Task.Run(() =>
            {
                return GetAll(pksFields).ToList();
            });
        }
        public bool Any(object pksFields)
        {
            ParameterValidator.ValidateObject(pksFields, nameof(pksFields));

            var selectQry = partsQryGenerator.GenerateSelect(pksFields);

            var result = conn.Query<TEntity>(selectQry, pksFields).Any();

            return result;
        }
        public async Task<bool> AnyAsync(object pksFields)
        {
            return await Task.Run(() =>
            {
                return Any(pksFields);
            });
        }

        /// <summary>
        /// Find Async entity in DB for PK
        /// </summary>
        /// <param name="pksFields">Object with pk properties</param>
        /// <returns>Entity for pk values</returns>
        public Task<TEntity> FindAsync(object pksFields)
        {
            return Task.Run(() =>
            {
                return Find(pksFields);
            });
        }

        #endregion

        #region Add/Async

        /// <summary>
        /// Add a new Entity in DB
        /// </summary>
        /// <param name="entity">ha</param>
        /// <returns>number cnges in DB</returns>
        public int Add(TEntity entity)
        {
            if (conn == null) throw new ArgumentNullException(nameof(entity), $"The parameter {nameof(entity)} can't be null");

            CreateInsertQry();

            int result = conn.Execute(qryInsert, entity);

            return result;
        }


        /// <summary>
        /// Add Async a new Entity in DB
        /// </summary>
        /// <param name="entity"></param>
        /// <returns>number changes in DB</returns>
        public Task<int> AddAsync(TEntity entity)
        {
            if (conn == null) throw new ArgumentNullException(nameof(entity), $"The parameter {nameof(entity)} can't be null");

            CreateInsertQry();

            var result = conn.ExecuteAsync(qryInsert, entity);

            return result;

        }


        /// <summary>
        /// Add a Sequence of items in BulkInsert
        /// </summary>
        /// <param name="entities">Sequence of entities</param>
        /// <returns>number changes in DB</returns>
        public int Add(IEnumerable<TEntity> entities)
        {
            ParameterValidator.ValidateEnumerable(entities, nameof(entities));

            CreateInsertQry();

            int result = conn.Execute(qryInsert, entities);

            return result;
        }

        /// <summary>
        /// Add Async a Sequence of items in BulkInsert (DP)
        /// </summary>
        /// <param name="entities">Sequence of entities</param>
        /// <returns>number changes in DB</returns>
        public Task<int> AddAsync(IEnumerable<TEntity> entities)
        {
            ParameterValidator.ValidateEnumerable(entities, nameof(entities));

            CreateInsertQry();

            var result = conn.ExecuteAsync(qryInsert, entities);

            return result;
        }

        #endregion

        #region Remove/Async

        public void Remove(object key)
        {
            ParameterValidator.ValidateObject(key, nameof(key));

            var deleteQry = partsQryGenerator.GenerateDelete(key);

            conn.Execute(deleteQry, key);
        }

        public Task RemoveAsync(object key)
        {
            ParameterValidator.ValidateObject(key, nameof(key));

            var deleteQry = partsQryGenerator.GenerateDelete(key);

            return conn.ExecuteAsync(deleteQry, key);
        }


        #endregion



        #region Update/Async

        public int Update(TEntity entity, object pks)
        {
            ParameterValidator.ValidateObject(entity, nameof(entity));
            ParameterValidator.ValidateObject(pks, nameof(pks));

            var updateQry = partsQryGenerator.GenerateUpdate(pks);

            var result = conn.Execute(updateQry, entity);

            return result;
        }

        public Task<int> UpdateAsync(TEntity entity, object pks)
        {
            ParameterValidator.ValidateObject(entity, nameof(entity));
            ParameterValidator.ValidateObject(pks, nameof(pks));

            var updateQry = partsQryGenerator.GenerateUpdate(pks);

            var result = conn.ExecuteAsync(updateQry, entity);

            return result;
        }

        #endregion

        #region InsertOrUpdate/Async

        public int InstertOrUpdate(TEntity entity, object pks)
        {
            ParameterValidator.ValidateObject(entity, nameof(entity));
            ParameterValidator.ValidateObject(pks, nameof(pks));

            int result = 0;

            var entityInTable = Find(pks);

            if (entityInTable == null)
            {
                result = Add(entity);
            }
            else
            {
                result = Update(entity, pks);
            }

            return result;
        }

        public Task<int> InstertOrUpdateAsync(TEntity entity, object pks)
        {
            return Task.Run(() => InstertOrUpdate(entity, pks));
        }



        #endregion
        #region StoredProcedure/Query
        public async Task<int> ExecuteAsyncSp(string sql, CommandType commandType, object parameters = null)
        {
            //_logger.LogDebug($"QUERY EXECUTE COMMAND | { sql.Trim() }");
            //_logger.LogDebug($"QUERY EXECUTE PARAMETERS | { parameters }");

            //var transaction = conn.BeginTransaction();

            return await conn.ExecuteAsync(
                sql: sql,
                param: parameters,
                transaction: null,
                commandType: commandType
            );

            //_logger.LogDebug("QUERY EXECUTE EXECUTED");
        }


        public async Task<int> ExecuteAsyncSpTran(string sql, CommandType commandType, object parameters = null)
        {
            //_logger.LogDebug($"QUERY EXECUTE COMMAND | { sql.Trim() }");
            //_logger.LogDebug($"QUERY EXECUTE PARAMETERS | { parameters }");
            using var transaction = conn.BeginTransaction();
            try
            {

                var result = await conn.ExecuteAsync(
                    sql: sql,
                    param: parameters,
                    transaction: transaction,
                    commandType: commandType
                );
                transaction.Commit();
                return result;
            }
            catch (Exception)
            {
                transaction.Rollback();
                throw;
            }

            //_logger.LogDebug("QUERY EXECUTE EXECUTED");
        }

        public async Task<TEntity> ExecuteScalarAsyncSp(string sql, CommandType commandType, object parameters = null)
        {
            /*_logger.LogDebug($"QUERY SCALAR COMMAND | { sql.Trim() }");
            _logger.LogDebug($"QUERY SCALAR PARAMETERS | { parameters }");*/
            // var transaction = conn.BeginTransaction();

            var result = await conn.ExecuteScalarAsync<TEntity>(
                sql: sql,
                param: parameters,
                //transaction: transaction,
                commandType: commandType
            );

            // _logger.LogDebug("QUERY SCALAR EXECUTED");

            return result;
        }

        public async Task<IEnumerable<TEntity>> QueryAsyncSp(string sql, CommandType commandType, object parameters = null)
        {
            /*_logger.LogDebug($"QUERY LIST COMMAND | { sql.Trim() }");
            _logger.LogDebug($"QUERY LIST PARAMETERS | { parameters }");*/

            //var transaction = conn.BeginTransaction();

            var result = await conn.QueryAsync<TEntity>(
                sql: sql,
                param: parameters,
                //transaction: transaction,
                commandType: commandType
            );

            // _logger.LogDebug("QUERY LIST EXECUTED");

            return result;
        }

        public async Task<IEnumerable<T>> QueryAsyncWithTypeSp<T>(string sql, CommandType commandType, object parameters = null)
        {
            /*_logger.LogDebug($"QUERY LIST COMMAND | { sql.Trim() }");
            _logger.LogDebug($"QUERY LIST PARAMETERS | { parameters }");*/

            //var transaction = conn.BeginTransaction();

            var result = await conn.QueryAsync<T>(
                sql: sql,
                param: parameters,
                //transaction: transaction,
                commandType: commandType
            );

            //string jsonConvert = JsonConvert.SerializeObject(result);
            //var finalResponse = JsonConvert.DeserializeObject<T>(jsonConvert);

            // _logger.LogDebug("QUERY LIST EXECUTED");

            return result;
        }


        public async Task<IEnumerable<T>> QueryAllAsyncWithTypeSp<T>(string sql, CommandType commandType, object parameters = null)
        {
            /*_logger.LogDebug($"QUERY LIST COMMAND | { sql.Trim() }");
            _logger.LogDebug($"QUERY LIST PARAMETERS | { parameters }");*/

            //var transaction = conn.BeginTransaction();

            var result = await conn.QueryAsync<T>(
                sql: sql,
                param: parameters,
                //transaction: transaction,
                commandType: commandType
            );

            //string jsonConvert = JsonConvert.SerializeObject(result);
            //var finalResponse = JsonConvert.DeserializeObject<T>(jsonConvert);

            // _logger.LogDebug("QUERY LIST EXECUTED");

            return result;
        }


        public async Task<TEntity> QueryFirstOrDefaultAsyncSp(string sql, CommandType commandType, object parameters = null)
        {
            /*_logger.LogDebug($"QUERY GET COMMAND | { sql.Trim() }");
            _logger.LogDebug($"QUERY GET PARAMETERS | { parameters }");*/

            //var transaction = conn.BeginTransaction();

            var result = await conn.QueryFirstOrDefaultAsync<TEntity>(
                      sql: sql,
                      param: parameters,
                      commandType: commandType
                  );

            //_logger.LogDebug("QUERY GET EXECUTED");

            return result;
        }





        public async Task<T> QueryFirstOrDefaultAsyncWithTypeSp<T>(string sql, CommandType commandType, object parameters = null)
        {
            /*_logger.LogDebug($"QUERY GET COMMAND | { sql.Trim() }");
            _logger.LogDebug($"QUERY GET PARAMETERS | { parameters }");*/

            //var transaction = conn.BeginTransaction();

            var result = await conn.QueryFirstOrDefaultAsync<T>(
                      sql: sql,
                      param: parameters,
                      commandType: commandType
                  );

            //_logger.LogDebug("QUERY GET EXECUTED");

            return result;
        }






        public async Task<Tuple<IEnumerable<T1>, IEnumerable<T2>>> GetMultiple<T1, T2>(string sql, object parameters,
                                        Func<GridReader, IEnumerable<T1>> func1,
                                        Func<GridReader, IEnumerable<T2>> func2)
        {
            var objs = await getMultiple(sql, parameters, func1, func2);
            return Tuple.Create(objs[0] as IEnumerable<T1>, objs[1] as IEnumerable<T2>);
        }
        public async Task<Tuple<IEnumerable<T1>>> GetSingleTableList<T1>(string sql, object parameters,
                                        Func<GridReader, IEnumerable<T1>> func1)
        {
            var objs = await getMultiple(sql, parameters, func1);
            return Tuple.Create(objs[0] as IEnumerable<T1>);
        }

        public async Task<Tuple<IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>>> GetMultiple<T1, T2, T3>(string sql, object parameters,
                                        Func<GridReader, IEnumerable<T1>> func1,
                                        Func<GridReader, IEnumerable<T2>> func2,
                                        Func<GridReader, IEnumerable<T3>> func3)
        {
            var objs = await getMultiple(sql, parameters, func1, func2, func3);
            return Tuple.Create(objs[0] as IEnumerable<T1>, objs[1] as IEnumerable<T2>, objs[2] as IEnumerable<T3>);
        }
        public async Task<Tuple<IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>, IEnumerable<T4>>> GetMultiple<T1, T2, T3, T4>(string sql, object parameters,
                                        Func<GridReader, IEnumerable<T1>> func1,
                                        Func<GridReader, IEnumerable<T2>> func2,
                                        Func<GridReader, IEnumerable<T3>> func3,
                                        Func<GridReader, IEnumerable<T4>> func4)
        {
            var objs = await getMultiple(sql, parameters, func1, func2, func3, func4);
            return Tuple.Create(objs[0] as IEnumerable<T1>, objs[1] as IEnumerable<T2>, objs[2] as IEnumerable<T3>, objs[3] as IEnumerable<T4>);
        }

        public async Task<Tuple<IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>, IEnumerable<T4>, IEnumerable<T5>>> GetMultiple<T1, T2, T3, T4, T5>(string sql, object parameters,
                                        Func<GridReader, IEnumerable<T1>> func1,
                                        Func<GridReader, IEnumerable<T2>> func2,
                                        Func<GridReader, IEnumerable<T3>> func3,
                                        Func<GridReader, IEnumerable<T4>> func4,
                                        Func<GridReader, IEnumerable<T5>> func5)

        {
            var objs = await getMultiple(sql, parameters, func1, func2, func3, func4, func5);
            return Tuple.Create(objs[0] as IEnumerable<T1>, objs[1] as IEnumerable<T2>, objs[2] as IEnumerable<T3>, objs[3] as IEnumerable<T4>, objs[4] as IEnumerable<T5>);
        }
        /*    private DynamicParameters getDynamicParameter(object parameters)
            {
              DynamicParameters _parameters = new DynamicParameters();
              if (parameters != null)
              {
                foreach (var property in parameters.GetType().GetProperties())
                {
                  var name = property.Name;
                  var value = property.GetValue(parameters)?.ToString() ?? null;
                  _parameters.Add($"@{name}", value);

                }

              }
              else return null;

              return _parameters;
            }*/
        private async Task<List<object>> getMultiple(string sql, object parameters, params Func<GridReader, object>[] readerFuncs)
        {
            var returnResults = new List<object>();
            //DynamicParameters dyna = getDynamicParameter(parameters);
            var gridReader = await conn.QueryMultipleAsync(sql, param: parameters, commandType: CommandType.StoredProcedure);

            foreach (var readerFunc in readerFuncs)
            {
                var obj = readerFunc(gridReader);
                returnResults.Add(obj);
            }


            return returnResults;
        }
        #endregion


        public void Dispose()
        {
            throw new NotImplementedException();
        }
            

    }

}
