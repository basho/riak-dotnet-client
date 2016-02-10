namespace RiakClientTests.Live.Extensions
{
    using System;
    using System.Linq;
    using RiakClient;
    using RiakClient.Models.MapReduce;

    public static class MapReduceTestHelpers
    {
        public static Func<RiakResult<RiakMapReduceResult>> RunMapReduceQuery(
            this IRiakClient client, RiakMapReduceQuery req)
        {
            Func<RiakResult<RiakMapReduceResult>> runMapRedQuery =
                () => client.MapReduce(req);
            return runMapRedQuery;
        }

        public static bool OnePhaseWithOneResultFound(RiakResult<RiakMapReduceResult> result)
        {
            return OnePhaseWith_M_ResultsFound(result, 1);
        }

        public static bool OnePhaseWithTwoResultsFound(RiakResult<RiakMapReduceResult> result)
        {
            return OnePhaseWith_M_ResultsFound(result, 2);
        }

        public static bool OnePhaseWithFiveResultsFound(RiakResult<RiakMapReduceResult> result)
        {
            return OnePhaseWith_M_ResultsFound(result, 5);
        }

        public static bool OnePhaseWithTenResultsFound(RiakResult<RiakMapReduceResult> result)
        {
            return OnePhaseWith_M_ResultsFound(result, 10);
        }

        public static bool OnePhaseWithTwelveResultsFound(RiakResult<RiakMapReduceResult> result)
        {
            return OnePhaseWith_M_ResultsFound(result, 12);
        }




        public static bool OnePhaseWith_M_ResultsFound(RiakResult<RiakMapReduceResult> result, int numResults)
        {
            if (!result.IsSuccess || result.Value == null)
            {
                return false;
            }

            var phaseResults = result.Value.PhaseResults.ToList();

            if (phaseResults.Count != 1)
            {
                return false;
            }

            var phase1Results = phaseResults[0].Values;

            return phase1Results.Count == numResults;
        }
    }
}
