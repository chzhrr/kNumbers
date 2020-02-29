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

        // thx Fluffy :)
        public override void DoHeader(Rect rect, PawnTable table)
        {
            bool moveDown = false;

            int idx = Numbers_Utility.GetColumnIndex(table.ColumnsListForReading, this.def);
            if (idx % 2 == 0) { moveDown = true; }

            Rect labelRect = GetHeaderLabelRect(rect, moveDown);
            base.DoHeader(labelRect, table);

            // vertical line
            GUI.color = new Color(1f, 1f, 1f, .3f);
            Widgets.DrawLineVertical(labelRect.center.x, labelRect.yMax - 3f, rect.y + HeaderHeight - labelRect.yMax + 3f);
            Widgets.DrawLineVertical(labelRect.center.x + 1f, labelRect.yMax - 3f, rect.y + HeaderHeight - labelRect.yMax + 3f);
            GUI.color = Color.white;
        }

        public Rect GetHeaderLabelRect(Rect rect, bool moveDown)
        {
            Vector2 labelSize = Text.CalcSize(this.def.LabelCap.Resolve());
            labelSize.x = Mathf.Min(labelSize.x, MaxHeaderWidth);

            float x = rect.center.x;
            var result = new Rect(x - (labelSize.x + ExtraHeaderLabelWidth) / 2f, rect.y, labelSize.x + ExtraHeaderLabelWidth, HeaderHeight - AlternatingHeaderLabelOffset);
            if (moveDown)
                result.y += AlternatingHeaderLabelOffset;

            return result;
        }

        public override int GetMinHeaderHeight(PawnTable table)
            => HeaderHeight;
    }
}