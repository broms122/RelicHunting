using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;

namespace RelicHunting
{
	[HotSwappable]
	[HarmonyPatch(typeof(Dialog_BeginLordJob), "DrawQualityDescription")]
	public static class Dialog_BeginLordJob_DrawQualityDescription_Patch
	{
		public static Dictionary<Precept_Ritual, (Ideo, Precept_Relic)> ideos = new Dictionary<Precept_Ritual, (Ideo, Precept_Relic)>();
		public static List<Ideo> IdeosWithRelics => Find.IdeoManager.IdeosListForReading.Where(x => x.GetAllPreceptsOfType<Precept_Relic>().Any(x => x.CanGenerateRelic)).ToList();

		public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
		{
			foreach (var item in instructions)
			{
				yield return item;
				if (item.Calls(AccessTools.Method(typeof(Widgets), "BeginScrollView")))
				{
					yield return new CodeInstruction(OpCodes.Ldarg_0);
					yield return new CodeInstruction(OpCodes.Ldloc_0);
					yield return new CodeInstruction(OpCodes.Ldloca_S, 2);
					yield return new CodeInstruction(OpCodes.Ldarga_S, 4);
					yield return new CodeInstruction(OpCodes.Call,
					AccessTools.Method(typeof(Dialog_BeginLordJob_DrawQualityDescription_Patch), nameof(DrawTargetIdeology)));
				}
			}
		}

		public static void DrawTargetIdeology(Dialog_BeginLordJob __instance, Rect viewRect, ref float curY, ref float totalInfoHeight)
		{
			if (__instance is Dialog_BeginRitual beginRitual
				&& beginRitual.ritual?.attachableOutcomeEffect?.Worker
				is RitualAttachableOutcomeEffectWorker_DiscoverRelics)
			{
				Rect rect6 = new Rect(viewRect.x, 0, viewRect.width, 24f);
				Rect rect7 = new Rect(rect6.xMax - 24f - 4f, rect6.y, 24f, 24f);
				curY += 32;
				totalInfoHeight += 32;
				rect6.xMax = rect7.xMin;
				Text.Anchor = TextAnchor.MiddleLeft;
				Widgets.Label(rect6, "IdeoConversionTarget".Translate());
				Text.Anchor = TextAnchor.UpperLeft;
				Widgets.DrawHighlightIfMouseover(rect6);
				TooltipHandler.TipRegionByKey(rect6, "RelicHunting_IdeoTargetDesc");
				if (ideos.TryGetValue(beginRitual.ritual, out var ideoPreceptTuple) is false)
				{
					ideoPreceptTuple = (Faction.OfPlayer.ideos.PrimaryIdeo, null);
					ideos[beginRitual.ritual] = ideoPreceptTuple;
				}
				ideoPreceptTuple.Item1.DrawIcon(rect7.ContractedBy(2f));
				if (Mouse.IsOver(rect7))
				{
					Widgets.DrawHighlight(rect7);
					TooltipHandler.TipRegion(rect7, ideoPreceptTuple.Item1.name);
				}
				if (Widgets.ButtonInvisible(rect7))
				{
					List<FloatMenuOption> list = new List<FloatMenuOption>();
					foreach (Ideo allIdeo in IdeosWithRelics)
					{
						Ideo newIdeo = allIdeo;
						string text5 = allIdeo.name;
						Action action = delegate
						{
							ideos[beginRitual.ritual] = (newIdeo, null);
						};
						list.Add(new FloatMenuOption(text5, action, newIdeo.Icon, newIdeo.Color));
					}
					Find.WindowStack.Add(new FloatMenu(list));
				}
				var height = 24;
				curY += height;
				rect6.y += height;
				rect7.y += height;
				totalInfoHeight += height;
				rect6.xMax = rect7.xMin;
				Text.Anchor = TextAnchor.MiddleLeft;
				Widgets.Label(rect6, "RelicHunting_TargetRelic".Translate());
				Text.Anchor = TextAnchor.UpperLeft;
				Widgets.DrawHighlightIfMouseover(rect6);
				TooltipHandler.TipRegionByKey(rect6, "RelicHunting_RelicTargetDesc");
				Precept_Relic selectedRelicPrecept = null; // To store selected relic precept
				Texture2D icon = Dialog_BeginLordJob.questionMark;
				string label = "RelicHunting_RandomRelicOption".Translate();
				string tooltip = "RelicHunting_RandomRelic".Translate();
				if (ideos.TryGetValue(beginRitual.ritual, out var ideoPreceptTuple2)
					&& ideoPreceptTuple2.Item2 != null)
				{
					selectedRelicPrecept = ideoPreceptTuple2.Item2;
					icon = selectedRelicPrecept.ThingDef.uiIcon;
					label = selectedRelicPrecept.ThingDef.label;
					tooltip = selectedRelicPrecept.ThingDef.description;
				}

				if (Widgets.ButtonImage(rect7.ContractedBy(2f), icon))
				{
					List<FloatMenuOption> list = new List<FloatMenuOption>();
					List<Precept_Relic> availableRelics = ideoPreceptTuple.Item1.GetAllPreceptsOfType<Precept_Relic>().Where(x => x.CanGenerateRelic).ToList();
					foreach (Precept_Relic relicPrecept in availableRelics)
					{
						foreach (var quest in Find.QuestManager.QuestsListForReading)
						{
							var part = quest.PartsListForReading.OfType<QuestPart_SubquestGenerator_RelicHunt>()
							.FirstOrDefault();
							if (part is not null && part.relic.ThingDef == relicPrecept.ThingDef)
							{
								continue;
							}
						}
						string text5 = relicPrecept.ThingDef.LabelCap;
						Action action = delegate
						{
							selectedRelicPrecept = relicPrecept;
							ideos[beginRitual.ritual] = (ideos[beginRitual.ritual].Item1, selectedRelicPrecept);
						};
						list.Add(new FloatMenuOption(text5, action, relicPrecept.ThingDef.uiIcon, Color.white));
					}
					FloatMenuOption randomOption = new FloatMenuOption("RelicHunting_RandomRelic".Translate(),
					() =>
					{
						selectedRelicPrecept = null;
						ideos[beginRitual.ritual] = (ideos[beginRitual.ritual].Item1, selectedRelicPrecept);
					}, Dialog_BeginLordJob.questionMark, Color.white);
					list.Add(randomOption);
					Find.WindowStack.Add(new FloatMenu(list));
				}

				if (Mouse.IsOver(rect7))
				{
					Widgets.DrawHighlight(rect7);
					TooltipHandler.TipRegion(rect7, selectedRelicPrecept?.ThingDef.description ?? "RelicHunting_RandomRelic".Translate());
				}
			}
		}
	}
}