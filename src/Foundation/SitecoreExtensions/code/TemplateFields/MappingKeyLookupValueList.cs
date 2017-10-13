using Sitecore;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Globalization;
using Sitecore.Shell.Applications.ContentEditor;
using Sitecore.Text;
using Sitecore.Web.UI.HtmlControls;
using Sitecore.Web.UI.HtmlControls.Data;
using System;
using System.Collections.Specialized;
using System.IO;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;

namespace XC.Foundation.SitecoreExtensions.TemplateFields
{
    [UsedImplicitly]
    public class MappingKeyLookupValueList : NameValue
    {
        /// <summary>Gets or sets the name of the field.</summary>
        /// <value>The name of the field.</value>
        /// <contract>
        ///   <requires name="value" condition="not null" />
        ///   <ensures condition="nullable" />
        /// </contract>
        public string FieldName
        {
            get
            {
                return this.GetViewStateString("FieldName");
            }
            set
            {
                Assert.ArgumentNotNull((object)value, "value");
                this.SetViewStateString("FieldName", value);
            }
        }

        /// <summary>Gets or sets the item ID.</summary>
        /// <value>The item ID.</value>
        /// <contract>
        ///   <requires name="value" condition="not null" />
        ///   <ensures condition="nullable" />
        /// </contract>
        public string ItemID
        {
            get
            {
                return this.GetViewStateString("ItemID");
            }
            set
            {
                Assert.ArgumentNotNull((object)value, "value");
                this.SetViewStateString("ItemID", value);
            }
        }

        /// <summary>Gets or sets the source.</summary>
        /// <value>The source.</value>
        /// <contract>
        ///   <requires name="value" condition="not null" />
        ///   <ensures condition="nullable" />
        /// </contract>
        public string Source
        {
            get
            {
                return this.GetViewStateString("Source");
            }
            set
            {
                Assert.ArgumentNotNull((object)value, "value");
                this.SetViewStateString("Source", value);
            }
        }

        /// <summary>Gets or sets the item language.</summary>
        /// <value>The item language.</value>
        public string ItemLanguage
        {
            get
            {
                return StringUtil.GetString(this.ViewState["ItemLanguage"]);
            }
            set
            {
                Assert.ArgumentNotNull((object)value, "value");
                this.ViewState["ItemLanguage"] = (object)value;
            }
        }

