// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PersonModelValidator.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

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
