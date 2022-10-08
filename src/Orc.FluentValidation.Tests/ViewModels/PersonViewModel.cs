namespace Orc.FluentValidation.Tests.ViewModels
{
    using Catel.Data;
    using Catel.MVVM;

    /// <summary>
    /// The person view model.
    /// </summary>
    public class PersonViewModel : ViewModelBase
    {
        /// <summary>
        /// The first name property.
        /// </summary>
        public static readonly IPropertyData FirstNameProperty = RegisterProperty<string>("PersonFirstName");

        /// <summary>
        /// The last name property.
        /// </summary>
        public static readonly IPropertyData LastNameProperty = RegisterProperty<string>("PersonLastName");

        /// <summary>
        /// Gets or sets PersonFirstName.
        /// </summary>
        [Catel.ComponentModel.DisplayName("First name")]
        public string PersonFirstName
        {
            get { return GetValue<string>(FirstNameProperty); }
            set { SetValue(FirstNameProperty, value); }
        }

        /// <summary>
        /// Gets or sets PersonLastName.
        /// </summary>
        [Catel.ComponentModel.DisplayName("Last name")]
        public string PersonLastName
        {
            get { return GetValue<string>(LastNameProperty); }
            set { SetValue(LastNameProperty, value); }
        }
    }
}
