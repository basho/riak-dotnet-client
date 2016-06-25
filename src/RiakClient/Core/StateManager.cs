namespace Riak.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;

    internal class StateManager : IDisposable
    {
        private static readonly Type ByteType = typeof(byte);

        private readonly string ownerName;
        private readonly ReaderWriterLockSlim sync;
        private readonly IDictionary<byte, string> states;

        private bool disposed = false;
        private byte state;
        private byte finalState;

        private StateManager(object owner, IDictionary<byte, string> states, byte state, ReaderWriterLockSlim sync = null)
        {
            if (owner == null)
            {
                throw new ArgumentNullException("owner");
            }

            ownerName = string.Format("{0}|{1}", owner.GetType().Name, owner.ToString());

            this.states = states;
            if (this.states == null)
            {
                throw new ArgumentNullException("states");
            }

            if (this.states.Count == 0)
            {
                throw new ArgumentException("states", Properties.Resources.Riak_Core_StateManagerRequiresAtLeastOneState);
            }

            this.state = state;

            this.sync = sync;
            if (this.sync == null)
            {
                this.sync = new ReaderWriterLockSlim();
            }
        }

        public static StateManager FromEnum<T>(object owner, ReaderWriterLockSlim sync = null) where T : struct, IConvertible
        {
            if (owner == null)
            {
                throw new ArgumentNullException("owner");
            }

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

            return new StateManager(owner, dict, values.First(), sync);
        }

        public byte GetState()
        {
            if (disposed)
            {
                return finalState;
            }
            else
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
        }

        public void SetState(byte state)
        {
            if (disposed)
            {
                throw new ObjectDisposedException(ownerName, Properties.Resources.Riak_Core_StateManagerDisposedException);
            }

            sync.EnterWriteLock();
            try
            {
                this.state = state;
            }
            finally
            {
                sync.ExitWriteLock();
            }
        }

        public bool IsCurrentState(params byte[] states)
        {
            if (disposed)
            {
                throw new ObjectDisposedException(ownerName, Properties.Resources.Riak_Core_StateManagerDisposedException);
            }

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

        public bool IsStateLessThan(byte state, bool alreadyLocked = false)
        {
            if (disposed)
            {
                throw new ObjectDisposedException(ownerName, Properties.Resources.Riak_Core_StateManagerDisposedException);
            }

            if (!alreadyLocked)
            {
                sync.EnterReadLock();
            }

            try
            {
                return this.state < state;
            }
            finally
            {
                if (!alreadyLocked)
                {
                    sync.ExitReadLock();
                }
            }
        }

        public void StateCheck(params byte[] states)
        {
            if (disposed)
            {
                throw new ObjectDisposedException(ownerName, Properties.Resources.Riak_Core_StateManagerDisposedException);
            }

            sync.EnterReadLock();
            try
            {
                if (!states.Contains(state))
                {
                    string expected = string.Join(", ", states.Select(s => this.states[s]));
                    var message = string.Format(
                        Properties.Resources.Riak_Core_StateManagerStateCheckExpectedStateButGot_fmt,
                        expected,
                        this.states[this.state]);
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
                finalState = state;
                disposed = true;
            }
        }

        public override string ToString()
        {
            return states[GetState()];
        }
    }
}
