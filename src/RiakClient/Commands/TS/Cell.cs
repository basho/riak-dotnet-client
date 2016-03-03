namespace RiakClient.Commands.TS
{
    using System;
    using Messages;

    public class Cell
    {
        public virtual object AsObject
        {
            get
            {
                throw new NotImplementedException("Must be implemented by Cell subclasses.");
            }
        }

        internal virtual TsCell ToTsCell()
        {
            throw new NotImplementedException("Must be implemented by Cell subclasses.");
        }
    }
}
