namespace Test.Infrastructure.SQLDB.Repositories
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using global::Infrastructure.SQLDB;
    using global::Infrastructure.SQLDB.Models;
    using global::Infrastructure.SQLDB.Repositories;

    using Microsoft.Data.SqlClient;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Sqlite;
    using Moq;

    using NSubstitute;
    using NSubstitute.ReturnsExtensions;

    using Xunit;

    public class GenericRepositoryTests
    {
        private readonly DbContextOptions<AppDbContext> options;

        public GenericRepositoryTests()
        {
            options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: "SupplyNDemandTest")
                .Options;
        }

        private AppDbContext CreateDbContext() => new(options);

        [Fact]
        public async Task DeleteAsync_ShouldRemoveEntityFromDbContext()
        {
            // Arrange
            var units = new List<Unit>
            {
                new() { Id = 1, Code = "Code 1", Description = "Description 1" },
                new() { Id = 2, Code = "Code 2", Description = "Description 2" }
            };

            using (var context = CreateDbContext())
            {
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();
                context.Set<Unit>().AddRange(units);
                context.SaveChanges();
            }

            using (var context = CreateDbContext())
            {
                var repository = new GenericRepository<Unit>(context);

                // Act
                repository.DeleteAsync(units[0]);
                await context.SaveChangesAsync();

                // Assert
                Assert.Equal(1, await context.Set<Unit>().CountAsync());

                var deletedRecord = await context.Set<Unit>().FindAsync(units[0].Id);
                Assert.Null(deletedRecord);
            }
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnAllEntities()
        {
            // Arrange
            var entities = new List<Unit>
            {
                new() { Id = 1, Code = "Code 1", Description = "Description 1" },
                new() { Id = 2, Code = "Code 2", Description = "Description 2" },
                new() { Id = 3, Code = "Code 3", Description = "Description 3" }
            };

            using (var context = CreateDbContext())
            {
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();
                context.Set<Unit>().AddRange(entities);
                context.SaveChanges();
            }

            using (var context = CreateDbContext())
            {
                var repository = new GenericRepository<Unit>(context);

                // Act
                var result = await repository.GetAllAsync();

                // Assert
                Assert.Equal(entities.Count, result.Count());
                Assert.Equal(entities.Select(e => e.Id), result.Select(e => e.Id));
                Assert.Equal(entities.Select(e => e.Code), result.Select(e => e.Code));
                Assert.Equal(entities.Select(e => e.Description), result.Select(e => e.Description));
            }
        }

        [Fact]
        public async Task ExecuteReadOnlySql_ShouldReturnEntitiesFromDbContext()
        {
            // Arrange
            var outages = new List<Outage>
            {
                new()
                    {
                        OutageStart = "25 Aug 2024 (Market information)",
                        OutageEnd = "12 Nov 2024 (Market information)",
                        Country = "Country 1",
                        Company = "Company 1",
                        Site = "Site 1",
                        PlantNo = 1,
                        Cause = "Scheduled",
                        CapacityLoss = "100% (est. 48.2kt)",
                        TotalAnnualCapacity = 220.00m,
                        LastUpdated = "25 Jan 2024",
                        Comments = "Comments 1"
                    },
                    new()
                    {
                        OutageStart = "26 Sep 2024 (Market information)",
                        OutageEnd = "13 Dec 2024 (Market information)",
                        Country = "Country 2",
                        Company = "Company 2",
                        Site = "Site 2",
                        PlantNo = 1,
                        Cause = "Scheduled",
                        CapacityLoss = "100% (est. 11.0kt)",
                        TotalAnnualCapacity = 130.00m,
                        LastUpdated = "26 Jan 2024",
                        Comments = "--"
                    }
            };

            var mockRepository = new Mock<GenericRepository<Outage>>(CreateDbContext());
            mockRepository.Setup(mockRepository => mockRepository.ExecuteReadOnlySql(It.IsAny<string>(), 
                It.IsAny<List<SqlParameter>>(), 
                It.IsAny<int?>(), It.IsAny<int?>(), 
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(outages);

            using (var context = CreateDbContext())
            {
                var repository = mockRepository.Object;

                // Act
                var sqlParams = new List<SqlParameter>
                {
                    new("@Country", "Country 1")
                };
                var result = await repository.ExecuteReadOnlySql("SELECT * FROM Outages WHERE Country = @Country", sqlParams);

                // Assert
                Assert.Equal(outages.Count, result.Count());
                Assert.Equal(outages.Select(e => e.OutageStart), result.Select(e => e.OutageStart));
                Assert.Equal(outages.Select(e => e.OutageEnd), result.Select(e => e.OutageEnd));
                Assert.Equal(outages.Select(e => e.Country), result.Select(e => e.Country));
                Assert.Equal(outages.Select(e => e.Company), result.Select(e => e.Company));
                Assert.Equal(outages.Select(e => e.Site), result.Select(e => e.Site));
                Assert.Equal(outages.Select(e => e.PlantNo), result.Select(e => e.PlantNo));
                Assert.Equal(outages.Select(e => e.Cause), result.Select(e => e.Cause));
                Assert.Equal(outages.Select(e => e.CapacityLoss), result.Select(e => e.CapacityLoss));
                Assert.Equal(outages.Select(e => e.TotalAnnualCapacity), result.Select(e => e.TotalAnnualCapacity));
                Assert.Equal(outages.Select(e => e.LastUpdated), result.Select(e => e.LastUpdated));
                Assert.Equal(outages.Select(e => e.Comments), result.Select(e => e.Comments));
            }
        }

        [Fact]
        public async Task GetAsync_ShouldReturnEntityFromDbContext()
        {
            // Arrange
            var units = new List<Unit>
            {
                new() { Id = 1, Code = "Code 1", Description = "Description 1" },
                new() { Id = 2, Code = "Code 2", Description = "Description 2" },
                new() { Id = 3, Code = "Code 3", Description = "Description 3" }
            };

            using (var context = CreateDbContext())
            {
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();
                context.Set<Unit>().AddRange(units);
                context.SaveChanges();
            }

            using (var context = CreateDbContext())
            {
                var repository = new GenericRepository<Unit>(context);

                // Act
                var result = await repository.GetAsync(2);

                // Assert
                Assert.NotNull(result);
                Assert.Equal(units[1].Id, result.Id);
                Assert.Equal(units[1].Code, result.Code);
                Assert.Equal(units[1].Description, result.Description);
            }
        }

        [Fact]
        public async Task InsertAsync_ShouldAddEntityToDbContext()
        {
            // Arrange
            var unit = new Unit { Id = 4, Code = "Code 4", Description = "Description 4" };
            var dbContextMock = new Mock<AppDbContext>(options);
            var repository = new GenericRepository<Unit>(dbContextMock.Object);
            dbContextMock.Setup(db => db.Set<Unit>().AddAsync(unit, default))
                .Returns(new ValueTask<Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry<Unit>>(Task.FromResult(dbContextMock.Object.Entry(unit))));

            // Act
            await repository.InsertAsync(unit);

            // Assert
            dbContextMock.Verify(db => db.Set<Unit>().AddAsync(unit, default), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_ShouldUpdateEntityInDbContext()
        {
            // Arrange
            var unit = new Unit { Id = 5, Code = "Code 5", Description = "Description 5" };

            using (var context = CreateDbContext())
            {
                context.Database.EnsureCreated();
                context.Set<Unit>().Add(unit);
                context.SaveChanges();
            }

            using (var context = CreateDbContext())
            {
                var repository = new GenericRepository<Unit>(context);

                // Act
                unit.Description = "Updated Description";
                repository.UpdateAsync(unit);
                await context.SaveChangesAsync();

                // Assert
                Assert.Equal("Updated Description", context.Set<Unit>().Find(unit.Id).Description);
            }
        }
    }
}
