namespace Catel.Test.Extensions.FluentValidation
{
    using Catel.Data;
    using Catel.IoC;

    using NUnit.Framework;
    using Orc.FluentValidation;
    using Orc.FluentValidation.Tests.Models;
    using Orc.FluentValidation.Tests.ViewModels;

    /// <summary>
    /// The person view model test fixture.
    /// </summary>
    public class FluentValidatorProviderFacts
    {
        /// <summary>
        /// The fluent validator provider test.
        /// </summary>
        [TestFixture]
        public class FluentValidatorProviderTest
        {
            [SetUp]
            public void SetUp()
            {
                ServiceLocator.Default.RegisterType<IValidatorProvider, FluentValidatorProvider>();
            }

            /// <summary>
            /// The validation 
            /// </summary>
            [TestCase]
            public void ModelBaseWithFieldValidationTest()
            {
                var personViewModel = new PersonViewModelWithModel { Person = new Person { FirstName = "Igr Alexánder", LastName = string.Empty } };
                
                // I have to add this call here
                ((IValidatable)personViewModel).Validate();

                var validationSummary = personViewModel.GetValidationContext().GetValidationSummary("Person");

                Assert.That(validationSummary.HasErrors, Is.True);
            }

            /// <summary>
            /// The person view model no errors test.
            /// </summary>
            [TestCase]
            public void ViewModelBaseNoErrorsTest()
            {
                var personViewModel = new PersonViewModel { PersonFirstName = "Igr Alexánder", PersonLastName = "Fernández Saúco" };

                ((IValidatable)personViewModel).Validate();

                var validationSummary = personViewModel.GetValidationContext().GetValidationSummary("Person");

                Assert.That(validationSummary.HasErrors, Is.False);
                Assert.That(validationSummary.HasWarnings, Is.False);
            }

            /// <summary>
            /// The person view model with field errors and business rule warnings test.
            /// </summary>
            [TestCase]
            public void ViewModelBaseWithFieldErrorsAndBusinessRuleWarningsValidationTest()
            {
                var personViewModel = new PersonViewModel();
                ((IValidatable)personViewModel).Validate();

                var validationSummary = personViewModel.GetValidationContext().GetValidationSummary("Person");

                Assert.That(validationSummary.HasErrors, Is.True);
                Assert.That(validationSummary.HasFieldErrors, Is.True);
                Assert.That(validationSummary.FieldErrors.Count, Is.EqualTo(2));
                Assert.That(validationSummary.FieldErrors[0].Message.Contains("First name"), Is.True);
                Assert.That(validationSummary.FieldErrors[1].Message.Contains("Last name"), Is.True);

                Assert.That(validationSummary.HasWarnings, Is.True);
                Assert.That(validationSummary.HasBusinessRuleWarnings, Is.True);
                Assert.That(validationSummary.BusinessRuleWarnings.Count, Is.EqualTo(2));
                Assert.That(validationSummary.BusinessRuleWarnings[0].Message.Contains("First name"), Is.True);
                Assert.That(validationSummary.BusinessRuleWarnings[1].Message.Contains("Last name"), Is.True);
            }

            /// <summary>
            /// The person view with out validator test.
            /// </summary>
            [TestCase]
            public void PersonViewWithOutValidatorTest()
            {
                var validatorProvider = ServiceLocator.Default.ResolveType<IValidatorProvider>();
                var validator = validatorProvider.GetValidator(typeof(NoFluentValidatorViewModel));

                Assert.That(validator, Is.Null);
            }
        }

        /// <summary>
        /// The the cache usage.
        /// </summary>
        [TestFixture]
        public class GetValidatorMethod
        {
            /// <summary>
            /// The init.
            /// </summary>
            [SetUp]
            public void SetUp()
            {
                ServiceLocator.Default.RegisterType<IValidatorProvider, FluentValidatorProvider>();
            }

            /// <summary>
            /// The person view with out validator test.
            /// </summary>
            [TestCase]
            public void MustReturnsTheSameInstanceIfCacheIsActive()
            {
                var validatorProvider = ServiceLocator.Default.ResolveType<IValidatorProvider>();

                var validator1 = validatorProvider.GetValidator(typeof(PersonViewModel));
                Assert.That(validator1, Is.Not.Null);

                var validator2 = validatorProvider.GetValidator(typeof(PersonViewModel));
                Assert.That(validator2, Is.EqualTo(validator1));
            }

            /// <summary>
            /// The returns different instances if it turned off.
            /// </summary>
            [TestCase]
            public void ReturnsDifferentInstancesIfCacheTurnedOff()
            {
                var validatorProvider = ServiceLocator.Default.ResolveType<IValidatorProvider>();
                var validator1 = validatorProvider.GetValidator(typeof(PersonViewModel));
                Assert.That(validator1, Is.Not.Null);
                ((ValidatorProviderBase)validatorProvider).UseCache = false;
                var validator2 = validatorProvider.GetValidator(typeof(PersonViewModel));
                Assert.That(validator2, Is.Not.EqualTo(validator1));
            }
        }
    }
}
