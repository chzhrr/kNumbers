﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;
using RimWorld;
using System.Reflection;

namespace kNumbers
{
    public class MainTabWindow_Numbers : MainTabWindow_PawnList
    {

        public enum pawnType
        {
            Colonists,
            Prisoners,
            Enemies,
            Animals,
            WildAnimals,
        }

        public enum orderBy
        {
            Name,
            Column
        }

        bool pawnListDescending = false;
        bool pawnListNeedsUpdate = true;

        //global lists
        List<StatDef> pawnHumanlikeStatDef = new List<StatDef>();
        List<StatDef> pawnAnimalStatDef = new List<StatDef>();
        List<NeedDef> pawnHumanlikeNeedDef = new List<NeedDef>();
        List<NeedDef> pawnAnimalNeedDef = new List<NeedDef>();
        List<SkillDef> pawnSkillDef = new List<SkillDef>();

        //local lists - content depends on pawn type
        List<StatDef> pStatDef;
        List<NeedDef> pNeedDef;

        List<KListObject> kList = new List<KListObject>();
        Dictionary<pawnType, List<KListObject>> savedKLists = new Dictionary<pawnType, List<KListObject>>(5);

        float maxWindowWidth = 1060f;

        public override Vector2 RequestedTabSize
        {
            get
            {
                return new Vector2(maxWindowWidth, 90f + (float)base.PawnsCount * PawnRowHeight + 65f);
            }
        }

        float kListDesiredWidth = 0f;

        pawnType chosenPawnType = pawnType.Colonists; 
        orderBy chosenOrderBy = orderBy.Name;
        KListObject sortObject;

        public MainTabWindow_Numbers()
        {
            Pawn tmpPawn;

            MethodInfo statsToDraw = typeof(StatsReportUtility).GetMethod("StatsToDraw", BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.InvokeMethod, null, new Type[] { typeof(Thing) }, null);

            tmpPawn = PawnGenerator.GeneratePawn(PawnKindDefOf.Colonist, Faction.OfColony, false, 0);
            pawnHumanlikeStatDef = (from s in ((IEnumerable<StatDrawEntry>)statsToDraw.Invoke(null, new[] { tmpPawn })) where s.ShouldDisplay && s.stat != null select s.stat).ToList();
            pawnHumanlikeNeedDef.AddRange(tmpPawn.needs.AllNeeds.Where(x => x.def.label == "mood").Select(x => x.def).ToList()); //why it's not normally returned is beyond me
            pawnHumanlikeNeedDef.AddRange(tmpPawn.needs.AllNeeds.Where(x => x.def.showOnNeedList).Select(x => x.def).ToList());

            tmpPawn = PawnGenerator.GeneratePawn(PawnKindDefOf.Thrumbo, null, false, 0);
            pawnAnimalStatDef = (from s in ((IEnumerable<StatDrawEntry>)statsToDraw.Invoke(null, new[] { tmpPawn })) where s.ShouldDisplay && s.stat != null select s.stat).ToList();
            pawnAnimalNeedDef = tmpPawn.needs.AllNeeds.Where(x => x.def.showOnNeedList).Select(x => x.def).ToList();

        }

        public override void PreOpen()
        {
            base.PreOpen();
            pawnListNeedsUpdate = true;
        }

        bool fits(float desiredSize)
        {
            return (kListDesiredWidth + desiredSize + 70 < maxWindowWidth);
        }

