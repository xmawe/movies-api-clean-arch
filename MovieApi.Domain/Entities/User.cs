namespace MovieApi.Domain.Entities;

public class User
{
    public int Id { get; private set; }
    public string Username { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public string PasswordHash { get; private set; } = string.Empty;
    
    public ICollection<Movie> Movies { get; private set; } = new List<Movie>();

    private User() { }

    public static User Create(string username, string email, string passwordHash)
    {
        ValidateUsername(username);
        ValidateEmail(email);
        if (string.IsNullOrWhiteSpace(passwordHash))
            throw new ArgumentException("Password hash cannot be empty", nameof(passwordHash));

        return new User
        {
            Username = username,
            Email = email,
            PasswordHash = passwordHash
        };
    }

    public void UpdatePasswordHash(string newPasswordHash)
    {
        if (string.IsNullOrWhiteSpace(newPasswordHash))
            throw new ArgumentException("Password hash cannot be empty", nameof(newPasswordHash));
        PasswordHash = newPasswordHash;
    }

    private static void ValidateUsername(string username)
    {
        if (string.IsNullOrWhiteSpace(username))
            throw new ArgumentException("Username cannot be empty", nameof(username));
        
        if (username.Length < 3 || username.Length > 20)
            throw new ArgumentException("Username must be between 3 and 20 characters", nameof(username));
    }

    private static void ValidateEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email cannot be empty", nameof(email));
        
        if (!email.Contains("@"))
            throw new ArgumentException("Invalid email format", nameof(email));
        
        if (email.Length > 100)
            throw new ArgumentException("Email cannot exceed 100 characters", nameof(email));
    }
}