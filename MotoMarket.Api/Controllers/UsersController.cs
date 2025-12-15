using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MotoMarket.Application.Users.Commands.LoginUser;
using MotoMarket.Application.Users.Commands.RegisterUser;
using MotoMarket.Application.Users.Commands.UpdateUserProfile;
using MotoMarket.Application.Users.Commands.ChangePassword;
using MotoMarket.Application.Users.Queries;

namespace MotoMarket.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IMediator _mediator;

        public UsersController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("register")]
        public async Task<ActionResult<AuthDto>> Register(RegisterUserCommand command)
        {
            return await _mediator.Send(command);
        }

        [HttpPost("login")]
        public async Task<ActionResult<AuthDto>> Login(LoginUserCommand command)
        {
            return await _mediator.Send(command);
        }

        [Authorize]
        [HttpPost("update-profile")]
        public async Task<ActionResult> UpdateProfile(UpdateUserProfileCommand command)
        {
            await _mediator.Send(command);
            return NoContent();
        }

        [Authorize]
        [HttpPost("change-password")]
        public async Task<ActionResult> ChangePassword(ChangePasswordCommand command)
        {
            await _mediator.Send(command);
            return NoContent();
        }
    }
}