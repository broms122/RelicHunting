using System.Collections.Generic;
using RimWorld;
using RimWorld.Planet;
using Verse;
using System.Linq;

namespace RelicHunting
{
	public class RitualAttachableOutcomeEffectWorker_DiscoverRelics : RitualAttachableOutcomeEffectWorker
	{
		public static Ideo targetIdeo;
		public static Precept_Relic targetRelic;
		public override void Apply(Dictionary<Pawn, int> totalPresence, LordJob_Ritual jobRitual,
			RitualOutcomePossibility outcome, out string extraOutcomeDesc, ref LookTargets letterLookTargets)
		{
			extraOutcomeDesc = def.letterInfoText;
			targetIdeo = LordJob_Ritual_ExposeData_Patch.targetIdeology.TryGetValue(jobRitual);
			targetRelic = LordJob_Ritual_ExposeData_Patch.targetRelicPrecept.TryGetValue(jobRitual);
			Quest quest = QuestUtility.GenerateQuestAndMakeAvailable(DefsOf.RelicHunt, StorytellerUtility.DefaultThreatPointsNow(jobRitual.Map));
			QuestUtility.SendLetterQuestAvailable(quest);
			letterLookTargets = new LookTargets((letterLookTargets.targets ?? new List<GlobalTargetInfo>()).Concat(quest.QuestLookTargets));
		}
	}
}