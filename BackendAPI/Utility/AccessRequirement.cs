using Microsoft.AspNetCore.Authorization;

// [Authorize(Policy = "AccessRequirement")]
public class AccessRequirement : IAuthorizationRequirement { }