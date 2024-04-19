using LBoL.Core;
using LBoLEntitySideloader.Resource;

namespace PvB {
    public abstract class UsefulFunctions {
        #pragma warning disable
            public static GlobalLocalization LocalizationPlayer(DirectorySource dirsorc)
            {
                var loc = new GlobalLocalization(dirsorc);
                loc.LocalizationFiles.AddLocaleFile(Locale.En, "PlayerUnitEn.yaml");
                return loc;
            }
        #pragma warning restore
    }
}