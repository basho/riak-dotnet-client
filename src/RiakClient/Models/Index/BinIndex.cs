namespace RiakClient.Models.Index
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.InteropServices;

    /// <summary>
    /// A binary secondary index for a <see cref="RiakObject"/>.
    /// </summary>
    public class BinIndex : SecondaryIndex<BinIndex, string>
    {
        internal BinIndex(RiakObject container, string name)
            : base(container, name)
        {
        }

        protected override BinIndex TypedThis
        {
            get { return this; }
        }

        protected override string IndexSuffix
        {
            get { return RiakConstants.IndexSuffix.Binary; }
        }

        /// <summary>
        /// Delete this index from it's parent <see cref="RiakObject"/>.
        /// </summary>
        /// <returns>
        /// A reference to the updated parent <see cref="RiakObject"/>.
        /// </returns>
        public RiakObject Delete()
        {
            Container.BinIndexes.Remove(Name);
            return Container;
        }

        /// <summary>
        /// Add the <see cref="IEnumerable{TIndex}"/> collection of term values to the index.
        /// </summary>
        /// <param name="values">An <see cref="IEnumerable{TIndex}"/> of new terms to add.</param>
        /// <returns>A reference to this updated instance, for fluent chaining.</returns>
        public override BinIndex Add(IEnumerable<string> values)
        {
            return base.Add(values.Where(value => !string.IsNullOrEmpty(value)).ToArray());
        }

        /// <summary>
        /// Adds the params array collection of term values to the index.
        /// </summary>
        /// <param name="values">A params array of term values to add.</param>
        /// <returns>A reference to this updated instance, for fluent chaining.</returns>
        public override BinIndex Add(params string[] values)
        {
            return base.Add(values.Where(value => !string.IsNullOrEmpty(value)).ToArray());
        }
    }
}
