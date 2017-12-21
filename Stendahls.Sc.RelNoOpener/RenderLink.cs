using System;
using Sitecore.Diagnostics;
using Sitecore.Pipelines.RenderField;

namespace Stendahls.Sc.RelNoOpener
{
    public class RenderLink
    {
        public void Process(RenderFieldArgs args)
        {
            try
            {
                Assert.ArgumentNotNull(args, "args");

                // This applies to both "general link" and "general link with search"
                if (!args.FieldTypeKey.StartsWith("general link"))
                    return;

                // The tests here could be more granular (as they are in the Rich Text save action),
                // but since this is performed runtime, it's better to run it fast. The 
                // rel ="noopener" attribute doesn't do any harm more than just adding a few
                // more bytes to the payload. (However, it can do harm on internal links if
                // there are javascripts on the site that needs to talk between the windows)
                if (args.FieldValue.Contains("external") && args.FieldValue.Contains("target") &&
                    (args.FieldValue.Contains("http://") || args.FieldValue.Contains("https://")))
                {
                    if (!args.Parameters.ContainsKey("rel"))
                        args.Parameters.Add("rel", SettingsHelper.RelString);
                    else
                        args.Parameters["rel"] = SettingsHelper.AddRelString(args.Parameters["rel"]);
                }
            }
            catch (Exception ex)
            {
                Log.Error("Unable to add rel=noopener tag on external link", ex, $"{nameof(RenderLink)}.{nameof(Process)}");
            }
        }
    }
}
