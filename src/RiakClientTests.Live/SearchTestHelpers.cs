namespace RiakClientTests.Live.Extensions
{
    using System;
    using RiakClient;
    using RiakClient.Models.Search;

    public static class SearchTestHelpers
    {
        public static Func<RiakResult<RiakSearchResult>, bool> AnyMatchIsFound
        {
            get
            {
                Func<RiakResult<RiakSearchResult>, bool> matchIsFound =
                    result => result.IsSuccess &&
                              result.Value != null &&
                              result.Value.NumFound > 0;
                return matchIsFound;
            }
        }

        public static Func<RiakResult<RiakSearchResult>, bool> TwoMatchesFound
        {
            get
            {
                Func<RiakResult<RiakSearchResult>, bool> twoMatchesFound =
                    result => result.IsSuccess &&
                              result.Value != null &&
                              result.Value.NumFound == 2;
                return twoMatchesFound;
            }
        }

        public static Func<RiakResult<RiakSearchResult>> RunSolrQuery(this IRiakClient client, RiakSearchRequest req)
        {
            Func<RiakResult<RiakSearchResult>> runSolrQuery =
                () => client.Search(req);
            return runSolrQuery;
        }
    }
}