        void UpdatePawnList()
        {
            
            this.pawns.Clear();
            IEnumerable<Pawn> tempPawns = new List<Pawn>();

            switch (chosenPawnType)
            {
                default:
                case pawnType.Colonists:
                    tempPawns = Find.ListerPawns.FreeColonists;
                    pStatDef = pawnHumanlikeStatDef;
                    pNeedDef = pawnHumanlikeNeedDef;
                    break;

                case pawnType.Prisoners:
                    tempPawns = Find.ListerPawns.PrisonersOfColony;
                    pStatDef = pawnHumanlikeStatDef;
                    pNeedDef = pawnHumanlikeNeedDef;
                    break;

                case pawnType.Enemies:
                    tempPawns = Find.ListerPawns.PawnsHostileToColony;
                    pStatDef = pawnHumanlikeStatDef;
                    pNeedDef = pawnHumanlikeNeedDef;
                    break;

                case pawnType.Animals:
                    tempPawns = (from p in Find.ListerPawns.PawnsInFaction(Faction.OfColony) where p.RaceProps.Animal select p).ToList();
                    pStatDef = pawnAnimalStatDef;
                    pNeedDef = pawnAnimalNeedDef;
                    break;

                case pawnType.WildAnimals:
                    tempPawns = (from p in Find.ListerPawns.AllPawns where p.RaceProps.Animal && p.Faction == null && !p.Position.Fogged() select p).ToList();
                    pStatDef = pawnAnimalStatDef;
                    pNeedDef = pawnAnimalNeedDef;
                    break;
            }

            switch (chosenOrderBy)
            {
                default:
                case orderBy.Name:
                    this.pawns = (from p in tempPawns
                                  orderby p.LabelCap ascending
                                  select p).ToList();
                    break;

                case orderBy.Column:
                    switch (sortObject.oType)
                    {
                        case KListObject.objectType.Stat:
                            this.pawns = (from p in tempPawns
                                          orderby p.GetStatValue((StatDef)sortObject.displayObject, true) ascending
                                          select p).ToList();
                            break;

                        case KListObject.objectType.Need:
                            //pause game if we're sorting by needs because these things change
                            if (!Find.TickManager.Paused)
                                Find.TickManager.TogglePaused();

                            this.pawns = (from p in tempPawns
                                          orderby p.needs.TryGetNeed((NeedDef)sortObject.displayObject).CurLevel ascending
                                          select p).ToList();
                            break;

                        case KListObject.objectType.Skill:
                            this.pawns = (from p in tempPawns
                                          orderby p.skills.GetSkill((SkillDef)sortObject.displayObject).XpTotalEarned ascending
                                          select p).ToList();
                            break;
                    }
                    
                    break;
            }

            if (pawnListDescending)
            {
                this.pawns.Reverse();
            }

            pawnListNeedsUpdate = false;
        }

        public void PawnSelectOptionsMaker()
        {
            List<FloatMenuOption> list = new List<FloatMenuOption>();
            foreach (pawnType pawn in Enum.GetValues(typeof(pawnType)))
            {
                Action action = delegate
                {
                    if (pawn != chosenPawnType)
                    { 
                        savedKLists[chosenPawnType] = kList;
                        savedKLists.TryGetValue(pawn, out kList);
                        if (kList == null)
                            kList = new List<KListObject>();
                        chosenPawnType = pawn;
                        pawnListNeedsUpdate = true;
                    }
                };

                list.Add(new FloatMenuOption(("koisama.pawntype."+pawn.ToString()).Translate(), action, MenuOptionPriority.Medium, null, null));
            }
            Find.WindowStack.Add(new FloatMenu(list, false));
        }

        public void StatsOptionsMaker()
        {
            
            List<FloatMenuOption> list = new List<FloatMenuOption>();
            foreach(StatDef stat in pStatDef)
            {
                Action action = delegate
                {
                    KListObject kl = new KListObject(KListObject.objectType.Stat, stat.LabelCap, stat);
                    if(fits(kl.minWidthDesired))
                        kList.Add(kl);
                };
                list.Add(new FloatMenuOption(stat.LabelCap, action, MenuOptionPriority.Medium, null, null));
            }
            Find.WindowStack.Add(new FloatMenu(list, false));
        }

        public void SkillsOptionsMaker()
        {
            List<FloatMenuOption> list = new List<FloatMenuOption>();
            foreach (SkillDef skill in DefDatabase<SkillDef>.AllDefsListForReading)
            {
                Action action = delegate
                {
                    KListObject kl = new KListObject(KListObject.objectType.Skill, skill.LabelCap, skill);
                    if (fits(kl.minWidthDesired))
                        kList.Add(kl);
                };
                list.Add(new FloatMenuOption(skill.LabelCap, action, MenuOptionPriority.Medium, null, null));
            }
            Find.WindowStack.Add(new FloatMenu(list, false));
        }

        public void NeedsOptionsMaker()
        {
            List<FloatMenuOption> list = new List<FloatMenuOption>();
            foreach (NeedDef need in pNeedDef)
            {
                Action action = delegate
                {
                    KListObject kl = new KListObject(KListObject.objectType.Need, need.LabelCap, need);
                    if (fits(kl.minWidthDesired))
                        kList.Add(kl);
                };
                list.Add(new FloatMenuOption(need.LabelCap, action, MenuOptionPriority.Medium, null, null));
            }
            Find.WindowStack.Add(new FloatMenu(list, false));
        }

