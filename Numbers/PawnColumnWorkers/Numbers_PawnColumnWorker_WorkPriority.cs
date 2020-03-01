using RimWorld;
using UnityEngine;
using Verse;
using static Numbers.Constants;

namespace Numbers
{

    /* This is *not* a child of the actual PCW_Workpriority, instead we intercept the call to DoHeader with a Harmony patch in numbers.cs.
     * We do this to change the implementation of alternating header label heights. In the original method the height of the label is determined by
     * 'PawnColumnDef.moveWorkTypeLabelDown'. The state of each label seems hardcoded. This is not what we want: we want to move them around
     * and have the height adjust based on their order (odd/even) like how all other Workers in Numbers handle labels. This way they can be integrated
     * seemlessly into numbers tables.
     */



	public class Numbers_PawnColumnWorker_WorkPriority
    { 

        public static void DoHeader(PawnColumnWorker __instance, Rect rect, PawnTable table)
        {
            // determine odd/even (up/down)
            bool moveDown = false;
            int idx = Numbers_Utility.GetColumnIndex(table.ColumnsListForReading, __instance.def);
            if (idx % 2 == 0) { moveDown = true; }

            string label = __instance.def.workType.labelShort;
            Rect labelRect = Numbers_Utility.GetHeaderLabelRect(rect, label, moveDown);

            // from PawnColumnWorker.WorkPriority.DoHeader
            Text.Font = GameFont.Small;
            Text.Anchor = TextAnchor.MiddleCenter;
            Widgets.Label(labelRect, label);
            GUI.color = new Color(1f, 1f, 1f, 0.3f);
            Widgets.DrawLineVertical(labelRect.center.x, labelRect.yMax - 3f, rect.y + 50f - labelRect.yMax + 3f);
            Widgets.DrawLineVertical(labelRect.center.x + 1f, labelRect.yMax - 3f, rect.y + 50f - labelRect.yMax + 3f);
            GUI.color = Color.white;
            Text.Anchor = TextAnchor.UpperLeft;
        }
    }
}

