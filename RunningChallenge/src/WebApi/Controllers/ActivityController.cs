﻿using Application.Activities.Commands;
using Application.Activities.Queries;
using AutoMapper;
using Domain.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using WebAPI.ControllersHelpers;
using WebAPI.Dto;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ActivityController : ControllerBase
    {
        public readonly IMediator _mediator;
        public readonly IMapper _mapper;
        private readonly ILogger<ActivityController> _logger;
        private readonly LoggerHelper _loggerHelper;

        public ActivityController(IMediator mediator, IMapper mapper, ILogger<ActivityController> logger, LoggerHelper loggerHelper)
        {
            _mediator = mediator;
            _mapper = mapper;
            _logger = logger;
            _loggerHelper = loggerHelper;
        }

        [HttpGet]
        [Route("all-activities")]
        public async Task<IActionResult> GetAll()
        {
            _logger.LogInformation(_loggerHelper.LogControllerAndAction(this));

            var result = await _mediator.Send(new GetAllActivitiesQuery());
            if(result == null)
            {
                _logger.LogWarning("No activities found.");
                return NotFound();
            }

            _logger.LogInformation($"Found {result.Count} activities.");
            var mappedResult = _mapper.Map<List<ActivityGetDto>>(result);
            return Ok(mappedResult);
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> GetActivityById(int id)
        {
            _logger.LogInformation(_loggerHelper.LogControllerAndAction(this));

            var result = await _mediator.Send(new GetActivityByIdQuery(){ Id = id });

            if (result == null)
            {
                _logger.LogWarning($"No activity found with id: {id}.");
                return NotFound();
            }

            var mappedResult = _mapper.Map<ActivityGetDto>(result);
            return Ok(mappedResult);
        }

        [HttpGet]
        [Route("user-activities/{id}")]
        public async Task<IActionResult> GetAllUserActivities(int id)
        {
            _logger.LogInformation(_loggerHelper.LogControllerAndAction(this));

            var result = await _mediator.Send(new GetAllUserActivitiesQuery() { UserId = id });
            if (result == null)
            {
                _logger.LogWarning($"No activity found for user with id: {id}");
                return NotFound();
            }

            var mappedResult = _mapper.Map<List<ActivityGetDto>>(result);
            _logger.LogInformation($"Found {result.Count} activities for user with id: {id}");
            return Ok(mappedResult);
        }

        [HttpPost]
        [Route("create-activity")]
        public async Task<IActionResult> CreateActivity([FromBody] ActivityPutPostDto activity)
        {
            _logger.LogInformation(_loggerHelper.LogControllerAndAction(this));

            var result = await _mediator.Send(new CreateActivityCommand
            {
                RunnerId = activity.UserId,
                Distance = activity.Distance,
                Date = activity.Date,
                Duration = activity.Duration,
            });

            if(result == null)
            {
                _logger.LogWarning("Failed to create activity");
                _logger.LogDebug("Tried to create activity with the following values\n" +
                    $"RunnerId: {activity.UserId}\n" +
                    $"Distance: {activity.Distance}\n" +
                    $"Date: {activity.Date}\n" +
                    $"Duration: {activity.Duration}");
                return NotFound();
            }

            var mappedResult = _mapper.Map<ActivityGetDto>(result);
            _logger.LogInformation($"Succesfully created activity with id: {mappedResult.Id}.");

            return CreatedAtAction(nameof(GetActivityById), new { id = mappedResult.Id }, mappedResult);
        }

        [HttpDelete]
        [Route("deleteActivity/{activityId}")]
        public async Task<IActionResult> DeleteActivity(int activityId)
        {
            _logger.LogInformation(_loggerHelper.LogControllerAndAction(this));

            var command = new DeleteActivity { Id = activityId };
            var result = await _mediator.Send(command);

            if (result == null) {
                _logger.LogWarning($"Failed to delete activity. No activity found with id: {activityId}.");
                return NotFound(); }

            _logger.LogInformation($"Successfully deleted activity with id: {activityId}.");
            return NoContent();
        }
    }
}
