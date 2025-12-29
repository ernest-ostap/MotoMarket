using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MotoMarket.Application.Common.Interfaces.Identity;
using MotoMarket.Application.Users.Commands.ChangePassword;
using MotoMarket.Application.Users.Commands.LoginUser;
using MotoMarket.Application.Users.Commands.RegisterUser;
using MotoMarket.Application.Users.Commands.ToggleUserAdminRole;
using MotoMarket.Application.Users.Commands.ToggleUserBan;
using MotoMarket.Application.Users.Commands.UpdateUserProfile;
using MotoMarket.Application.Users.Queries;

namespace MotoMarket.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IUserManagementService _userManagementService;

        public UsersController(IMediator mediator, IUserManagementService userManagementService)
        {
            _mediator = mediator;
            _userManagementService = userManagementService;
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

        //metody dla admina
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetAll()
        {
            return Ok(await _mediator.Send(new GetUsersQuery()));
        }

        [HttpPatch("{id}/toggle-ban")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ToggleBan(string id)
        {
            await _mediator.Send(new ToggleUserBanCommand(id));
            return NoContent();
        }

        [HttpGet("{id}/is-banned")]
        // [Authorize] - Możemy to zostawić otwarte lub zabezpieczone, 
        // ale Middleware w Webie musi mieć możliwość to sprawdzić.
        public async Task<ActionResult<bool>> IsBanned(string id)
        {
            var isBanned = await _userManagementService.IsUserBanned(id); 
            return Ok(isBanned);
        }

        [HttpPatch("{id}/toggle-admin")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ToggleAdminRole(string id)
        {
            await _mediator.Send(new ToggleUserAdminRoleCommand(id));
            return NoContent();
        }
    }
}