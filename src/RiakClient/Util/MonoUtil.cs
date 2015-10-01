namespace RiakClient.Util
{
    using System;

    internal static class MonoUtil
    {
        private static readonly bool IsRunningOnMonoValue = false;

        static MonoUtil()
        {
            IsRunningOnMonoValue = null != Type.GetType("Mono.Runtime");
        }

        internal static bool IsRunningOnMono
        {
            get
            {
                return IsRunningOnMonoValue;
            }
        }
    }
}
