// <copyright file="TestObjects.cs" company="Basho Technologies, Inc.">
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

using System;
using System.Collections.Generic;
using System.Linq;

namespace RiakClient.Tests.Json
{
    public class Person
    {
        public Name Name { get; set; }
        public List<PhoneNumber> PhoneNumbers { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Email { get; set; }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }
            if (ReferenceEquals(this, obj))
            {
                return true;
            }
            if (obj.GetType() != typeof(Person))
            {
                return false;
            }
            return Equals((Person)obj);
        }

        public bool Equals(Person other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }
            if (ReferenceEquals(this, other))
            {
                return true;
            }
            return Equals(other.Name, Name) && other.PhoneNumbers.SequenceEqual(PhoneNumbers) && other.DateOfBirth.Equals(DateOfBirth) && Equals(other.Email, Email);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int result = (Name != null ? Name.GetHashCode() : 0);
                result = (result * 397) ^ (PhoneNumbers != null ? PhoneNumbers.GetHashCode() : 0);
                result = (result * 397) ^ DateOfBirth.GetHashCode();
                result = (result * 397) ^ (Email != null ? Email.GetHashCode() : 0);
                return result;
            }
        }
    }

    public class Name
    {
        public string FirstName { get; set; }
        public string Surname { get; set; }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }
            if (ReferenceEquals(this, obj))
            {
                return true;
            }
            if (obj.GetType() != typeof(Name))
            {
                return false;
            }
            return Equals((Name)obj);
        }

        public bool Equals(Name other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }
            if (ReferenceEquals(this, other))
            {
                return true;
            }
            return Equals(other.FirstName, FirstName) && Equals(other.Surname, Surname);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((FirstName != null ? FirstName.GetHashCode() : 0) * 397) ^ (Surname != null ? Surname.GetHashCode() : 0);
            }
        }
    }

    public enum PhoneNumberType
    {
        Mobile = 0,
        Home = 1,
        Work = 2
    }

    public class PhoneNumber
    {
        public string Number { get; set; }
        public PhoneNumberType NumberType { get; set; }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }
            if (ReferenceEquals(this, obj))
            {
                return true;
            }
            if (obj.GetType() != typeof(PhoneNumber))
            {
                return false;
            }
            return Equals((PhoneNumber)obj);
        }

        public bool Equals(PhoneNumber other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }
            if (ReferenceEquals(this, other))
            {
                return true;
            }
            return Equals(other.Number, Number) && Equals(other.NumberType, NumberType);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((Number != null ? Number.GetHashCode() : 0) * 397) ^ NumberType.GetHashCode();
            }
        }
    }
}
