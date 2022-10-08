namespace Orc.FluentValidation.Tests.ViewModels
{
    using System.ComponentModel;
    using Catel.Data;
    using Catel.MVVM;
    using Models;

    /// <summary>
    /// The person view model with model.
    /// </summary>
    public class PersonViewModelWithModel : ViewModelBase
    {
        /// <summary>Register the Person property so it is known in the class.</summary>
        public static readonly IPropertyData PersonProperty = RegisterProperty("Person", default(Person), (s, e) => ((PersonViewModelWithModel)s).OnPersonChanged(e));

        /// <summary>
        /// Gets or sets Person.
        /// </summary>
        [Model]
        public Person Person
        {
            get { return GetValue<Person>(PersonProperty); }
            set { SetValue(PersonProperty, value); }
        }

        /// <summary>
        /// Occurs when the value of the Person property is changed.
        /// </summary>
        /// <param name="e">
        /// The event argument
        /// </param>
        private void OnPersonChanged(PropertyChangedEventArgs e)
        {
        }
    }
}
