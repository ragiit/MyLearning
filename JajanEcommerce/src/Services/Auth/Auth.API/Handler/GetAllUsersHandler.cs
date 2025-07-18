using Auth.API.Dtos;
using BuildingBlocks.CQRS;
using Microsoft.EntityFrameworkCore;

namespace Auth.API.Handler;

// QUERY
public sealed record GetAllUsersQuery() : IQuery<GetAllUsersResult>;

// RESULT
public sealed record GetAllUsersResult(List<UserDto> Users);

public sealed class GetAllUsersHandler(AppDbContext db)
    : IQueryHandler<GetAllUsersQuery, GetAllUsersResult>
{
    public async Task<GetAllUsersResult> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
    {
        var users = await db.ApplicationUsers
            .AsNoTracking()
            .Select(u => new UserDto
            {
                ID = u.Id,
                Email = u.Email ?? string.Empty,
                Name = u.Name ?? string.Empty,
                PhoneNumber = u.PhoneNumber ?? string.Empty
            })
            .ToListAsync(cancellationToken);

        return new GetAllUsersResult(users);
    }
}