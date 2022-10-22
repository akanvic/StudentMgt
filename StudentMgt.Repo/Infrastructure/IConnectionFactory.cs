using System;
using System.Data;

namespace StudentMgt.Repo.Infrastructure
{
    public interface IConnectionFactory : IDisposable
    {
        IDbConnection GetConnection { get; }
    }
}
