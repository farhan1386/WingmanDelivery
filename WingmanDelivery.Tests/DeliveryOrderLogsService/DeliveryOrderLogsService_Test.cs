using Moq;
using WingmanDelivery.BusinessLogic.Interfaces;
using WingmanDelivery.BusinessLogic.UnitOfWork;
using WingmanDelivery.Models;

namespace WingmanDelivery.Tests.DeliveryOrderLogsService
{
    public class DeliveryOrderLogsService_Test
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IDeliveryOrderLogsRepository> _mockLogsRepository;
        private readonly WingmanDelivery.BusinessLogic.Services.DeliveryOrderLogsService _service;

        public DeliveryOrderLogsService_Test()
        {
            // 1. Initialize testing mock instances
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockLogsRepository = new Mock<IDeliveryOrderLogsRepository>();

            // 2. Setup mock return structures for dynamic factory resolutions matching Program.cs
            _mockUnitOfWork.Setup(u => u.Logs).Returns(_mockLogsRepository.Object);

            var fakeContext = new InvokeDataModel
            {
                userUid = Guid.NewGuid(),
                schema = "dbo",
                commandTimeout = 30
            };
            _mockUnitOfWork.Setup(u => u.Data).Returns(fakeContext);

            // 3. Inject our mocks into the production service pipeline target
            _service = new WingmanDelivery.BusinessLogic.Services.DeliveryOrderLogsService(_mockUnitOfWork.Object);
        }

        [Fact]
        public async Task AddDeliveryOrderLogAsync_Should_Commit_Transaction_On_Success()
        {
            // Arrange
            var fakeLog = new DeliveryLogsModel
            {
                f_delivery_uid = Guid.NewGuid(),
                f_status_from = 0, // Pending
                f_status_to = 1    // DriverAssigned
            };

            _mockLogsRepository.Setup(r => r.Add(It.IsAny<DeliveryLogsModel>()))
                .ReturnsAsync((DeliveryLogsModel m) => m);

            // Act
            var result = await _service.AddDeliveryOrderLogAsync(fakeLog);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(fakeLog.f_delivery_uid, result.f_delivery_uid);

            // Verify clean N-Layer transactional lifecycle sequencing for audit logging
            _mockUnitOfWork.Verify(u => u.BeginAsync(), Times.Once);
            _mockLogsRepository.Verify(r => r.Add(It.IsAny<DeliveryLogsModel>()), Times.Once);
            _mockUnitOfWork.Verify(u => u.CommitAsync(), Times.Once);
            _mockUnitOfWork.Verify(u => u.RollbackAsync(), Times.Never);
        }

        [Fact]
        public async Task AddDeliveryOrderLogAsync_Should_Trigger_Rollback_On_Database_Failure()
        {
            // Arrange
            var brokenLog = new DeliveryLogsModel { f_delivery_uid = Guid.NewGuid() };

            // Simulate a raw SQL server connection deadlock scenario inside the transaction pool
            _mockLogsRepository.Setup(r => r.Add(It.IsAny<DeliveryLogsModel>()))
                .ThrowsAsync(new InvalidOperationException("SQL Server Deadlock Connection Timeout"));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _service.AddDeliveryOrderLogAsync(brokenLog));

            Assert.Equal("SQL Server Deadlock Connection Timeout", exception.Message);

            // Verify safety properties executed flawlessly
            _mockUnitOfWork.Verify(u => u.BeginAsync(), Times.Once);
            _mockUnitOfWork.Verify(u => u.CommitAsync(), Times.Never); // Never commit corrupt records
            _mockUnitOfWork.Verify(u => u.RollbackAsync(), Times.Once); // Must invoke clean rollback boundary transitions
        }

        [Fact]
        public async Task GetDeliveryOrderLogsForGridAsync_Should_Return_Paginated_Log_Data()
        {
            // Arrange
            var filter = new FilterModel { Skip = 0, Take = 20 };
            var fakeGridResult = new GridDataModel<DeliveryLogsModel>
            {
                Count = 1,
                Items = new List<DeliveryLogsModel>
                {
                    new DeliveryLogsModel { f_status_from = 1, f_status_to = 2 }
                }
            };

            _mockLogsRepository.Setup(r => r.GetExtendedForGrid(filter))
                .ReturnsAsync(fakeGridResult);

            // Act
            var result = await _service.GetDeliveryOrderLogsForGridAsync(filter);

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