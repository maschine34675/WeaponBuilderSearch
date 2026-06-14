using EFT.InventoryLogic;

namespace WeaponBuilderSearch
{
    internal static class AttachmentSearchMatcher
    {
        public static bool Matches(Item item, string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                return true;

            if (item == null)
                return false;

            var normalized = query.Trim().ToLowerInvariant();

            if (ContainsIgnoreCase(item.ShortName.Localized(null), normalized))
                return true;

            if (ContainsIgnoreCase(item.Name.Localized(null), normalized))
                return true;

            return false;
        }

        private static bool ContainsIgnoreCase(string source, string value)
        {
            return !string.IsNullOrEmpty(source)
                && source.IndexOf(value, System.StringComparison.OrdinalIgnoreCase) >= 0;
        }
    }
}
