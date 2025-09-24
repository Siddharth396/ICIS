namespace BusinessLayer.Tests.Services.Region
{
    using System.Threading.Tasks;

    using Serilog;

    using Infrastructure.SQLDB.Repositories;

    using Moq;
    using Infrastructure.SQLDB.Models;
    using BusinessLayer.Services.Region;
    using Xunit;

    public class RegionServiceTest
    {
        Mock<IGenericRepository<RegionCC>> mockGenericRepository;
        Mock<ILogger> mockLogger;
        RegionService regionService;
        public RegionServiceTest() 
        {
            mockGenericRepository=new Mock<IGenericRepository<RegionCC>>();
            mockLogger=new Mock<ILogger>();
            regionService=new RegionService(mockGenericRepository.Object, mockLogger.Object);
        }
        [Fact]
        public async Task GetRegion_Should_Get_Success()
        {
            //Arrange
            RegionCC regionCC = new RegionCC();
            regionCC.Id = 1;
            regionCC.Code = "OdAsia";
            regionCC.Description = "Order_Asia";
            RegionCC regionCC1 = new RegionCC();
            regionCC1.Id = 2;
            regionCC1.Code = "OdEurope";
            regionCC1.Description = "Order_Europe";
            List<RegionCC> regions = new List<RegionCC>() { regionCC, regionCC1 };
            mockGenericRepository.Setup(x => x.GetQueryable()).Returns(regions.AsQueryable());

            //Act
            var response = await regionService.GetRegion();

            //Assert
            Assert.NotNull(response.Data);
            Assert.True(response.IsSuccess);
            Assert.Equal(regions.Count, response.Data.Count());
            Assert.IsAssignableFrom<IEnumerable<RegionCC>>(response.Data);
        }
    }
}
