// Copyright (c) 2011 - OJ Reeves & Jeremiah Peschka
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

using System;
using Newtonsoft.Json;
using CorrugatedIron.Extensions;

namespace CorrugatedIron.Models
{
    [JsonConverter(typeof(RiakObjectIdConverter))]
    public class RiakObjectId
    {
        public string Bucket { get; set; }
        public string Key { get; set; }

        public RiakObjectId()
        {
            
        }
        
        public RiakObjectId(string [] objectId)
        {
            Bucket = objectId[0];
            Key = objectId[1];
        }
  
        
        public RiakObjectId(string bucket, string key)
        {
            Bucket = bucket;
            Key = key;
        }

        internal RiakLink ToRiakLink(string tag)
        {
            return new RiakLink(Bucket, Key, tag);
        }
    }
    
    public class RiakObjectIdConverter : JsonConverter
    {
        public override object ReadJson (JsonReader reader, Type objectType, Object existingValue, JsonSerializer serializer)
        {
            int pos = 0;
            string[] objectIdParts = new string[2];
            
            while (reader.Read())
            {
                if (pos < 1)
                {
                    if (reader.TokenType == JsonToken.String)
                    {
                        objectIdParts[pos] = reader.Value.ToString();
                        pos++;
                    }
                }
                // read until the end of the JsonReader
            }
            
            return new RiakObjectId(objectIdParts);
        }
        
        public override void WriteJson (JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException ();
        }
        
        public override bool CanWrite {
            get {
                return base.CanWrite;
            }
        }
        
        public override bool CanRead { get { return true; } }
        public override bool CanConvert (Type objectType) 
        {
            return true;
        }
    }
}
