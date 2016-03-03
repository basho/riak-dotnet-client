namespace RiakClient.Commands.TS
{
    using System;

    /// <summary>
    /// Base class for all Riak TS command options.
    /// </summary>
    public abstract class TimeseriesCommandOptions : CommandOptions, IEquatable<TimeseriesCommandOptions>
    {
        private readonly RiakString table;

        /// <summary>
        /// Initializes a new instance of the <see cref="TimeseriesCommandOptions"/> class.
        /// </summary>
        /// <param name="table">The table in Riak TS. Required.</param>
        public TimeseriesCommandOptions(string table)
            : this(table, CommandDefaults.Timeout)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TimeseriesCommandOptions"/> class.
        /// </summary>
        /// <param name="table">The table in Riak TS. Required.</param>
        /// <param name="timeout">The command timeout in Riak. Default is <b>60 seconds</b></param>
        public TimeseriesCommandOptions(string table, Timeout timeout)
            : base(timeout)
        {
            if (string.IsNullOrEmpty(table))
            {
                throw new ArgumentNullException("table");
            }
            else
            {
                this.table = table;
            }
        }

        /// <summary>
        /// The table in Riak TS.
        /// </summary>
        /// <value>A <see cref="RiakString"/> representing the table.</value>
        public RiakString Table
        {
            get { return table; }
        }

        public bool Equals(TimeseriesCommandOptions other)
        {
            if (object.ReferenceEquals(other, null))
            {
                return false;
            }

            if (object.ReferenceEquals(this, other))
            {
                return true;
            }

            return this.GetHashCode() == other.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as TimeseriesCommandOptions);
        }

        /// <summary>
        /// Returns a hash code for the current object.
        /// Uses a combination of the public properties to generate a unique hash code.
        /// </summary>
        /// <returns>A hash code for the current object.</returns>
        public override int GetHashCode()
        {
            unchecked
            {
                int result = table.GetHashCode();
                result = (result * 397) ^ base.GetHashCode();
                return result;
            }
        }
    }
}
