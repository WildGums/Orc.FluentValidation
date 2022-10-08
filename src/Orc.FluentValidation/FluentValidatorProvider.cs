namespace Orc.FluentValidation
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Catel.ComponentModel;
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
        /// <summary>
        /// Initializes a new instance of the <see cref="FluentValidatorProvider"/> class.
        /// </summary>
        public FluentValidatorProvider()
        {
            ValidatorOptions.Global.DefaultClassLevelCascadeMode = CascadeMode.Stop;
            ValidatorOptions.Global.DefaultRuleLevelCascadeMode = CascadeMode.Stop;
            ValidatorOptions.Global.DisplayNameResolver = (type, member, expression) =>
            {
                var displayName = member.Name;

                if (member.TryGetAttribute<DisplayNameAttribute>(out var displayNameAttribute))
                {
                    displayName = displayNameAttribute.DisplayName;
                }

                return displayName;
            };
        }

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
        protected override Catel.Data.IValidator? GetValidator(Type targetType)
        {
            ArgumentNullException.ThrowIfNull(targetType);

            IValidator? validator = null;

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
                        if (currentType is not null)
                        {
                            var genericArguments = currentType.GetGenericArgumentsEx();

                            found = currentType.IsGenericTypeEx() && genericArguments.FirstOrDefault(type => type.IsAssignableFromEx(targetType)) is not null;
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
    }
}
