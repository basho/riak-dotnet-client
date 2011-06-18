// Copyright (c) 2010 - OJ Reeves & Jeremiah Peschka
//
// This file is provided to you under the Apache License,
// Version 2.0 (the "License"); you may not use this file
// except in compliance with the License.  You may obtain
// a copy of the License at
//
//   http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing,
// software distributed under the License is distributed on an
// "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
// KIND, either express or implied.  See the License for the
// specific language governing permissions and limitations
// under the License.

using CorrugatedIron.Extensions;
using CorrugatedIron.Messages;
using Newtonsoft.Json;

namespace CorrugatedIron.Models
{
    public class RiakLink
    {
        public string Bucket { get; private set; }
        public string Key { get; private set; }
        public string Tag { get; private set; }

        public RiakLink(string bucket, string key, string tag)
        {
            Bucket = bucket;
            Key = key;
            Tag = tag;
        }

        public override bool Equals(object obj)
        {
            if (obj.GetType() != typeof(RiakLink))
                return false;

            var l = (RiakLink) obj;

            return ((l.Bucket == Bucket) && (l.Key == Key) && (l.Tag == Tag));
        }
        
        public static RiakLink FromJsonString(string jsonString)
        {
            var rawLink = JsonConvert.DeserializeObject<string[]>(jsonString);

            return new RiakLink(rawLink[0], rawLink[1], rawLink[2]);
        }

        internal RiakLink(RpbLink link)
        {
            Bucket = link.Bucket.FromRiakString();
            Key = link.Key.FromRiakString();
            Tag = link.Tag.FromRiakString();
        }

        internal RpbLink ToMessage()
        {
            var message = new RpbLink
            {
                Bucket = Bucket.ToRiakString(),
                Key = Key.ToRiakString(),
                Tag = Tag.ToRiakString()
            };

            return message;
        }
    }
}
