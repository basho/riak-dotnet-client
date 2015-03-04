// <copyright file="RiakFluentKeyFilter.cs" company="Basho Technologies, Inc.">
// Copyright (c) 2011 - OJ Reeves & Jeremiah Peschka
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
// </copyright>

namespace RiakClient.Models.MapReduce.Fluent
{
    using System;
    using System.Collections.Generic;
    using Models.MapReduce.KeyFilters;
    
    /// <summary>
    /// A fluent builder class for defining input key filters.
    /// Please see http://docs.basho.com/riak/latest/dev/using/keyfilters/ and
    /// http://docs.basho.com/riak/latest/dev/references/keyfilters/ for more information
    /// on using Key Filters.
    /// </summary>
    [Obsolete("Key Filters are a deprecated feature of Riak and will eventually be removed.")]
    public class RiakFluentKeyFilter
    {
        private readonly List<IRiakKeyFilterToken> filters;

        internal RiakFluentKeyFilter(List<IRiakKeyFilterToken> filters)
        {
            this.filters = filters;
        }

        /// <summary>
        /// Joins two key-filter operations with a logical AND operation.
        /// </summary>
        /// <param name="left">The left <see cref="RiakFluentKeyFilter"/> operand to AND together.</param>
        /// <param name="right">The right <see cref="RiakFluentKeyFilter"/> operand to AND together.</param>
        /// <returns>A reference to this updated instance, for fluent chaining.</returns>
        public RiakFluentKeyFilter And(Action<RiakFluentKeyFilter> left, Action<RiakFluentKeyFilter> right)
        {
            var leftFilters = new List<IRiakKeyFilterToken>();
            var leftFluent = new RiakFluentKeyFilter(leftFilters);
            left(leftFluent);

            var rightFilters = new List<IRiakKeyFilterToken>();
            var rightFluent = new RiakFluentKeyFilter(rightFilters);
            right(rightFluent);

            filters.Add(new And(leftFilters, rightFilters));

            return this;
        }

        /// <summary>
        /// Tests that the input is between the <paramref name="first"/> and <paramref name="second"/> parameters. 
        /// </summary>
        /// <typeparam name="T">
        /// The type of the <paramref name="first"/> and <paramref name="second"/> objects.
        /// </typeparam>
        /// <param name="first">The lower boundary object to compare the keys against.</param>
        /// <param name="second">The upper boundary object to compare the keys against.</param>
        /// <param name="inclusive">
        /// If <b>true</b>, the query will include the parameters in the range.
        /// If <b>false</b>, it will exclude them from the range.
        /// </param>
        /// <returns>A reference to this updated instance, for fluent chaining.</returns>
        /// <remarks>
        /// This function follows Erlang's precedence rules for comparisons. 
        /// Generally this means that numbers will be compared by value (including appropriate coercions) and strings will be compared lexically.
        /// </remarks>
        public RiakFluentKeyFilter Between<T>(T first, T second, bool inclusive)
        {
            filters.Add(new Between<T>(first, second, inclusive));
            return this;
        }

        /// <summary>
        /// Tests that the input ends with the <paramref name="arg"/> parameter (a string).
        /// </summary>
        /// <param name="arg">The string to compare against the end of the key.</param>
        /// <returns>A reference to this updated instance, for fluent chaining.</returns>
        public RiakFluentKeyFilter EndsWith(string arg)
        {
            filters.Add(new EndsWith(arg));
            return this;
        }

        /// <summary>
        /// Tests that the input is equal to the argument.
        /// </summary>
        /// <typeparam name="T">The type of the <paramref name="arg"/> object.</typeparam>
        /// <param name="arg">The other object to compare the key against.</param>
        /// <returns>A reference to this updated instance, for fluent chaining.</returns>
        public RiakFluentKeyFilter Equal<T>(T arg)
        {
            filters.Add(new Equal<T>(arg));
            return this;
        }

        /// <summary>
        /// Turns a floating point number (previously extracted with <see cref="StringToFloat"/>), into a string.
        /// </summary>
        /// <returns>A reference to this updated instance, for fluent chaining.</returns>
        public RiakFluentKeyFilter FloatToString()
        {
            filters.Add(new FloatToString());
            return this;
        }

