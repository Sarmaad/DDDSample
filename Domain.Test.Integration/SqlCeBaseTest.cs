using System;
using System.Linq.Expressions;
using NUnit.Framework;

namespace Domain.Test.Integration
{
    public abstract class SqlCeBaseTest
    {
        protected IAppContext Context { get; private set; }
        protected string DatabaseConnection { get; set; }

        protected void Read(Action<IAppContext> action)
        {
            using (var context = new AppContext(DatabaseConnection))
                action(context);
        }
            
        [SetUp]
        public void BaseSetup()
        {
            DatabaseConnection = string.Format("Data Source=domain_{0}.sdf;Persist Security Info=False;", Guid.NewGuid());
            Context = new AppContext(DatabaseConnection);
        }

        [TearDown]
        public void BaseTearDown()
        {
            ((AppContext)Context).Database.Delete();
        }
    }
}