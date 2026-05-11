using E_Satis_Auction.Common.Entities.Interfaces;
using E_Satis_Auction.Enums;
using Microsoft.AspNetCore.Identity;

namespace E_Satis_Auction.Models.Users;

public sealed class AppUser : IdentityUser, IAuditableEntity, ISoftDeletable
{
    public string FirstName { get; private set; }
    public string LastName { get; private set; }
    public string? TCNumber { get; private set; }
    public Gender Gender { get; private set; }
    public DateTime BirthDate { get; private set; }
    public UserStatus UserStatus { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }
    public bool IsDeleted { get; private set; } = false;
    public DateTime? DeletedAt { get; private set; }
    public string? RefreshToken { get; private set; }
    public DateTime? RefreshTokenExpiryTime { get; private set; }

    private AppUser()
    {
        FirstName = string.Empty;
        LastName = string.Empty;
        Gender = Gender.PreferNotToSay;
        RefreshToken = string.Empty;
    }

    private AppUser(
        string firstName,
        string lastName,
        string email,
        string phoneNumber,
        string? tcNumber,
        Gender gender,
        DateTime birthDate,
        UserStatus userStatus)
    {
        FirstName = firstName;
        LastName = lastName;
        Email = email;
        PhoneNumber = phoneNumber;
        UserName = email;
        TCNumber = tcNumber;
        Gender = gender;
        BirthDate = birthDate.ToUniversalTime();
        UserStatus = userStatus;
    }

    public static AppUser Add(
        string firstName,
        string lastName,
        string email,
        string phoneNumber,
        string? tcNumber,
        Gender gender,
        DateTime birthDate)
    {
        return new AppUser(firstName, lastName, email, phoneNumber, tcNumber, gender, birthDate, UserStatus.Active);
    }

    public static AppUser AddInvited(string firstName, string lastName, string email)
    {
        return new AppUser(
            firstName,
            lastName,
            email,
            string.Empty,
            null,
            Gender.PreferNotToSay,
            DateTime.UtcNow,
            UserStatus.Invited);
    }

    public void CompleteInvitation(
        string firstName,
        string lastName,
        string? tcNumber,
        string phoneNumber,
        DateTime birthDate,
        Gender gender)
    {
        FirstName = firstName;
        LastName = lastName;
        TCNumber = tcNumber;
        PhoneNumber = phoneNumber;
        BirthDate = birthDate.ToUniversalTime();
        Gender = gender;
        EmailConfirmed = true;
        UserStatus = UserStatus.Active;
    }

    public void UpdateRefreshToken(string refreshToken, DateTime refreshTokenExpiryTime)
    {
        RefreshToken = refreshToken;
        RefreshTokenExpiryTime = refreshTokenExpiryTime.ToUniversalTime();
    }

    public void Delete()
    {
        IsDeleted = true;
        DeletedAt = DateTime.UtcNow;
        UserStatus = UserStatus.Suspended;
    }
}