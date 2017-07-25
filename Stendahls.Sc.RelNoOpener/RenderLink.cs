using System;
using Sitecore.Diagnostics;
using Sitecore.Pipelines.RenderField;

namespace Sitecore.RelNoOpener
{
    public class RenderLink
    {
        public void Process(RenderFieldArgs args)
        {
            Assert.ArgumentNotNull((object)args, "args");

            if (!args.FieldTypeKey.StartsWith("general link"))
                return;

            if (args.FieldValue.Contains("external") && args.FieldValue.Contains("_blank") &&
                (args.FieldValue.Contains("http://") || args.FieldValue.Contains("https://")))
            {
                if (!args.Parameters.ContainsKey("rel"))
                {
                    args.Parameters.Add("rel", "noopener");
                }
                else
                {
                    var rel = args.Parameters["rel"];
                    if (string.IsNullOrWhiteSpace(rel))
                        rel = "noopener";
                    else if (rel.IndexOf("noopener", StringComparison.InvariantCultureIgnoreCase) < 0)
                        rel = $"{rel} noopener";
                    args.Parameters["rel"] = rel;
                }
            }
        }
    }
}
