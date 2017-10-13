using Sitecore;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Globalization;
using Sitecore.Text;
using Sitecore.Web.UI.HtmlControls;
using Sitecore.Web.UI.HtmlControls.Data;
using Sitecore.Web.UI.Sheer;
using System;
using System.Collections.Specialized;
using System.IO;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;

namespace XC.Foundation.SitecoreExtensions.TemplateFields
{
    [UsedImplicitly]
    public class MappingWFFMFieldName : Input
    {

        public MappingWFFMFieldName()
        {
            IsFormDataSourceAlreadyCreated = false;
        }

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

        /// <summary>
        /// Gets or sets a value indicating whether this instance is from source exist.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is from source exist; otherwise, <c>false</c>.
        /// </value>
        public bool IsFormDataSourceAlreadyCreated
        {
            get
            {
                return GetViewStateBool("IsFormDataSourceAlreadyCreated");
            }
            set
            {
                Assert.ArgumentNotNull((object)value, "value");
                this.SetViewStateBool("IsFormDataSourceAlreadyCreated", value);
            }
        }

       
        /// <summary>Name html control style</summary>
        protected virtual string NameStyle
        {
            get
            {
                return "width:150px";
            }
        }

        /// <summary>Is control vertical</summary>
        protected virtual bool IsVertical
        {
            get
            {
                return false;
            }
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
               
        /// <summary>Parameters the change.</summary>
        [UsedImplicitly]
        protected void ParameterChange()
        {
            ClientPage clientPage = Sitecore.Context.ClientPage;
            if (clientPage.ClientRequest.Source == StringUtil.GetString(clientPage.ServerProperties[this.ID + "_LastParameterID"]) && !string.IsNullOrEmpty(clientPage.ClientRequest.Form[clientPage.ClientRequest.Source]))
            {
                string str = this.BuildParameterMappingFieldNameFromSource(string.Empty,string.Empty, string.Empty);
                clientPage.ClientResponse.Insert(this.ID, "beforeEnd", str);
            }
            NameValueCollection form = (NameValueCollection)null;
            System.Web.UI.Page handler = HttpContext.Current.Handler as System.Web.UI.Page;
            if (handler != null)
                form = handler.Request.Form;
            if (form == null || !this.Validate(form))
                return;
            clientPage.ClientResponse.SetReturnValue(true);
        }

        /// <summary>Validates the specified client page.</summary>
        /// <param name="form">The form.</param>
        /// <returns>The result of the validation.</returns>
        private bool Validate(NameValueCollection form)
        {
            Assert.ArgumentNotNull((object)form, "form");
            foreach (string key in form.Keys)
            {
                if (key != null && key.StartsWith(this.ID + "_Param", StringComparison.InvariantCulture) && !key.EndsWith("_value", StringComparison.InvariantCulture))
                {
                    string input = form[key];
                    //if (!string.IsNullOrEmpty(input) && !Regex.IsMatch(input, "^\\w*$"))
                    //{
                    //    SheerResponse.Alert(string.Format("The key \"{0}\" is invalid.\n\nA key may only contain letters and numbers.", (object)input));
                    //    SheerResponse.SetReturnValue(false);
                    //    return false;
                    //}
                }
            }
            return true;
        }

        /// <summary>Sets the modified flag.</summary>
        protected override void SetModified()
        {
            base.SetModified();
            if (!this.TrackModified)
                return;
            Sitecore.Context.ClientPage.Modified = true;
        }


        /// <summary>
        /// Raises the <see cref="E:Load" /> event.
        /// </summary>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected override void OnLoad(EventArgs e)
        {
            Assert.ArgumentNotNull((object)e, "e");
            base.OnLoad(e);
            if (Sitecore.Context.ClientPage.IsEvent)
            {
                this.LoadValue();
            }
            else
            {
                this.BuildControl();
            }
        }

        /// <summary>
        /// Builds the control.
        /// </summary>
        private void BuildControl()
        {
            this.Controls.Clear();

            UrlString urlString = new UrlString(this.Value);
            
            foreach (string key in urlString.Parameters.Keys)
            {

                if (key == "formSourceID")
                    continue;

                if (key.Length > 0)
                {
                    var fieldMapping = urlString.Parameters[key];
                    var fieldSource = urlString.Parameters["formSourceID"] != null ? urlString.Parameters["formSourceID"] : string.Empty;
                    this.Controls.Add(new LiteralControl(this.BuildParameterMappingFieldNameFromSource(key, fieldMapping, fieldSource)));
                }
                else
                {
                   
                    this.Controls.Add(new LiteralControl(this.BuildMappingFormFieldSource("", "", urlString.Parameters["formSourceID"])));
                }
            }
            this.Controls.Add(new LiteralControl(this.BuildParameterMappingFieldNameFromSource(string.Empty, string.Empty, string.Empty)));
        }

        /// <summary>
        /// Loads the value.
        /// </summary>
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
                    string fieldFormName = nameValueCollection[key + "_value"];
                    string fieldFormSource = nameValueCollection[key + "_source"];

                    if (!string.IsNullOrEmpty(input))
                    {
                        string index = Regex.Replace(input, "\\W", "_");
                        urlString[index] = fieldFormName ?? string.Empty;
                    }
                    if(!string.IsNullOrWhiteSpace(fieldFormSource))
                    {
                        urlString["formSourceID"] = fieldFormSource;
                    }
                }
                else if(!string.IsNullOrEmpty(key) && key.EndsWith("_source", StringComparison.InvariantCulture))
                {
                    string fieldFormSource = nameValueCollection[key];
                    if (!string.IsNullOrWhiteSpace(fieldFormSource))
                    {
                        this.Controls.Add(new LiteralControl(this.BuildParameterMappingFieldNameFromSource(key, string.Empty, fieldFormSource)));
                        urlString["formSourceID"] = fieldFormSource;                      
                    }
                }
            }

