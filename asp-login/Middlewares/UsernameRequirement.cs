using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace asp_login.Middlewares
{
    public class UsernameRequirement : IAuthorizationRequirement
    {
        public string RequiredUsername { get; }

        public UsernameRequirement(string requiredUsername)
        {
            RequiredUsername = requiredUsername;
        }
    }

    public class UsernameAuthorizationHandler : AuthorizationHandler<UsernameRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, UsernameRequirement requirement)
        {
            if (context.User.Identity != null && context.User.Identity.Name == requirement.RequiredUsername)
            {
                context.Succeed(requirement);
            }
            else
            {
                context.Fail();
            }

            return Task.CompletedTask;
        }
    }
}