        /// <summary>
        /// Tests that the input is greater than the <paramref name="arg"/> parameter.
        /// </summary>
        /// <typeparam name="T">The type of the <paramref name="arg"/> object.</typeparam>
        /// <param name="arg">The string or number to compare the key against.</param>
        /// <returns>A reference to this updated instance, for fluent chaining.</returns>
        /// <remarks>
        /// This function follows Erlang's precedence rules for comparisons. 
        /// Generally this means that numbers will be compared by value (including appropriate coercions) and strings will be compared lexically.
        /// </remarks>
        public RiakFluentKeyFilter GreaterThan<T>(T arg)
        {
            filters.Add(new GreaterThan<T>(arg));
            return this;
        }

        /// <summary>
        /// Tests that the input is greater than or equal to the <paramref name="arg"/> parameter.
        /// </summary>
        /// <typeparam name="T">The type of the <paramref name="arg"/> object.</typeparam>
        /// <param name="arg">The string or number to compare the key against.</param>
        /// <returns>A reference to this updated instance, for fluent chaining.</returns>
        /// <remarks>
        /// This function follows Erlang's precedence rules for comparisons. 
        /// Generally this means that numbers will be compared by value (including appropriate coercions) and strings will be compared lexically.
        /// </remarks>
        public RiakFluentKeyFilter GreaterThanOrEqualTo<T>(T arg)
        {
            filters.Add(new GreaterThanOrEqualTo<T>(arg));
            return this;
        }

        /// <summary>
        /// Turns an integer (previously extracted with <see cref="StringToInt"/>), into a string.
        /// </summary>
        /// <returns>A reference to this updated instance, for fluent chaining.</returns>
        public RiakFluentKeyFilter IntToString()
        {
            filters.Add(new IntToString());
            return this;
        }

        /// <summary>
        /// Tests that the input is less than the <paramref name="arg"/> parameter.
        /// </summary>
        /// <typeparam name="T">The type of the <paramref name="arg"/> object.</typeparam>
        /// <param name="arg">The string or number to compare the key against.</param>
        /// <returns>A reference to this updated instance, for fluent chaining.</returns>
        /// <remarks>
        /// This function follows Erlang's precedence rules for comparisons. 
        /// Generally this means that numbers will be compared by value (including appropriate coercions) and strings will be compared lexically.
        /// </remarks>
        public RiakFluentKeyFilter LessThan<T>(T arg)
        {
            filters.Add(new LessThan<T>(arg));
            return this;
        }

        /// <summary>
        /// Tests that the input is less than or equal to the <paramref name="arg"/> parameter.
        /// </summary>
        /// <typeparam name="T">The type of the <paramref name="arg"/> object.</typeparam>
        /// <param name="arg">The string or number to compare the key against.</param>
        /// <returns>A reference to this updated instance, for fluent chaining.</returns>
        /// <remarks>
        /// This function follows Erlang's precedence rules for comparisons. 
        /// Generally this means that numbers will be compared by value (including appropriate coercions) and strings will be compared lexically.
        /// </remarks>
        public RiakFluentKeyFilter LessThanOrEqualTo<T>(T arg)
        {
            filters.Add(new LessThanOrEqualTo<T>(arg));
            return this;
        }

        /// <summary>
        /// Tests that the input matches the regular expression given in the <paramref name="regex"/> parameter.
        /// </summary>
        /// <param name="regex">The regular expression string to match the keys against.</param>
        /// <returns>A reference to this updated instance, for fluent chaining.</returns>
        public RiakFluentKeyFilter Matches(string regex)
        {
            filters.Add(new Matches(regex));
            return this;
        }

        /// <summary>
        /// Negates the result of key-filter operations.
        /// </summary>
        /// <param name="setup">Setup action
        /// An <see cref="Action{T}"/> that accepts a <see cref="RiakFluentKeyFilter"/>,
        /// which the <see cref="Not"/> function will logically negate it's output. 
        /// </param>
        /// <returns>A reference to this updated instance, for fluent chaining.</returns>
        /// <remarks>
        /// Configure the phase with a lambda similar to:
        /// <code>.Not(kf => kf.EndsWith("foo"))</code>
        /// The above filter will return all input keys that don't end in "foo".
        /// </remarks>
        public RiakFluentKeyFilter Not(Action<RiakFluentKeyFilter> setup)
        {
            var filters = new List<IRiakKeyFilterToken>();
            var fluent = new RiakFluentKeyFilter(filters);
            setup(fluent);
            filters.Add(new Not(filters));

            return this;
        }

        /// <summary>
        /// Tests that the input is not equal to the argument.
        /// </summary>
        /// <typeparam name="T">The type of the <paramref name="arg"/> object.</typeparam>
        /// <param name="arg">The other object to compare the key against.</param>
        /// <returns>A reference to this updated instance, for fluent chaining.</returns>
        public RiakFluentKeyFilter NotEqual<T>(T arg)
        {
            filters.Add(new NotEqual<T>(arg));
            return this;
        }

