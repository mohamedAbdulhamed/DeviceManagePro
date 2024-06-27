using DevicesApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using DevicesApp.Dtos.Responses;
using DevicesApp.Dtos.Requests;
using DevicesApp.Core.IConfiguration;
using System.Security.Claims;

namespace DeviceApp.Controllers;

[Authorize(Roles = "Admin,DataEntryUser")]
[Route("api/[controller]")]
[ApiController]
public class ClientController(IMapper mapper, IUnitOfWork unitOfWork, ILogger<ClientController> logger) : ControllerBase
{
    private readonly IMapper _mapper = mapper;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly ILogger<ClientController> _logger = logger;

    [HttpGet("GetAll")]
    public async Task<IActionResult> GetAll()
    {

        return Ok(await _unitOfWork.Clients.GetAll());
    }

    [HttpGet("GetById/{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var client = await _unitOfWork.Clients.GetById(id);
        if (client is not null)
        {
            var _mappedClient = _mapper.Map<GetClientResponse>(client);

            return Ok(_mappedClient);
        }
        return NotFound();
    }

    [HttpPost("Add")]
    public async Task<IActionResult> Add([FromBody] CreateClientRequest model)
    {
        if (model == null) return BadRequest();

        var _mappedClient = _mapper.Map<Client>(model);

        if (await _unitOfWork.Clients.Add(_mappedClient))
        {
            await _unitOfWork.CompleteAsync();

            return CreatedAtAction(nameof(GetById), new { id = _mappedClient.Id }, model);
        }

        return StatusCode(500, "Something went wronge");
    }

    // If Entity does not exist it will be created
    [HttpPut("Upsert")]
    public async Task<IActionResult> Upsert([FromBody] UpdateClientRequest model)
    {
        if (!ModelState.IsValid || model is null) return BadRequest();

        var _mappedClient = _mapper.Map<Client>(model);

        if (await _unitOfWork.Clients.Upsert(_mappedClient))
        {
            await _unitOfWork.CompleteAsync();

            return NoContent();
        }

        return StatusCode(500, "Something went wrong");
    }

    [HttpDelete("Delete/{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var client = await _unitOfWork.Clients.GetById(id);
        if (client == null) return NotFound();

        if (await _unitOfWork.Clients.Delete(id))
        {
            await _unitOfWork.CompleteAsync();

            return NoContent();
        }

        return StatusCode(500, "Something went wrong");
    }
}

