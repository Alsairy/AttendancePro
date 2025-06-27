using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using AttendancePlatform.Integrations.Api.Services;
using System.Threading.Tasks;

namespace AttendancePlatform.Integrations.Api.Controllers
{
    [ApiController]
    [Route("scim/v2")]
    [Authorize]
    public class ScimController : ControllerBase
    {
        private readonly IScimService _scimService;

        public ScimController(IScimService scimService)
        {
            _scimService = scimService;
        }

        [HttpGet("Users")]
        public async Task<IActionResult> GetUsers([FromQuery] int startIndex = 1, [FromQuery] int count = 100, [FromQuery] string filter = null)
        {
            var users = await _scimService.GetUsersAsync(startIndex, count, filter);
            var response = new
            {
                schemas = new[] { "urn:ietf:params:scim:api:messages:2.0:ListResponse" },
                totalResults = users.Count(),
                startIndex,
                itemsPerPage = count,
                Resources = users
            };
            return Ok(response);
        }

        [HttpGet("Users/{id}")]
        public async Task<IActionResult> GetUser(string id)
        {
            var user = await _scimService.GetUserAsync(id);
            if (user == null) return NotFound();
            
            user.Schemas = new[] { "urn:ietf:params:scim:schemas:core:2.0:User" };
            return Ok(user);
        }

        [HttpPost("Users")]
        public async Task<IActionResult> CreateUser([FromBody] ScimUserDto user)
        {
            var createdUser = await _scimService.CreateUserAsync(user);
            createdUser.Schemas = new[] { "urn:ietf:params:scim:schemas:core:2.0:User" };
            return CreatedAtAction(nameof(GetUser), new { id = createdUser.Id }, createdUser);
        }

        [HttpPut("Users/{id}")]
        public async Task<IActionResult> UpdateUser(string id, [FromBody] ScimUserDto user)
        {
            var updatedUser = await _scimService.UpdateUserAsync(id, user);
            if (updatedUser == null) return NotFound();
            
            updatedUser.Schemas = new[] { "urn:ietf:params:scim:schemas:core:2.0:User" };
            return Ok(updatedUser);
        }

        [HttpDelete("Users/{id}")]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var result = await _scimService.DeleteUserAsync(id);
            if (!result) return NotFound();
            
            return NoContent();
        }

        [HttpGet("Groups")]
        public async Task<IActionResult> GetGroups([FromQuery] int startIndex = 1, [FromQuery] int count = 100, [FromQuery] string filter = null)
        {
            var groups = await _scimService.GetGroupsAsync(startIndex, count, filter);
            var response = new
            {
                schemas = new[] { "urn:ietf:params:scim:api:messages:2.0:ListResponse" },
                totalResults = groups.Count(),
                startIndex,
                itemsPerPage = count,
                Resources = groups
            };
            return Ok(response);
        }

        [HttpGet("Groups/{id}")]
        public async Task<IActionResult> GetGroup(string id)
        {
            var group = await _scimService.GetGroupAsync(id);
            if (group == null) return NotFound();
            
            group.Schemas = new[] { "urn:ietf:params:scim:schemas:core:2.0:Group" };
            return Ok(group);
        }

        [HttpPost("Groups")]
        public async Task<IActionResult> CreateGroup([FromBody] ScimGroupDto group)
        {
            var createdGroup = await _scimService.CreateGroupAsync(group);
            createdGroup.Schemas = new[] { "urn:ietf:params:scim:schemas:core:2.0:Group" };
            return CreatedAtAction(nameof(GetGroup), new { id = createdGroup.Id }, createdGroup);
        }

        [HttpPut("Groups/{id}")]
        public async Task<IActionResult> UpdateGroup(string id, [FromBody] ScimGroupDto group)
        {
            var updatedGroup = await _scimService.UpdateGroupAsync(id, group);
            if (updatedGroup == null) return NotFound();
            
            updatedGroup.Schemas = new[] { "urn:ietf:params:scim:schemas:core:2.0:Group" };
            return Ok(updatedGroup);
        }

        [HttpDelete("Groups/{id}")]
        public async Task<IActionResult> DeleteGroup(string id)
        {
            var result = await _scimService.DeleteGroupAsync(id);
            if (!result) return NotFound();
            
            return NoContent();
        }

        [HttpGet("ResourceTypes")]
        public async Task<IActionResult> GetResourceTypes()
        {
            var resourceTypes = await _scimService.GetResourceTypesAsync();
            return Ok(resourceTypes);
        }

        [HttpGet("ResourceTypes/{resourceType}")]
        public async Task<IActionResult> GetResourceType(string resourceType)
        {
            var resource = await _scimService.GetResourceTypeAsync(resourceType);
            if (resource == null) return NotFound();
            
            return Ok(resource);
        }

        [HttpGet("Schemas")]
        public async Task<IActionResult> GetSchemas()
        {
            var schemas = await _scimService.GetSchemasAsync();
            return Ok(schemas);
        }

        [HttpGet("Schemas/{schemaId}")]
        public async Task<IActionResult> GetSchema(string schemaId)
        {
            var schema = await _scimService.GetSchemaAsync(schemaId);
            if (schema == null) return NotFound();
            
            return Ok(schema);
        }

        [HttpGet("ServiceProviderConfig")]
        public async Task<IActionResult> GetServiceProviderConfig()
        {
            var config = await _scimService.GetServiceProviderConfigAsync();
            return Ok(config);
        }
    }

}
