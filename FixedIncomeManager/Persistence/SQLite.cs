using FixedIncomeManager.Models;
using Microsoft.Data.Sqlite;

namespace FixedIncomeManager.Persistence
{
    internal class SQLite
        : IBondsPersistency<FixedIncomeModel, IndexerModel>
    {
        public string SourceString => throw new NotImplementedException();
        public void Initialize()
        {
            using(var connection = new SqliteConnection(SourceString))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = string.Format(
                    $"{ GetTableBonds() }" +
                    $"{ GetTableIndexers() }" +
                    $"{ GetTableBrokers() }" +
                    $"{ GetTableBondNames() }");
            }
        }
        public ICollection<FixedIncomeModel> GetBonds()
        {
            throw new NotImplementedException();
        }
        public ICollection<IndexerModel> GetRates()
        {
            throw new NotImplementedException();
        }
        public bool SaveBonds(ICollection<FixedIncomeModel> items)
        {
            throw new NotImplementedException();
        }
        public bool SaveRates(ICollection<IndexerModel> rates)
        {
            throw new NotImplementedException();
        }
        private string GetTableBonds()
        {
            return @"CREATE TABLE IF NOT EXISTS BONDS(
                        Capital             REAL    NOT NULL,
                        LastUpdateValue     REAL    NOT NULL,
                        Remuneration        REAL    NOT NULL,
                        Hiring              TEXT    NOT NULL,
                        Maturity            TEXT    NOT NULL,
                        Name                TEXT    NOT NULL,
                        Broker              TEXT    NOT NULL,
                        Indexer             INTEGER NOT NULL,
                        Type                INTEGER NOT NULL,
                        RemunerationType    INTEGER NOT NULL
                    )";
        }
        private string GetTableIndexers()
        {
            return @"CREATE TABLE IF NOT EXISTS INDEXERS(
                        Value       REAL    NOT NULL,
                        UpdatedAt   TEXT    NOT NULL,
                        Type        INTEGER NOT NULL
                    )";
        }
        private string GetTableBrokers()
        {
            return @"CREATE TABLE IF NOT EXISTS BROKERS(
                        Name        TEXT    NOT NULL,
                        Description TEXT
                    )";
        }
        private string GetTableBondNames()
        {
            return @"CREATE TABLE IF NOT EXISTS BOND_NAMES(
                        Name        TEXT    NOT NULL,
                        Description TEXT
                    )";
        }
    }
}
