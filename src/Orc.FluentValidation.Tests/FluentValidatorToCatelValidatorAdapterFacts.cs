namespace Orc.FluentValidation.Tests
{
    using System;
    using System.Collections.Generic;

    using Catel.Data;
    using NUnit.Framework;
    using Validators;

    /// <summary>
    /// The fluent validator to catel validator adapter test.
    /// </summary>
    public class FluentValidatorToCatelValidatorAdapterFacts
    {
        /// <summary>
        /// The the from generic method.
        /// </summary>
        [TestFixture]
        public class TheFromGenericMethod
        {
            /// <summary>
            /// The creates the adapter validator from a collection with a single validator type.
            /// </summary>
            [TestCase]
            public void CreatesTheAdapterValidatorFromACollectionWithASingleValidatorType()
            {
                var validator = FluentValidatorToCatelValidatorAdapter.From<PersonViewModelValidatorWarnings>();
                Assert.IsInstanceOf(typeof(FluentValidatorToCatelValidatorAdapter), validator);
            }
        }

        /// <summary>
        /// The the from method.
        /// </summary>
        [TestFixture]
        public class TheFromMethod
        {
            /// <summary>
            /// The from helper method creates validator from a collection with a single validator type element.
            /// </summary>
            [TestCase]
            public void CreatesACompositeValidatorFromACollectionOfValidatorType()
            {
                IValidator validator =
                    FluentValidatorToCatelValidatorAdapter.From(
                        new List<Type> { typeof(PersonViewModelValidatorWarnings), typeof(PersonViewModelValidator) });
                Assert.IsInstanceOf(typeof(CompositeValidator), validator);
            }

            /// <summary>
            /// The from helper method creates validator from a collection with a single validator type element.
            /// </summary>
            [TestCase]
            public void CreatesTheAdapterValidatorFromACollectionWithASingleValidatorType()
            {
                IValidator validator =
                    FluentValidatorToCatelValidatorAdapter.From(
                        new List<Type> { typeof(PersonViewModelValidatorWarnings) });
                Assert.IsInstanceOf(typeof(FluentValidatorToCatelValidatorAdapter), validator);
            }

            /// <summary>
            /// The from helper method must throw argument exception if the list is empty.
            /// </summary>
            [TestCase]
            public void ThrowsArgumentExceptionIfTheListIsEmpty()
            {
                Assert.Throws<ArgumentException>(() => FluentValidatorToCatelValidatorAdapter.From(new List<Type>()));
            }
        }
    }
}
