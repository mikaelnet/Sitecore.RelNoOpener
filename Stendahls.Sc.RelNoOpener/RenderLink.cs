using System;
using Sitecore.Diagnostics;
using Sitecore.Pipelines.RenderField;

namespace Stendahls.Sc.RelNoOpener
{
    public class RenderLink
    {
        public void Process(RenderFieldArgs args)
        {
            Assert.ArgumentNotNull(args, "args");

            if (!args.FieldTypeKey.StartsWith("general link"))
                return;

            if (args.FieldValue.Contains("external") && args.FieldValue.Contains("target") &&
                (args.FieldValue.Contains("http://") || args.FieldValue.Contains("https://")))
            {
                if (!args.Parameters.ContainsKey("rel"))
                    args.Parameters.Add("rel", SettingsHelper.RelString);
                else
                    args.Parameters["rel"] = SettingsHelper.AddRelString(args.Parameters["rel"]);
            }
        }
    }
}
