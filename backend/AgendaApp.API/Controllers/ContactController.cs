using AgendaApp.API.Helpers;
using AgendaApp.Application.Features.Contact.Commands;
using AgendaApp.Application.Features.Contact.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AgendaApp.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ContactController : ControllerBase
{
    private readonly IMediator _mediator;

    public ContactController(IMediator mediator)
    {
        _mediator = mediator;
    }

    private Guid GetUserId() => UserClaimsHelper.GetUserIdFromClaim(User);

    /// <summary>
    /// Retrieves a filtered list of contacts.
    /// </summary>
    /// <param name="query">Filtering parameters.</param>
    /// <returns>A paginated list of contacts.</returns>
    [HttpGet("ByFilter")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> GetAllByFilterAsync([FromQuery] GetAllContactsFromFilterQuery query)
    {
        query.SetUserId(GetUserId());

        var result = await _mediator.Send(query);

        return !result.Result.Any() ? NoContent() : Ok(result);
    }

    /// <summary>
    /// Retrieves a contact by its ID.
    /// </summary>
    /// <param name="id">The ID of the contact.</param>
    /// <returns>The contact details.</returns>
    [HttpGet("{id}", Name = "GetContactById")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetByIdAsync(Guid id)
    {
        var query = new GetContactByIdQuery(GetUserId(), id);
        var contact = await _mediator.Send(query);

        return contact is null ? NotFound() : Ok(contact);
    }

    /// <summary>
    /// Creates a new contact.
    /// </summary>
    /// <param name="command">The contact data to create.</param>
    /// <returns>The result of the creation operation.</returns>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateAsync([FromBody] CreateContactCommand command)
    {
        command.SetUserId(GetUserId());

        var result = await _mediator.Send(command);

        if (!result.IsSuccess)
            return BadRequest(result);

        return CreatedAtRoute("GetContactById", new { id = result.Data.Id }, result);
    }

    /// <summary>
    /// Updates an existing contact.
    /// </summary>
    /// <param name="id">The ID of the contact to update.</param>
    /// <param name="command">The updated contact data.</param>
    /// <returns>The result of the update operation.</returns>
    [HttpPatch("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateAsync(Guid id, [FromBody] UpdateContactCommand command)
    {
        command.SetId(id);
        command.SetUserId(GetUserId());

        var result = await _mediator.Send(command);

        return result.IsSuccess ? Ok(result) : BadRequest(result);
    }

    /// <summary>
    /// Deletes a contact by its ID.
    /// </summary>
    /// <param name="id">The ID of the contact to delete.</param>
    /// <returns>No content if the operation was successful.</returns>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> DeleteAsync(Guid id)
    {
        var command = new DeleteContactCommand(GetUserId(), id);
        var result = await _mediator.Send(command);

        return result.IsSuccess ? NoContent() : BadRequest(result);
    }

    /// <summary>
    /// Deletes all contacts of the current user.
    /// </summary>
    /// <returns>No content if the operation was successful.</returns>
    [HttpDelete("All")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> DeleteAllAsync()
    {
        var command = new DeleteAllContactsCommand(GetUserId());
        var result = await _mediator.Send(command);

        return result.IsSuccess ? NoContent() : BadRequest(result);
    }
}