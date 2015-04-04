using System;
using NUnit.Framework;

namespace Domain.Test.Integration
{
    public abstract class SqlCeBaseTest
    {
        protected IAppContext Context { get; private set; }

        [SetUp]
        public void BaseSetup()
        {
            Context = new AppContext(string.Format("Data Source=domain_{0}.sdf;Persist Security Info=False;", Guid.NewGuid()));
        }

        [TearDown]
        public void BaseTearDown()
        {
            ((AppContext)Context).Database.Delete();
        }
    }
}