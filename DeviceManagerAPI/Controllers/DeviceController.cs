using DevicesApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using DevicesApp.Dtos.Requests;
using AutoMapper;
using DevicesApp.Core.IConfiguration;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace DevicesApp.Controllers;

[Authorize(Roles = "Admin,DataEntryUser,DeviceControllerUser")]
[Route("api/[controller]")]
[ApiController]
public class DeviceController(IMapper mapper, IUnitOfWork unitOfWork, ILogger<DeviceController> logger, UserManager<ApplicationUser> userManager) : ControllerBase
{
    private readonly IMapper _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    private readonly IUnitOfWork _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    private readonly ILogger<DeviceController> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    private readonly UserManager<ApplicationUser> _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));

    [HttpGet("GetAll")]
    [Authorize(Roles = "Admin,DataEntryUser")]
    public async Task<IActionResult> GetAll()
    {
        try
        {
            var devices = await _unitOfWork.Devices.GetAll(d => d.Type, d => d.Client);

            if (devices is not null && devices.Any())
            {
                //var response = _mapper.Map<IEnumerable<GetDeviceResponse>>(devices);
                return Ok(devices);
            }

            return NotFound("No devices found.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching all devices");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpGet("GetClientDevices/{clientId}")]
    public async Task<IActionResult> GetClientDevices(Guid clientId)
    {
        try
        {
            var devices = await _unitOfWork.Devices.FindAll(d => d.ClientId == clientId, d => d.Type);

            if (devices is not null && devices.Any())
            {
                //var response = _mapper.Map<IEnumerable<GetDeviceResponse>>(devices);
                return Ok(devices);
            }

            return NotFound("No devices found for that client.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching devices by client ID");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpGet("GetAllDeviceTypes")]
    [Authorize(Roles = "Admin,DataEntryUser")]
    public async Task<IActionResult> GetAllDeviceTypes()
    {
        try
        {
            var deviceTypes = await _unitOfWork.DeviceTypes.GetAll(t => t.Devices);
            if (!deviceTypes.Any()) return NotFound("No device types found.");

            return Ok(deviceTypes);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching all device types");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpGet("GetById/{id}")]
    [Authorize(Roles = "Admin,DataEntryUser")]
    public async Task<IActionResult> GetById(Guid id)
    {
        try
        {
            var device = await _unitOfWork.Devices.GetById(id, d => d.Type, d => d.Client);
            if (device == null) return NotFound($"Device with ID {id} not found");

            //var mappedDevice = _mapper.Map<GetDeviceResponse>(device);
            return Ok(device);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error fetching device by ID {id}");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpGet("GetBySerialno/{serialNo}")]
    public async Task<IActionResult> GetBySerialno(string serialNo)
    {
        try
        {
            var device = await _unitOfWork.Devices.Find(d => d.SerialNo == serialNo, d => d.Type, d => d.Client);

            if (device is not null)
            {
                //var response = _mapper.Map<IEnumerable<GetDeviceResponse>>(devices);
                return Ok(device);
            }

            return NotFound("No devices found.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching devices by client ID");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpGet("GetStatus/{id}")]
    [Authorize(Roles = "Admin,DataEntryUser")]
    public async Task<IActionResult> GetStatus(Guid id)
    {
        try
        {
            var device = await _unitOfWork.Devices.GetById(id);
            if (device == null) return NotFound($"Device with ID {id} not found");

            return Ok(device.Status);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error fetching device status by ID {id}");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpGet("ToggleStatus/{id}")]
    [Authorize(Roles = "Admin,DeviceControllerUser")]
    public async Task<IActionResult> ToggleStatus(Guid id)
    {
        try
        {
            var device = await _unitOfWork.Devices.GetById(id);
            if (device == null) return NotFound($"Device with ID {id} not found");

            device.Status = device.Status == DeviceStatus.ON ? DeviceStatus.OFF : DeviceStatus.ON;

            await _unitOfWork.CompleteAsync();

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error toggling status for device by ID {id}");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpPost("Add")]
    [Authorize(Roles = "Admin,DataEntryUser")]
    public async Task<IActionResult> Add([FromBody] CreateDeviceRequest model)
    {
        if (!ModelState.IsValid) return BadRequest("Invalid model state");

        try
        {
            var device = _mapper.Map<Device>(model);

            var userIdentifier = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "Unknown";

            var user = await _userManager.FindByNameAsync(userIdentifier);

            if (user is null) return StatusCode(500, "Internal server error");

            device.CreatedBy = user.UserId;

            await _unitOfWork.Devices.Add(device);

            await _unitOfWork.CompleteAsync();

            return CreatedAtAction(nameof(GetById), new { id = device.Id }, device);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding device");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpPost("AddDeviceType")]
    [Authorize(Roles = "Admin,DataEntryUser")]
    public async Task<IActionResult> AddDeviceType([FromBody] string typeName)
    {
        if (string.IsNullOrWhiteSpace(typeName)) return BadRequest("Device type name cannot be empty");

        try
        {
            var deviceType = new DeviceType { Name = typeName };

            await _unitOfWork.DeviceTypes.Add(deviceType);

            await _unitOfWork.CompleteAsync();

            return CreatedAtAction(nameof(GetAllDeviceTypes), new { id = deviceType.Id }, deviceType);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding device type");
            return StatusCode(500, "Internal server error");
        }
    }


    [HttpPost("Assign/{deviceId}")]
    [Authorize(Roles = "Admin,DataEntryUser")]
    public async Task<IActionResult> Assign(Guid deviceId, [FromBody] Guid clientId)
    {
        try
        {
            var device = await _unitOfWork.Devices.GetById(deviceId);

            if (device == null) return NotFound($"Device with ID {deviceId} not found");

            var client = await _unitOfWork.Clients.GetById(clientId);

            if (client == null) return NotFound($"Client with ID {clientId} not found");

            device.ClientId = clientId;

            await _unitOfWork.CompleteAsync();

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error assigning device with ID {deviceId} to client ID {clientId}");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpPost("Unassign/{deviceId}")]
    [Authorize(Roles = "Admin,DataEntryUser")]
    public async Task<IActionResult> Unassign(Guid deviceId)
    {
        try
        {
            var device = await _unitOfWork.Devices.GetById(deviceId);

            if (device == null) return NotFound($"Device with ID {deviceId} not found");

            if (device.ClientId == null) return BadRequest("Device is already unassigned");

            device.ClientId = null;

            await _unitOfWork.CompleteAsync();

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error unassigning device with ID {deviceId}");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpPut("Update")]
    [Authorize(Roles = "Admin,DataEntryUser")]
    public async Task<IActionResult> Update([FromBody] UpdateDeviceRequest model)
    {
        if (!ModelState.IsValid) return BadRequest("Invalid model state");

        try
        {
            var existingDevice = await _unitOfWork.Devices.Find(d => d.Id == model.Id);

            if (existingDevice is null) return BadRequest("Device does not exist!");

            var device = _mapper.Map<Device>(model);

            if (await _unitOfWork.Devices.Update(device))
            {
                await _unitOfWork.CompleteAsync();

                return NoContent();
            }

            return StatusCode(500, "Couldn't update the device.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating device");
            return StatusCode(500,"Internal server error");
        }
    }

    [HttpDelete("Delete/{id}")]
    [Authorize(Roles = "Admin,DataEntryUser")]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            var device = await _unitOfWork.Devices.GetById(id);
            if (device == null) return NotFound($"Device with ID {id} not found");

            await _unitOfWork.Devices.Delete(id);
            await _unitOfWork.CompleteAsync();

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error deleting device by ID {id}");
            return StatusCode(500, "Internal server error");
        }
    }
}
