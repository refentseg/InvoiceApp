using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Data;
using API.DTO;
using API.Entity;
using API.Extensions;
using API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    /// <summary>
    /// Handles user account-related operations such as login, registration, and retrieving current user info.
    /// </summary>
    public class AccountController:BaseApiController
    {
        private readonly UserManager<User> _userManager;
        private readonly TokenService _tokenService;
        private readonly InvoiceContext _context;
        
        /// <summary>
        /// Constructor for AccountController.
        /// </summary>
        /// <param name="userManager">UserManager service for user operations.</param>
        /// <param name="tokenService">TokenService for generating JWT tokens.</param>
        /// <param name="context">Database context for persistence operations.</param>
        public AccountController(UserManager<User> userManager, TokenService tokenService, InvoiceContext context)
        {
            _context = context;
            _tokenService = tokenService;
            _userManager = userManager;

        }

        /// <summary>
        /// Authenticates a user and returns user details with a JWT token.
        /// </summary>
        /// <param name="loginDto">Login credentials.</param>
        /// <returns>User details and JWT token if credentials are valid; otherwise, Unauthorized.</returns>
        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
        {
            var user = await _userManager.FindByNameAsync(loginDto.Username);

            if(user == null || !(await _userManager.CheckPasswordAsync(user,loginDto.Password)))
             return Unauthorized();

            return new UserDto
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email!,
                Token = await _tokenService.GenerateToken(user),
            };
        }

        /// <summary>
        /// Registers a new user account.
        /// </summary>
        /// <param name="registerDto">User registration data.</param>
        /// <returns>HTTP 201 Created on success; ValidationProblem on failure.</returns>
        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<ActionResult> Register(RegisterDto registerDto)
        {
            var existingUser = await _userManager.FindByNameAsync(registerDto.Username);
            if (existingUser != null)
            {
                ModelState.AddModelError("Username", "Username is already taken");
                return ValidationProblem();
            }
            if (registerDto.Password != registerDto.ConfirmPassword)
            {
                ModelState.AddModelError("ConfirmPassword", "Passwords do not match");
                return ValidationProblem();
            }

            var user = new User{FirstName=registerDto.FirstName,LastName=registerDto.LastName, UserName = registerDto.Username,Email = registerDto.Email,Company = registerDto.Comapny};

            var result = await _userManager.CreateAsync(user,registerDto.Password);

            if(!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(error.Code,error.Description);
                }
                return ValidationProblem();
            }
            await _userManager.AddToRoleAsync(user,"Member");

            return StatusCode(201);
        }
        
          /// <summary>
        /// Retrieves the currently authenticated user's details and a new JWT token.
        /// </summary>
        /// <returns>The authenticated user's information and a new token.</returns>
        [Authorize]
        [HttpGet("currentUser")]
        public async Task<ActionResult<UserDto>> GetCurrentUser()
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            
            return new UserDto
            {
                FirstName = user!.FirstName,
                LastName = user.LastName,
                Email = user.Email!,
                Token = await _tokenService.GenerateToken(user),
            };
        }
       
    }

}