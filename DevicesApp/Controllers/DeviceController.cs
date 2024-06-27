using DevicesApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using DevicesApp.Dtos.Requests;
using AutoMapper;
using DevicesApp.Core.IConfiguration;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using DevicesApp.Dtos.Responses;

namespace DevicesApp.Controllers;

[Authorize(Roles = "Admin,DataEntryUser,DeviceControllerUser")]
[Route("api/[controller]")]
[ApiController]
public class DeviceController(IMapper mapper, IUnitOfWork unitOfWork, ILogger<DeviceController> logger) : ControllerBase
{
    private readonly IMapper _mapper = mapper;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly ILogger<DeviceController> _logger = logger;

    [HttpGet("GetAll")]
    [Authorize(Roles = "Admin, DataEntryUser")]
    public async Task<IActionResult> GetAll()
    {
        var devices = await _unitOfWork.Devices.GetAll(d => d.Type, d => d.Client);

        if (devices is not null && devices.Any()) return Ok(devices);

        return NotFound();
    }

    [HttpGet("GetAllByClientId/{ClientId}")]
    public async Task<IActionResult> GetAllByClientId(Guid ClientId)
    {
        var devices = await _unitOfWork.Devices.FindAll(d => d.ClientId == ClientId, d => d.Type, d => d.Client);

        if (devices is not null && devices.Any()) return Ok(devices);

        return NotFound();
    }

    [HttpGet("GetById/{id}")]
    [Authorize(Roles = "Admin, DataEntryUser")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var device = await _unitOfWork.Devices.GetById(id, d => d.Type, d => d.Client);

        if (device is not null)
        {
            var _mappedDevice = _mapper.Map<GetDeviceResponse>(device);

            return Ok(_mappedDevice);
        }

        return NotFound();
    }
    
    [HttpGet("GetStatus/{id}")]
    [Authorize(Roles = "Admin, DataEntryUser")]
    public async Task<IActionResult> GetStatus(Guid id)
    {
        var device = await _unitOfWork.Devices.GetById(id);

        if (device is not null) return Ok(device.Status);

        return NotFound();
    }

    [HttpPost("Add")]
    [Authorize(Roles = "Admin, DataEntryUser")]
    public async Task<IActionResult> Add([FromBody] CreateDeviceRequest model)
    {
        if (model is null) return BadRequest("The device can not be empty!");

        var _mappedDevice = _mapper.Map<Device>(model);

        if (await _unitOfWork.Devices.Add(_mappedDevice))
        {
            await _unitOfWork.CompleteAsync();

            return CreatedAtAction(nameof(GetById), new { id = _mappedDevice.Id }, model);
        }

        return StatusCode(500, "Something went wrong.");
    }

    [HttpPost("AddDeviceType")]
    [Authorize(Roles = "Admin, DataEntryUser")]
    public async Task<IActionResult> AddDeviceType([FromBody] string typeName)
    {


        if (string.IsNullOrWhiteSpace(typeName)) return BadRequest("A device type can not be empty!");

        DeviceType deviceType = new() { Name = typeName };

        if (await _unitOfWork.DeviceTypes.Add(deviceType))
        {
            await _unitOfWork.CompleteAsync();

            return Created();
        }

        return StatusCode(500, "Something went wrong.");
    }

    // If Entity does not exist it will be created
    [HttpPut("Upsert")]
    [Authorize(Roles = "Admin, DataEntryUser")]
    public async Task<IActionResult> Upsert([FromBody] UpdateDeviceRequest model)
    {
        var _mappedDevice = _mapper.Map<Device>(model);

        if (await _unitOfWork.Devices.Upsert(_mappedDevice))
        {
            await _unitOfWork.CompleteAsync();

            return NoContent();
        }

        return StatusCode(500, $"Something went wrong");
    }


    [HttpDelete("Delete/{id}")]
    [Authorize(Roles = "Admin, DataEntryUser")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var device = await _unitOfWork.Devices.GetById(id);
        if (device == null) return NotFound();


        if (await _unitOfWork.Devices.Delete(id))
        {
            await _unitOfWork.CompleteAsync();

            return NoContent();
        }

        return StatusCode(500, $"Something went wrong");
    }

    [HttpGet("toggle/{id}")]
    [Authorize(Roles = "Admin, DeviceControllerUser")]
    public async Task<IActionResult> ToggleStatus(Guid id)
    {
        var device = await _unitOfWork.Devices.GetById(id);
        if (device == null) return NotFound();

        device.Status = device.Status == DeviceStatus.ON ? DeviceStatus.OFF : DeviceStatus.ON;

        if (await _unitOfWork.Devices.Upsert(device))
        {
            await _unitOfWork.CompleteAsync();

            return NoContent();
        }

        return StatusCode(500, $"Something went wrong");
    }

    // Assign or reassign by overriding the ClientId prop
    [HttpPost("assign/{DeviceId}")] 
    [Authorize(Roles = "Admin, DataEntryUser")]
    public async Task<IActionResult> Assign(Guid DeviceId, [FromBody] Guid ClientId)
    {
        var device = await _unitOfWork.Devices.GetById(DeviceId);
        if (device is null) return NotFound(DeviceId);

        var client = await _unitOfWork.Clients.GetById(ClientId);
        if (client is null) return NotFound(ClientId);

        device.ClientId = ClientId;

        if (await _unitOfWork.Devices.Upsert(device))
        {
            await _unitOfWork.CompleteAsync();

            return NoContent();
        }

        return StatusCode(500, $"Something went wrong");
    }

    [HttpPost("unassign/{DeviceId}")]
    [Authorize(Roles = "Admin,DataEntryUser")]
    public async Task<IActionResult> UnAssign(Guid DeviceId)
    {
        var device = await _unitOfWork.Devices.GetById(DeviceId);
        if (device is null) return NotFound(DeviceId);

        if (device.ClientId is null) return BadRequest("Device already unassigned!");

        device.ClientId = null;

        if (await _unitOfWork.Devices.Upsert(device))
        {
            await _unitOfWork.CompleteAsync();

            return NoContent();
        }

        return StatusCode(500, $"Something went wrong");
    }

}