using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;
using RimWorld;
using RimWorld.QuestGen;

namespace RelicHunting
{
	[HarmonyPatch(typeof(QuestNode_Root_RelicHunt), "RunInt")]
	public static class QuestNode_Root_RelicHunt_RunInt_Patch
	{
		public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
		{
			foreach (var item in instructions)
			{
				yield return item;
				if (item.opcode == OpCodes.Stloc_3)
				{
					yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(QuestNode_Root_RelicHunt_RunInt_Patch), nameof(GetIdeo)));
					yield return new CodeInstruction(OpCodes.Stloc_3);
				}
			}
		}

		public static Ideo GetIdeo()
		{
			var ideo = RitualAttachableOutcomeEffectWorker_DiscoverRelics.targetIdeo;
			if (ideo is null)
			{
				return Faction.OfPlayer.ideos.PrimaryIdeo;
			}
			return ideo;
		}
	}
}