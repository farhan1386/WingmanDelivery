using Microsoft.Data.SqlClient;
using WingmanDelivery.BusinessLogic.Interfaces;
using WingmanDelivery.BusinessLogic.Repositories;
using WingmanDelivery.BusinessLogic.Services;
using WingmanDelivery.BusinessLogic.Services.Interfaces;
using WingmanDelivery.BusinessLogic.UnitOfWork;
using WingmanDelivery.Models;
using WingmanDelivery.Server.BackgroundServices;

namespace WingmanDelivery.Server.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddUnitOfWork(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IUnitOfWork>(provider =>
            {
                var connectionString = configuration.GetConnectionString("DefaultConnection")
                    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found in configuration.");

                var connection = new SqlConnection(connectionString);
                var httpContext = provider.GetService<IHttpContextAccessor>()?.HttpContext;

                string? headerValue = httpContext?.Request.Headers["X-Tenant-Schema"];
                string activeSchema = string.IsNullOrWhiteSpace(headerValue) ? "dbo" : headerValue;

                var invokeData = httpContext?.Items["InvokeData"] as InvokeDataModel
                                 ?? new InvokeDataModel
                                 {
                                     userUid = Guid.NewGuid(),
                                     schema = activeSchema,
                                     commandTimeout = 30,
                                     configuration = configuration
                                 };

                return new UnitOfWork(connection, invokeData);
            });

            return services;
        }

        public static IServiceCollection AddRepositories(this IServiceCollection services)
        {
            services.AddScoped<IDeliveryOrderRepository, DeliveryOrderRepository>();
            services.AddScoped<IDeliveryOrderLogsRepository, DeliveryOrderLogsRepository>();
            services.AddScoped<IRoleRepository, RoleRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<INotificationRepository, NotificationRepository>();
            services.AddScoped<IOrderRepository, OrderRepository>();
            services.AddScoped<IOrderItemsRepository, OrderItemRepository>();
            services.AddScoped<IPaymentRepository, PaymentRepository>();

            return services;
        }

        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddScoped<IDeliveryOrderService, DeliveryOrderService>();
            services.AddScoped<IDeliveryOrderLogsService, DeliveryOrderLogsService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IRoleService, RoleService>();
            services.AddScoped<INotificationService, NotificationService>();
            services.AddScoped<IOrderService, OrderService>();
            services.AddScoped<IOrderItemsService, OrderItemsService>();
            services.AddScoped<IPaymentService, PaymentService>();
            services.AddHostedService<DeliverySimulationWorker>();

            return services;
        }
    }
}
