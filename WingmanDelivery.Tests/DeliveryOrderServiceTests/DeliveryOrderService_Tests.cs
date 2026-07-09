using Moq;
using WingmanDelivery.BusinessLogic.Interfaces;
using WingmanDelivery.BusinessLogic.Services;
using WingmanDelivery.BusinessLogic.UnitOfWork;
using WingmanDelivery.Models;
using WingmanDelivery.Models.Enums;
using WingmanDelivery.Models.Models;

namespace WingmanDelivery.Tests.DeliveryOrderServiceTests
{
    public class DeliveryOrderService_Tests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IDeliveryOrderRepository> _mockOrderRepository;
        private readonly DeliveryOrderService _service;

        public DeliveryOrderService_Tests()
        {
            // 1. Initialize testing mock instances
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockOrderRepository = new Mock<IDeliveryOrderRepository>();

            // ❌ FIXED BUG: Removed the invalid _mockUnitOfWork.Setup(u => u.Orders) mapping completely.

            var fakeContext = new InvokeDataModel
            {
                userUid = Guid.NewGuid(),
                schema = "dbo",
                commandTimeout = 30
            };
            _mockUnitOfWork.Setup(u => u.Data).Returns(fakeContext);

            // 2. Inject our mocks into the production service pipeline constructor parameters
            _service = new DeliveryOrderService(_mockOrderRepository.Object, _mockUnitOfWork.Object);
        }

        [Fact]
        public async Task AddDeliveryOrderAsync_Should_Commit_Transaction_On_Success()
        {
            // Arrange
            var fakeOrder = new DeliveryOrderModel
            {
                f_pickup_address = "789 Express Lane",
                f_status = OrderStatus.Pending,
                f_total_cost = 25.00m
            };

            _mockOrderRepository.Setup(r => r.Add(It.IsAny<DeliveryOrderModel>()))
                .ReturnsAsync((DeliveryOrderModel m) => m);

            // Act
            var result = await _service.AddDeliveryOrderAsync(fakeOrder);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("789 Express Lane", result.f_pickup_address);

            // Verify clean N-Layer transactional lifecycle sequencing
            _mockUnitOfWork.Verify(u => u.BeginAsync(), Times.Once);
            _mockOrderRepository.Verify(r => r.Add(It.IsAny<DeliveryOrderModel>()), Times.Once);
            _mockUnitOfWork.Verify(u => u.CommitAsync(), Times.Once);
            _mockUnitOfWork.Verify(u => u.RollbackAsync(), Times.Never);
        }

        [Fact]
        public async Task AddDeliveryOrderAsync_Should_Trigger_Rollback_On_Database_Failure()
        {
            // Arrange
            var brokenOrder = new DeliveryOrderModel { f_pickup_address = "Deadlock Alley" };

            // Simulate a raw SQL or infrastructure network failure mapping boundary crash
            _mockOrderRepository.Setup(r => r.Add(It.IsAny<DeliveryOrderModel>()))
                .ThrowsAsync(new InvalidOperationException("SQL Server connection pool exhausted."));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _service.AddDeliveryOrderAsync(brokenOrder));

            Assert.Equal("SQL Server connection pool exhausted.", exception.Message);

            // Verify safety properties executed flawlessly
            _mockUnitOfWork.Verify(u => u.BeginAsync(), Times.Once);
            _mockUnitOfWork.Verify(u => u.CommitAsync(), Times.Never); // Never commit corrupt parameters
            _mockUnitOfWork.Verify(u => u.RollbackAsync(), Times.Once); // Must invoke clean rollback boundary transitions
        }

        [Fact]
        public async Task GetDeliveryOrdersForGridAsync_Should_Return_Paginated_Grid_Data()
        {
            // Arrange
            var filter = new FilterModel { Skip = 0, Take = 10, SearchValue = "Hub" };
            var fakeGridResult = new GridDataModel<DeliveryOrderExtendedModel>
            {
                Count = 1,
                Items = new List<DeliveryOrderExtendedModel>
                {
                    new DeliveryOrderExtendedModel { f_pickup_address = "Main Hub Station" }
                }
            };

            _mockOrderRepository.Setup(r => r.GetExtendedForGrid(filter))
                .ReturnsAsync(fakeGridResult);

            // Act
            var result = await _service.GetDeliveryOrdersForGridAsync(filter);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.Count);
            Assert.Single(result.Items);

            // Read-only queries should bypass active transactional overhead boundaries
            _mockUnitOfWork.Verify(u => u.BeginAsync(), Times.Never);
            _mockUnitOfWork.Verify(u => u.CommitAsync(), Times.Never);
        }
    }
}