namespace Riak.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;

    internal class StateManager : IDisposable
    {
        private static readonly Type ByteType = typeof(byte);

        private readonly ReaderWriterLockSlim sync;
        private readonly IDictionary<byte, string> states;

        private bool disposed = false;
        private byte state;

        private StateManager(IDictionary<byte, string> states, byte state, ReaderWriterLockSlim sync = null)
        {
            this.sync = sync;

            if (this.sync == null)
            {
                this.sync = new ReaderWriterLockSlim();
            }

            if (states == null)
            {
                throw new ArgumentNullException("states");
            }

            if (states.Count == 0)
            {
                throw new ArgumentException("states", Properties.Resources.Riak_Core_StateManagerRequiresAtLeastOneState);
            }

            this.states = states;
            this.state = state;
        }

        public byte State
        {
            get
            {
                sync.EnterReadLock();
                try
                {
                    return state;
                }
                finally
                {
                    sync.ExitReadLock();
                }
            }

            set
            {
                sync.EnterWriteLock();
                try
                {
                    state = value;
                }
                finally
                {
                    sync.ExitWriteLock();
                }
            }
        }

        public static StateManager FromEnum<T>(ReaderWriterLockSlim sync = null) where T : struct, IConvertible
        {
            Type enumType = typeof(T);

            if (!enumType.IsEnum)
            {
                throw new ArgumentException(Properties.Resources.Riak_Core_StateManagerMustUseEnum);
            }

            if (ByteType != Enum.GetUnderlyingType(enumType))
            {
                throw new ArgumentException(Properties.Resources.Riak_Core_StateManagerMustUseByteEnum);
            }

            string[] names = Enum.GetNames(enumType);
            byte[] values = Enum.GetValues(enumType).Cast<byte>().ToArray();
            var dict = new Dictionary<byte, string>();

            for (ushort i = 0; i < names.Length; i++)
            {
                dict[values[i]] = names[i];
            }

            return new StateManager(dict, values.First(), sync);
        }

        public bool IsCurrentState(params byte[] states)
        {
            sync.EnterReadLock();
            try
            {
                return states.Contains(state);
            }
            finally
            {
                sync.ExitReadLock();
            }
        }

        public bool IsStateLessThan(byte state)
        {
            sync.EnterReadLock();
            try
            {
                return this.state < state;
            }
            finally
            {
                sync.ExitReadLock();
            }
        }

        public void StateCheck(byte state)
        {
            sync.EnterReadLock();
            try
            {
                if (this.state != state)
                {
                    var message = string.Format(
                        Properties.Resources.Riak_Core_StateManagerStateCheckExpectedStateButGot_fmt,
                        states[state],
                        states[this.state]);
                    throw new InvalidOperationException(message);
                }
            }
            finally
            {
                sync.ExitReadLock();
            }
        }

        public void Dispose()
        {
            if (!disposed)
            {
                sync.Dispose();
                disposed = true;
            }
        }

        public override string ToString()
        {
            return states[State];
        }
    }
}
