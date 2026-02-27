using DataAccessClass.Common;
using DataAccessClass.DataAccess;
using DataAccessClass.Repository;

internal class Program
{
    private static void Main(string[] args)
    {
        // DoAccessWithDataAccessClass();
        DoAccessWithRepository();
    }

    private static void DoAccessWithDataAccessClass()
    {
        var da = new EmployeeDataAccess();

        Console.WriteLine("Before insert:");
        da.GetAllEmployees().ForEach(Console.WriteLine);

        var newEmployee = new Employee("Nicholas Lennox", 10000m);
        int newId = da.AddEmployee(newEmployee);

        Console.WriteLine($"Inserted employee with Id = {newId}");

        var inserted = da.GetEmployeeById(newId);
        Console.WriteLine("Fetched inserted:");
        Console.WriteLine(inserted);

        Console.WriteLine("After insert:");
        da.GetAllEmployees().ForEach(Console.WriteLine);
    }

    private static void DoAccessWithRepository()
    {
        // Program to abstraction (interface), not implementation (class)
        IEmployeeRepository repo = new EmployeeRepository();

        Console.WriteLine("All:");
        repo.GetAll().ForEach(Console.WriteLine);

        Console.WriteLine("Active:");
        repo.GetAllActive().ForEach(Console.WriteLine);

        Console.WriteLine("Short:");
        repo.GetAllShort(true).ForEach(Console.WriteLine);
    }
}