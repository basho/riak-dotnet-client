namespace RiakClient.Commands
{
    using System;

    /// <summary>
    /// Base class for all Riak command options.
    /// </summary>
    public abstract class CommandOptions : IEquatable<CommandOptions>
    {
        private Timeout timeout = CommandDefaults.Timeout;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandOptions"/> class.
        /// </summary>
        public CommandOptions()
            : this(CommandDefaults.Timeout)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandOptions"/> class.
        /// </summary>
        /// <param name="timeout">The command timeout in Riak. Default is <b>60 seconds</b></param>
        public CommandOptions(Timeout timeout)
        {
            this.timeout = timeout;
        }

        /// <summary>
        /// The timeout for this command.
        /// </summary>
        public Timeout Timeout
        {
            get { return timeout; }
            set { timeout = value; }
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
            return timeout.GetHashCode();
        }
    }
}
