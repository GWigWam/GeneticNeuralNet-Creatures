using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gnn.Genetic.Params {

    public class AdaptiveParamsProvider {
        private const float BaseChangeFrac = 0.1f;

        public float DesiredVariety { get; set; }
        public float VarietyAcceptableDif { get; set; }

        private float EletismFraction;
        private float MutationFraction;

        public AdaptiveParamsProvider(float initEletismFraction, float initMutationFraction, float desiredVariety = 0.5f, float varietyAcceptableDif = 0.15f) {
            EletismFraction = initEletismFraction;
            MutationFraction = initMutationFraction;

            DesiredVariety = desiredVariety;
            VarietyAcceptableDif = varietyAcceptableDif;
        }

        public GeneticParams GetParams(Genetic gen) {
            var hist = gen.History.ToArray();
            if(hist.Length > 1) {
                var varietyHist = hist.Select(gs => gs.Variety).ToArray();
                var lastVariety = varietyHist.Last();

                if(VarietyTooLow(lastVariety)) {
                    if(varietyHist[hist.Length - 2] >= varietyHist[hist.Length - 1]) { //Variety is falling
                        var ctLtDesired = varietyHist.Reverse().TakeWhile(VarietyTooLow).Count(); //Number of iterations variety has been < desired

                        MutationFraction *= (float)Math.Pow(1 + BaseChangeFrac, ctLtDesired);
                    }
                } else if(VarietyTooHigh(lastVariety)) {
                    if(varietyHist[hist.Length - 2] <= varietyHist[hist.Length - 1]) { //Variety is rising
                        var ctGtDesired = varietyHist.Reverse().TakeWhile(VarietyTooHigh).Count(); //Number of iterations variety has been > desired

                        MutationFraction *= (float)Math.Pow(1 - BaseChangeFrac, ctGtDesired);
                    }
                }
            }

            return new GeneticParams(EletismFraction, MutationFraction);
        }

        private bool VarietyTooLow(float variety) => variety < DesiredVariety - VarietyAcceptableDif;

        private bool VarietyTooHigh(float variety) => variety > DesiredVariety + VarietyAcceptableDif;
    }
}