namespace Game.Backend.Modules.Purchases;

public sealed class PurchaseProductCatalog
{
    private static readonly IReadOnlyDictionary<string, PurchaseProduct> Products =
        new Dictionary<string, PurchaseProduct>(StringComparer.Ordinal)
        {
            ["cosmetic_avatar_skin_neon"] = new(
                "cosmetic_avatar_skin_neon",
                "cosmetic",
                IsPowerAffecting: false),
            ["ui_theme_static"] = new(
                "ui_theme_static",
                "cosmetic",
                IsPowerAffecting: false),
            ["profile_frame_founder"] = new(
                "profile_frame_founder",
                "profile_customization",
                IsPowerAffecting: false),
            ["remove_ads"] = new(
                "remove_ads",
                "convenience",
                IsPowerAffecting: false),
            ["combat_stat_boost"] = new(
                "combat_stat_boost",
                "power",
                IsPowerAffecting: true),
            ["leaderboard_score_boost"] = new(
                "leaderboard_score_boost",
                "power",
                IsPowerAffecting: true)
        };

    public PurchaseProduct? GetProduct(string productId)
    {
        Products.TryGetValue(productId, out var product);
        return product;
    }
}
