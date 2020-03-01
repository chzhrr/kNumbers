using RimWorld;
using UnityEngine;
using Verse;
using static Numbers.Constants;

namespace Numbers
{
    public class Numbers_PawnColumnWorker_Checkbox : PawnColumnWorker_Checkbox
    {
        protected override bool GetValue(Pawn pawn) 
            => false;

        protected override void SetValue(Pawn pawn, bool value)
        { return; }

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
