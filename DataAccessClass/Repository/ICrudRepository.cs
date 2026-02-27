namespace DataAccessClass.Repository
{
    /*
        Generic CRUD contract.

        Defines the core operations that most repositories support.
        This represents capability, not implementation.

        No SQL knowledge lives here.
        This is purely an abstraction.
    */
    public interface ICrudRepository<T, ID>
    {
        List<T> GetAll();
        T? GetById(ID id);
        ID Add(T entity);
        void Update(T entity);
        void Delete(ID id);
    }
}