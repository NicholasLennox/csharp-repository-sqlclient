using System;
using DataAccessClass.Common;
using Microsoft.Data.SqlClient;

namespace DataAccessClass.DataAccess
{
    /*
        Data Access Class (DAC) for Employees.

        Why this exists (separation of concerns):
        - Program/application code should express intent (e.g. "get employees", "add employee").
        - This class handles storage mechanics (SQL text, parameters, connections, mapping).

        Think in layers:
        - Application layer: calls methods like GetEmployeeById / AddEmployee
        - Data access layer: translates those calls into SQL + maps rows back to objects

        ADO.NET flow (you'll see this pattern everywhere):
        1) Connect  - open a connection
        2) Prepare  - SQL + parameters
        3) Execute  - run command (returns a reader for SELECT)
        4) Map      - convert rows to C# objects

        Short-lived connection per call.
        ADO.NET handles connection pooling internally.
    */
    public class EmployeeDataAccess
    {
        private readonly string connectionString =
            "Server=localhost,1433;Database=CompanyDb;User Id=sa;Password=@Admin123;TrustServerCertificate=True;";

        public List<Employee> GetAllEmployees(bool isActive = true)
        {

            // Connect
            using SqlConnection conn = new SqlConnection(connectionString);
            conn.Open();

            // Prepare
            string sql = @"
                SELECT Id, Name, Salary, IsActive 
                FROM Employees 
                WHERE IsActive = @IsActive;
            ";

            using SqlCommand cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@IsActive", isActive);

            // Execute
            using SqlDataReader reader = cmd.ExecuteReader();

            // Map
            List<Employee> employees = new List<Employee>();

            while (reader.Read())
            {
                Employee employee = new Employee(
                    Id: reader.GetInt32(0),
                    Name: reader.GetString(1),
                    Salary: reader.GetDecimal(2),
                    IsActive: reader.GetBoolean(3)
                );

                employees.Add(employee);
            }

            return employees;
        }

        public Employee? GetEmployeeById(int id)
        {
            /*
                "Not found" is a normal outcome, not an exception.
                We return null if the query returns zero rows.

                SqlDataReader starts *before* the first row.
                Read() moves onto the next row (row 1 on the first call).
            */

            // Connect
            using SqlConnection conn = new SqlConnection(connectionString);
            conn.Open();

            // Prepare
            string sql = @"
                SELECT Id, Name, Salary, IsActive
                FROM Employees
                WHERE Id = @Id;
            ";

            using SqlCommand cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@Id", id);

            // Execute
            using SqlDataReader reader = cmd.ExecuteReader();

            if (!reader.Read())
                return null;

            // Map single row -> single object
            return new Employee(
                Id: reader.GetInt32(0),
                Name: reader.GetString(1),
                Salary: reader.GetDecimal(2),
                IsActive: reader.GetBoolean(3)
            );
        }

        public int AddEmployee(Employee employee)
        {
            /*
                INSERT normally returns no data.
                If we want the generated Id back, we must return it from SQL.

                OUTPUT INSERTED.Id returns the Id of the row that was inserted.
                That makes ExecuteScalar() appropriate here (single value result).
            */

            // Connect
            using SqlConnection conn = new SqlConnection(connectionString);
            conn.Open();

            // Prepare
            string sql = @"
                INSERT INTO Employees (Name, Salary, IsActive)
                OUTPUT INSERTED.Id
                VALUES (@Name, @Salary, @IsActive);
            ";

            using SqlCommand cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@Name", employee.Name);
            cmd.Parameters.AddWithValue("@Salary", employee.Salary);
            cmd.Parameters.AddWithValue("@IsActive", employee.IsActive);

            // Execute
            return (int)cmd.ExecuteScalar();
        }

        public bool Exists(int id)
        {
            /*
                Simple existence check (similar to patterns seen in EF, e.g. Any()).

                Query shape: SELECT 1 ... WHERE ...
                - returns 1 if a row exists
                - returns null if no row exists

                In practice, GetEmployeeById returning null often makes Exists unnecessary.
                This method is here mainly as a teaching bridge.
            */

            using var conn = new SqlConnection(connectionString);
            conn.Open();

            string sql = "SELECT 1 FROM Employees WHERE Id = @Id;";

            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@Id", id);

            return cmd.ExecuteScalar() != null;
        }
    }
}