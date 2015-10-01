namespace RiakClient.Models.RiakDt
{
    using System.Collections.ObjectModel;
    using Messages;

    // TODO: Deprecate this?

    /// <summary>
    /// Generic interface for a Riak data type.
    /// </summary>
    /// <typeparam name="T">The type of the data type.</typeparam>
    /// <remarks>Not used.</remarks>
    [System.Obsolete("RiakDt is deprecated. Please use Commands/CRDT namespace.")]
    public interface IRiakDtType<T>
    {
        /// <summary>
        /// The bucket name of the data type object.
        /// </summary>
        string Bucket { get; }

        /// <summary>
        /// The bucket type of the data type object.
        /// </summary>
        string BucketType { get; }

        /// <summary>
        /// The key of the data type object.
        /// </summary>
        string Key { get; }

        /// <summary>
        /// A collection of operations for the data type object.
        /// </summary>
        ReadOnlyCollection<T> Operations { get; }

        /// <summary>
        /// Convert this object to a new <see cref="MapEntry"/>.
        /// </summary>
        /// <param name="fieldName">The field name for the map entry.</param>
        /// <returns>A newly initialized and configured <see cref="MapEntry"/>.</returns>
        MapEntry ToMapEntry(string fieldName);
    }
}
