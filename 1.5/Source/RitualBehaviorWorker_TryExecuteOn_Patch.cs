using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using RimWorld;

namespace RelicHunting
{
	[HarmonyPatch(typeof(RitualBehaviorWorker), "TryExecuteOn")]
	public static class RitualBehaviorWorker_TryExecuteOn_Patch
	{
		public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
		{
			var instructionsList = new List<CodeInstruction>(instructions);
			for (int i = 0; i < instructionsList.Count; i++)
			{
				yield return instructionsList[i];

				// Intercept the specific stfld instruction for lordJob
				if (instructionsList[i].opcode == OpCodes.Stfld &&
					instructionsList[i].operand is FieldInfo fieldInfo &&
					fieldInfo.FieldType == typeof(LordJob_Ritual) &&
					fieldInfo.Name == "lordJob")
				{
					// Insert the method call after setting the lordJob variable
					yield return new CodeInstruction(OpCodes.Ldloc_0); // Load the local variable that holds the DisplayClass instance
					yield return new CodeInstruction(OpCodes.Ldfld, fieldInfo); // Load the 'lordJob' field from the DisplayClass instance
					yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(RitualBehaviorWorker_TryExecuteOn_Patch), nameof(RegisterIdeo)));
				}
			}
		}

		public static void RegisterIdeo(LordJob_Ritual ritual)
		{
			if (Dialog_BeginLordJob_DrawQualityDescription_Patch.ideos.TryGetValue(ritual.ritual, out var ideoPreceptTuple))
			{
				LordJob_Ritual_ExposeData_Patch.targetIdeology[ritual] = ideoPreceptTuple.Item1;
				LordJob_Ritual_ExposeData_Patch.targetRelicPrecept[ritual] = ideoPreceptTuple.Item2;
			}
		}
	}
}