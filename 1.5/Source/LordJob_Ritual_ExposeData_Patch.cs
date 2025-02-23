using System.Collections.Generic;
using HarmonyLib;
using RimWorld;
using Verse;

namespace RelicHunting
{
	[HarmonyPatch(typeof(LordJob_Ritual), "ExposeData")]
	public static class LordJob_Ritual_ExposeData_Patch
	{
		public static Dictionary<LordJob_Ritual, Ideo> targetIdeology = new Dictionary<LordJob_Ritual, Ideo>();
		public static Dictionary<LordJob_Ritual, Precept_Relic> targetRelicPrecept = new Dictionary<LordJob_Ritual, Precept_Relic>();
		public static void Postfix(LordJob_Ritual __instance)
		{
			Ideo ideo = null;
			if (!targetIdeology.TryGetValue(__instance, out ideo))
			{
				ideo = null;
			}
			Scribe_References.Look(ref ideo, "targetIdeology");
			if (ideo != null)
			{
				targetIdeology[__instance] = ideo;
			}

			Precept_Relic relicPrecept = null;
			if (!targetRelicPrecept.TryGetValue(__instance, out relicPrecept))
			{
				relicPrecept = null;
			}
			Scribe_References.Look(ref relicPrecept, "targetRelicPrecept");
			if (relicPrecept != null)
			{
				targetRelicPrecept[__instance] = relicPrecept;
			}
		}
	}
}