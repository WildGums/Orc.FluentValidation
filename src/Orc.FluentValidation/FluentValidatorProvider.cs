﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FluentValidatorProvider.cs" company="WildGums">
//   Copyright (c) 2008 - 2017 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.FluentValidation
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Catel.Data;
    using Catel.Reflection;
    using global::FluentValidation;
    using IValidator = Catel.Data.IValidator;

    /// <summary>
    /// The fluent validator provider.
    /// </summary>
    /// <remarks>
    /// This class will automatically retrieve the right fluent validation class associated with the view models. 
    /// </remarks>
    public class FluentValidatorProvider : ValidatorProviderBase
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="FluentValidatorProvider"/> class.
        /// </summary>
        public FluentValidatorProvider()
        {
            ValidatorOptions.Global.CascadeMode = CascadeMode.Stop;
            ValidatorOptions.Global.DisplayNameResolver = (type, member, expression) =>
            {
                var displayName = member.Name;

                if (member.TryGetAttribute(out Catel.ComponentModel.DisplayNameAttribute displayNameAttribute))
                {
                    displayName = displayNameAttribute.DisplayName;
                }

                return displayName;
            };
        }
        #endregion

        #region Methods
        /// <summary>
        /// Gets a validator for the specified target type.
        /// </summary>
        /// <remarks>
        /// This method only searches for fluent validators on the assembly which the <paramref name="targetType"/> belongs to, 
        /// and creates adapters that allow fluent validator talks with catel validation approach. 
        /// </remarks>
        /// <param name="targetType">
        ///   The target type.
        /// </param>
        /// <returns>
        /// The <see cref="Catel.Data.IValidator"/> for the specified type or <c>null</c> if no validator is available for the specified type. 
        /// If only one Validator is found an instance of <see cref="FluentValidatorToCatelValidatorAdapter"/> is returned, otherwise a
        /// <see cref="CompositeValidator"/> is created from a collection of it's corresponding <see cref="FluentValidatorToCatelValidatorAdapter"/>.
        /// </returns>
        protected override Catel.Data.IValidator GetValidator(Type targetType)
        {
            IValidator validator = null;

            // NOTE: Patch for performance issue the validator of a viewmodel must be in the same assembly of the view model.

            var assembly = targetType.GetAssemblyEx();
            var exportedTypes = assembly.GetExportedTypesEx();

            var validatorTypes = new List<Type>();
            foreach (var exportedType in exportedTypes)
            {
                if (typeof(global::FluentValidation.IValidator).IsAssignableFromEx(exportedType))
                {
                    var currentType = exportedType;
                    bool found = false;
                    while (!found && currentType != typeof(object))
                    {
                        if (currentType != null)
                        {
                            var genericArguments = currentType.GetGenericArgumentsEx();

                            found = currentType.IsGenericTypeEx() && genericArguments.FirstOrDefault(type => type.IsAssignableFromEx(targetType)) != null;
                            if (!found)
                            {
                                currentType = currentType.GetBaseTypeEx();
                            }
                        }
                    }

                    if (found)
                    {
                        validatorTypes.Add(exportedType);
                    }
                }
            }

            if (validatorTypes.Count > 0)
            {
                validator = FluentValidatorToCatelValidatorAdapter.From(validatorTypes);
            }

            return validator;
        }
        #endregion
    }
}
