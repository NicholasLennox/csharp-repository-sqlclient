using DataAccessClass.Common;

namespace DataAccessClass.Repository
{
    /*
        Employee-specific repository contract.

        Extends generic CRUD with queries that are meaningful
        only in the Employee domain.

        The interface name provides the noun (Employee),
        so methods can stay focused on behavior.
    */
    public interface IEmployeeRepository : ICrudRepository<Employee, int>
    {
        List<Employee> GetAllActive();
        List<Employee> GetBySalaryAbove(decimal minSalary);
        List<EmployeeListItem> GetAllShort(bool isActive);
    }
}