// <copyright file="EntityManager.cs" company="Basho Technologies, Inc.">
// Copyright 2015 - Basho Technologies, Inc.
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

namespace RiakClientExamples.Dev.DataModeling
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics;
    using RiakClient;

    public class EntityManager
    {
        private static readonly char[] interestsEventChars = new char[] { ':' };

        private readonly IRiakClient client;
        private readonly IList<INotifyPropertyChanged> models = new List<INotifyPropertyChanged>();

        public EntityManager(IRiakClient client)
        {
            this.client = client;
        }

        public void Add(IModel model)
        {
            var inpc = model as INotifyPropertyChanged;
            if (inpc != null)
            {
                inpc.PropertyChanged += HandlePropertyChanged;
                models.Add(inpc);
            }
        }

        private void HandlePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var user = sender as User;
            if (user != null)
            {
                var repository = new UserRepository(client);
                if (e.PropertyName == "PageVisits")
                {
                    repository.IncrementPageVisits(user);
                }
                else if (e.PropertyName.StartsWith("Interests:"))
                {
                    var op = e.PropertyName.Split(interestsEventChars);
                    Debug.Assert(op[0] == "Interests");
                    switch (op[1])
                    {
                        case "Added":
                            repository.AddInterest(user, op[2]);
                            break;
                        case "Removed":
                            repository.RemoveInterest(user, op[2]);
                            break;
                        default:
                            throw new InvalidOperationException(
                                string.Format("Unexpected Interests event action: {0}", op[1]));
                    }
                }
            }
        }
    }
}
