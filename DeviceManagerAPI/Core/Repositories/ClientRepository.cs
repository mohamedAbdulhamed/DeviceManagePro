using DeviceApp.Data;
using DevicesApp.Core.IRepositories;
using DevicesApp.Models;
using Microsoft.EntityFrameworkCore;

namespace DevicesApp.Core.Repositories;

public class ClientRepository(AppDbContext context, ILogger logger) : GenericRepository<Client>(context, logger), IClientRepository
{
    public override async Task<bool> Update(Client client)
    {
        try
        {
            var existingClient = await dbSet.Where(x => x.Id == client.Id).FirstOrDefaultAsync();

            if (existingClient is not null)
            {
                existingClient.Name = client.Name;
                existingClient.NationalId = client.NationalId;
                existingClient.Longitude = client.Longitude;
                existingClient.Latitude = client.Latitude;
                existingClient.UpdatedAt = DateOnly.FromDateTime(DateTime.UtcNow);

                return true;
            }

            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "{Repo} Update", typeof(ClientRepository));
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
