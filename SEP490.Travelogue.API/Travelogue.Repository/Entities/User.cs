using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Travelogue.Repository.Bases.BaseEntities;
using Travelogue.Repository.Entities.Enums;

namespace Travelogue.Repository.Entities;

public sealed class User : BaseEntity
{
    [Required, EmailAddress]
    public required string Email { get; set; }
    public bool? EmailConfirmed { get; set; }
    [Column(TypeName = "VARBINARY(128)")]
    public byte[]? PasswordSalt { get; set; }

    [Column(TypeName = "VARBINARY(64)")]
    public byte[]? PasswordHash { get; set; }
    public string? PhoneNumber { get; set; }
    public bool? PhoneNumberConfirmed { get; set; }
    public required string FullName { get; set; } = string.Empty;
    public string? ProfilePictureUrl { get; set; }
    public string? EmailCode { get; set; }
    public string? AvatarUrl { get; set; }

    public Gender Sex { get; set; } = Gender.Male;

    [StringLength(200)]
    public string? Address { get; set; }
    public string? GoogleId { get; set; }
    public bool? IsEmailVerified { get; set; }
    public string? ResetToken { get; set; }
    public DateTimeOffset? ResetTokenExpires { get; set; }
    public string? VerificationToken { get; set; }
    public DateTimeOffset? VerificationTokenExpires { get; set; }
    public DateTimeOffset? LockoutEnd { get; set; }
    public Wallet? Wallet { get; set; }
    public ICollection<UserRole>? UserRoles { get; set; }
    public ICollection<FavoriteLocation> FavoriteLocations { get; set; } = new List<FavoriteLocation>();

    public void SetPassword(string password)
    {
        var passwordSalt = new byte[0];
        var passwordHash = new byte[0];

        using (var hmac = new HMACSHA512())
        {
            passwordSalt = hmac.Key;
            passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
        }

        PasswordSalt = passwordSalt;
        PasswordHash = passwordHash;

        //using var rng = RandomNumberGenerator.Create();
        //var saltBytes = new byte[16];
        //rng.GetBytes(saltBytes);
        //PasswordSalt = saltBytes;

        //PasswordHash = KeyDerivation.Pbkdf2(
        //    password: password,
        //    salt: saltBytes,
        //    prf: KeyDerivationPrf.HMACSHA256,
        //    iterationCount: 10000,
        //    numBytesRequested: 32);
    }

    public bool VerifyPassword(string password)
    {
        if (PasswordSalt == null || PasswordHash == null)
        {
            return false;
        }

        var hashed = KeyDerivation.Pbkdf2(
            password: password,
            salt: PasswordSalt,
            prf: KeyDerivationPrf.HMACSHA256,
            iterationCount: 10000,
            numBytesRequested: 32);

        return PasswordHash.SequenceEqual(hashed);
    }

    // public void SetPassword(string password)
    // {
    //     var passwordSalt = new byte[0];
    //     var passwordHash = new byte[0];

    //     using (var hmac = new HMACSHA512())
    //     {
    //         passwordSalt = hmac.Key;
    //         passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
    //     }

    //     PasswordSalt = passwordSalt;
    //     PasswordHash = passwordHash;

    //     //using var rng = RandomNumberGenerator.Create();
    //     //var saltBytes = new byte[16];
    //     //rng.GetBytes(saltBytes);
    //     //PasswordSalt = saltBytes;

    //     //PasswordHash = KeyDerivation.Pbkdf2(
    //     //    password: password,
    //     //    salt: saltBytes,
    //     //    prf: KeyDerivationPrf.HMACSHA256,
    //     //    iterationCount: 10000,
    //     //    numBytesRequested: 32);
    // }

    // public bool VerifyPassword(string password)
    // {
    //     if (PasswordSalt == null || PasswordHash == null)
    //     {
    //         return false;
    //     }

    //     var hashed = KeyDerivation.Pbkdf2(
    //         password: password,
    //         salt: PasswordSalt,
    //         prf: KeyDerivationPrf.HMACSHA256,
    //         iterationCount: 10000,
    //         numBytesRequested: 32);

    //     return PasswordHash.SequenceEqual(hashed);
    // }

    // Navigation properties
    public ICollection<TripPlan> TripPlans { get; set; } = new List<TripPlan>();
    public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
    public ICollection<Report> Reports { get; set; } = new List<Report>();
    public ICollection<Review> Reviews { get; set; } = new List<Review>();
    public ICollection<TourGuide> TourGuides { get; set; } = new List<TourGuide>();
    public ICollection<Message> SentMessages { get; set; } = new List<Message>();
    public ICollection<Message> ReceivedMessages { get; set; } = new List<Message>();
    public ICollection<UserAnnouncement> UserAnnouncements { get; set; } = new List<UserAnnouncement>();
    public ICollection<BankAccount> BankAccounts { get; set; } = new List<BankAccount>();
    // public ICollection<TourGroupMember> TourGroupMembers { get; set; } = new List<TourGroupMember>();
}
