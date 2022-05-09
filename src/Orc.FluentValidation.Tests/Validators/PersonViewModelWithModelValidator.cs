// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PersonViewModelWithModelValidator.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Orc.FluentValidation.Tests.Validators
{
    using global::FluentValidation;
    using ViewModels;

    /// <summary>
    /// The person view model with model validator.
    /// </summary>
    [ValidatorDescription("Person")]
    public class PersonViewModelWithModelValidator : AbstractValidator<PersonViewModelWithModel>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PersonViewModelWithModelValidator"/> class.
        /// </summary>
        public PersonViewModelWithModelValidator()
        {
            ClassLevelCascadeMode = CascadeMode.Continue;

            RuleFor(model => model.Person).SetValidator(new PersonModelValidator());
        }
    }
}
