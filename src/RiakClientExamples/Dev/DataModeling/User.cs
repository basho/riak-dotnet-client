namespace RiakClientExamples.Dev.DataModeling
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;

    public class User : IModel, INotifyPropertyChanged
    {
        private readonly string firstName;
        private readonly string lastName;
        private readonly ICollection<string> interests;
        private uint pageVisits = 0;
        private bool accountStatus = false;

        public User(
            string firstName,
            string lastName,
            ICollection<string> interests,
            uint pageVisits = 0,
            bool accountStatus = false)
        {
            if (string.IsNullOrWhiteSpace(firstName))
            {
                throw new ArgumentNullException("firstName", "firstName is required");
            }

            this.firstName = firstName;

            if (string.IsNullOrWhiteSpace(lastName))
            {
                throw new ArgumentNullException("lastName", "lastName is required");
            }

            this.lastName = lastName;
            this.interests = interests;
            this.pageVisits = pageVisits;
            this.accountStatus = accountStatus;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public string ID
        {
            get
            {
                return string.Format("{0}_{1}",
                    firstName.ToLowerInvariant(), lastName.ToLowerInvariant());
            }
        }

        public string FirstName
        {
            get { return firstName; }
        }

        public string LastName
        {
            get { return lastName; }
        }

        public IEnumerable<string> Interests
        {
            get { return interests; }
        }

        public void AddInterest(string interest)
        {
            if (!interests.Contains(interest))
            {
                interests.Add(interest);
                /*
                 * Real-world you would be using your own entity changed
                 * event interface that allows custom events to be
                 * raised instead of using formatted strings
                 */
                var data = string.Format("Interests:Added:{0}", interest);
                var e = new PropertyChangedEventArgs(data);
                PropertyChanged(this, e);
            }
        }

        public void RemoveInterest(string interest)
        {
            if (interests.Contains(interest))
            {
                interests.Remove(interest);
                var data = string.Format("Interests:Removed:{0}", interest);
                var e = new PropertyChangedEventArgs(data);
                PropertyChanged(this, e);
            }
        }

        public uint PageVisits
        {
            get { return pageVisits; }
        }

        public void VisitPage()
        {
            ++pageVisits;
            var e = new PropertyChangedEventArgs("PageVisits");
            PropertyChanged(this, e);
        }

        public void SetAccountStatus(bool accountStatus)
        {
            this.accountStatus = accountStatus;
            var data = string.Format("AccountStatus:{0}", this.accountStatus.ToString());
            var e = new PropertyChangedEventArgs(data);
            PropertyChanged(this, e);
        }

        public bool AccountStatus
        {
            get { return accountStatus; }
        }
    }
}