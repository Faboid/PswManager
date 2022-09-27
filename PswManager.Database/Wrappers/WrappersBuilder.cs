using PswManager.Database.DataAccess;

namespace PswManager.Database.Wrappers;

internal class WrappersBuilder {

    private readonly IInternalDBConnection _connection;

    public WrappersBuilder(IInternalDBConnection connection) {
        _connection = connection;
    }

    public IDataConnection BuildWrappers() {
        
        //current workflow:
        //consumer call -> validation wrapper -> concurrency wrapper -> check existence wrapper -> db call
        CheckExistenceWrapper checkExistenceWrapper = new(_connection);
        ConcurrencyWrapper concurrencyWrapper = new(checkExistenceWrapper);
        ValidationWrapper validationWrapper = new(concurrencyWrapper);
        return validationWrapper;
    }

}