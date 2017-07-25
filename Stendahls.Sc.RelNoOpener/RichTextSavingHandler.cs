using System;
using System.Linq;
using HtmlAgilityPack;
using Sitecore.Data.Fields;
using Sitecore.Data.Items;
using Sitecore.Events;

namespace Sitecore.RelNoOpener
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
                    field.Value = UpdateRelNoOpener(field.Value);
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
                var target = aNode.GetAttributeValue("target", null);
                if (string.IsNullOrWhiteSpace(target) || target == "_self")
                    continue;

                var href = aNode.GetAttributeValue("href", null);
                if (string.IsNullOrWhiteSpace(href))
                    continue;

                if (!href.StartsWith("http://", StringComparison.InvariantCultureIgnoreCase) && !href.StartsWith("https://", StringComparison.CurrentCultureIgnoreCase))
                    continue;

                var rel = aNode.GetAttributeValue("rel", null);
                if (rel == null)
                {
                    aNode.SetAttributeValue("rel", "noopener");
                    isModified = true;
                }
                else if (rel.IndexOf("noopener", StringComparison.InvariantCultureIgnoreCase) < 0)
                {
                    aNode.SetAttributeValue("rel", $"{rel} noopener".Trim());
                    isModified = true;
                }
            }

            return isModified ? doc.DocumentNode.OuterHtml : html;
        }
    }
}