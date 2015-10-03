namespace RiakClientExamples.Dev.DataModeling
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics;
    using RiakClient;

    public class EntityManager
    {
        private static readonly char[] EventDataChars = new char[] { ':' };
        private static readonly string TrueStr = true.ToString();
        private static readonly string FalseStr = false.ToString();

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
                    var op = e.PropertyName.Split(EventDataChars);
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
                else if (e.PropertyName.StartsWith("AccountStatus:"))
                {
                    var op = e.PropertyName.Split(EventDataChars);
                    Debug.Assert(op[0] == "AccountStatus");
                    if (op[1] == TrueStr)
                    {
                        repository.UpgradeAccount(user);
                    }
                    else if (op[1] == FalseStr)
                    {
                        repository.DowngradeAccount(user);
                    }
                    else
                    {
                        throw new InvalidOperationException(
                            string.Format("Unexpected AccountStatus event action: {0}", op[1]));
                    }
                }
            }
        }
    }
}