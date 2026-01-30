using BookStore.Api.Common;
using BookStore.Application.Common;
using BookStore.Application.DTOs;
using BookStore.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookStore.Api.Controllers;

public class UsersController(ILoanService loanService) : BaseController
{
    [HttpGet("{userId:int}/loans")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<PagedList<LoanDto>>>> GetUserLoans(int userId, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        var result = await loanService.GetUserLoansPagedAsync(userId, pageNumber, pageSize);
        return Ok(new ApiResponse<PagedList<LoanDto>>(result));
    }
}
