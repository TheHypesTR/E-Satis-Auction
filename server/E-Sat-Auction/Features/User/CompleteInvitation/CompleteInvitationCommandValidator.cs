using e_Sat_Auction.Common.Constants;
using e_Sat_Auction.Common.Helpers;
using FluentValidation;

namespace e_Sat_Auction.Features.User.CompleteInvitation;

public class CompleteInvitationCommandValidator : AbstractValidator<CompleteInvitationCommand>
{
    public CompleteInvitationCommandValidator()
    {
        RuleFor(x => x.EncryptedPayload)
            .NotEmpty().WithMessage(ErrorMessages.Validation.InvalidInvitationLink);

        RuleFor(x => x.FirstName)
            .Must(x => !string.IsNullOrWhiteSpace(x)).WithMessage(ErrorMessages.Validation.FirstNameRequired)
            .Length(2, 64).WithMessage(ErrorMessages.Validation.FirstNameLength);

        RuleFor(x => x.LastName)
            .Must(x => !string.IsNullOrWhiteSpace(x)).WithMessage(ErrorMessages.Validation.LastNameRequired)
            .Length(2, 64).WithMessage(ErrorMessages.Validation.LastNameLength);

        RuleFor(x => x.PhoneNumber)
            .NotEmpty().WithMessage(ErrorMessages.Validation.PhoneRequired)
            .Matches(@"^\+?[1-9]\d{1,14}$").WithMessage(ErrorMessages.Validation.InvalidPhone);

        RuleFor(x => x.NewPassword)
            .NotEmpty().WithMessage(ErrorMessages.Validation.PasswordRequired)
            .MinimumLength(8).WithMessage(ErrorMessages.Validation.PasswordMinLength)
            .Matches("[A-Z]").WithMessage(ErrorMessages.Validation.PasswordUpper)
            .Matches("[a-z]").WithMessage(ErrorMessages.Validation.PasswordLower)
            .Matches("[0-9]").WithMessage(ErrorMessages.Validation.PasswordNumber);

        RuleFor(x => x.ConfirmPassword)
            .NotEmpty().WithMessage(ErrorMessages.Validation.ConfirmPassword)
            .Equal(x => x.NewPassword).WithMessage(ErrorMessages.Validation.ConfirmPassword);

        RuleFor(x => x.TCNumber)
            .Must(TCNoHelper.IsValid).WithMessage(ErrorMessages.Validation.InvalidTC)
            .When(x => !string.IsNullOrEmpty(x.TCNumber));

        RuleFor(x => x.Gender)
            .NotEmpty().WithMessage(ErrorMessages.Validation.GenderRequired)
            .IsInEnum().WithMessage(ErrorMessages.Validation.InvalidGender);

        RuleFor(x => x.BirthDate)
            .NotEmpty().WithMessage(ErrorMessages.Validation.BirthDateRequired)
            .LessThan(DateTime.Now.Date).WithMessage(ErrorMessages.Validation.BirthDateInPast)
            .GreaterThan(new DateTime(1900, 1, 1)).WithMessage(ErrorMessages.Validation.BirthDateInvalid);
    }
}