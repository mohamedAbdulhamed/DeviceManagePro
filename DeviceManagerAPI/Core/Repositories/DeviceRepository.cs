using DeviceApp.Data;
using DevicesApp.Core.IRepositories;
using DevicesApp.Models;
using Microsoft.EntityFrameworkCore;

namespace DevicesApp.Core.Repositories
{
    public class DeviceRepository : GenericRepository<Device>, IDeviceRepository
    {
        public DeviceRepository(AppDbContext context, ILogger logger) : base(context, logger)
        {
        }

        public override async Task<bool> Update(Device device)
        {
            try
            {
                var existingDevice = await dbSet.Where(x => x.Id == device.Id).FirstOrDefaultAsync();

                if (existingDevice is not null)
                {
                    existingDevice.SerialNo = device.SerialNo;
                    existingDevice.Name = device.Name;
                    existingDevice.TypeId = device.TypeId;
                    existingDevice.UpdatedAt = DateOnly.FromDateTime(DateTime.UtcNow);

                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{Repo} Update", typeof(DeviceRepository));
                return false;
            }
        }

        public override async Task<bool> Delete(Guid id)
        {
            try
            {
                var existingDevice = await dbSet.FirstOrDefaultAsync(x => x.Id == id);

                if (existingDevice != null)
                {
                    dbSet.Remove(existingDevice);
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{Repo} Delete", typeof(DeviceRepository));
                return false;
            }
        }
    }
}
