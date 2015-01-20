﻿// Copyright (c) 2011 - 2014 OJ Reeves & Jeremiah Peschka
// Copyright (c) 2014 - Basho Technologies, Inc.
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

using System.Numerics;
using CorrugatedIron.Models;
using CorrugatedIron.Models.MapReduce.Inputs;
using CorrugatedIron.Tests.Extensions;
using CorrugatedIron.Util;
using NUnit.Framework;

namespace CorrugatedIron.Tests.Models.MapReduce.Inputs
{
    [TestFixture]
    public class RiakIndexStaticTests : MapReduceSerializationTestsBase
    {
        private const string BucketType = "my_bucket_type";
        private const string Bucket = "my_bucket";
        private const string Index = "index";
        private const string BinKey = "dave";
        private const string BinEndKey = "ed";
        private static readonly BigInteger IntKey = 42;
        private static readonly BigInteger IntEndKey = 100;
        private static readonly RiakIndexId IndexId = new RiakIndexId(BucketType, Bucket, Index);

        [Test]
        public void TestIntMatchWorks()
        {
            var indexInput = RiakIndex.Match(IndexId, IntKey);
            indexInput.IndexId.IndexName.EndsWith(RiakConstants.IndexSuffix.Integer).ShouldBeTrue();

            var json = Serialize(indexInput.WriteJson);
            json.Contains(Bucket).ShouldBeTrue();
            json.Contains(BucketType).ShouldBeTrue();
            json.Contains(IntKey.ToString()).ShouldBeTrue();
        }

        [Test]
        public void TestIntRangeWorks()
        {
            var indexInput = RiakIndex.Range(IndexId, IntKey, IntEndKey);
            indexInput.IndexId.IndexName.EndsWith(RiakConstants.IndexSuffix.Integer).ShouldBeTrue();

            var json = Serialize(indexInput.WriteJson);
            json.Contains(Bucket).ShouldBeTrue();
            json.Contains(BucketType).ShouldBeTrue();
            json.Contains(IntKey.ToString()).ShouldBeTrue();
            json.Contains(IntEndKey.ToString()).ShouldBeTrue();
        }

        [Test]
        public void TestBinMatchWorks()
        {
            var indexInput = RiakIndex.Match(IndexId, BinKey);
            indexInput.IndexId.IndexName.EndsWith(RiakConstants.IndexSuffix.Binary).ShouldBeTrue();

            var json = Serialize(indexInput.WriteJson);
            json.Contains(Bucket).ShouldBeTrue();
            json.Contains(BucketType).ShouldBeTrue();
            json.Contains(BinKey).ShouldBeTrue();
        }

        [Test]
        public void TestBinRangeWorks()
        {
            var indexInput = RiakIndex.Range(IndexId, BinKey, BinEndKey);
            indexInput.IndexId.IndexName.EndsWith(RiakConstants.IndexSuffix.Binary).ShouldBeTrue();

            var json = Serialize(indexInput.WriteJson);
            json.Contains(Bucket).ShouldBeTrue();
            json.Contains(BucketType).ShouldBeTrue();
            json.Contains(BinEndKey).ShouldBeTrue();
            json.Contains(BinEndKey).ShouldBeTrue();
        }
#pragma warning disable 612, 618

        [Test]
        public void TestIntMatchOldInterfaceWorks()
        {
            var indexInput = RiakIndex.Match(Bucket, Index, IntKey);
            indexInput.IndexId.IndexName.EndsWith(RiakConstants.IndexSuffix.Integer).ShouldBeTrue();

            var json = Serialize(indexInput.WriteJson);
            json.Contains(Bucket).ShouldBeTrue();
            json.Contains(IntKey.ToString()).ShouldBeTrue();
        }

        [Test]
        public void TestIntRangeOldInterfaceWorks()
        {
            var indexInput = RiakIndex.Range(Bucket, Index, IntKey, IntEndKey);
            indexInput.IndexId.IndexName.EndsWith(RiakConstants.IndexSuffix.Integer).ShouldBeTrue();

            var json = Serialize(indexInput.WriteJson);
            json.Contains(Bucket).ShouldBeTrue();
            json.Contains(IntKey.ToString()).ShouldBeTrue();
            json.Contains(IntEndKey.ToString()).ShouldBeTrue();
        }


        [Test]
        public void TestBinMatchOldInterfaceWorks()
        {
            var indexInput = RiakIndex.Match(Bucket, Index, BinKey);
            indexInput.IndexId.IndexName.EndsWith(RiakConstants.IndexSuffix.Binary).ShouldBeTrue();

            var json = Serialize(indexInput.WriteJson);
            json.Contains(Bucket).ShouldBeTrue();
            json.Contains(BinKey).ShouldBeTrue();
        }

        [Test]
        public void TestBinRangeOldInterfaceWorks()
        {
            var indexInput = RiakIndex.Range(Bucket, Index, BinKey, BinEndKey);
            indexInput.IndexId.IndexName.EndsWith(RiakConstants.IndexSuffix.Binary).ShouldBeTrue();

            var json = Serialize(indexInput.WriteJson);
            json.Contains(Bucket).ShouldBeTrue();
            json.Contains(BinEndKey).ShouldBeTrue();
            json.Contains(BinEndKey).ShouldBeTrue();
        }
#pragma warning restore 612, 618
    }
}