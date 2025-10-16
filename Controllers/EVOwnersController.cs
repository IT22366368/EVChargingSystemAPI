// ========================================
// Controllers/EVOwnersController.cs
// ========================================
/*
 * EVOwnersController.cs
 * EV Owner management controller
 * Date: October 2025
 * Description: Handles EV owner registration, profile management,
 * account activation/deactivation, and profile retrieval operations.
 */



using System;
using System.Security.Claims;
using System.Web.Http;
using SparkPoint_Server.Models;
using SparkPoint_Server.Services;
using SparkPoint_Server.Attributes;
using SparkPoint_Server.Helpers;
using SparkPoint_Server.Enums;

namespace SparkPoint_Server.Controllers
{
    [RoutePrefix("api/evowners")]
    public class EVOwnersController : ApiController
    {
        private readonly EVOwnerService _evOwnerService;

        public EVOwnersController()
        {
            _evOwnerService = new EVOwnerService();
        }

        /// <summary>
        /// Registers a new EV owner.
        /// Route: POST /api/evowners/register
        /// Access: Public
        /// </summary>
        [HttpPost]
        [Route("register")]
        public IHttpActionResult Register(EVOwnerRegisterModel model)
        {
            var result = _evOwnerService.Register(model);
            
            if (!result.IsSuccess)
            {
                return GetErrorResponse(result.Status, result.Message);
            }

            return Ok(result.Message);
        }

        /// <summary>
        /// Updates the EV owner's profile.
        /// Route: PUT /api/evowners/update
        /// Access: EV Owner Only
        /// </summary>
        

        [HttpPut]
        [Route("update")]
        [EVOwnerOnly]
        [OwnAccountMiddleware]
        public IHttpActionResult UpdateProfile(EVOwnerUpdateModel model)
        {
            var currentUserId = UserContextHelper.GetCurrentUserId(this);
            if (string.IsNullOrEmpty(currentUserId))
                return Unauthorized();

            var result = _evOwnerService.UpdateProfile(currentUserId, model);
            
            if (!result.IsSuccess)
            {
                return GetErrorResponse(result.Status, result.Message);
            }

            return Ok(result.Message);
        }


        /// <summary>
        /// Reactivates a deactivated EV owner account by NIC.
        /// Route: PUT /api/evowners/reactivate/{nic}
        /// Access: Admin Only
        /// </summary>
        

        [HttpPut]
        [Route("reactivate/{nic}")]
        [AdminOnly]
        [OwnAccountMiddleware("nic")]
        public IHttpActionResult ReactivateAccount(string nic)
        {
            var result = _evOwnerService.ReactivateAccount(nic);
            
            if (!result.IsSuccess)
            {
                return GetErrorResponse(result.Status, result.Message);
            }

            return Ok(result.Message);
        }

        /// <summary>
        /// Deactivates the logged-in EV owner’s account.
        /// Route: PUT /api/evowners/deactivate
        /// Access: EV Owner Only
        /// </summary>
        
        [HttpPut]
        [Route("deactivate")]
        [EVOwnerOnly]
        [OwnAccountMiddleware]
        public IHttpActionResult DeactivateAccount()
        {
            var currentUserId = UserContextHelper.GetCurrentUserId(this);
            if (string.IsNullOrEmpty(currentUserId))
                return Unauthorized();

            var result = _evOwnerService.DeactivateAccount(currentUserId);

            if (!result.IsSuccess)
            {
                return GetErrorResponse(result.Status, result.Message);
            }

            return Ok(result.Message);
        }

        /// <summary>
        /// Retrieves an EV owner profile by NIC.
        /// Route: GET /api/evowners/profile/{nic}
        /// Access: Admin and Station User
        /// </summary>

        [HttpGet]
        [Route("profile/{nic}")]
        [AdminAndStationUser]
        [OwnAccountMiddleware("nic")]
        public IHttpActionResult GetProfileByNic(string nic)
        {
            var result = _evOwnerService.GetProfileByNIC(nic);
            
            if (!result.IsSuccess)
            {
                return BadRequest(result.ErrorMessage);
            }

            return Ok(result.UserProfile);
        }

        /// <summary>
        /// Retrieves the logged-in EV owner’s profile.
        /// Route: GET /api/evowners/profile
        /// Access: EV Owner Only
        /// </summary>

