using System;
using System.Linq.Expressions;
using Domain.Infrastructure;
using Domain.Storage.EF;
using NUnit.Framework;

namespace Domain.Test.Integration
{
    public abstract class SqlCeBaseTest
    {
        string DatabaseConnection { get; set; }

        protected void Repository(Action<Repository> action )
        {
            using (var context = new AppContext(DatabaseConnection))
            using (var repository = new Repository(context))
                action(repository);
        }

        [SetUp]
        public void BaseSetup()
        {
            DatabaseConnection = string.Format("Data Source=domain_{0}.sdf;Persist Security Info=False;", Guid.NewGuid());
            
            
        }

        [TearDown]
        public void BaseTearDown()
        {
            using (var context = new AppContext(DatabaseConnection))
                context.Database.Delete();
        }
    }
}