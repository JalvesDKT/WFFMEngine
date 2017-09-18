using Sitecore.Form.Core.Attributes;
using Sitecore.Form.Core.Visual;
using Sitecore.Form.Web.UI.Controls;
namespace XC.Foundation.SitecoreExtensions.WFFM
{
    /// <summary>
    /// Checkbox HTML
    /// </summary>
    /// <seealso cref="Sitecore.Form.Web.UI.Controls.Checkbox" />
    public class CheckboxHTML : Checkbox
    {
        /// <summary>
        /// The terms label
        /// </summary>
        private string _termsLabel;

        /// <summary>
        /// Gets or sets the terms label HTML.
        /// </summary>
        /// <value>
        /// The terms label HTML.
        /// </value>
        /// 
        [VisualCategory("Appearance")]
        [VisualProperty("HTML Text (HTML):", 400)]
        [VisualFieldType(typeof(TextAreaField))]
        [Localize]
        public string TermsLabelHTML
        {
            get
            {
                return !string.IsNullOrWhiteSpace(this._termsLabel) ? _termsLabel : base.Title;

            }

            set
            {
                _termsLabel = value;
            }
        }
    }
}