        [HttpGet]
        [Route("profile")]
        [EVOwnerOnly]
        [OwnAccountMiddleware]
        public IHttpActionResult GetProfile()
        {
            var currentUserId = UserContextHelper.GetCurrentUserId(this);
            if (string.IsNullOrEmpty(currentUserId))
                return Unauthorized();

            var result = _evOwnerService.GetProfile(currentUserId);

            if (!result.IsSuccess)
            {
                return BadRequest(result.ErrorMessage);
            }

            return Ok(result.UserProfile);
        }

        /// <summary>
        /// Get all EV owners (active + inactive)
        /// Route: GET /api/evowners/all
        /// Access: Admin only
        /// </summary>
        [HttpGet]
        [Route("all")]
        [AdminOnly]
        public IHttpActionResult GetAllOwners()
        {
            var owners = _evOwnerService.GetAllEVOwners(); // null = all
            return Ok(owners);
        }

        /// <summary>
        /// Get only deactivated EV owners
        /// Route: GET /api/evowners/deactivated
        /// Access: Admin only
        /// </summary>
        [HttpGet]
        [Route("deactivated")]
        [AdminOnly]
        public IHttpActionResult GetDeactivatedOwners()
        {
            var owners = _evOwnerService.GetAllEVOwners(false); // false = only inactive
            return Ok(owners);
        }

        [HttpDelete]
        [Route("delete/{nic}")]
        [AdminOnly]
        public IHttpActionResult DeleteEVOwner(string nic)
        {
            var result = _evOwnerService.DeleteEVOwner(nic);

            if (!result.IsSuccess)
                return BadRequest(result.Message);

            return Ok(result.Message);
        }


        /// <summary>
        /// Admin creates a new EV owner account.
        /// Route: POST /api/evowners/admin/create
        /// Access: Admin Only
        /// </summary>
        [HttpPost]
        [Route("admin/create")]
        [AdminOnly]
        public IHttpActionResult AdminCreateEVOwner(EVOwnerRegisterModel model)
        {
            var result = _evOwnerService.Register(model); // Reuse existing Register method!

            if (!result.IsSuccess)
            {
                return GetErrorResponse(result.Status, result.Message);
            }

            return Ok(result.Message);
        }

        /// <summary>
        /// Admin updates any EV owner profile by NIC.
        /// Route: PUT /api/evowners/admin/update/{nic}
        /// Access: Admin Only
        /// </summary>
        [HttpPut]
        [Route("admin/update/{nic}")]
        [AdminOnly]
        public IHttpActionResult AdminUpdateEVOwner(string nic, EVOwnerUpdateModel model)
        {
            var result = _evOwnerService.AdminUpdateEVOwnerByNIC(nic, model);

            if (!result.IsSuccess)
            {
                return GetErrorResponse(result.Status, result.Message);
            }

            return Ok(result.Message);
        }

        /// <summary>
        /// Admin deactivates any EV owner account by NIC.
        /// Route: PUT /api/evowners/admin/deactivate/{nic}
        /// Access: Admin Only
        /// </summary>
        [HttpPut]
        [Route("admin/deactivate/{nic}")]
        [AdminOnly]
        public IHttpActionResult AdminDeactivateEVOwner(string nic)
        {
            var evOwner = _evOwnerService.GetEVOwnerByNIC(nic);
            if (evOwner == null)
            {
                return BadRequest("EV Owner not found.");
            }

            var result = _evOwnerService.DeactivateAccount(evOwner.UserId);

            if (!result.IsSuccess)
            {
                return GetErrorResponse(result.Status, result.Message);
            }

            return Ok(result.Message);
        }

        private IHttpActionResult GetErrorResponse(EVOwnerOperationStatus status, string errorMessage)
        {
            switch (status)
            {
                case EVOwnerOperationStatus.UserNotFound:
                case EVOwnerOperationStatus.EVOwnerNotFound:
                    return BadRequest(errorMessage);
                case EVOwnerOperationStatus.UsernameExists:
                case EVOwnerOperationStatus.EmailExists:
                case EVOwnerOperationStatus.PhoneExists:
                case EVOwnerOperationStatus.NICExists:
                    return BadRequest(errorMessage);
                case EVOwnerOperationStatus.ValidationFailed:
                    return BadRequest(errorMessage);
                case EVOwnerOperationStatus.AccountDeactivated:
                case EVOwnerOperationStatus.AccountAlreadyDeactivated:
                case EVOwnerOperationStatus.AccountAlreadyActive:
                    return BadRequest(errorMessage);
                case EVOwnerOperationStatus.NotAuthorized:
                    return Unauthorized();
                default:
                    return BadRequest(errorMessage);
            }
        }
    }
}
