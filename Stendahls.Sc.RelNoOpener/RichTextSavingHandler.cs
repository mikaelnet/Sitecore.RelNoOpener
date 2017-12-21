using System;
using System.Linq;
using HtmlAgilityPack;
using Sitecore.Data.Fields;
using Sitecore.Data.Items;
using Sitecore.Events;

namespace Stendahls.Sc.RelNoOpener
{
    public class RichTextSavingHandler
    {
        public void OnItemSaving(object sender, EventArgs args)
        {
            var savingItem = Event.ExtractParameter(args, 0) as Item;
            // Updates should only be performed in the content tree of the master database
            if (savingItem == null || savingItem.Database.Name.ToLower() != "master" || !savingItem.Paths.IsContentItem)
            {
                return;
            }

            TemplateItem templateItem = savingItem.Template;
            savingItem.Fields.ReadAll(); // Neded to ensure all fields are loaded
            // Loop over all modified rich text fields
            foreach (Field field in savingItem.Fields.Where(f => f.IsModified && f.TypeKey == "rich text"))
            {
                var templateFieldItem = templateItem.GetField(field.ID);
                if (!string.IsNullOrWhiteSpace(templateFieldItem?.Source))
                {
                    try
                    {
                        field.Value = UpdateRelNoOpener(field.Value);
                    }
                    catch (Exception ex)
                    {
                        Sitecore.Diagnostics.Log.Error($"Unable to apply rel=\"{SettingsHelper.RelString}\" on rich text field {field.ID} for item {savingItem.Uri}", ex, nameof(OnItemSaving));
                    }
                }
            }
        }

        protected virtual string UpdateRelNoOpener(string html)
        {
            var doc = new HtmlDocument();
            doc.LoadHtml(html);

            var linkNodes = doc.DocumentNode.SelectNodes("//a");
            if (linkNodes == null)
                return html;

            bool isModified = false;
            foreach (var aNode in linkNodes)
            {
                // The rel attribute is only needed when opening new windows. 
                // "_self" and "_top" will replace the current page in the browser, 
                // so it's therefore not vunerable.
                var target = aNode.GetAttributeValue("target", null);
                if (string.IsNullOrWhiteSpace(target) || target == "_self" || target == "_top")
                    continue;

                var href = aNode.GetAttributeValue("href", null);
                if (string.IsNullOrWhiteSpace(href))
                    continue;

                // This is only needed when linking to external sites. Need to support http,
                // https and protocol independent links.
                if (!href.StartsWith("http://", StringComparison.InvariantCultureIgnoreCase) && 
                    !href.StartsWith("https://", StringComparison.InvariantCultureIgnoreCase) &&
                    !href.StartsWith("//", StringComparison.InvariantCulture))
                    continue;

                var rel = aNode.GetAttributeValue("rel", null);
                var newRel = SettingsHelper.AddRelString(rel);
                if (newRel != rel)
                {
                    aNode.SetAttributeValue("rel", newRel);
                    isModified = true;
                }
            }

            return isModified ? doc.DocumentNode.OuterHtml : html;
        }
    }
}