using DeviceApp.Data;
using DevicesApp.Core.IRepositories;
using DevicesApp.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DevicesApp.Core.Repositories
{
    public class ActionLogRepository(AppDbContext context, ILogger logger) : GenericRepository<ActionLog>(context, logger), IActionLogRepository
    {
        public override async Task<bool> Delete(Guid id)
        {
            try
            {
                var existingActionLog = await dbSet.Where(x => x.Id == id).FirstOrDefaultAsync();

                if (existingActionLog is not null)
                {
                    dbSet.Remove(existingActionLog);
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{Repo} Delete", typeof(ActionLogRepository));
                return false;
            }
        }
    }
}
