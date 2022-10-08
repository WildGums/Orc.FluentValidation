namespace Orc.FluentValidation.Tests.Validators
{
    using global::FluentValidation;
    using Models;

    /// <summary>
    /// The person model validator.
    /// </summary>
    public class PersonModelValidator : AbstractValidator<Person>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PersonModelValidator"/> class.
        /// </summary>
        public PersonModelValidator()
        {
            ClassLevelCascadeMode = CascadeMode.Continue;

            RuleFor(person => person.FirstName).NotNull().NotEmpty();
            RuleFor(person => person.LastName).NotNull().NotEmpty();
        }
    }
}
