namespace Orc.FluentValidation.Tests.Validators
{
    using Catel.Data;

    using global::FluentValidation;

    using ViewModels;

    using NUnit.Framework;

    /// <summary>
    /// The person view model validator.
    /// </summary>
    [ValidatorDescription("Person")]
    public class PersonViewModelValidator : AbstractValidator<PersonViewModel>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PersonViewModelValidator"/> class.
        /// </summary>
        public PersonViewModelValidator()
        {
            ClassLevelCascadeMode = CascadeMode.Continue;

            RuleFor(model => model.PersonFirstName).NotNull().NotEmpty();
            RuleFor(model => model.PersonLastName).NotNull().NotEmpty();
        }
    }

    /// <summary>
    /// The person view model validator warnings.
    /// </summary>
    [ValidatorDescription("Person", ValidationResultType.Warning, ValidationType.BusinessRule)]
    public class PersonViewModelValidatorWarnings : AbstractValidator<PersonViewModel>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PersonViewModelValidatorWarnings"/> class.
        /// </summary>
        public PersonViewModelValidatorWarnings()
        {
            ClassLevelCascadeMode = CascadeMode.Continue;

            RuleFor(model => model.PersonFirstName).NotNull().Length(3, 20);
            RuleFor(model => model.PersonLastName).NotNull().Length(3, 20);
        }
    }

    public class JustAnotherPersonViewModelValidatorButDoNothing : AbstractValidator<PersonViewModel>
    {
    }
}
