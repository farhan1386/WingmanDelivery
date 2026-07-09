using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using WingmanDelivery.BusinessLogic.Services.Interfaces;
using WingmanDelivery.Models;
using WingmanDelivery.Server.Extensions;

namespace WingmanDelivery.Server.Endpoints;

public class UserEndpoints : IEndpointModule
{
    public void MapEndpoints(IEndpointRouteBuilder app)
    {
        // Define route group prefix: /api/users
        var group = app.MapGroup("/api/users").WithTags("System Members Profiles");

        group.MapGet("/", GetUsers);
        group.MapGet("/{uid:guid}", FindUser);
        group.MapGet("/email/{email}", GetUserByEmail);
        group.MapPost("/grid", GetUsersForGrid);
        group.MapPost("/", CreateUser);
        group.MapPut("/", UpdateUser);
        group.MapPost("/remove", DeleteUser);
    }

    private static async Task<IResult> GetUsers(IUserService userService)
    {
        var users = await userService.GetUsersAsync();
        return Results.Ok(users);
    }

    private static async Task<IResult> FindUser(Guid uid, IUserService userService)
    {
        var user = await userService.FindUserAsync(uid);
        return user is not null
            ? Results.Ok(user)
            : Results.NotFound(new { Message = $"User workspace node {uid} not found." });
    }

    private static async Task<IResult> GetUserByEmail(string email, IUserService userService)
    {
        var user = await userService.GetByEmailAsync(email);
        return user is not null
            ? Results.Ok(user)
            : Results.NotFound(new { Message = $"Profile containing email {email} not found." });
    }

    private static async Task<IResult> GetUsersForGrid([FromBody] FilterModel filter, IUserService userService)
    {
        if (filter is null) return Results.BadRequest("Grid filter bounds cannot be null.");

        var gridData = await userService.GetUsersForGridAsync(filter);
        return Results.Ok(gridData);
    }

    private static async Task<IResult> CreateUser([FromBody] UserModel model, IUserService userService)
    {
        if (model is null) return Results.BadRequest("User profile instantiation payload parameters cannot be null.");

        try
        {
            var result = await userService.CreateUserAsync(model);
            return Results.Created($"/api/users/{result.f_uid}", result);
        }
        catch (Exception ex)
        {
            return Results.Problem(ex.Message, statusCode: StatusCodes.Status500InternalServerError);
        }
    }

    private static async Task<IResult> UpdateUser([FromBody] UserModel model, IUserService userService)
    {
        if (model is null) return Results.BadRequest("User alteration target parameter models cannot be null.");

        try
        {
            var result = await userService.UpdateUserAsync(model);
            return Results.Ok(result);
        }
        catch (Exception ex)
        {
            return Results.Problem(ex.Message, statusCode: StatusCodes.Status500InternalServerError);
        }
    }

    private static async Task<IResult> DeleteUser([FromBody] UserModel model, IUserService userService)
    {
        if (model is null) return Results.BadRequest("Target context deletion parameters cannot be null.");

        try
        {
            var linesDestroyed = await userService.DeleteUserAsync(model);
            return Results.Ok(new { Success = true, RowsAffected = linesDestroyed });
        }
        catch (Exception ex)
        {
            return Results.Problem(ex.Message, statusCode: StatusCodes.Status500InternalServerError);
        }
    }
}