        /// <summary>
        /// Joins two or more key-filter operations with a logical OR operation.
        /// </summary>
        /// <param name="left">The left <see cref="RiakFluentKeyFilter"/> operand to OR together.</param>
        /// <param name="right">The right <see cref="RiakFluentKeyFilter"/> operand to OR together.</param>
        /// <returns>A reference to this updated instance, for fluent chaining.</returns>
        public RiakFluentKeyFilter Or(Action<RiakFluentKeyFilter> left, Action<RiakFluentKeyFilter> right)
        {
            var leftFilters = new List<IRiakKeyFilterToken>();
            var leftFluent = new RiakFluentKeyFilter(leftFilters);
            left(leftFluent);

            var rightFilters = new List<IRiakKeyFilterToken>();
            var rightFluent = new RiakFluentKeyFilter(rightFilters);
            right(rightFluent);

            filters.Add(new Or(leftFilters, rightFilters));

            return this;
        }

        /// <summary>
        /// Tests that the input is contained in the set given as the arguments.
        /// </summary>
        /// <typeparam name="T">The type of the <paramref name="arg"/> object.</typeparam>
        /// <param name="arg">The set of members to test the keys for membership against.</param>
        /// <returns>A reference to this updated instance, for fluent chaining.</returns>
        public RiakFluentKeyFilter SetMember<T>(List<T> arg)
        {
            filters.Add(new SetMember<T>(arg));
            return this;
        }

        /// <summary>
        /// Tests that input is within the Levenshtein distance of the first argument given by the <paramref name="distance"/> argument.
        /// </summary>
        /// <typeparam name="T">The type of the <paramref name="arg"/> object.</typeparam>
        /// <param name="arg">The string to calculate the distance of the key against.</param>
        /// <param name="distance">The maximum Levenshtein distance to accept.</param>
        /// <returns>A reference to this updated instance, for fluent chaining.</returns>
        /// <remarks>See http://en.wikipedia.org/wiki/Levenshtein_distance for more information.</remarks>
        public RiakFluentKeyFilter SimilarTo<T>(T arg, int distance)
        {
            filters.Add(new SimilarTo<T>(arg, distance));
            return this;
        }

        /// <summary>
        /// Tests that the input begins with the <paramref name="arg"/> string.
        /// </summary>
        /// <param name="arg">The string to compare against the beginning of the key.</param>
        /// <returns>A reference to this updated instance, for fluent chaining.</returns>
        public RiakFluentKeyFilter StartsWith(string arg)
        {
            filters.Add(new StartsWith(arg));
            return this;
        }

        /// <summary>
        /// Turns a string into a floating point number.
        /// </summary>
        /// <returns>A reference to this updated instance, for fluent chaining.</returns>
        public RiakFluentKeyFilter StringToFloat()
        {
            filters.Add(new StringToFloat());
            return this;
        }

        /// <summary>
        /// Turns a string into an integer.
        /// </summary>
        /// <returns>A reference to this updated instance, for fluent chaining.</returns>
        public RiakFluentKeyFilter StringToInt()
        {
            filters.Add(new StringToInt());
            return this;
        }

        /// <summary>
        /// Splits the input on the string given as the <paramref name="token"/> argument and returns the 
        /// nth token specified by the <paramref name="position"/> argument.
        /// </summary>
        /// <param name="token">The token to split the string with.</param>
        /// <param name="position">The n-th token to return from the filter.</param>
        /// <returns>A reference to this updated instance, for fluent chaining.</returns>
        public RiakFluentKeyFilter Tokenize(string token, int position)
        {
            filters.Add(new Tokenize(token, position));
            return this;
        }

        /// <summary>
        /// Changes all letters to lowercase.
        /// </summary>
        /// <returns>A reference to this updated instance, for fluent chaining.</returns>
        public RiakFluentKeyFilter ToLower()
        {
            filters.Add(new ToLower());
            return this;
        }

        /// <summary>
        /// Changes all letters to uppercase.
        /// </summary>
        /// <returns>A reference to this updated instance, for fluent chaining.</returns>
        public RiakFluentKeyFilter ToUpper()
        {
            filters.Add(new ToUpper());
            return this;
        }

        /// <summary>
        /// URL-decodes the string.
        /// </summary>
        /// <returns>A reference to this updated instance, for fluent chaining.</returns>
        public RiakFluentKeyFilter UrlDecode()
        {
            filters.Add(new UrlDecode());
            return this;
        }
    }
}
