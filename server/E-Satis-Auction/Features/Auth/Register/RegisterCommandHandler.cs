using E_Satis_Auction.Extensions;
using E_Satis_Auction.Common;
using E_Satis_Auction.Common.Constants;
using E_Satis_Auction.Common.Exceptions;
using E_Satis_Auction.Common.Interfaces;
using E_Satis_Auction.Common.Interfaces.Messaging;
using E_Satis_Auction.Common.Options;
using E_Satis_Auction.Interfaces;
using E_Satis_Auction.Models.Users;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace E_Satis_Auction.Features.Auth.Register;

public class RegisterCommandHandler : ICommandHandler<RegisterCommand, Guid>
{
    private readonly UserManager<AppUser> _userManager;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IEmailService _emailService;
    private readonly IDataProtector _dataProtector;
    private readonly ClientOptions _clientOptions;
    
    private const string VERIFY_EMAIL = "verify-email";
    private const string VERIFY_EMAIL_PAYLOAD = "payload";

    public RegisterCommandHandler(
        UserManager<AppUser> userManager,
        IUnitOfWork unitOfWork,
        IEmailService emailService,
        IDataProtectionProvider dataProtectionProvider,
        IOptions<ClientOptions> clientOptions)
    {
        _userManager = userManager;
        _unitOfWork = unitOfWork;
        _emailService = emailService;
        _dataProtector = dataProtectionProvider.CreateProtector(DataProtectionPurposes.EmailVerification);
        _clientOptions = clientOptions.Value;
    }

    public async Task<Guid> Handle(RegisterCommand command, CancellationToken cancellationToken)
    {
        await CheckIfUserExistsAsync(command, cancellationToken);
        AppUser newUser = AppUser.Add(
            command.FirstName,
            command.LastName,
            command.Email,
            command.PhoneNumber,
            command.TCNumber,
            command.Gender!.Value,
            command.BirthDate!.Value);

        await ExecuteTransactionAsync(newUser, command.Password, cancellationToken);
        await SendVerificationEmailAsync(newUser);

        return Guid.Parse(newUser.Id);
    }

    private async Task CheckIfUserExistsAsync(RegisterCommand command, CancellationToken cancellationToken)
    {
        AppUser? existingUserByEmail = await _userManager.FindByEmailAsync(command.Email);
        BusinessException.ThrowIfNotNull(
            existingUserByEmail,
            ErrorMessages.Auth.EmailAlreadyRegistered,
            ErrorMessages.Exception.RegistrationTitle);

        if (!string.IsNullOrEmpty(command.TCNumber))
        {
            bool isTCNumberTaken = await _userManager.Users.AnyAsync(u =>
                u.TCNumber == command.TCNumber, cancellationToken);

            BusinessException.ThrowIfTrue(
                isTCNumberTaken,
                ErrorMessages.Auth.TCNAlreadyRegistered,
                ErrorMessages.Exception.RegistrationTitle);
        }
    }

    private async Task ExecuteTransactionAsync(AppUser newUser, string password, CancellationToken cancellationToken)
    {
        await _unitOfWork.BeginTransactionAsync(cancellationToken);
        try
        {
            IdentityResult result = await _userManager.CreateAsync(newUser, password);
            ThrowIfIdentityFailed(result);

            IdentityResult roleResult = await _userManager.AddToRoleAsync(newUser, AppRoles.User);
            ThrowIfIdentityFailed(roleResult);

            await _unitOfWork.CommitTransactionAsync(cancellationToken);
        }
        catch (Exception)
        {
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            throw;
        }
    }

    private async Task SendVerificationEmailAsync(AppUser user)
    {
        string token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        string urlEncodedPayload = _dataProtector.GenerateUrlEncodedPayload(user.Id, token);
        
        string baseUrl = _clientOptions.Url.TrimEnd('/');
        string verificationLink = $"{baseUrl}/{VERIFY_EMAIL}?{VERIFY_EMAIL_PAYLOAD}={urlEncodedPayload}";

        await _emailService.SendVerificationEmailAsync(user.Email!, user.FirstName, verificationLink);
    }

    private static void ThrowIfIdentityFailed(IdentityResult result)
    {
        if (result.Succeeded)
        {
            return;
        }

        List<ValidationFailure> failures = result.Errors
            .Select(e => new ValidationFailure(e.Code, MapIdentityErrorToMessageKey(e.Code)))
            .ToList();

        throw new ValidationException(failures);
    }

    private static string MapIdentityErrorToMessageKey(string identityCode)
    {
        return identityCode switch
        {
            "DuplicateEmail" or "DuplicateUserName" => ErrorMessages.Auth.EmailAlreadyRegistered,
            "InvalidEmail" => ErrorMessages.Validation.InvalidEmail,
            "PasswordTooShort" => ErrorMessages.Validation.PasswordMinLength,
            "PasswordRequiresUpper" => ErrorMessages.Validation.PasswordUpper,
            "PasswordRequiresLower" => ErrorMessages.Validation.PasswordLower,
            "PasswordRequiresDigit" => ErrorMessages.Validation.PasswordNumber,
            _ => ErrorMessages.Exception.ValidationErrorDetail
        };
    }
}