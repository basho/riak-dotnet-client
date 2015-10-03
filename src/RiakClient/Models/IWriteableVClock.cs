namespace RiakClient.Models
{
    /// <summary>
    /// <para>Implements a writeable vector clock interface. Callers must explicitly use the
    /// IWriteableVClock interface to set the vector clock value. This is by design and
    /// is implemented in an attempt to prevent developers new to Riak from causing themselves
    /// more pain. This trade off should present developers with a reliable way to explicitly
    /// drop down to mucking about with vector clocks - it becomes apparent to a casual 
    /// observer that something out of the ordinary is happening.</para>
    /// <para>A better understanding of the usefulness of vector clocks can be found in 
    /// John Daily's Understanding Riak’s Configurable Behaviors: Part 2
    /// (http://basho.com/riaks-config-behaviors-part-2/).
    /// </para>
    /// </summary>
    public interface IWriteableVClock
    {
        /// <summary>
        /// Sets the VClock.
        /// </summary>
        /// <param name="vclock">The value to set the VClock to.</param>
        void SetVClock(byte[] vclock);
    }
}
