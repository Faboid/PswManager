using PswManager.Database.DataAccess;

namespace PswManager.Database.Wrappers;

internal class WrappersBuilder {

    private readonly IDBConnection _connection;

    public WrappersBuilder(IDBConnection connection) {
        _connection = connection;
    }

    public IDataConnection BuildWrappers() {

        //current workflow:
        //consumer call -> validation wrapper -> concurrency wrapper -> check existence wrapper -> edit simplification wrapper -> db call
        EditSimplificationWrapper editSimplificationWrapper = new(_connection);
        CheckExistenceWrapper checkExistenceWrapper = new(editSimplificationWrapper);
        ConcurrencyWrapper concurrencyWrapper = new(checkExistenceWrapper);
        ValidationWrapper validationWrapper = new(concurrencyWrapper);
        return validationWrapper;
    }

}