using DeviceApp.Data;
using DevicesApp.Core.IRepositories;
using DevicesApp.Models;
using Microsoft.Extensions.Logging;

namespace DevicesApp.Core.Repositories
{
    public class ActionLogRepository(AppDbContext context, ILogger logger) : GenericRepository<ActionLog>(context, logger), IActionLogRepository
    {
        
    }
}
