using Microsoft.EntityFrameworkCore;
using RbacDashboard.DAL.Data;

namespace RbacDashboard.Test.DAL.Base;

/// <summary>
/// An abstract base class for unit tests that involve the RbacSqlDbContext using an in-memory database.
/// Provides methods for setting up the database context and seeding it with test data.
/// </summary>
public abstract class TestBase
{
    /// <summary>
    /// Gets the options for configuring the RbacSqlDbContext.
    /// </summary>
    protected DbContextOptions<RbacSqlDbContext> DbContextOptions { get; private set; }

    /// <summary>
    /// Sets up the in-memory database context options before each test.
    /// </summary>
    [SetUp]
    public void Setup()
    {
        DbContextOptions = new DbContextOptionsBuilder<RbacSqlDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
    }

    /// <summary>
    /// Creates a new instance of RbacSqlDbContext using the configured options.
    /// </summary>
    /// <returns>A new RbacSqlDbContext instance.</returns>
    protected RbacSqlDbContext CreateContext()
    {
        return new RbacSqlDbContext(DbContextOptions);
    }

    /// <summary>
    /// Seeds the in-memory database with a specified number of entities.
    /// </summary>
    /// <typeparam name="T">The type of entities to seed.</typeparam>
    /// <param name="createEntity">A function that generates an entity instance given an integer index.</param>
    /// <param name="count">The number of entities to create.</param>
    protected void SeedData<T>(Func<int, T> createEntity, int count) where T : class
    {
        using var context = CreateContext();
        var entities = new List<T>();
        for (int i = 0; i < count; i++)
        {
            entities.Add(createEntity(i));
        }

        context.Set<T>().AddRange(entities);
        context.SaveChanges();
    }

    /// <summary>
    /// Seeds the specified list of entries into the in-memory database.
    /// </summary>
    /// <typeparam name="T">The type of the entities to seed. Must be a class.</typeparam>
    /// <param name="Entries">The list of entries to add to the database.</param>
    protected void SeedData<T>(List<T> Entries) where T : class
    {
        using var context = CreateContext();
        var entities = new List<T>();
        context.Set<T>().AddRange(Entries);
        context.SaveChanges();
    }
}