        public override void DoWindowContents(Rect r)
        {
            base.DoWindowContents(r);

            if (pawnListNeedsUpdate)
            {
                UpdatePawnList();
            }

            Rect position = new Rect(0f, 0f, r.width, 110f);
            GUI.BeginGroup(position);

            float x = 0f;
            Text.Font = GameFont.Small;

            //pawn/prisoner list switch
            Rect sourceButton = new Rect(x, 0f, 180f, PawnRowHeight);
            if (Widgets.TextButton(sourceButton, ("koisama.pawntype." + chosenPawnType.ToString()).Translate()))
            {
                PawnSelectOptionsMaker();
            }
            x += 190;

            //add column button
            Rect addColumnButton = new Rect(x, 0f, 180f, PawnRowHeight);
            if (Widgets.TextButton(addColumnButton, "koisama.Numbers.AddColumnLabel".Translate()))
            {
                StatsOptionsMaker();
            }
            x += 190;

            if (new []{ pawnType.Colonists, pawnType.Prisoners, pawnType.Enemies }.Contains(chosenPawnType))
            {
                Rect skillColumnButton = new Rect(x, 0f, 180f, PawnRowHeight);
                if (Widgets.TextButton(skillColumnButton, "koisama.Numbers.AddSkillColumnLabel".Translate()))
                {
                    SkillsOptionsMaker();
                }
                x += 190;
            }

            Rect needsColumnButton = new Rect(x, 0f, 180f, PawnRowHeight);
            if (Widgets.TextButton(needsColumnButton, "koisama.Numbers.AddNeedsColumnLabel".Translate()))
            {
                NeedsOptionsMaker();
            }
            x += 190;


            x = 0;

            //names
            Rect nameLabel = new Rect(x, 75f, NameColumnWidth, PawnRowHeight);
            Text.Anchor = TextAnchor.LowerCenter;
            Widgets.Label(nameLabel, "koisama.Numbers.Name".Translate());
            if (Widgets.InvisibleButton(nameLabel))
            {
                if (chosenOrderBy == orderBy.Name)
                {
                    pawnListDescending = !pawnListDescending;
                }
                else
                {
                    chosenOrderBy = orderBy.Name;
                    pawnListDescending = false;
                }
                pawnListNeedsUpdate = true;
            }

            TooltipHandler.TipRegion(nameLabel, "koisama.Numbers.SortByTooltip".Translate("koisama.Numbers.Name".Translate()));
            Widgets.DrawHighlightIfMouseover(nameLabel);
            x += NameColumnWidth;

            //header
            //TODO: better interface - auto width calculation
            bool offset = true;
            kListDesiredWidth = 175f;

            for(int i=0;i<kList.Count; i++)
            {
                float colWidth = kList[i].minWidthDesired;
                kListDesiredWidth += colWidth;

                Rect defLabel = new Rect(x-35, 20f + (offset ? 10f : 50f), colWidth+70 , 40f);
                Widgets.DrawLine(new Vector2(x + colWidth/2 , 52f + (offset ? 15f : 55f)), new Vector2(x + colWidth/2 , 110f), Color.gray, 1);
                Widgets.Label(defLabel, kList[i].label);

                StringBuilder labelSB = new StringBuilder();
                labelSB.AppendLine("koisama.Numbers.SortByTooltip".Translate(kList[i].label));
                labelSB.AppendLine("koisama.Numbers.RemoveTooltip".Translate());
                TooltipHandler.TipRegion(defLabel, labelSB.ToString());
                Widgets.DrawHighlightIfMouseover(defLabel);

                if (Widgets.InvisibleButton(defLabel))
                {
                    if (Event.current.button == 1)
                    {
                        kList.RemoveAt(i);
                    }
                    else
                    {

                        if (chosenOrderBy == orderBy.Column && kList[i].Equals(sortObject))
                        {
                            pawnListDescending = !pawnListDescending;
                        }
                        else
                        {
                            sortObject = kList[i];
                            chosenOrderBy = orderBy.Column;
                            pawnListDescending = false;
                        }
                    }
                    pawnListNeedsUpdate = true;
                }
                offset = !offset;
                x += colWidth;
            }
            GUI.EndGroup();

            //content
            Rect content = new Rect(0f, position.yMax, r.width, r.height - position.yMax);
            GUI.BeginGroup(content);
            base.DrawRows(new Rect(0f, 0f, content.width, content.height));
            GUI.EndGroup();
        }

        protected override void DrawPawnRow(Rect r, Pawn p)
        {
            float x = 175f;
            float y = r.yMin;

            Text.Anchor = TextAnchor.MiddleCenter;

            //TODO: better interface - auto width calculation, make sure columns won't overlap
            for (int i = 0; i < kList.Count; i++)
            {
                float colWidth = kList[i].minWidthDesired;
                Rect capCell = new Rect(x, y, colWidth, 30f);
                kList[i].Draw(capCell, p);
                x += colWidth;

            }

        }

    }
}
