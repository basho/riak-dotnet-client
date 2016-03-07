namespace RiakClient.Commands.TS
{
    using System;
    using Messages;

    public class Cell
    {
        private readonly object value;

        protected Cell(object value)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }

            this.value = value;
        }

        public object AsObject
        {
            get { return value; }
        }

        internal virtual TsCell ToTsCell()
        {
            throw new NotImplementedException("Must be implemented by Cell subclasses.");
        }
    }
}
