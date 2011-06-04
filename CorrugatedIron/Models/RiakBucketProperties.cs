using System.Collections.Generic;
using System.IO;
using System.Text;
using CorrugatedIron.Extensions;
using CorrugatedIron.Messages;
using CorrugatedIron.Util;
using Newtonsoft.Json;

namespace CorrugatedIron.Models
{
    // TODO: handle pre/post commits
    public class RiakBucketProperties
    {
        // TODO: figure out what to do with these fields given that they can
        // be of different types. How would a "getter" work in this scenario?
        private uint? _rVal;
        private string _rValString;
        private uint? _rwVal;
        private string _rwValString;
        private uint? _dwVal;
        private string _dwValString;
        private uint? _wVal;
        private string _wValString;

        // At the moment, only the NVal and AllowMult can be set via the PBC
        // so if the request has any other value set, we can't use that interface.
        // We check those values and if they're missing we go with PBC as it's
        // substantially quicker.
        public bool CanUsePbc
        {
            get
            {
                return !LastWriteWins.HasValue
                    && !_rVal.HasValue
                    && string.IsNullOrEmpty(_rValString)
                    && !_rwVal.HasValue
                    && string.IsNullOrEmpty(_rwValString)
                    && !_wVal.HasValue
                    && string.IsNullOrEmpty(_wValString)
                    && !_dwVal.HasValue
                    && string.IsNullOrEmpty(_dwValString)
                    && string.IsNullOrEmpty(Backend);
            }
        }

        public bool? LastWriteWins { get; private set; }

        public uint? NVal { get; private set; }

        public bool? AllowMultiple { get; private set; }

        public string Backend { get; private set; }

        public RiakBucketProperties SetAllowMultiple(bool value)
        {
            AllowMultiple = value;
            return this;
        }

        public RiakBucketProperties SetLastWriteWins(bool value)
        {
            LastWriteWins = value;
            return this;
        }

        public RiakBucketProperties SetNVal(uint value)
        {
            NVal = value;
            return this;
        }

        public RiakBucketProperties SetRVal(uint value)
        {
            return WriteQuorum(value, ref _rVal, ref _rValString);
        }

        public RiakBucketProperties SetRVal(string value)
        {
            return WriteQuorum(value, ref _rVal, ref _rValString);
        }

        public RiakBucketProperties SetRwVal(uint value)
        {
            return WriteQuorum(value, ref _rwVal, ref _rwValString);
        }

        public RiakBucketProperties SetRwVal(string value)
        {
            return WriteQuorum(value, ref _rwVal, ref _rwValString);
        }

        public RiakBucketProperties SetDwVal(uint value)
        {
            return WriteQuorum(value, ref _dwVal, ref _dwValString);
        }

        public RiakBucketProperties SetDwVal(string value)
        {
            return WriteQuorum(value, ref _dwVal, ref _dwValString);
        }

        public RiakBucketProperties SetWVal(uint value)
        {
            return WriteQuorum(value, ref _wVal, ref _wValString);
        }

        public RiakBucketProperties SetWVal(string value)
        {
            return WriteQuorum(value, ref _wVal, ref _wValString);
        }

        public RiakBucketProperties SetBackend(string backend)
        {
            Backend = backend;
            return this;
        }

        private RiakBucketProperties WriteQuorum(uint value, ref uint? targetVal, ref string targetString)
        {
            System.Diagnostics.Debug.Assert(value > 1);
            targetVal = value;
            targetString = null;
            return this;
        }

        private RiakBucketProperties WriteQuorum(string value, ref uint? targetVal, ref string targetString)
        {
            System.Diagnostics.Debug.Assert(new HashSet<string> { "all", "quorum", "one" }.Contains(value), "Incorrect quorum value");
            targetVal = null;
            targetString = value;
            return this;
        }

        public RiakBucketProperties()
        {
        }

        public RiakBucketProperties(RiakRestResponse response)
        {
            System.Diagnostics.Debug.Assert(response.ContentType == Constants.ContentTypes.ApplicationJson);
        }

        public RiakBucketProperties(RpbBucketProps bucketProps)
        {
            AllowMultiple = bucketProps.AllowMultiple;
            NVal = bucketProps.NVal;
        }

        internal RpbBucketProps ToMessage()
        {
            var message = new RpbBucketProps();
            if(AllowMultiple.HasValue)
            {
                message.AllowMultiple = AllowMultiple.Value;
            }
            if (NVal.HasValue)
            {
                message.NVal = NVal.Value;
            }
            return message;
        }

        internal string ToJsonString()
        {
            var sb = new StringBuilder();
            
            using(var sw = new StringWriter(sb))
            using (JsonWriter jw = new JsonTextWriter(sw))
            {
                jw.WriteStartObject();
                jw.WritePropertyName("props");
                jw.WriteStartObject();
                jw.WriteNullableProperty("n_val", NVal)
                    .WriteNullableProperty("allow_mult", AllowMultiple)
                    .WriteNullableProperty("last_write_wins", LastWriteWins)
                    .WriteNullableProperty("r", _rVal)
                    .WriteNonNullProperty("r", _rValString)
                    .WriteNullableProperty("rw", _rwVal)
                    .WriteNonNullProperty("rw", _rwValString)
                    .WriteNullableProperty("dw", _dwVal)
                    .WriteNonNullProperty("dw", _dwValString)
                    .WriteNullableProperty("w", _wVal)
                    .WriteNonNullProperty("w", _wValString)
                    .WriteNonNullProperty("backend", _wValString);
                jw.WriteEndObject();
                jw.WriteEndObject();
            }
            
            return sb.ToString();
        }
    }
}
