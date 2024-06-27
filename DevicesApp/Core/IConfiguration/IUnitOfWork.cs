using DevicesApp.Core.IRepositories;

namespace DevicesApp.Core.IConfiguration
{
    public interface IUnitOfWork
    {
        IClientRepository Clients {  get; }
        IDeviceRepository Devices { get; }
        IDeviceTypeRepository DeviceTypes { get; }
        IActionLogRepository ActionLogs { get; }

        Task CompleteAsync();
    }
}
