namespace Numbers
{
    using RimWorld;
    using UnityEngine;
    using Verse;
    using System.Collections.Generic;
    using System.Linq;

    //simply copy from PawnColumnWorker_Jobcurrent and implement formatted trait
    //test with English and Chinese
    //tested compatible with the mod [more practical traits], even with colorful traits
    public class PawnColumnWorker_Traits : PawnColumnWorker_Text
    {
        protected override string GetTextFor(Pawn pawn)
        {
            if (pawn.story == null)
            {
                return null;
            }
            if (pawn.story.traits.allTraits != null && pawn.story.traits.allTraits.Count!=0)
            {
                
                List<string> trait_name_list= new List<string>();
                foreach (Trait i_trait in pawn.story.traits.allTraits){
                    if (i_trait.def.degreeDatas.Count == 1)
                    {
                        trait_name_list.Add(i_trait.def.degreeDatas.First().label);//use label for multiple language
                    }
                    else
                    {
                        //for something like beauty, the degree matters
                        foreach(TraitDegreeData j_degree in i_trait.def.degreeDatas)
                        {
                            if (j_degree.degree == i_trait.Degree)
                            {
                                trait_name_list.Add(j_degree.label);
                            }
                        }
                    }
                    
                }
                string text = string.Join(" , ", trait_name_list);
                GenText.SetTextSizeToFit(text, new Rect(0f, 0f, Mathf.CeilToInt(Text.CalcSize(def.LabelCap).x), GetMinCellHeight(pawn)));

                return text;
            }
            return null;
        }

        protected override string GetTip(Pawn pawn) => GetTextFor(pawn);

        //use a stupid method to sort, maybe it needs to be sharpen
        public override int Compare(Pawn a, Pawn b)
            => (GetTextFor(a) ?? string.Empty).CompareTo((GetTextFor(b) ?? string.Empty));

        public override int GetMinWidth(PawnTable table)
            //adjust width because traits need more space
            => Mathf.Max(base.GetMinWidth(table), 350);

        public override int GetMinHeaderHeight(PawnTable table)
            => Mathf.CeilToInt(Text.CalcSize(Numbers_Utility.WordWrapAt(def.LabelCap, GetMinWidth(table))).y);
    }
}
