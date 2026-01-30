using BookStore.Application.Common;
using BookStore.Application.DTOs;

namespace BookStore.Application.Interfaces;

public interface ILoanService
{
    Task<LoanDto> CreateLoanAsync(int userId, CreateLoanRequest request);
    Task<List<LoanDto>> GetUserLoansAsync(int userId);
    Task<PagedList<LoanDto>> GetUserLoansPagedAsync(int userId, int pageNumber, int pageSize);

}
