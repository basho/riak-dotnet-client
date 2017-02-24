namespace RiakClient.Commands.CRDT
{
    using System;
    using System.Collections.Generic;
    using Messages;
    using Util;

    /// <inheritdoc />
    public abstract class UpdateSetBase : UpdateCommand<SetResponse>
    {
        /// <inheritdoc />
        public UpdateSetBase(UpdateCommandOptions options)
            : base(options)
        {
        }

        protected override SetResponse CreateResponse(DtUpdateResp response)
        {
            RiakString key = GetKey(CommandOptions.Key, response);

            if (EnumerableUtil.NotNullOrEmpty(response.set_value))
            {
                return new SetResponse(key, response.context, new HashSet<byte[]>(response.set_value));
            }

            if (EnumerableUtil.NotNullOrEmpty(response.gset_value))
            {
                return new SetResponse(key, response.context, new HashSet<byte[]>(response.gset_value));
            }

            throw new InvalidOperationException("DtUpdateResp should have a value at this point!");
        }
    }
}