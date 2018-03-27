// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PublicApiFacts.cs" company="WildGums">
//   Copyright (c) 2008 - 2017 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.FluentValidation.Tests
{
    using ApiApprover;
    using NUnit.Framework;

    [TestFixture]
    public class PublicApiFacts
    {
        [Test]
        public void Orc_FluentValidation_HasNoBreakingChanges()
        {
            var assembly = typeof(FluentValidatorProvider).Assembly;

            PublicApiApprover.ApprovePublicApi(assembly);
        }
    }
}