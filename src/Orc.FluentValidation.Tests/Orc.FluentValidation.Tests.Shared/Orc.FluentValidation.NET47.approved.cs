[assembly: System.Resources.NeutralResourcesLanguageAttribute("en-US")]
[assembly: System.Runtime.InteropServices.ComVisibleAttribute(false)]
[assembly: System.Runtime.Versioning.TargetFrameworkAttribute(".NETFramework,Version=v4.7", FrameworkDisplayName=".NET Framework 4.7")]


public class static MethodTimeLogger
{
    public static void Log(System.Reflection.MethodBase methodBase, long milliseconds) { }
    public static void Log(System.Type type, string methodName, long milliseconds) { }
}
public class static ModuleInitializer
{
    public static void Initialize() { }
}
namespace Orc.FluentValidation
{
    
    public class FluentValidatorProvider : Catel.Data.ValidatorProviderBase
    {
        public FluentValidatorProvider() { }
        protected override Catel.Data.IValidator GetValidator(System.Type targetType) { }
    }
    public class FluentValidatorToCatelValidatorAdapter : Catel.Data.ValidatorBase<Catel.Data.ModelBase>
    {
        public static Catel.Data.IValidator From(System.Type validatorType) { }
        public static Catel.Data.IValidator From(System.Collections.Generic.IList<System.Type> validatorTypes) { }
        public static Catel.Data.IValidator From<TValidator>()
            where TValidator : FluentValidation.IValidator, new () { }
        protected override void ValidateBusinessRules(Catel.Data.ModelBase instance, System.Collections.Generic.List<Catel.Data.IBusinessRuleValidationResult> validationResults) { }
        protected override void ValidateFields(Catel.Data.ModelBase instance, System.Collections.Generic.List<Catel.Data.IFieldValidationResult> validationResults) { }
    }
    public enum ValidationType
    {
        Field = 0,
        BusinessRule = 1,
    }
    [System.AttributeUsageAttribute(System.AttributeTargets.Class | System.AttributeTargets.All, AllowMultiple=false, Inherited=false)]
    public class ValidatorDescriptionAttribute : System.Attribute
    {
        public ValidatorDescriptionAttribute(string tag, Catel.Data.ValidationResultType validationResultType = 1, Orc.FluentValidation.ValidationType validationType = 0) { }
        public string Tag { get; }
        public Catel.Data.ValidationResultType ValidationResultType { get; }
        public Orc.FluentValidation.ValidationType ValidationType { get; }
    }
}