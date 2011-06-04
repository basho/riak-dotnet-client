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
        private bool? _allowMultiple;
        private uint? _nVal;
        private bool? _lastWriteWins;

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
                return !_lastWriteWins.HasValue
                    && !_rVal.HasValue
                    && string.IsNullOrEmpty(_rValString)
                    && !_rwVal.HasValue
                    && string.IsNullOrEmpty(_rwValString)
                    && !_wVal.HasValue
                    && string.IsNullOrEmpty(_wValString)
                    && !_dwVal.HasValue
                    && string.IsNullOrEmpty(_dwValString);
            }
        }

        public bool? LastWriteWins
        {
            get { return _lastWriteWins; }
        }

        public uint? NVal
        {
            get { return _nVal; }
        }

        public bool? AllowMultiple
        {
            get { return _allowMultiple; }
        }

        public RiakBucketProperties SetAllowMultiple(bool value)
        {
            _allowMultiple = value;
            return this;
        }

        public RiakBucketProperties SetLastWriteWins(bool value)
        {
            _lastWriteWins = value;
            return this;
        }

        public RiakBucketProperties SetNVal(uint value)
        {
            _nVal = value;
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

        private RiakBucketProperties WriteQuorum(uint value, ref uint? targetVal, ref string targetString)
        {
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
            _allowMultiple = bucketProps.AllowMultiple;
            _nVal = bucketProps.NVal;
        }

        internal RpbBucketProps ToMessage()
        {
            var message = new RpbBucketProps();
            if(_allowMultiple.HasValue)
            {
                message.AllowMultiple = _allowMultiple.Value;
            }
            if (_nVal.HasValue)
            {
                message.NVal = _nVal.Value;
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
                jw.WriteNullableProperty("n_val", _nVal)
                    .WriteNullableProperty("allow_mult", _allowMultiple)
                    .WriteNullableProperty("last_write_wins", _lastWriteWins)
                    .WriteNullableProperty("r", _rVal)
                    .WriteNonNullProperty("r", _rValString)
                    .WriteNullableProperty("rw", _rwVal)
                    .WriteNonNullProperty("rw", _rwValString)
                    .WriteNullableProperty("dw", _dwVal)
                    .WriteNonNullProperty("dw", _dwValString)
                    .WriteNullableProperty("w", _wVal)
                    .WriteNonNullProperty("w", _wValString);
                jw.WriteEndObject();
                jw.WriteEndObject();
            }
            
            return sb.ToString();
        }
    }
}
