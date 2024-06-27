using DeviceApp.Data;
using DevicesApp.Core.IRepositories;
using DevicesApp.Models;

namespace DevicesApp.Core.Repositories
{
    public class DeviceTypeRepository(AppDbContext context, ILogger logger) : GenericRepository<DeviceType>(context, logger), IDeviceTypeRepository
    {

    }
}
