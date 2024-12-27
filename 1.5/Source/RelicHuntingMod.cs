using HarmonyLib;
using Verse;

namespace RelicHunting
{
	public class RelicHuntingMod : Mod
	{
		public RelicHuntingMod(ModContentPack pack) : base(pack)
		{
			new Harmony("RelicHuntingMod").PatchAll();
		}
	}
}