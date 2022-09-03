﻿using Application.Activities.Commands;
using Application.Users.Commands;
using Application.Users.Queries;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using WebAPI.ControllersHelpers;
using WebAPI.Dto;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        public readonly IMediator _mediator;
        public readonly IMapper _mapper;
        private readonly ILogger<UsersController> _logger;
        private readonly ControllerHelper _controllerHelper;

        public UsersController(IMediator mediator, IMapper mapper, ILogger<UsersController> logger, ControllerHelper controllerHelper)
        {
            _mediator = mediator;
            _mapper = mapper;
            _logger = logger;
            _controllerHelper = controllerHelper;
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> GetUsersById(int id)
        {
            var actionName = _controllerHelper.GetActionName(this);
            var controllerName = _controllerHelper.GetControllerName(this);

            _logger.LogInformation($"Controller: {controllerName}\n" +
                $"Action: {actionName}\n" +
                $"Called at: {DateTime.Now.TimeOfDay}");

            var result = await _mediator.Send(new GetUserByIdQueryCommand() { Id = id });

            if (result == null)
            {
                _logger.LogWarning($"No user found with Id: {id}.");
                return NotFound();
            }

            var mappedResult = _mapper.Map<UserGetDto>(result);

            return Ok(mappedResult);
        }

        [HttpGet]
        [Route("all-users")]
        public async Task<IActionResult> GetAll()
        {
            var actionName = _controllerHelper.GetActionName(this);
            var controllerName = _controllerHelper.GetControllerName(this);

            _logger.LogInformation($"Controller: {controllerName}\n" +
                $"Action: {actionName}\n" +
                $"Called at: {DateTime.Now.TimeOfDay}");

            var result = await _mediator.Send(new GetAllUsers());
            if (result == null)
            {
                _logger.LogWarning("No users found");
                return NotFound();
            }

            var mappedResult = _mapper.Map<List<UserGetDto>>(result);
            _logger.LogInformation($"Found {result.Count} users");

            return Ok(mappedResult);
        }

        [HttpGet]
        [Route("login")]
        public async Task<IActionResult> GetUserLogin(string userName, string password)
        {
            var actionName = _controllerHelper.GetActionName(this);
            var controllerName = _controllerHelper.GetControllerName(this);

            _logger.LogInformation($"Controller: {controllerName}\n" +
                $"Action: {actionName}\n" +
                $"Called at: {DateTime.Now.TimeOfDay}");

            var result = await _mediator.Send(new GetUserQueryLoginCommand() { UserName = userName, Password = password });

            if (result == null)
            {
                _logger.LogWarning($"Login failed, no user found with provided credentials");
                return NotFound();
            }

            var mappedResult = _mapper.Map<UserGetDto>(result);

            return Ok(mappedResult);
        }

        [HttpPost]
        [Route("create-user")]
        public async Task<IActionResult> CreateUser([FromBody] UserPutPostDto user)
        {
            var actionName = _controllerHelper.GetActionName(this);
            var controllerName = _controllerHelper.GetControllerName(this);

            _logger.LogInformation($"Controller: {controllerName}\n" +
                $"Action: {actionName}\n" +
                $"Called at: {DateTime.Now.TimeOfDay}");

            var result = await _mediator.Send(new CreateUserCommand 
            { 
                FirstName = user.FirstName, 
                LastName = user.LastName,
                UserName = user.UserName,
                Password = user.Password,
            });

            if (result == null)
            {
                _logger.LogWarning($"Could not create user.");
                _logger.LogDebug("Tried to create user with the following:\n" +
                    $"FirstName: {user.FirstName}\n" +
                    $"LastName: {user.LastName}\n" +
                    $"UserName: {user.UserName}\n" +
                    $"Password: {user.Password}");
                return NotFound();
            }
                
            var mappedResult = _mapper.Map<UserGetDto>(result);

            return CreatedAtAction(nameof(GetUsersById), new { Id = mappedResult.Id }, mappedResult);
        }

        [HttpPost]
        [Route("{userId}/activities/{activityId}")]
        public async Task<IActionResult> AddActivityToUser(int userId, int activityId)
        {
            var actionName = _controllerHelper.GetActionName(this);
            var controllerName = _controllerHelper.GetControllerName(this);

            _logger.LogInformation($"Controller: {controllerName}\n" +
                $"Action: {actionName}\n" +
                $"Called at: {DateTime.Now.TimeOfDay}");

            var command = new AddActivityToUser
            {
                UserId = userId,
                ActivityId = activityId
            };
            var user = await _mediator.Send(command);

            if (user == null)
                return NotFound();

            return Ok(_mapper.Map<UserGetDto>(user));
        }

        [HttpDelete]
        [Route("deleteUser/{userId}")]
        public async Task<IActionResult> DeleteUser(int userId)
        {
            var actionName = _controllerHelper.GetActionName(this);
            var controllerName = _controllerHelper.GetControllerName(this);

            _logger.LogInformation($"Controller: {controllerName}\n" +
                $"Action: {actionName}\n" +
                $"Called at: {DateTime.Now.TimeOfDay}");

            var command = new DeleteUser { Id = userId };
            var result = await _mediator.Send(command);

            if (result == null)
            {
                _logger.LogWarning($"Could not delete user. User with id: {userId} was not found.");
                return NotFound();
            }

            return NoContent();
        }
    }
}
