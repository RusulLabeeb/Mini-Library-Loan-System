using BookStore.Application.DTOs;
using BookStore.Application.Common;
using BookStore.Application.Interfaces;
using BookStore.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BookStore.Application.Services;

public class LoanService : ILoanService
{
    private readonly IBookStoreDbContext _context;

    public LoanService(IBookStoreDbContext context)
    {
        _context = context;
    }

    public async Task<LoanDto> CreateLoanAsync(int userId, CreateLoanRequest request)
    {
        var loan = new Loan
        {
            UserId = userId,
            BookId = request.BookId,
            LoanDate = DateTime.UtcNow,
            ReturnDate = null
        };

        _context.Loans.Add(loan);
        await _context.SaveChangesAsync(CancellationToken.None);

        // Fetch the book title for the response
        var book = await _context.Books.FindAsync(request.BookId);

        return new LoanDto
        {
            Id = loan.Id,
            UserId = loan.UserId,
            BookId = loan.BookId,
            BookTitle = book?.Title ?? string.Empty,
            LoanDate = loan.LoanDate,
            ReturnDate = loan.ReturnDate
        };
    }

    public async Task<List<LoanDto>> GetUserLoansAsync(int userId)
    {
        var loans = await _context.Loans
            .Include(l => l.Book)
            .Where(l => l.UserId == userId)
            .Select(l => new LoanDto
            {
                Id = l.Id,
                UserId = l.UserId,
                BookId = l.BookId,
                BookTitle = l.Book.Title,
                LoanDate = l.LoanDate,
                ReturnDate = l.ReturnDate
            })
            .ToListAsync();

        return loans;
    }

    public async Task<PagedList<LoanDto>> GetUserLoansPagedAsync(int userId, int pageNumber, int pageSize)
    {
        var query = _context.Loans
            .Include(l => l.Book)
            .Where(l => l.UserId == userId)
            .Select(l => new LoanDto
            {
                Id = l.Id,
                UserId = l.UserId,
                BookId = l.BookId,
                BookTitle = l.Book.Title,
                LoanDate = l.LoanDate,
                ReturnDate = l.ReturnDate
            });

        return PagedList<LoanDto>.Create(query, pageNumber, pageSize);
    }
}