        /// <summary>Name html control style</summary>
        protected override string NameStyle
        {
            get
            {
                return "width:150px;background-color:lightgrey'";
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Sitecore.Shell.Applications.ContentEditor.NameLookupValue" /> class.
        /// </summary>
        public void NameLookupValue()
        {
            this.Class += " scCombobox";
        }

        /// <summary>Gets the items.</summary>
        /// <param name="current">The current.</param>
        /// <returns>The items.</returns>
        protected virtual Item[] GetItems(Item current)
        {
            Assert.ArgumentNotNull((object)current, "current");
            using (new LanguageSwitcher(this.ItemLanguage))
                return LookupSources.GetItems(current, this.Source);
        }

        /// <summary>Gets the item header.</summary>
        /// <param name="item">The item.</param>
        /// <returns>The item header.</returns>
        /// <contract>
        ///   <requires name="item" condition="not null" />
        ///   <ensures condition="not null" />
        /// </contract>
        protected virtual string GetItemHeader(Item item)
        {
            Assert.ArgumentNotNull((object)item, "item");
            string str = StringUtil.GetString(new string[1]
            {
        this.FieldName
            });
            return !str.StartsWith("@", StringComparison.InvariantCulture) ? (str.Length <= 0 ? item.DisplayName : item[this.FieldName]) : item[str.Substring(1)];
        }

        /// <summary>Gets the item value.</summary>
        /// <param name="item">The item.</param>
        /// <returns></returns>
        /// <contract>
        ///   <requires name="item" condition="not null" />
        ///   <ensures condition="not null" />
        /// </contract>
        protected virtual string GetItemValue(Item item)
        {
            Assert.ArgumentNotNull((object)item, "item");
            return item.ID.ToString();
        }

        /// <summary>Determines whether the specified item is selected.</summary>
        /// <param name="item">The item.</param>
        /// <returns>
        /// 	<c>true</c> if the specified item is selected; otherwise, <c>false</c>.
        /// </returns>
        protected virtual bool IsSelected(Item item)
        {
            Assert.ArgumentNotNull((object)item, "item");
            if (!(this.Value == item.ID.ToString()))
                return this.Value == item.Paths.LongID;
            return true;
        }

        /// <summary>Gets value html control.</summary>
        /// <param name="id">The id.</param>
        /// <param name="value">The value.</param>
        /// <returns>The formatted value html control.</returns>
        protected override string GetValueHtmlControl(string id, string value)
        {
            HtmlTextWriter htmlTextWriter = new HtmlTextWriter((TextWriter)new StringWriter());
            Item[] items = this.GetItems(Sitecore.Context.ContentDatabase.GetItem(this.ItemID));
            htmlTextWriter.Write("<select id=\"" + id + "_source\" name=\"" + id + "_source\"" + this.GetControlAttributes() + ">");
            htmlTextWriter.Write("<option" + (string.IsNullOrEmpty(value) ? " selected=\"selected\"" : string.Empty) + " value=\"\"></option>");
            foreach (Item obj in items)
            {
                string itemHeader = this.GetItemHeader(obj);
                bool flag = obj.ID.ToString() == value;
                htmlTextWriter.Write("<option value=\"" + this.GetItemValue(obj) + "\"" + (flag ? " selected=\"selected\"" : string.Empty) + ">" + itemHeader + "</option>");
            }
            htmlTextWriter.Write("</select>");
            return htmlTextWriter.InnerWriter.ToString();
        }

        #region Mapping Key Lookup Value List

        /// <summary>
        /// Raises the <see cref="E:Load" /> event.
        /// </summary>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected override void OnLoad(EventArgs e)
        {
            Assert.ArgumentNotNull((object)e, "e");
            if (Sitecore.Context.ClientPage.IsEvent)
                this.LoadValue();
            else
                this.BuildControl();
        }

        /// <summary>Loads the post data.</summary>
        private void LoadValue()
        {
            if (this.ReadOnly || this.Disabled)
                return;
            System.Web.UI.Page handler = HttpContext.Current.Handler as System.Web.UI.Page;
            NameValueCollection nameValueCollection = handler == null ? new NameValueCollection() : handler.Request.Form;
            UrlString urlString = new UrlString();
            foreach (string key in nameValueCollection.Keys)
            {
                if (!string.IsNullOrEmpty(key) && key.StartsWith(this.ID + "_Param", StringComparison.InvariantCulture) && !key.EndsWith("_value", StringComparison.InvariantCulture) && !key.EndsWith("_source", StringComparison.InvariantCulture))
                {
                    string input = nameValueCollection[key];
                    string str = nameValueCollection[key + "_value"];
                    string sourceValue = nameValueCollection[key + "_source"];

                    if (!string.IsNullOrEmpty(input))
                    {
                        string index = Regex.Replace(input, "\\W", "_");
                        urlString[index] = str + "&" + sourceValue ?? string.Empty;
                    }
                }
            }
            string str1 = urlString.ToString();
            if (this.Value == str1)
                return;
            this.Value = str1;
            this.SetModified();
        }

        /// <summary>Builds the control.</summary>
        private void BuildControl()
        {

            this.Value = this.Value.IndexOf("&") > 0 ? this.Value.Substring(0, this.Value.IndexOf("&")) : this.Value;

            UrlString urlString = new UrlString(this.Value);
            foreach (string key in urlString.Parameters.Keys)
            {
                if (key.Length > 0)
                    this.Controls.Add((System.Web.UI.Control)new LiteralControl(this.BuildParameterKeyValue(key, urlString.Parameters[key], urlString.Parameters[key])));
            }
            this.Controls.Add((System.Web.UI.Control)new LiteralControl(this.BuildParameterKeyValue(string.Empty, string.Empty, string.Empty)));
        }

        /// <summary>Builds the parameter key value.</summary>
        /// <param name="key">The parameter key.</param>
        /// <param name="value">The value.</param>
        /// <returns>The parameter key value.</returns>
        /// <contract><requires name="key" condition="not null" /><requires name="value" condition="not null" /><ensures condition="not null" /></contract>
        private string BuildParameterKeyValue(string key, string fieldName, string value)
        {
            Assert.ArgumentNotNull((object)key, "key");
            Assert.ArgumentNotNull((object)value, "value");
            Assert.ArgumentNotNull((object)fieldName, "fieldName");

            string uniqueId = Sitecore.Web.UI.HtmlControls.Control.GetUniqueID(this.ID + "_Param");
            Sitecore.Context.ClientPage.ServerProperties[this.ID + "_LastParameterID"] = (object)uniqueId;
            string clientEvent = Sitecore.Context.ClientPage.GetClientEvent(this.ID + ".ParameterChange");
            string str1 = this.ReadOnly ? " readonly=\"readonly\"" : string.Empty;
            string str2 = this.Disabled ? " disabled=\"disabled\"" : string.Empty;
            string str3 = this.IsVertical ? "</tr><tr>" : string.Empty;

            string mappingUI = string.Format(
                        "<table width=\"100%\" class='scAdditionalParameters'><tr><td>{0}</td><td width=\"20%\">{1}</td>{3}<td width=\"80%\">{2}</td></tr></table>", 
                        (object)string.Format("<input id=\"{0}\" name=\"{1}\" type=\"text\"{2}{3} style=\"{6}\" value=\"{4}\" onchange=\"{5}\"/>", 
                                                (object)uniqueId, 
                                                (object)uniqueId, 
                                                (object)str1, 
                                                (object)str2, 
                                                (object)StringUtil.EscapeQuote(key), 
                                                (object)clientEvent, 
                                                (object)this.NameStyle
                        ), 
                        (object)string.Format("<input id=\"{0}\" name=\"{1}\" type=\"text\"{2}{3} style=\"{6}\" value=\"{4}\" onchange=\"{5}\"/>",
                                                (object)string.Concat(uniqueId, "_value"),
                                                (object)string.Concat(uniqueId, "_value"),
                                                (object)str1,
                                                (object)str2,
                                                (object)StringUtil.EscapeQuote(fieldName),
                                                (object)clientEvent,
                                                (object)this.NameStyle

                        ),
                        (object)this.GetValueHtmlControl(uniqueId, StringUtil.EscapeQuote(HttpUtility.UrlDecode(value))), 
                        (object)str3);

            return mappingUI;
        }

        #endregion

    }
}