            string str1 = urlString.ToString();
            if (this.Value == str1)
                return;
            this.Value = str1;
            this.SetModified();
        }


        /// <summary>
        /// Builds the parameter mapping field name from source.
        /// </summary>
        /// <param name="mappingField">The mapping field.</param>
        /// <param name="fieldName">Name of the field.</param>
        /// <param name="source">The source.</param>
        /// <returns></returns>
        private string BuildParameterMappingFieldNameFromSource(string mappingField, string fieldName, string source)
        {
            Assert.ArgumentNotNull((object)mappingField, "mappingField");
            Assert.ArgumentNotNull((object)fieldName, "fieldName");
            string uniqueId = GetUniqueID(this.ID + "_Param");
            Sitecore.Context.ClientPage.ServerProperties[this.ID + "_LastParameterID"] = (object)uniqueId;
            string clientEvent = Sitecore.Context.ClientPage.GetClientEvent(this.ID + ".ParameterChange");
            string str1 = this.ReadOnly ? " readonly=\"readonly\"" : string.Empty;
            string str2 = this.Disabled ? " disabled=\"disabled\"" : string.Empty;
            string str3 = this.IsVertical ? "</tr><tr>" : string.Empty;

            
            string controlSourceHTML = string.Empty;
            if(!IsFormDataSourceAlreadyCreated)
            {
                controlSourceHTML = BuildMappingFormFieldSource(mappingField, fieldName, source);
                IsFormDataSourceAlreadyCreated = true;
            }
            string controlRowsHTML = string.Format("<table width=\"100%\" class='scAdditionalParameters'><tr><td>{0}</td>{2}<td width=\"100%\">{1}</td></tr></table>",
                                                    (object)string.Format("<input id=\"{0}\" name=\"{1}\" type=\"text\"{2}{3} style=\"{6}\" value=\"{4}\" onchange=\"{5}\"/>", 
                                                                           (object)uniqueId, 
                                                                           (object)uniqueId, 
                                                                           (object)str1, 
                                                                           (object)str2, 
                                                                           (object)StringUtil.EscapeQuote(mappingField), 
                                                                           (object)clientEvent, 
                                                                           (object)this.NameStyle
                                                     ),
                                                    (object)this.GetValueHtmlControlFromSource(uniqueId, StringUtil.EscapeQuote(HttpUtility.UrlDecode(fieldName)), StringUtil.EscapeQuote(HttpUtility.UrlDecode(source))), //{1}
                                                    (object)str3); //{2}
            
            return string.Concat( controlSourceHTML, controlRowsHTML);
        }

        /// <summary>
        /// Builds the mapping form field source.
        /// </summary>
        /// <param name="mappingField">The mapping field.</param>
        /// <param name="fieldName">Name of the field.</param>
        /// <param name="source">The source.</param>
        /// <returns></returns>
        private string BuildMappingFormFieldSource(string mappingField, string fieldName, string source)
        {
            Assert.ArgumentNotNull((object)mappingField, "mappingField");
            Assert.ArgumentNotNull((object)fieldName, "fieldName");
            string uniqueId = GetUniqueID(this.ID + "_Param");
            Sitecore.Context.ClientPage.ServerProperties[this.ID + "_LastParameterID"] = (object)uniqueId;
            string clientEvent = Sitecore.Context.ClientPage.GetClientEvent(this.ID + ".ParameterChange");
            string controlRowsHTML = string.Format("<table width=\"100%\" class='scAdditionalParameters'><tr><td width=\"100%\" onchange=\"{1}\">{0}</td></tr></table>",
                                                    (object)this.GetValueHtmlControl(uniqueId, StringUtil.EscapeQuote(HttpUtility.UrlDecode(source))),//{0}
                                                    (object)clientEvent); //{1}

            return controlRowsHTML;
        }

        /// <summary>
        /// Gets the value HTML control.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        protected virtual string GetValueHtmlControl(string id, string value)
        {
            HtmlTextWriter htmlTextWriter = new HtmlTextWriter((TextWriter)new StringWriter());
            Item[] items = this.GetItems(Sitecore.Context.ContentDatabase.GetItem(this.ItemID));
            htmlTextWriter.Write("<select id=\"" + id + "_source\" name=\"" + id + "_source\"" + this.GetControlAttributes() + ">");
            htmlTextWriter.Write("<option" + (string.IsNullOrEmpty(value) ? " selected=\"selected\"" : string.Empty) + " value=\"Default\">Select Forms</option>");
            foreach (Item obj in items)
            {
                string itemHeader = this.GetItemHeader(obj);
                bool flag = obj.ID.ToString() == value;
                htmlTextWriter.Write("<option value=\"" + this.GetItemValue(obj) + "\"" + (flag ? " selected=\"selected\"" : string.Empty) + ">" + itemHeader + "</option>");
            }
            htmlTextWriter.Write("</select>");
            return htmlTextWriter.InnerWriter.ToString();
        }

        /// <summary>
        /// Gets the value HTML control from source.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="fieldName">Name of the field.</param>
        /// <param name="sourceFormID">The source form identifier.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        protected virtual string GetValueHtmlControlFromSource(string id, string fieldName, string sourceFormID)
        {
            HtmlTextWriter htmlTextWriter = new HtmlTextWriter((TextWriter)new StringWriter());

            Guid isGuid = new Guid();
            if(string.IsNullOrWhiteSpace(sourceFormID) || !Guid.TryParse(sourceFormID, out isGuid))
            {
                // Form Field Empty: No source has been selected.
                htmlTextWriter.Write("<select id=\"" + id + "_value\" name=\"" + id + "_value\"" + id + ">");
                htmlTextWriter.Write("<option value=\"Default\">Select Form Field</option>");
                htmlTextWriter.Write("<option" + (string.IsNullOrEmpty(fieldName) ? " selected=\"selected\"" : string.Empty) + " value=\"\"></option>");
                htmlTextWriter.Write("</select>");
                return htmlTextWriter.InnerWriter.ToString();
            }

            Item form = Sitecore.Context.ContentDatabase.GetItem(new Sitecore.Data.ID(sourceFormID));
            if (form == null)
                throw new ArgumentNullException(string.Format("Not Found form by ID: {0}", sourceFormID));

            if (!form.HasChildren)
            {
                htmlTextWriter.Write("<select id=\"" + id + "_value\" name=\"" + id + "_value\"" + id + ">");
                htmlTextWriter.Write("<option" + (string.IsNullOrEmpty(fieldName) ? " selected=\"selected\"" : string.Empty) + " value=\"\"></option>");
                htmlTextWriter.Write("</select>");
                return htmlTextWriter.InnerWriter.ToString();
            }

            htmlTextWriter.Write("<select id=\"" + id + "_value\" name=\"" + id + "_value\" value=\"" +id +"_value\" > ");
            htmlTextWriter.Write("<option value=\"Default\">Select Form Field</option>");

            foreach ( Item secionForm in form.Children)
            {

                if (secionForm.TemplateName.ToLowerInvariant() == "field")
                    continue;

                foreach (Item formfield in secionForm.Children)
                {

                    bool flag = formfield.ID.ToString() == fieldName;
                    htmlTextWriter.Write("<option value=\"" + formfield.ID + "\"" + (flag ? " selected=\"selected\"" : string.Empty) + " id=\"" + id + "_value\" name=\"" + formfield.Name + "_value\">" + formfield.Name + "</option>");
                }
            }
                      

            htmlTextWriter.Write("</select>");
            return htmlTextWriter.InnerWriter.ToString();           
        }
        
    }
}