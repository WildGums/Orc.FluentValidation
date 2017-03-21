// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FluentValidatorToCatelValidatorAdapter.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Orc.FluentValidation
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Catel;
    using Catel.Data;
    using Catel.Reflection;
    using global::FluentValidation;
    using IValidator = Catel.Data.IValidator;

    /// <summary>
    /// The fluent to catel validator adapter.
    /// </summary>
    public class FluentValidatorToCatelValidatorAdapter : ValidatorBase<ModelBase>
    {
        #region Constants and Fields

        /// <summary>
        /// The validator.
        /// </summary>
        private readonly global::FluentValidation.IValidator _validator;

        /// <summary>
        /// The validator description attribute.
        /// </summary>
        private readonly ValidatorDescriptionAttribute _validatorDescriptionAttribute;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="FluentValidatorToCatelValidatorAdapter"/> class.
        /// </summary>
        /// <param name="validatorType">
        /// The validator type.
        /// </param>
        private FluentValidatorToCatelValidatorAdapter(Type validatorType)
        {
            _validator = (global::FluentValidation.IValidator)Activator.CreateInstance(validatorType);
            if (!validatorType.TryGetAttribute(out _validatorDescriptionAttribute))
            {
                _validatorDescriptionAttribute = new ValidatorDescriptionAttribute(validatorType.Name);
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The validate business rules.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <param name="validationResults">The validation results.</param>
        protected override void ValidateBusinessRules(ModelBase instance, List<IBusinessRuleValidationResult> validationResults)
        {
            if (_validatorDescriptionAttribute.ValidationType == ValidationType.BusinessRule)
            {
                var validationResult = _validator.Validate(instance);

                if (!validationResult.IsValid)
                {
                    validationResults.AddRange(validationResult.Errors.Select(validationFailure => new BusinessRuleValidationResult(
                    _validatorDescriptionAttribute.ValidationResultType, validationFailure.ErrorMessage)
                    {
                        Tag = _validatorDescriptionAttribute.Tag 
                    }).Cast<IBusinessRuleValidationResult>());
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
            if (_validatorDescriptionAttribute.ValidationType == ValidationType.Field)
            {
                var validationResult = _validator.Validate(instance);
                if (!validationResult.IsValid)
                {
                    validationResults.AddRange(validationResult.Errors.Select(fieldValidationResult =>new FieldValidationResult(
                                fieldValidationResult.PropertyName, _validatorDescriptionAttribute.ValidationResultType,
                                fieldValidationResult.ErrorMessage)
                                {
                                    Tag = _validatorDescriptionAttribute.Tag 
                                }).Cast<IFieldValidationResult>());
                }
            }
        }

        #endregion

        #region Methods

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
            Argument.IsNotNull("validatorType", validatorType);
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
            Argument.IsNotNull("validatorTypes", validatorTypes);
            
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
                validator = From(validatorTypes.FirstOrDefault());
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

        #endregion
    }
}