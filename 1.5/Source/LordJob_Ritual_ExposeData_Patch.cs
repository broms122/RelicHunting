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
		public static void Postfix(LordJob_Ritual __instance)
		{
			if (!targetIdeology.TryGetValue(__instance, out Ideo ideo))
			{
				ideo = null;
			}
			Scribe_References.Look(ref ideo, "targetIdeology");
			if (ideo != null)
			{
				targetIdeology[__instance] = ideo;
			}
		}
	}
}