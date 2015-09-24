namespace RiakClient
{
    using System;

    /// <summary>
    /// Represents a wall-clock timeout for Riak operations. 
    /// </summary>
    public struct Timeout : IEquatable<Timeout>
    {
        // TODO 3.0 get this from configuration and ensure it matches internal Riak timeouts
        public static readonly Timeout DefaultCommandTimeout = new Timeout(TimeSpan.FromSeconds(60));

        private readonly TimeSpan timeout;

        /// <summary>
        /// Initializes a new instance of the <see cref="Timeout"/> struct.
        /// </summary>
        /// <param name="timeout">The <see cref="TimeSpan"/> to base this <see cref="Timeout"/> off of.</param>
        public Timeout(TimeSpan timeout)
        {
            this.timeout = timeout;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Timeout"/> struct.
        /// </summary>
        /// <param name="milliseconds">The number of milliseconds to base this <see cref="Timeout"/> off of.</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="milliseconds"/> must be greater than or equal to zero.</exception>
        public Timeout(int milliseconds)
        {
            if (milliseconds < 0)
            {
                throw new ArgumentOutOfRangeException("milliseconds must be greater than or equal to zero");
            }

            this.timeout = TimeSpan.FromMilliseconds(milliseconds);
        }

        /// <summary>
        /// Determines whether the specified object is equal to the current object.
        /// </summary>
        /// <param name="a">The first value.</param>
        /// <param name="b">The second value.</param>
        /// <returns><b>true</b> if the values are equal.</returns>
        public static bool operator ==(Timeout a, Timeout b)
        {
            if (object.ReferenceEquals(a, b))
            {
                return true;
            }

            return a.Equals(b);
        }

        /// <summary>
        /// Determines whether the specified object is not equal to the current object.
        /// </summary>
        /// <param name="a">The first value.</param>
        /// <param name="b">The second value.</param>
        /// <returns><b>true</b> if the values are not equal.</returns>
        public static bool operator !=(Timeout a, Timeout b)
        {
            return !(a == b);
        }

        /// <summary>
        /// Cast the value of this <see cref="Int32"/> to a <see cref="Timeout"/>.
        /// </summary>
        /// <param name="timeout">The <see cref="Int32"/> value to cast to a <see cref="Timeout"/>.</param>
        /// <returns>A <see cref="Timeout"/> based on the value of the this <see cref="Int32"/>.</returns>
        public static explicit operator Timeout(int timeout)
        {
            return new Timeout(timeout);
        }

        /// <summary>
        /// Cast the value of this <see cref="Timeout"/> to an <see cref="Int32"/>.
        /// </summary>
        /// <param name="timeout">The <see cref="Timeout"/> value to cast to an <see cref="Int32"/>.</param>
        /// <returns>An <see cref="Int32"/> based on the value of the this <see cref="Timeout"/>.</returns>
        public static explicit operator int(Timeout timeout)
        {
            return (int)timeout.timeout.TotalMilliseconds;
        }

        /// <summary>
        /// Cast the value of this <see cref="TimeSpan"/> to a <see cref="Timeout"/>.
        /// </summary>
        /// <param name="timespan">The <see cref="TimeSpan"/> value to cast to a <see cref="Timeout"/>.</param>
        /// <returns>A <see cref="Timeout"/> based on the value of the this <see cref="TimeSpan"/>.</returns>
        public static implicit operator Timeout(TimeSpan timespan)
        {
            return new Timeout(timespan);
        }

        /// <summary>
        /// Cast the value of this <see cref="Timeout"/> to a <see cref="TimeSpan"/>.
        /// </summary>
        /// <param name="timeout">The <see cref="Timeout"/> value to cast to a <see cref="TimeSpan"/>.</param>
        /// <returns>A <see cref="TimeSpan"/> based on the value of the this <see cref="Timeout"/>.</returns>
        public static implicit operator TimeSpan(Timeout timeout)
        {
            return timeout.timeout;
        }

        /// <summary>
        /// Cast the value of this <see cref="Timeout"/> to a <see cref="String"/>.
        /// </summary>
        /// <param name="timeout">The <see cref="Timeout"/> value to cast to a <see cref="String"/>.</param>
        /// <returns>A <see cref="String"/> based on the value of the this <see cref="Timeout"/>.</returns>
        public static implicit operator string(Timeout timeout)
        {
            return timeout.timeout.TotalMilliseconds.ToString();
        }

        /// <summary>
        /// Cast the value of this <see cref="Timeout"/> to a <see cref="UInt32"/>.
        /// </summary>
        /// <param name="timeout">The <see cref="Timeout"/> value to cast to a <see cref="UInt32"/>.</param>
        /// <returns>A <see cref="UInt32"/> based on the value of the this <see cref="Timeout"/>.</returns>
        [CLSCompliant(false)]
        public static explicit operator uint(Timeout timeout)
        {
            return (uint)timeout.timeout.TotalMilliseconds;
        }

        /// <summary>
        /// Returns a string that represents the total milliseconds of the timeout.
        /// </summary>
        /// <returns>A string that represents the total milliseconds of the timeout.</returns>
        public override string ToString()
        {
            return timeout.TotalMilliseconds.ToString();
        }

        /// <summary>
        /// Determines whether the specified object is equal to the current object.
        /// </summary>
        /// <param name="obj">The object to compare with the current object.</param>
        /// <returns><b>true</b> if the specified object is equal to the current object, otherwise, <b>false</b>.</returns>
        public override bool Equals(object obj)
        {
            if (obj is Timeout)
            {
                return Equals((Timeout)obj);
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Determines whether the specified object is equal to the current object.
        /// </summary>
        /// <param name="other">The object to compare with the current object.</param>
        /// <returns><b>true</b> if the specified object is equal to the current object, otherwise, <b>false</b>.</returns>
        public bool Equals(Timeout other)
        {
            if (object.ReferenceEquals(other, null))
            {
                return false;
            }

            if (object.ReferenceEquals(other, this))
            {
                return true;
            }

            return this.GetHashCode() == other.GetHashCode();
        }

        /// <summary>
        /// Returns a hash code for the current object.
        /// Uses the HashCode of the internal <see cref="TimeSpan"/> value.
        /// </summary>
        /// <returns>A hash code for the current object.</returns>
        public override int GetHashCode()
        {
            return timeout.GetHashCode();
        }
    }
}