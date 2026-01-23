using BookStore.Application.Interfaces;

namespace BookStore.Infrastructure.Services;

public class PasswordHasher : IPasswordHasher
{
    // Generate a salt and hash the password with it.
    // BCrypt handles the salt storage inside the hash string itself.
    public string Hash(string password) => BCrypt.Net.BCrypt.HashPassword(password);
    
    // Extracts the salt from 'hashedPassword', 
    // hashes the input 'password' with that salt, 
    // and checks if the result matches.
    public bool Verify(string password, string hash) => BCrypt.Net.BCrypt.Verify(password, hash);
}