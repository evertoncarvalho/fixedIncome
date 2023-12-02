using FixedIncomeManager.Models;
using Microsoft.Data.Sqlite;

namespace FixedIncomeManager.Persistence
{
    internal class SQLite
        : IBondsPersistency<FixedIncomeModel, IndexerModel>
    {
        public string SourceString => "Data Source=fixedIncome.db";
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
                command.ExecuteNonQuery();
            }
        }
        public ICollection<FixedIncomeModel> GetBonds()
        {
            List<FixedIncomeModel> bonds = new(0);
            using(var connection = new SqliteConnection(SourceString))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = "SELECT * FROM BONDS";
                using(var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        bonds.Add(new FixedIncomeModel(
                            reader.GetString(5),
                            reader.GetString(6),
                            reader.GetDouble(0),
                            reader.GetDouble(1),
                            reader.GetDouble(2),
                            (FixedIncomeType)reader.GetInt32(8),
                            (FixedIncomeRemunerationType)reader.GetInt32(9),
                            (FixedIncomeIndexer)reader.GetInt32(7),
                            reader.GetDateTime(3),
                            reader.GetDateTime(4)));
                    }
                }
            }
            return bonds;
        }
        public ICollection<IndexerModel> GetRates()
        {
            List<IndexerModel> rates = new(0);
            using (var connection = new SqliteConnection(SourceString))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = "SELECT * FROM BONDS";
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        rates.Add(new IndexerModel(
                            (IndexerType)reader.GetInt32(2),
                            reader.GetDouble(0),
                            reader.GetDateTime(1)));
                    }
                }
            }
            return rates;
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
