using RimWorld;
using UnityEngine;
using Verse;
using static Numbers.Constants;

namespace Numbers
{
	public class Numbers_PawnColumnWorker_WorkPriority
    { 

        public static void DoHeader(PawnColumnWorker __instance, Rect rect, PawnTable table)
        {
            // determine odd/even (up/down)
            bool moveDown = false;
            int idx = Numbers_Utility.GetColumnIndex(table.ColumnsListForReading, __instance.def);
            if (idx % 2 == 0) { moveDown = true; }

            string label = __instance.def.workType.labelShort;

            Rect labelRect = GetHeaderLabelRect(rect, moveDown, label);

            // from PawnColumnWorker.WorkPriority.DoHeader
            
            Text.Font = GameFont.Small;
            //MouseoverSounds.DoRegion(labelRect);
            Text.Anchor = TextAnchor.MiddleCenter;
            Widgets.Label(labelRect, label);
            GUI.color = new Color(1f, 1f, 1f, 0.3f);
            Widgets.DrawLineVertical(labelRect.center.x, labelRect.yMax - 3f, rect.y + 50f - labelRect.yMax + 3f);
            Widgets.DrawLineVertical(labelRect.center.x + 1f, labelRect.yMax - 3f, rect.y + 50f - labelRect.yMax + 3f);
            GUI.color = Color.white;
            Text.Anchor = TextAnchor.UpperLeft;
        }

        public static Rect GetHeaderLabelRect(Rect rect, bool moveDown, string label)
        {
            Vector2 labelSize = Text.CalcSize(label);
            labelSize.x = Mathf.Min(labelSize.x, MaxHeaderWidth);

            float x = rect.center.x;
            var result = new Rect(x - (labelSize.x + ExtraHeaderLabelWidth) / 2f, rect.y, labelSize.x + ExtraHeaderLabelWidth, HeaderHeight - AlternatingHeaderLabelOffset);
            if (moveDown)
                result.y += AlternatingHeaderLabelOffset;

            return result;
        }
    }
}

