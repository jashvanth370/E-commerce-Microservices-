using AuthenticationApi.Application.DTOs;
using AuthenticationApi.Application.Interfaces;
using eCommerce.SharedLibrary.Responses;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AuthenticationApi.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController(IUser userInterface) : ControllerBase
    {
        [HttpPost("register")]
        public async Task<IActionResult> Register(AppUserDTO appUserDTO)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var response = await userInterface.Register(appUserDTO);
            if (!response.flag)
                return BadRequest(response);
            return Ok(response);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDTO loginDTO)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var response = await userInterface.Login(loginDTO);
            if (!response.flag)
                return BadRequest(response);
            return Ok(response);
        }

        [HttpGet("getUser/{UserId:int}")]
        public async Task<ActionResult<GetUserDTO>> GetUser(int UserId)
        {
            if(UserId <=0)
                return BadRequest(new Response
                {
                    flag = false,
                    message = "Invalid user ID."
                });

            var user = await userInterface.GetUser(UserId);
           
            return user.Id > 0 ? Ok(user) : NotFound(Request);
        }


    }
}
