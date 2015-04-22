namespace Domain.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CustomersAndOrders : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Customers",
                c => new
                    {
                        CustomerId = c.Guid(nullable: false),
                        FirstName = c.String(),
                        LastName = c.String(),
                        Email = c.String(),
                        CustomerCreditLimit = c.Decimal(precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.CustomerId);
            
            CreateTable(
                "dbo.Orders",
                c => new
                    {
                        OrderId = c.Guid(nullable: false),
                        CustomerId = c.Guid(nullable: false),
                        CustomerFullName = c.String(),
                        TotalValue = c.Decimal(precision: 18, scale: 2),
                        TotalPaid = c.Decimal(precision: 18, scale: 2),
                        ShippingAddress_Address1 = c.String(),
                        ShippingAddress_Address2 = c.String(),
                        ShippingAddress_Suburb = c.String(),
                        ShippingAddress_Postcode = c.String(),
                        ShippingAddress_State = c.String(),
                        ShippingAddress_Country = c.String(),
                        OrderStatus = c.Int(nullable: false),
                        Version = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.OrderId);
            
            CreateTable(
                "dbo.OrderLines",
                c => new
                    {
                        OrderLineId = c.Int(nullable: false, identity: true),
                        OrderId = c.Guid(nullable: false),
                        ProductName = c.String(),
                        QTY = c.Int(nullable: false),
                        Price = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.OrderLineId)
                .ForeignKey("dbo.Orders", t => t.OrderId, cascadeDelete: true)
                .Index(t => t.OrderId);
            
            CreateTable(
                "dbo.OrderPayments",
                c => new
                    {
                        OrderPaymentId = c.Int(nullable: false, identity: true),
                        OrderId = c.Guid(nullable: false),
                        PaymentDate = c.DateTime(nullable: false),
                        PaymentAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        IsSuccessful = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.OrderPaymentId)
                .ForeignKey("dbo.Orders", t => t.OrderId, cascadeDelete: true)
                .Index(t => t.OrderId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.OrderPayments", "OrderId", "dbo.Orders");
            DropForeignKey("dbo.OrderLines", "OrderId", "dbo.Orders");
            DropIndex("dbo.OrderPayments", new[] { "OrderId" });
            DropIndex("dbo.OrderLines", new[] { "OrderId" });
            DropTable("dbo.OrderPayments");
            DropTable("dbo.OrderLines");
            DropTable("dbo.Orders");
            DropTable("dbo.Customers");
        }
    }
}
