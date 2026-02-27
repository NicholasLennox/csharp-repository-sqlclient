namespace DataAccessClass.Common
{
    /*
        Domain models used by the application.

        These types represent business data, not database mechanics.
        They should not depend on SqlClient or SQL concepts.

        The data access layer maps database rows into these records.
    */
    public record Employee(
        string Name,
        decimal Salary,
        bool IsActive = true,
        int Id = 0
    );

    /*
        Lightweight projection for scenarios where we do not need
        the full Employee object (e.g. dropdown lists, summaries).

        This encourages selecting only the data we actually need.
    */
    public record EmployeeListItem(int Id, string Name);
}