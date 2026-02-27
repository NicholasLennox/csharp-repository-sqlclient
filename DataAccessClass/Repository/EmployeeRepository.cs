using DataAccessClass.Common;
using Microsoft.Data.SqlClient;

namespace DataAccessClass.Repository
{
    /*
        Concrete implementation of IEmployeeRepository.

        This class:
        - Implements the abstraction defined by the interfaces
        - Contains all SQL details
        - Translates between database rows and domain objects

        Application code should depend on IEmployeeRepository,
        not on this concrete class.
    */
    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly string connectionString =
            "Server=localhost,1433;Database=CompanyDb;User Id=sa;Password=@Admin123;TrustServerCertificate=True;";

        public List<Employee> GetAll()
        {
            // Connect
            using var conn = new SqlConnection(connectionString);
            conn.Open();

            // Prepare
            string sql = @"
                SELECT Id, Name, Salary, IsActive
                FROM Employees;
            ";

            using var cmd = new SqlCommand(sql, conn);

            // Execute
            using var reader = cmd.ExecuteReader();

            // Map
            var employees = new List<Employee>();

            while (reader.Read())
                employees.Add(MapEmployee(reader));

            return employees;
        }

        public List<Employee> GetAllActive()
        {
            // Connect
            using var conn = new SqlConnection(connectionString);
            conn.Open();

            // Prepare
            string sql = @"
                SELECT Id, Name, Salary, IsActive
                FROM Employees
                WHERE IsActive = 1;
            ";

            using var cmd = new SqlCommand(sql, conn);

            // Execute
            using var reader = cmd.ExecuteReader();

            // Map
            var employees = new List<Employee>();

            while (reader.Read())
                employees.Add(MapEmployee(reader));

            return employees;
        }

        public List<Employee> GetBySalaryAbove(decimal minSalary)
        {
            // Connect
            using var conn = new SqlConnection(connectionString);
            conn.Open();

            // Prepare
            string sql = @"
                SELECT Id, Name, Salary, IsActive
                FROM Employees
                WHERE Salary > @MinSalary;
            ";

            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@MinSalary", minSalary);

            // Execute
            using var reader = cmd.ExecuteReader();

            // Map
            var employees = new List<Employee>();

            while (reader.Read())
                employees.Add(MapEmployee(reader));

            return employees;
        }

        public List<EmployeeListItem> GetAllShort(bool isActive)
        {
            /*
                Example of a projection.

                We are intentionally selecting fewer columns.
                This keeps queries focused on what the caller needs.
            */
            // Prepare
            using var conn = new SqlConnection(connectionString);
            conn.Open();

            string sql = @"
                SELECT Id, Name
                FROM Employees
                WHERE IsActive = @IsActive;
            ";

            // Execute
            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@IsActive", isActive);

            using var reader = cmd.ExecuteReader();

            var employees = new List<EmployeeListItem>();

            // Map
            while (reader.Read())
                employees.Add(MapEmployeeListItem(reader));

            return employees;
        }

        public Employee? GetById(int id)
        {
            // Connect
            using var conn = new SqlConnection(connectionString);
            conn.Open();

            // Prepare
            string sql = @"
                SELECT Id, Name, Salary, IsActive
                FROM Employees
                WHERE Id = @Id;
            ";

            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@Id", id);

            // Execute
            using var reader = cmd.ExecuteReader();

            if (!reader.Read())
                return null;

            // Map
            return MapEmployee(reader);
        }

        public int Add(Employee employee)
        {
            // Connect
            using var conn = new SqlConnection(connectionString);
            conn.Open();

            // Prepare
            string sql = @"
                INSERT INTO Employees (Name, Salary, IsActive)
                OUTPUT INSERTED.Id
                VALUES (@Name, @Salary, @IsActive);
            ";

            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@Name", employee.Name);
            cmd.Parameters.AddWithValue("@Salary", employee.Salary);
            cmd.Parameters.AddWithValue("@IsActive", employee.IsActive);
            
            // Execute
            return (int)cmd.ExecuteScalar();
        }

        public void Update(Employee employee)
        {
            // Connect
            using var conn = new SqlConnection(connectionString);
            conn.Open();

            string sql = @"
                UPDATE Employees
                SET Name = @Name,
                    Salary = @Salary,
                    IsActive = @IsActive
                WHERE Id = @Id;
            ";

            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@Id", employee.Id);
            cmd.Parameters.AddWithValue("@Name", employee.Name);
            cmd.Parameters.AddWithValue("@Salary", employee.Salary);
            cmd.Parameters.AddWithValue("@IsActive", employee.IsActive);

            cmd.ExecuteNonQuery();
        }

        public void Delete(int id)
        {
            using var conn = new SqlConnection(connectionString);
            conn.Open();

            string sql = "DELETE FROM Employees WHERE Id = @Id;";

            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@Id", id);

            cmd.ExecuteNonQuery();
        }


        /*
            Useful methods to help with mapping, since its a repetitive process
        */
        private static Employee MapEmployee(SqlDataReader reader)
        {
            return new Employee(
                Id: reader.GetInt32(0),
                Name: reader.GetString(1),
                Salary: reader.GetDecimal(2),
                IsActive: reader.GetBoolean(3)
            );
        }

        private static EmployeeListItem MapEmployeeListItem(SqlDataReader reader)
        {
            return new EmployeeListItem(
                Id: reader.GetInt32(0),
                Name: reader.GetString(1)
            );
        }
    }
}