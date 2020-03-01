using RimWorld;
using UnityEngine;
using Verse;
using static Numbers.Constants;

namespace Numbers
{
     public class Numbers_PawnColumnWorker_Icon : PawnColumnWorker_Icon
    {
        protected override Texture2D GetIconFor(Pawn pawn)
            => null;

        // thx Fluffy :)
        public override void DoHeader(Rect rect, PawnTable table)
        {
            bool moveDown = false;
            int idx = Numbers_Utility.GetColumnIndex(table.ColumnsListForReading, this.def);
            if (idx % 2 == 0) { moveDown = true; }
            string label = this.def.LabelCap.Resolve();
            Rect labelRect = Numbers_Utility.GetHeaderLabelRect(rect, label, moveDown);
            base.DoHeader(labelRect, table);
            Numbers_Utility.DrawHeaderLine(rect, labelRect);
        }

        public override int GetMinHeaderHeight(PawnTable table)
            => HeaderHeight;
    }
}
