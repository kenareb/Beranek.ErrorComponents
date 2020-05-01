namespace Beranek.ErrorComponents.Demo
{
    using System;
    using System.Data.SqlClient;

    class Program
    {
        static int counter;
        
        // Action that will fail:
        static Action OpenConnection = new Action(() =>
        {
            counter++;

            // Will throw a SqlException with Number = 2... (unless you really have such a database;)
            SqlConnection conn = new SqlConnection(@"Data Source=.;Database=GUARANTEED_TO_FAIL;Connection Timeout=2");
            conn.Open();
        });

        // Function that will fail:
        static Func<SqlConnection> GetConnection = new Func<SqlConnection>(() =>
        {
            counter++;

            // Will throw a SqlException with Number = 2... (unless you really have such a database;)
            SqlConnection conn = new SqlConnection(@"Data Source=.;Database=GUARANTEED_TO_FAIL;Connection Timeout=2");
            conn.Open();
            return conn;
        });


        static void Main(string[] args)
        {
            // ------------------------------------------------------------------------------------
            // Demo "ExceptionTolerantAction"
            // ------------------------------------------------------------------------------------
            counter = 0;
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("Trying to connect, please wait");
            
            var connector1 = new ExceptionTolerantAction<SqlException>(OpenConnection, 3, new LinearRetryStrategy(300))
                .Filter(ex => IsTransient((ex)));

            connector1.Invoke();

            Console.WriteLine(connector1.Exception.Message);
            Console.WriteLine($"Connection attempts: {counter}");

            // ------------------------------------------------------------------------------------
            // Demo "ExceptionTolerantAction" used as Method
            // ------------------------------------------------------------------------------------
            counter = 0; 
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("Trying to connect, please wait");

            var connector2 = new ExceptionTolerantAction<SqlException>(OpenConnection, 3, new LinearRetryStrategy(300))
                .Filter(ex => IsTransient((ex)));
            var openSql = connector2.Method();

            // Try to open the connection:
            openSql();

            Console.WriteLine(connector2.Exception.Message);
            Console.WriteLine($"Connection attempts: {counter}");

            // ------------------------------------------------------------------------------------
            // Demo "ExceptionTolerantFunction":
            // ------------------------------------------------------------------------------------
            counter = 0;
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("Trying to connect, please wait");

            var connector3 = new ExceptionTolerantFunction<SqlException, SqlConnection>(GetConnection, 3, new LinearRetryStrategy(300))
                .Filter(ex => IsTransient((ex)));                
            
            // Try to get the connection:
            var connection = connector3.Invoke();

            Console.WriteLine(connector3.Exception.Message);
            Console.WriteLine($"Connection attempts: {counter}");

            // ------------------------------------------------------------------------------------
            // Demo "ExceptionTolerantFunction" used as a method:
            // ------------------------------------------------------------------------------------
            counter = 0;
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("Trying to connect, please wait");

            var connector4 = new ExceptionTolerantFunction<SqlException, SqlConnection>(GetConnection, 3, new LinearRetryStrategy(300))
                .Filter(ex => IsTransient((ex)));
            var getConnection = connector4.Method();

            // Try to get the connection:
            connection = getConnection();

            Console.WriteLine(connector4.Exception.Message);
            Console.WriteLine($"Connection attempts: {counter}");
        }

        static bool IsTransient(SqlException ex) 
        {
            return ex.Number == 1205 || ex.Number == 2 || ex.Number == 11;
        }
    }

    
}
