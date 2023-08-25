# EntityFramework.Audit
Audit framework for Entity Framework Core.

This framework demonstrates how the EF **ChangeTracker** can be used to audit data changes to an application using EF. 
An example of using Fluent APIs is also provided to configure the audit.

One of the principles of the framework is to allow the consumer to use it without having to apply any base classes to their Entities or DbContext.

## Registering the Audit Tracker

The library works by using an AuditTracker to apply changes to an Audit collection. The AuditTracker can be registered and configured using the following code:

```csharp
services.AddDbContext<WeatherContext>(options =>
{
    options.UseSqlite(Connection);
});
Services.AuditDbContext<WeatherContext>(options =>
{
    // entities must be opted-in for auditing
    options.AuditEntity<Document>();
    options.AuditEntity<DocumentVersion>();

    // additional options can be configured per entity
    options.AuditEntity<WeatherForecast>()
           // ignore properties from audit
           .Ignore(e => e.ModifiedDate)
           // configure actions to be audited. All enabled by default
           .AuditActions(AuditActionFlags.Inserted | AuditActionFlags.Deleted);
});
```
