using RimWorld;
using UnityEngine;
using Verse;
using static Numbers.Constants;

namespace Numbers
{
    public class Numbers_PawnColumnWorker : PawnColumnWorker
    {
        public override void DoCell(Rect rect, Pawn pawn, PawnTable table)
        { return; }

        public override void DoHeader(Rect rect, PawnTable table)
        {

            Rect labelRect = GetHeaderLabelRect(rect);
            base.DoHeader(labelRect, table);

            // vertical line
            if (!this.def.moveWorkTypeLabelDown)
            {
                // vertical line
                var lineStart = new Vector2(Mathf.FloorToInt(rect.center.x), labelRect.yMax);
                // note that two 1px lines give a much crisper line than one 2px line. Obv.
                GUI.color = new Color(1f, 1f, 1f, .3f);
                Widgets.DrawLineVertical(lineStart.x, lineStart.y, 20f);
                Widgets.DrawLineVertical(lineStart.x + 1, lineStart.y, 20f);
                GUI.color = Color.white;
            }
        }

        public Rect GetHeaderLabelRect(Rect rect)
        {
            Vector2 labelSize = Text.CalcSize(def.LabelCap.RawText);

            float x = rect.center.x;
            var result = new Rect(x - (labelSize.x + ExtraHeaderLabelWidth) / 2f, rect.y, labelSize.x + ExtraHeaderLabelWidth, HeaderHeight - AlternatingHeaderLabelOffset);
            if (this.def.moveWorkTypeLabelDown)
                result.y += AlternatingHeaderLabelOffset;

            return result;
        }

        public override int GetMinHeaderHeight(PawnTable table)
            => 40;
    }
}