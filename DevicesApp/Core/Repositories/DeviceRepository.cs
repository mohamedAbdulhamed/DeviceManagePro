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

        public override async Task<bool> Upsert(Device entity)
        {
            try
            {
                var existingDevice = await dbSet.FirstOrDefaultAsync(x => x.Id == entity.Id);

                if (existingDevice == null)
                    return await Add(entity);

                existingDevice.Name = entity.Name;
                existingDevice.CreatedBy = entity.CreatedBy;

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{Repo} Upsert", typeof(DeviceRepository));
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
