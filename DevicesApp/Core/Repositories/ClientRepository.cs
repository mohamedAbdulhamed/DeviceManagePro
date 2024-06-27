using DeviceApp.Data;
using DevicesApp.Core.IRepositories;
using DevicesApp.Models;
using Microsoft.EntityFrameworkCore;

namespace DevicesApp.Core.Repositories;

public class ClientRepository(AppDbContext context, ILogger logger) : GenericRepository<Client>(context, logger), IClientRepository
{
    public override async Task<bool> Upsert(Client entity)
    {
        try
        {
            var existingClient = await dbSet.Where(x => x.Id == entity.Id).FirstOrDefaultAsync();

            if (existingClient is null)
                return await Add(entity);

            existingClient.Name = entity.Name;
            existingClient.CreatedBy = entity.CreatedBy;

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "{Repo} Upsert", typeof(ClientRepository));
            return false;
        }
    }

    public override async Task<bool> Delete(Guid id)
    {
        try
        {
            var existingClient = await dbSet.Where(x => x.Id == id).FirstOrDefaultAsync();

            if (existingClient is not null)
            {
                dbSet.Remove(existingClient);
                return true;
            }

            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "{Repo} Delete", typeof(ClientRepository));
            return false;
        }
    }
}
