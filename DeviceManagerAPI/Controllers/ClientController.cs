using DevicesApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using DevicesApp.Dtos.Responses;
using DevicesApp.Dtos.Requests;
using DevicesApp.Core.IConfiguration;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;

namespace DevicesApp.Controllers;

[Authorize(Roles = "Admin,DataEntryUser")]
[Route("api/[controller]")]
[ApiController]
public class ClientController(IMapper mapper, IUnitOfWork unitOfWork, ILogger<ClientController> logger, UserManager<ApplicationUser> userManager) : ControllerBase
{
    private readonly IMapper _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    private readonly IUnitOfWork _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    private readonly ILogger<ClientController> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    private readonly UserManager<ApplicationUser> _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));


    [HttpGet("GetAll")]
    public async Task<IActionResult> GetAll()
    {
        try
        {
            var clients = await _unitOfWork.Clients.GetAll(c => c.Devices);

            if (clients is not null && clients.Any())
            {
                //var response = _mapper.Map<IEnumerable<GetClientResponse>>(clients);
                return Ok(clients);
            }

            return NotFound("No clients found.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while fetching clients.");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpGet("GetById/{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        if (id == Guid.Empty) return BadRequest("Invalid client ID.");

        try
        {
            var client = await _unitOfWork.Clients.GetById(id, c => c.Devices);

            
            if (client is not null) 
            {
                //var response = _mapper.Map<GetClientResponse>(client);
                return Ok(client);
            }

            return NotFound("Client not found.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while fetching the client.");
            return StatusCode(500, "Internal server error");
        }
    }
    [HttpGet("GetByNationalId/{nationalId}")]
    public async Task<IActionResult> GetByNationalId(string nationalId)
    {
        if (string.IsNullOrWhiteSpace(nationalId)) return BadRequest("Invalid client national ID.");

        try
        {
            var client = await _unitOfWork.Clients.Find(c => c.NationalId == nationalId, client => client.Devices);

            if (client is not null)
            {
                //var response = _mapper.Map<GetClientResponse>(client);
                return Ok(client);
            }

            return NotFound("Client not found.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while fetching the client.");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpPost("Add")]
    public async Task<IActionResult> Add([FromBody] CreateClientRequest model)
    {
        if (!ModelState.IsValid) return BadRequest("Invalid model state.");

        try
        {
            var client = _mapper.Map<Client>(model);

            var userIdentifier = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "Unknown";

            var user = await _userManager.FindByNameAsync(userIdentifier);

            if (user is null) return StatusCode(500, "Internal server error");

            client.CreatedBy = user.UserId;

            if (await _unitOfWork.Clients.Add(client))
            {
                await _unitOfWork.CompleteAsync();
                var response = _mapper.Map<GetClientResponse>(client);
                return CreatedAtAction(nameof(GetById), new { id = client.Id }, response);
            }

            return StatusCode(500, "Could not add the client.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while adding the client.");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpPut("Update")]
    public async Task<IActionResult> Update([FromBody] UpdateClientRequest model)
    {
        if (!ModelState.IsValid) return BadRequest("Invalid model state.");

        try
        {
            var existingClient = await _unitOfWork.Clients.Find(c => c.Id == model.Id);

            if (existingClient is null) return BadRequest("Client does not exist!");

            var client = _mapper.Map<Client>(model);

            if (await _unitOfWork.Clients.Update(client))
            {
                await _unitOfWork.CompleteAsync();

                return NoContent();
            }

            return StatusCode(500, "Couldn't update the client.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while updating the client.");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpDelete("Delete/{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        if (id == Guid.Empty) return BadRequest("Invalid client ID.");

        try
        {
            var client = await _unitOfWork.Clients.GetById(id);

            if (client is null) return NotFound("Client not found.");

            if (await _unitOfWork.Clients.Delete(id))
            {
                await _unitOfWork.CompleteAsync();

                return NoContent();
            }

            return StatusCode(500, "Could not delete the client.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while deleting the client.");
            return StatusCode(500, "Internal server error");
        }
    }
}
