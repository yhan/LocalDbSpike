namespace LocalDBTest
{
    using System;
    using System.Data.SqlClient;
    using System.IO;
    using System.Linq;
    using System.Reflection;

    using Dapper;

    class Program
    {
        private const string DatabaseName = "DummyDatabase";

        private static string _connectionStringToMaster;

        static void Main(string[] args)
        {
            _connectionStringToMaster = @"Data Source=(localdb)\v11.0;Initial Catalog=master;Integrated Security=True;Trusted_Connection=yes";

            using (var sqlConnection = new SqlConnection(_connectionStringToMaster))
            {
                TryDetachDatabase(sqlConnection);
                AttachDatabase(sqlConnection);
            }

            string connectionStringToDummyDatabase = $@"Data Source=(localdb)\v11.0;Initial Catalog={DatabaseName};Integrated Security=True;Trusted_Connection=yes";
            using (var sqlConnection = new SqlConnection(connectionStringToDummyDatabase))
            {
                sqlConnection.Open();

                var tables = sqlConnection.Query<Table>("SELECT id FROM [Table]");

                Console.WriteLine($"Line counts = {tables.Count()}");
            }

            Console.WriteLine("Connection disposed");
            Console.ReadKey();
        }

        private static void AttachDatabase(SqlConnection connection)
        {
            var currentDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var databasePath = Path.Combine(currentDirectory, @"Caraibes-local.mdf");

            connection.Execute($@"create database [{DatabaseName}] on (filename = '{databasePath}') for attach;");
        }

        private static void TryDetachDatabase(SqlConnection connection)
        {
            connection.Execute(
                $@"if db_id('{DatabaseName}') is not null
				begin
					alter database [{DatabaseName}] set offline with rollback immediate;
					exec sp_detach_db '{DatabaseName}';
				end
			");
        }
    }

    public class Table
    {
        public int Id { get; set; }
    }
}