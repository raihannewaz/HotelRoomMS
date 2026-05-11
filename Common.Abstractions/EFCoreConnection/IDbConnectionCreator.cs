using System.Data;

namespace Common.Abstractions.EFCoreConnection
{
    public interface IDbConnectionCreator : IDisposable
    {
        IDbConnection GetOrCreateConnection();
    }
}
