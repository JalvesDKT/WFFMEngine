using Sitecore.Forms.Mvc.Attributes;
using Sitecore.Forms.Mvc.Controllers.ModelBinders.FieldBinders;
using Sitecore.Forms.Mvc.Validators;
using Sitecore.Forms.Mvc.ViewModels.Fields;
namespace XC.Foundation.SitecoreExtensions.WFFM
{
    /// <summary>
    /// CheckBox HTML Field
    /// </summary>
    /// <seealso cref="Sitecore.Forms.Mvc.ViewModels.Fields.CheckboxField" />
    public class CheckBoxHTMLField : CheckboxField
    {
        /// <summary>
        /// Gets or sets the HTML text.
        /// </summary>
        /// <value>
        /// The HTML text.
        /// </value>
        public string TermsLabelHTML { get; set; }

        /// <summary>
        /// Gets or sets the text.
        /// </summary>
        /// <value>
        /// The text.
        /// </value>
        public string TermsLabel
        {
            get { return !string.IsNullOrEmpty(TermsLabelHTML) ? TermsLabelHTML : base.Title; }
            set { TermsLabelHTML = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="CheckBoxHTMLField"/> is value.
        /// </summary>
        /// <value>
        ///   <c>true</c> if value; otherwise, <c>false</c>.
        /// </value>
        [DynamicRequired(ErrorMessage = "The {0} field is required.")]
        [PropertyBinder(typeof(DefaultFieldValueBinder))]
        [ParameterName("Title")]
        public override bool Value
        {
            get
            {
                return base.Value;
            }

            set
            {
                base.Value = value;
            }
        }       
    }
}