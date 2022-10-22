using StudentMgt.Core.Entities;
using Microsoft.Extensions.Options;
using System;
using System.Data;
using System.Data.SqlClient;

namespace StudentMgt.Repo.Infrastructure
{
    public class Connectionfactory : IConnectionFactory
    {
        IOptions<ReadConfig> _con;
        private readonly string connectionString;
        private static string dbSchema = "dbo";
        public Connectionfactory(IOptions<ReadConfig> con)
        {
            _con = con;
            connectionString = _con.Value.DefaultConnection;
        }
        public IDbConnection GetConnection
        {
            get
            {
                var conn = new SqlConnection(connectionString); //factory.CreateConnection(); //
                try
                {
                    //var factory = DbProviderFactories.GetFactory("System.Data.SqlClient");
                    conn.ConnectionString = connectionString;
                    conn.Open();
                    return conn;
                }
                catch(Exception ex)
                {
                    throw new Exception(ex.Message);
                }
                finally
                {
                    conn.Close();
                }
            }
        }



        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        public static class StoredProcedures
        {
            
            //Stored procedure for Students
            public static string uspAddStudents = $"{dbSchema}.usp_AddStudents";
            public static string uspUpdateStudents = $"{dbSchema}.usp_UpdateStudents";
            public static string uspGetAllStudents = $"{dbSchema}.usp_GetAllStudents";
            public static string uspGetAllStudentCount = $"{dbSchema}.usp_GetAllStudentCount";

        }
    }
}
