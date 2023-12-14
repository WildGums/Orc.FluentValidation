namespace Orc.FluentValidation
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using Catel;
    using Catel.Data;
    using Catel.Fody;
    using Catel.Reflection;
    using global::FluentValidation;
    using IValidator = Catel.Data.IValidator;

    /// <summary>
    /// The fluent to catel validator adapter.
    /// </summary>
    [NoWeaving]
    public class FluentValidatorToCatelValidatorAdapter : ValidatorBase<ModelBase>
    {
        /// <summary>
        /// The validator.
        /// </summary>
        private readonly global::FluentValidation.IValidator _validator;

        /// <summary>
        /// The validator description attribute.
        /// </summary>
        private readonly ValidatorDescriptionAttribute _validatorDescriptionAttribute;

        /// <summary>
        /// Initializes a new instance of the <see cref="FluentValidatorToCatelValidatorAdapter"/> class.
        /// </summary>
        /// <param name="validatorType">
        /// The validator type.
        /// </param>
        private FluentValidatorToCatelValidatorAdapter(Type validatorType)
        {
            ArgumentNullException.ThrowIfNull(validatorType);

            var validator = Activator.CreateInstance(validatorType) as global::FluentValidation.IValidator;
            if (validator is null)
            {
                throw new InvalidOperationException($"Cannot create validator type '{validatorType.GetType().GetSafeFullName()}'");
            }

            _validator = validator;

            if (!validatorType.TryGetAttribute<ValidatorDescriptionAttribute>(out var validatorDescriptionAttribute))
            {
                validatorDescriptionAttribute = new ValidatorDescriptionAttribute(validatorType.Name);
            }

            _validatorDescriptionAttribute = validatorDescriptionAttribute;
        }

        /// <summary>
        /// The validate business rules.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <param name="validationResults">The validation results.</param>
        protected override void ValidateBusinessRules(ModelBase instance, List<IBusinessRuleValidationResult> validationResults)
        {
            ArgumentNullException.ThrowIfNull(instance);
            ArgumentNullException.ThrowIfNull(validationResults);

            if (_validatorDescriptionAttribute.ValidationType == ValidationType.BusinessRule)
            {
                var validationContext = new global::FluentValidation.ValidationContext<ModelBase>(instance);
                var validationResult = _validator.Validate(validationContext);
                if (!validationResult.IsValid)
                {
                    var dinstinctValidationResults = validationResult.Errors.Select(validationFailure => new BusinessRuleValidationResult(
                        _validatorDescriptionAttribute.ValidationResultType, validationFailure.ErrorMessage)
                    {
                        Tag = _validatorDescriptionAttribute.Tag
                    }).Distinct(new BusinessRuleValidationResultEqualityComparer()).Cast<IBusinessRuleValidationResult>();

                    validationResults.AddRange(dinstinctValidationResults);
                }
            }
        }

        /// <summary>
        /// The validate fields.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <param name="validationResults">The validation results.</param>
        protected override void ValidateFields(ModelBase instance, List<IFieldValidationResult> validationResults)
        {
            ArgumentNullException.ThrowIfNull(instance);
            ArgumentNullException.ThrowIfNull(validationResults);

            if (_validatorDescriptionAttribute.ValidationType == ValidationType.Field)
            {
                var validationContext = new global::FluentValidation.ValidationContext<ModelBase>(instance);
                var validationResult = _validator.Validate(validationContext);
                if (!validationResult.IsValid)
                {
                    var distinctValidationResults = validationResult.Errors.Select(fieldValidationResult => new FieldValidationResult(
                        fieldValidationResult.PropertyName, _validatorDescriptionAttribute.ValidationResultType,
                        fieldValidationResult.ErrorMessage)
                    {
                        Tag = _validatorDescriptionAttribute.Tag
                    }).Distinct(new FieldValidationResultEqualityComparer()).Cast<IFieldValidationResult>();

                    validationResults.AddRange(distinctValidationResults);
                }
            }
        }

        /// <summary>
        /// Creates an instance of <see cref="IValidator" /> from an <see cref="AbstractValidator{T}" /> type implementation.
        /// </summary>
        /// <param name="validatorType"><c>FluentValidation.IValidator</c> type implementation.</param>
        /// <returns>An instance of <see cref="FluentValidatorToCatelValidatorAdapter" />.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="validatorType" /> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">The <paramref name="validatorType" /> is not of type <see cref="IValidator" />.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="validatorType" /> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">The <paramref name="validatorType" /> is not of type <see cref="IValidator" />.</exception>
        public static IValidator From(Type validatorType)
        {
            ArgumentNullException.ThrowIfNull(validatorType);
            Argument.IsOfType("validatorType", validatorType, typeof(global::FluentValidation.IValidator));

            return new FluentValidatorToCatelValidatorAdapter(validatorType);
        }

        /// <summary>
        /// Creates an instance of <see cref="IValidator" /> from a collection of <see cref="AbstractValidator{T}" /> types.
        /// </summary>
        /// <param name="validatorTypes">Collection of <c>FluentValidation.IValidator</c> types.</param>
        /// <returns>An instance of a class the implements <see cref="Catel.Data.IValidator" />. If the collection contains one element an instance of <see cref="FluentValidatorToCatelValidatorAdapter" /> is returned otherwise
        /// a <see cref="CompositeValidator" /> is created in order to aggregate the validators in a single one.</returns>
        /// <exception cref="System.ArgumentException">Argument 'validatorTypes' must contains at least one element.</exception>
        /// <exception cref="ArgumentNullException">If <paramref name="validatorTypes" /> is <c>null</c>.</exception>
        public static IValidator From(IList<Type> validatorTypes)
        {
            ArgumentNullException.ThrowIfNull(validatorTypes);

            if (validatorTypes.Count == 0)
            {
                throw new ArgumentException("Argument 'validatorTypes' must contains at least one element.");
            }

            IValidator validator;

            if (validatorTypes.Count > 1)
            {
                var compositeValidator = new CompositeValidator();

                foreach (var validatorType in validatorTypes)
                {
                    compositeValidator.Add(From(validatorType));
                }

                validator = compositeValidator;
            }
            else
            {
                var firstValidator = validatorTypes.First();
                validator = From(firstValidator);
            }

            return validator;
        }

        /// <summary>
        /// Creates an instance of <see cref="IValidator" /> from a generic type <c>FluentValidation.IValidator</c> parameter.
        /// </summary>
        /// <typeparam name="TValidator">Type of <c>FluentValidation.IValidator</c>.</typeparam>
        /// <returns>An instance of <see cref="FluentValidatorToCatelValidatorAdapter" />.</returns>
        public static IValidator From<TValidator>()
            where TValidator : global::FluentValidation.IValidator, new()
        {
            return new FluentValidatorToCatelValidatorAdapter(typeof(TValidator));
        }

        private class BusinessRuleValidationResultEqualityComparer : EqualityComparer<IBusinessRuleValidationResult>
        {
            public override bool Equals([AllowNull] IBusinessRuleValidationResult x, [AllowNull] IBusinessRuleValidationResult y)
            {
                if (x is null && y is null)
                {
                    return true;
                }

                if (x is null || y is null)
                {
                    return false;
                }

                if (x.Message != y.Message)
                {
                    return false;
                }

                if (!ObjectHelper.AreEqual(x.Tag, y.Tag))
                {
                    return false;
                }

                return true;
            }

            public override int GetHashCode([DisallowNull] IBusinessRuleValidationResult obj)
            {
                if (obj is null)
                {
                    return 0;
                }

                var key = $"{obj.Message}_{obj.ValidationResultType}_{obj.Tag}";
                return key.GetHashCode();
            }
        }

        private class FieldValidationResultEqualityComparer : EqualityComparer<IFieldValidationResult>
        {
            public override bool Equals([AllowNull] IFieldValidationResult x, [AllowNull] IFieldValidationResult y)
            {
                if (x is null && y is null)
                {
                    return true;
                }

                if (x is null || y is null)
                {
                    return false;
                }

                if (x.PropertyName != y.PropertyName)
                {
                    return false;
                }

                if (x.Message != y.Message)
                {
                    return false;
                }

                if (!ObjectHelper.AreEqual(x.Tag, y.Tag))
                {
                    return false;
                }

                return true;
            }

            public override int GetHashCode([DisallowNull] IFieldValidationResult obj)
            {
                if (obj is null)
                {
                    return 0;
                }

                var key = $"{obj.PropertyName}_{obj.Message}_{obj.ValidationResultType}_{obj.Tag}";
                return key.GetHashCode();
            }
        }
    }
}
