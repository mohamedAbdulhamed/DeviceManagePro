using DeviceApp.Data;
using DevicesApp.Core.IConfiguration;
using DevicesApp.Core.IRepositories;
using DevicesApp.Core.Repositories;

namespace DevicesApp.Data;

public class UnitOfWork : IUnitOfWork, IDisposable
{
    private readonly AppDbContext _context;
    private readonly ILogger _logger;

    public IClientRepository Clients { get; private set; }
    public IDeviceRepository Devices { get; private set; }
    public IDeviceTypeRepository DeviceTypes { get; private set; }
    public IActionLogRepository ActionLogs { get; private set; }

    public UnitOfWork(AppDbContext context, ILoggerFactory loggerFactory)
    {
        _context = context;
        _logger = loggerFactory.CreateLogger("logs");

        Clients = new ClientRepository(_context, _logger);
        Devices = new DeviceRepository(_context, _logger);
        DeviceTypes = new DeviceTypeRepository(_context, _logger);
        ActionLogs = new ActionLogRepository(_context, _logger);
    }

    public async Task CompleteAsync()
    {
        await _context.SaveChangesAsync();
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}
