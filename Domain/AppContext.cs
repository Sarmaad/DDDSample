using System.Data.Entity;
using System.Diagnostics;
using Domain.Models;

namespace Domain
{
    public class AppContext : DbContext, IAppContext
    {
        public AppContext(string connectionName):base(connectionName)
        {
            if (Debugger.IsAttached)
            {
                Database.Log = log => Debug.WriteLine(log);
            }
        }

        public IDbSet<Order> Orders { get; set; }
        public IDbSet<Customer> Customers { get; set; }
        
    }

    public interface IAppContext
    {
        IDbSet<Order> Orders { get; set; }
        IDbSet<Customer> Customers { get; set; }

        int SaveChanges();
    }
}
