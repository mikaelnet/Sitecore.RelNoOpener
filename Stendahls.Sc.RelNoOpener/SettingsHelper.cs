using System;

namespace Stendahls.Sc.RelNoOpener
{
    public static class SettingsHelper
    {
        public static bool RenderNoReferrer => Sitecore.Configuration.Settings.GetBoolSetting("Stendahls.Sc.RelNoOpener.AddNoReferrer", false);

        public static string RelString => RenderNoReferrer ? "noopener noreferrer" : "noopener";

        public static string AddRelString(string rel)
        {
            if (string.IsNullOrWhiteSpace(rel))
                return RelString;

            if (rel.IndexOf("noopener", StringComparison.InvariantCultureIgnoreCase) < 0)
                rel = $"{rel} noopener".Trim();
            if (RenderNoReferrer && rel.IndexOf("noreferrer", StringComparison.InvariantCultureIgnoreCase) < 0)
                rel = $"{rel} noreferrer".Trim();
            return rel;
        }
    }
}
