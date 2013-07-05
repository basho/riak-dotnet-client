using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CorrugatedIron.Extensions;
using CorrugatedIron.Messages;

namespace CorrugatedIron.Models.Index
{
    public class RiakStreamedIndexResult : IRiakIndexResult
    {
        private readonly IEnumerable<RiakResult<RpbIndexResp>> _responseReader;
        private readonly bool _includeTerms;

        public RiakStreamedIndexResult(bool includeTerms, IEnumerable<RiakResult<RpbIndexResp>> responseReader)
        {
            _responseReader = responseReader;
            _includeTerms = includeTerms;
        }

        public IEnumerable<RiakIndexKeyTerm> IndexKeyTerms 
        { 
            get
            {
                return _responseReader.SelectMany(item => GetIndexKeyTerm(item.Value));
            }
        }

        private IEnumerable<RiakIndexKeyTerm> GetIndexKeyTerm(RpbIndexResp response)
        {
            if (_includeTerms)
            {
                return response.results.Select(pair =>
                                                new RiakIndexKeyTerm(pair.value.FromRiakString(),
                                                                    pair.key.FromRiakString()));
            }

            return response.keys.Select(key => new RiakIndexKeyTerm(key.FromRiakString()));
        }
    }
}
