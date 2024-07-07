using DeviceApp.Data;
using DevicesApp.Core.IRepositories;
using DevicesApp.Models;
using Microsoft.EntityFrameworkCore;

namespace DevicesApp.Core.Repositories
{
    public class DeviceTypeRepository(AppDbContext context, ILogger logger) : GenericRepository<DeviceType>(context, logger), IDeviceTypeRepository
    {
        public override async Task<bool> Update(DeviceType deviceType)
        {
            try
            {
                var existingDeviceType = await dbSet.Where(x => x.Id == deviceType.Id).FirstOrDefaultAsync();

                if (existingDeviceType is not null)
                {
                    existingDeviceType.Name = deviceType.Name;

                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{Repo} Update", typeof(DeviceTypeRepository));
                return false;
            }
        }

        public override async Task<bool> Delete(Guid id)
        {
            try
            {
                var existingDeviceType = await dbSet.Where(x => x.Id == id).FirstOrDefaultAsync();

                if (existingDeviceType is not null)
                {
                    dbSet.Remove(existingDeviceType);
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{Repo} Delete", typeof(DeviceTypeRepository));
                return false;
            }
        }
    }
}
