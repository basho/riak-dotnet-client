namespace RiakClient.Commands.CRDT
{
    /// <summary>
    /// Represents options for a <see cref="UpdateMap"/> operation.
    /// </summary>
    /// <inheritdoc />
    public class UpdateMapOptions : UpdateCommandOptions
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateMapOptions"/> class.
        /// </summary>
        /// <inheritdoc />
        public UpdateMapOptions(string bucketType, string bucket, string key)
            : base(bucketType, bucket, key)
        {
        }

        /// <summary>
        /// The <see cref="UpdateMap.MapOperation"/>
        /// </summary>
        /// <value>The <see cref="UpdateMap.MapOperation"/> to be executed by the <see cref="UpdateMap"/> command.</value>
        public UpdateMap.MapOperation Op
        {
            get;
            set;
        }

        protected override bool GetHasRemoves()
        {
            return Op.HasRemoves;
        }
    }
}
