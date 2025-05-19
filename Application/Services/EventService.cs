using Application.Models;
using Persistence.Entities;
using Persistence.Repositories;

namespace Application.Services;

public class EventService(IEventRepository eventRepository) : IEventService
{
    private readonly IEventRepository _eventRepository = eventRepository;

    public async Task<EventResult> CreateEventAsync(CreateEventRequest request)
    {
		try
		{
			var eventEntitiy = new EventEntity
			{
				Image = request.Image,
				Title = request.Title,
				EventDate = request.EventDate,
				Location = request.Location,
				Description = request.Description
			};

			var result = await _eventRepository.AddAsync(eventEntitiy);
			return result.Success
				? new EventResult { Success = true }
				: new EventResult { Success = false, Error = result.Error };
		}
		catch (Exception ex)
		{
			return new EventResult
			{
				Success = false, Error = ex.Message
			};
		}
    }

	public async Task<EventResult<IEnumerable<Event>>> GetAllEventsAsync()
	{
		var result = await _eventRepository.GetAllAsync();
		var events = result.Result?.Select(x => new Event
		{
			Id = x.Id,
			Image = x.Image,
			Title = x.Title,
			EventDate = x.EventDate,
			Location = x.Location,
			Description = x.Description
		});

		return new EventResult<IEnumerable<Event>> { Success = true, Result = events };
	}
	
	public async Task<EventResult<Event?>> GetEventAsync(string eventId)
	{
		var result = await _eventRepository.GetAsync(x => x.Id == eventId);
		if (result.Success && result.Result != null)
		{
			var currentEvent = new Event
			{
				Id= result.Result.Id,
				Image= result.Result.Image,
				Title = result.Result.Title,
				EventDate = result.Result.EventDate,
				Location = result.Result.Location,
				Description = result.Result.Description
			};

			return new EventResult<Event?> { Success = true, Result = currentEvent };
		}

        return new EventResult<Event?> { Success = false, Error = "Event not found" };
    }
}
