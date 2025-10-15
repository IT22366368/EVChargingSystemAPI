// ========================================
// Middleware/OwnAccountMiddleware.cs
// ========================================
/*
 * OwnAccountMiddleware.cs
 * Authorization Middleware Attribute
 * Date: September 2025
 * Description: Ensures that only authorized users (EV Owners, Admins, Station Users)
 *              can access or modify their own accounts based on NIC or User ID.
 */


using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using SparkPoint_Server.Helpers;
using SparkPoint_Server.Models;
using SparkPoint_Server.Constants;
using MongoDB.Driver;

namespace SparkPoint_Server.Attributes
{
    /// <summary>
    /// Middleware attribute used to validate account ownership and access rights.
    /// Restricts EV Owners from accessing or modifying accounts that are not their own.
    /// Admins and Station Users have full access permissions.
    /// </summary>

    public class OwnAccountMiddleware : ActionFilterAttribute
    {
        private readonly string _nicParameterName;
        private readonly string _userIdParameterName;

        /// <summary>
        /// Constructor for User ID-based ownership validation.
        /// </summary>
   

        public OwnAccountMiddleware(string nicParameterName = "nic")
        {
            _nicParameterName = nicParameterName;
        }

        public OwnAccountMiddleware(string userIdParameterName, bool isUserIdBased)
        {
            if (isUserIdBased)
            {
                _userIdParameterName = userIdParameterName;
            }
        }

        /// <summary>
        /// Executes before the action method runs. 
        /// Validates that the current user is allowed to perform the requested action.
        /// </summary>

        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            try
            {
                var controller = actionContext.ControllerContext.Controller as System.Web.Http.ApiController;
                if (controller == null)
                {
                    actionContext.Response = actionContext.Request.CreateResponse(
                        HttpStatusCode.InternalServerError,
                        new { error = "Internal error", message = "Unable to access controller context" }
                    );
                    return;
                }

                var currentUserId = UserContextHelper.GetCurrentUserId(controller);
                if (string.IsNullOrEmpty(currentUserId))
                {
                    actionContext.Response = actionContext.Request.CreateResponse(
                        HttpStatusCode.Unauthorized,
                        new { error = "Authentication required", message = "Unable to identify current user" }
                    );
                    return;
                }

                var currentUserRole = UserContextHelper.GetCurrentUserRole(controller);

                // Admin and Station User roles have full access
                if (currentUserRole == AuthorizationConstants.Roles.Admin || 
                    currentUserRole == AuthorizationConstants.Roles.StationUser)
                {
                    base.OnActionExecuting(actionContext);
                    return;
                }

                // EV Owner role needs ownership validation
                if (currentUserRole == AuthorizationConstants.Roles.EVOwner)
                {
                    if (!ValidateEVOwnerOwnership(actionContext, currentUserId))
                    {
                        actionContext.Response = actionContext.Request.CreateResponse(
                            HttpStatusCode.Forbidden,
                            new { error = "Access denied", message = "You can only access your own account" }
                        );
                        return;
                    }
                }
                else
                {
                    actionContext.Response = actionContext.Request.CreateResponse(
                        HttpStatusCode.Forbidden,
                        new { error = "Access denied", message = "You are not authorized to perform this operation" }
                    );
                    return;
                }

                base.OnActionExecuting(actionContext);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"OwnAccountMiddleware error: {ex.Message}");
                
                actionContext.Response = actionContext.Request.CreateResponse(
                    HttpStatusCode.InternalServerError,
                    new { error = "Internal error", message = "Error validating account ownership" }
                );
            }
        }

        /// <summary>
        /// Validates that the current EV Owner has permission to access or modify their own data.
        /// </summary>
        /// <returns>True if ownership validation passes; otherwise, false.</returns>


        private bool ValidateEVOwnerOwnership(HttpActionContext actionContext, string currentUserId)
        {
            var dbContext = new MongoDbContext();
            var evOwnersCollection = dbContext.GetCollection<EVOwner>("EVOwners");

            if (!string.IsNullOrEmpty(_nicParameterName))
            {
                var nicValue = GetParameterValue(actionContext, _nicParameterName);

                if (string.IsNullOrEmpty(nicValue))
                {
                    var currentUserEvOwner = evOwnersCollection.Find(o => o.UserId == currentUserId).FirstOrDefault();
                    return currentUserEvOwner != null;
                }

                var evOwner = evOwnersCollection.Find(o => o.NIC == nicValue).FirstOrDefault();
                return evOwner != null && evOwner.UserId == currentUserId;
            }

            if (!string.IsNullOrEmpty(_userIdParameterName))
            {
                var userIdValue = GetParameterValue(actionContext, _userIdParameterName);
                return userIdValue == currentUserId;
            }

            var evOwnerRecord = evOwnersCollection.Find(o => o.UserId == currentUserId).FirstOrDefault();
            return evOwnerRecord != null;
        }

        /// <summary>
        /// Retrieves the specified parameter value from either the action arguments or route data.
        /// </summary>

        private string GetParameterValue(HttpActionContext actionContext, string parameterName)
        {
            if (actionContext.ActionArguments.ContainsKey(parameterName))
            {
                return actionContext.ActionArguments[parameterName]?.ToString();
            }

            if (actionContext.ControllerContext.RouteData.Values.ContainsKey(parameterName))
            {
                return actionContext.ControllerContext.RouteData.Values[parameterName]?.ToString();
            }

            return null;
        }
    }
}