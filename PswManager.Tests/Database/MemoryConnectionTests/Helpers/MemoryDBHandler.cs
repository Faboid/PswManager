﻿using PswManager.Database;
using PswManager.Database.DataAccess;
using PswManager.Database.Models;
using PswManager.Utils;
using PswManager.Tests.Database.Generic;
using PswManager.Tests.TestsHelpers;
using System.Linq;

namespace PswManager.Tests.Database.MemoryConnectionTests.Helpers {
    internal class MemoryDBHandler : ITestDBHandler {

        //constructs the class with a default value of five
        public MemoryDBHandler() : this(5) { }

        public MemoryDBHandler(int numDefaultValues) {
            numValues = numDefaultValues;
            factory = new DataFactory(DatabaseType.InMemory);
            dbConnection = factory.GetDataConnection();
            defaultValues = new(numValues);
        }

        readonly int numValues;
        readonly DefaultValues defaultValues;
        readonly DataFactory factory;
        readonly IDataConnection dbConnection;

        public ITestDBHandler SetUpDefaultValues() {
            //reset database
            dbConnection.GetAllAccounts().Value
                .Select(x => x.Value)
                .ForEach(x => dbConnection.DeleteAccount(x.Name));

            Enumerable.Range(0, numValues).ForEach(x => {
                var name = defaultValues.GetValue(x, DefaultValues.TypeValue.Name);
                var password = defaultValues.GetValue(x, DefaultValues.TypeValue.Password);
                var email = defaultValues.GetValue(x, DefaultValues.TypeValue.Email);

                var model = new AccountModel(name, password, email);
                dbConnection.CreateAccount(model);
            });

            return this;
        }

        public IDataFactory GetDBFactory() {
            return factory;
        }
        public DefaultValues GetDefaultValues() {
            return defaultValues;
        }

        public void Dispose() { }

    }
}