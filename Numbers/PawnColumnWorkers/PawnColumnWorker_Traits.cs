namespace Numbers
{
    using RimWorld;
    using UnityEngine;
    using Verse;

    public class PawnColumnWorker_Traits : PawnColumnWorker_Text
    {
        protected override string GetTextFor(Pawn pawn)
        {
            string text = "";
            if (pawn.story.traits != null)
            {
                //for (int i = 0; i < pawn.story.traits.Count; i++)
                //{
                //    text += pawn.story.traits[i].ToString()
                //}
                text = pawn.story.traits.ToString();
                GenText.SetTextSizeToFit(text, new Rect(0f, 0f, Mathf.CeilToInt(Text.CalcSize(def.LabelCap).x), GetMinCellHeight(pawn)));

                return text;
            }
            return null;
        }

        protected override string GetTip(Pawn pawn) => GetTextFor(pawn);

        public override int Compare(Pawn a, Pawn b)
            => (a.jobs?.curDriver.GetReport()).CompareTo(b.jobs?.curDriver.GetReport());//should be fixed

        public override int GetMinWidth(PawnTable table)
            => Mathf.Max(base.GetMinWidth(table), 200);

        public override int GetMinHeaderHeight(PawnTable table)
            => Mathf.CeilToInt(Text.CalcSize(Numbers_Utility.WordWrapAt(def.LabelCap, GetMinWidth(table))).y);
    }
}
