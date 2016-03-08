namespace RiakClient.Commands
{
    using System;

    /// <summary>
    /// Base class for all Riak command options.
    /// </summary>
    public abstract class CommandOptions : IEquatable<CommandOptions>
    {
        protected Timeout? commandTimeout;

        /// <summary>
        /// The timeout for this command.
        /// </summary>
        public Timeout? Timeout
        {
            get { return commandTimeout; }
            set { commandTimeout = value; }
        }

        public bool Equals(CommandOptions other)
        {
            if (ReferenceEquals(other, null))
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return GetHashCode() == other.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as CommandOptions);
        }

        /// <summary>
        /// Returns a hash code for the current object.
        /// Uses a combination of the public properties to generate a unique hash code.
        /// </summary>
        /// <returns>A hash code for the current object.</returns>
        public override int GetHashCode()
        {
            int result = base.GetHashCode();

            if (commandTimeout.HasValue)
            {
                result = (result * 397) ^ commandTimeout.GetHashCode();
            }

            return result;
        }
    }
}
