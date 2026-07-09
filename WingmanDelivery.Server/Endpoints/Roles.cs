using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using WingmanDelivery.BusinessLogic.Services.Interfaces;
using WingmanDelivery.Models;
using WingmanDelivery.Server.Extensions;

namespace WingmanDelivery.Server.Endpoints;

public class RoleEndpoints : IEndpointModule
{
    public void MapEndpoints(IEndpointRouteBuilder app)
    {
        // Define route group prefix: /api/roles
        var group = app.MapGroup("/api/roles").WithTags("Security Roles");

        group.MapGet("/", GetRoles);
        group.MapGet("/{uid:guid}", FindRole);
        group.MapPost("/grid", GetRolesForGrid);
        group.MapPost("/", CreateRole);
        group.MapPut("/", UpdateRole);
        group.MapPost("/remove", DeleteRole);
    }

    private static async Task<IResult> GetRoles(IRoleService roleService)
    {
        var roles = await roleService.GetRolesAsync();
        return Results.Ok(roles);
    }

    private static async Task<IResult> FindRole(Guid uid, IRoleService roleService)
    {
        var role = await roleService.FindRoleAsync(uid);
        return role is not null
            ? Results.Ok(role)
            : Results.NotFound(new { Message = $"Role {uid} not found." });
    }

    private static async Task<IResult> GetRolesForGrid([FromBody] FilterModel filter, IRoleService roleService)
    {
        if (filter is null) return Results.BadRequest("Grid filters cannot be null.");

        var gridData = await roleService.GetRolesForGridAsync(filter);
        return Results.Ok(gridData);
    }

    private static async Task<IResult> CreateRole([FromBody] RoleModel model, IRoleService roleService)
    {
        if (model is null) return Results.BadRequest("Role creation payload parameters cannot be null.");

        try
        {
            var result = await roleService.CreateRoleAsync(model);
            return Results.Created($"/api/roles/{result.f_uid}", result);
        }
        catch (Exception ex)
        {
            return Results.Problem(ex.Message, statusCode: StatusCodes.Status500InternalServerError);
        }
    }

    private static async Task<IResult> UpdateRole([FromBody] RoleModel model, IRoleService roleService)
    {
        if (model is null) return Results.BadRequest("Role validation modification parameters cannot be null.");

        try
        {
            var result = await roleService.UpdateRoleAsync(model);
            return Results.Ok(result);
        }
        catch (Exception ex)
        {
            return Results.Problem(ex.Message, statusCode: StatusCodes.Status500InternalServerError);
        }
    }

    private static async Task<IResult> DeleteRole([FromBody] RoleModel model, IRoleService roleService)
    {
        if (model is null) return Results.BadRequest("Target deletion parameters cannot be null.");

        try
        {
            var recordsRemoved = await roleService.DeleteRoleAsync(model);
            return Results.Ok(new { Success = true, RowsAffected = recordsRemoved });
        }
        catch (Exception ex)
        {
            return Results.Problem(ex.Message, statusCode: StatusCodes.Status500InternalServerError);
        }
    }
}