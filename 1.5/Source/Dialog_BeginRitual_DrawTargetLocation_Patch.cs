using System;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;

namespace RelicHunting
{
	[HotSwappable]
	[HarmonyPatch(typeof(Dialog_BeginRitual), "DrawExtraInfo")]
	public static class Dialog_BeginRitual_DrawExtraInfo_Patch
	{
		public static Dictionary<Precept_Ritual, Ideo> ideos = new Dictionary<Precept_Ritual, Ideo>();
		public static List<Ideo> IdeosWithRelics => Find.IdeoManager.IdeosListForReading.Where(x => x.GetAllPreceptsOfType<Precept_Relic>().Any(x => x.CanGenerateRelic)).ToList();
		public static void Postfix(Dialog_BeginRitual __instance, Rect viewRect, ref float curY, ref float totalInfoHeight)
		{
			if (__instance.ritual.attachableOutcomeEffect.Worker is RitualAttachableOutcomeEffectWorker_DiscoverRelics)
			{
				Rect rect6 = new Rect(viewRect.x, curY, viewRect.width, 24f);
				Rect rect7 = new Rect(rect6.xMax - 24f - 4f, rect6.y, 24f, 24f);
				curY += 24;
				totalInfoHeight += 24;
				rect6.xMax = rect7.xMin;
				Text.Anchor = TextAnchor.MiddleLeft;
				Widgets.Label(rect6, "IdeoConversionTarget".Translate());
				Text.Anchor = TextAnchor.UpperLeft;
				Widgets.DrawHighlightIfMouseover(rect6);
				TooltipHandler.TipRegionByKey(rect6, "RelicHunting_IdeoTargetDesc");
				if (ideos.TryGetValue(__instance.ritual, out Ideo ideo) is false)
				{
					ideo = ideos[__instance.ritual] = Faction.OfPlayer.ideos.PrimaryIdeo;
				}
				ideo.DrawIcon(rect7.ContractedBy(2f));
				if (Mouse.IsOver(rect7))
				{
					Widgets.DrawHighlight(rect7);
					TooltipHandler.TipRegion(rect7, ideo.name);
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
							ideo = ideos[__instance.ritual] = newIdeo;
						};
						list.Add(new FloatMenuOption(text5, action, newIdeo.Icon, newIdeo.Color));
					}
					Find.WindowStack.Add(new FloatMenu(list));
				}
			}
		}
	}
}