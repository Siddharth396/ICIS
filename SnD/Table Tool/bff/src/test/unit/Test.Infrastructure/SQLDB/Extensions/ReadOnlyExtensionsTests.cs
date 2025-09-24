namespace Test.Infrastructure.SQLDB.Extensions
{
    using global::Infrastructure.SQLDB.Extensions;

    using Microsoft.EntityFrameworkCore;

    using Xunit;

    public class ReadOnlyExtensionsTests
    {
        [Fact]
        public void AsReadOnly_ShouldApplyAsNoTracking()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<TestDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDb")
                .Options;

            using var context = new TestDbContext(options);
            context.TestEntities.Add(new TestEntity { Name = "Test" });
            context.SaveChanges();

            // Act
            var query = context.TestEntities.AsReadOnly();

            // Assert
            // Here we assert that the query is indeed treated as read-only.
            // Since there's no direct way to check if AsNoTracking was applied,
            // we ensure that changes to entities fetched by the query are not tracked.
            var entity = query.FirstOrDefault();
            entity.Name = "Modified";

            context.SaveChanges();

            var unchangedEntity = context.TestEntities.AsNoTracking().FirstOrDefault();
            Assert.Equal("Test", unchangedEntity.Name);
        }

        public class TestDbContext : DbContext
        {
            public TestDbContext(DbContextOptions<TestDbContext> options) : base(options) { }

            public DbSet<TestEntity> TestEntities { get; set; }
        }

        public class TestEntity
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }
    }
}
