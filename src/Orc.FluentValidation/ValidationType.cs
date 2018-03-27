// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ValidationType.cs" company="WildGums">
//   Copyright (c) 2008 - 2017 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.FluentValidation
{
    /// <summary>
    /// Type of validation type.
    /// </summary>
    public enum ValidationType
    {
        /// <summary>
        /// Indicates whether the validation will be reported as a field validation.
        /// </summary>
        Field,

        /// <summary>
        /// Indicates whether the validation will be reported as a business rule validation.
        /// </summary>
        BusinessRule
    }
}