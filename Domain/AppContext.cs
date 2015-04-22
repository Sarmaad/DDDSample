using System;
using System.Data.Entity;
using System.Diagnostics;
using Domain.Models;

namespace Domain
{
    public class AppContext : DbContext, IAppContext
    {

        public AppContext()
        {
            
        }

        public AppContext(string connectionString)
            : base(connectionString)
        {
            if (Debugger.IsAttached)
            {
                Database.Log = log => Debug.WriteLine(log);
            }
        }

        public IDbSet<Order> Orders { get; set; }
        public IDbSet<Customer> Customers { get; set; }
        
    }

    public interface IAppContext:IDisposable
    {
        IDbSet<Order> Orders { get; set; }
        IDbSet<Customer> Customers { get; set; }

        int SaveChanges();
    }
}
