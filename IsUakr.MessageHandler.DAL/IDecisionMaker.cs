using System;
using System.Data;
using System.Threading.Tasks;

namespace IsUakr.MessageHandler.DAL
{
    public interface IDecisionMaker
    {
        Task<string> MakeDecision(IDbConnection conn, int meterId);
        Task TableDelete(string tableName);
        Task Insert(IDbConnection conn, string query);
        Task UsingConnectionAsync(Func<IDbConnection, Task> action);
        void UsingConnection(Action<IDbConnection> action);
        Task<int> GetRowsCount(IDbConnection conn, string tableName);
    }
}
