using System.Linq;
using HarmonyLib;
using RimWorld;
using RimWorld.QuestGen;
using Verse;

namespace RelicHunting
{

	[HarmonyPatch(typeof(QuestNode_Root_RelicHunt), "TryGetRandomPlayerRelic")]
	public static class QuestNode_Root_RelicHunt_TryGetRandomPlayerRelic_Patch
	{
		public static bool Prefix(ref bool __result, out Precept_Relic relic)
		{
			var ideo = RitualAttachableOutcomeEffectWorker_DiscoverRelics.targetIdeo;
			if (ideo is null)
			{
				relic = null;
				return true;
			}
			
			relic = RitualAttachableOutcomeEffectWorker_DiscoverRelics.targetRelic;
			if (relic is not null)
			{
				__result = true;
				return false;
			}
			__result = (from p in ideo.GetAllPreceptsOfType<Precept_Relic>()
						where p.CanGenerateRelic
						select p).TryRandomElement(out relic);
			return false;
		}
	}
}