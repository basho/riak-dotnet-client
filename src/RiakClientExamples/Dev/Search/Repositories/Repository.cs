// <copyright file="Repository.cs" company="Basho Technologies, Inc.">
// Copyright (c) 2015 - Basho Technologies, Inc.
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
// </copyright>

namespace RiakClientExamples.Dev.Search.Repositories
{
    using System;
    using Models;
    using RiakClient;
    using RiakClient.Models;

    public abstract class Repository<TModel> : IRepository<TModel> where TModel : IModel
    {
        protected IRiakClient client;

        public Repository(IRiakClient client)
        {
            if (client == null)
            {
                throw new ArgumentNullException("client");
            }
            this.client = client;
        }

        public virtual TModel Get(string key, bool notFoundOK = false)
        {
            var riakObjectId = new RiakObjectId(BucketName, key);
            RiakResult<RiakObject> result = client.Get(riakObjectId);
            CheckResult(result, notFoundOK);
            RiakObject value = result.Value;
            if (notFoundOK && value == null)
            {
                return default(TModel);
            }
            else
            {
                return value.GetObject<TModel>();
            }
        }

        public virtual string Save(TModel model)
        {
            var riakObjectId = new RiakObjectId(BucketName, model.ID);
            var riakObject = new RiakObject(riakObjectId, model);
            RiakResult<RiakObject> result = client.Put(riakObject);
            CheckResult(result);
            RiakObject value = result.Value;
            return value.Key;
        }

        protected virtual string BucketName
        {
            get { return string.Empty; }
        }

        protected void CheckResult(RiakResult result, bool notFoundOK = false)
        {
            if (!result.IsSuccess)
            {
                if (notFoundOK && result.ResultCode == ResultCode.NotFound)
                {
                    // No-op since not_found response is OK
                }
                else
                {
                    throw new ApplicationException(string.Format("Riak failure: {0}", result.ErrorMessage));
                }
            }
        }
    }
}
