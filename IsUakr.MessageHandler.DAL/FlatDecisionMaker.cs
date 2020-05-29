using Dapper;
using Npgsql;
using System;
using System.Data;
using System.Threading.Tasks;

namespace IsUakr.MessageHandler.DAL
{
    public class FlatDecisionMaker: IDecisionMaker
    {
        private readonly string _connString;

        public FlatDecisionMaker(string connString)
        {
            _connString = connString;
        }

        public void UsingConnection(Action<IDbConnection> action)
        {
            try
            {
                using var conn = new NpgsqlConnection(_connString);
                conn.Open();
                action(conn);
            }
            catch (Exception ex)
            {
                throw new Exception("Возникла ошибка во время работы с БД.\n Message: " + ex.Message);
            }
        }

        public async Task UsingConnectionAsync(Func<IDbConnection, Task> action)
        {
            try
            {
                using var conn = new NpgsqlConnection(_connString);
                await conn.OpenAsync();
                await action(conn);
            }
            catch (Exception ex)
            {
                throw new Exception("Возникла ошибка во время работы с БД.\n Message: " + ex.Message);
            }
        }

        public async Task<string> MakeDecision(IDbConnection conn, int meterId)
        {
            try
            {
                string tableName = null;

                var flatId = await conn.QueryFirstOrDefaultAsync<int>("select \"FlatId\" from \"Meters\" where \"id\" = " + meterId);
                if (flatId != 0)
                {
                    tableName = "flat_" + flatId;
                    if (!await TableExists(tableName, conn))
                        await conn.ExecuteAsync(Utilities.CreateMeterMessageTable(tableName));
                }
                return tableName;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<bool> TableExists(string tableName, IDbConnection conn)
        {
            try
            {
                var res = await conn.QueryFirstOrDefaultAsync<int>($"select count(*) from \"{tableName}\"");
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task TableDelete(string tableName)
        {
            using var conn = new NpgsqlConnection(_connString);
            try
            {
                await conn.OpenAsync();
                await conn.ExecuteAsync($"drop table \"{tableName}\"");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task Insert(IDbConnection conn, string query)
        {
            using var tr = conn.BeginTransaction();
            try
            {
                await conn.ExecuteAsync(query);
                tr.Commit();
            }
            catch (Exception)
            {
                tr.Rollback();
                throw;
            }
        }

        public async Task<int> GetRowsCount(IDbConnection conn, string tableName)
        {
            return await conn.QueryFirstOrDefaultAsync<int>($"select count(*) from {tableName}");
        }
    }
}
