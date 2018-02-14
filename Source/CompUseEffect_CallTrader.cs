using RimWorld;
using System;
using System.Linq;
using Verse;

namespace TraderWhereAreYou
{
    public class CompUseEffect_CallTrader : CompUseEffect
    {
        // this is a weird amalgamation between CompDoEffect and IncidentWorker_OrbitalTraderArrival
        public override void DoEffect(Pawn usedBy)
        {
            base.DoEffect(usedBy);

            Map map = usedBy.Map;
            if (map.passingShipManager.passingShips.Count >= 5)
            {
                return;
            }
            if ((from x in DefDatabase<TraderKindDef>.AllDefs
                 where x.orbital
                 select x).TryRandomElementByWeight((TraderKindDef traderDef) => traderDef.commonality, out TraderKindDef def))
            {
                TradeShip tradeShip = new TradeShip(def);
                if (map.listerBuildings.allBuildingsColonist.Any((Building b) => b.def.IsCommsConsole && b.GetComp<CompPowerTrader>().PowerOn))
                {
                    Find.LetterStack.ReceiveLetter(tradeShip.def.LabelCap, "TraderArrival".Translate(new object[]
                    {
                        tradeShip.name,
                        tradeShip.def.label
                    }), LetterDefOf.PositiveEvent, null);
                }
                map.passingShipManager.AddShip(tradeShip);
                tradeShip.GenerateThings();
            }
            else throw new InvalidOperationException();
        }
    }
}