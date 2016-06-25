namespace Test
{
    using System;
    using Common.Logging;
    using Common.Logging.Simple;

    public static class Logging
    {
        static Logging()
        {
            LogManager.Adapter = new ConsoleOutLoggerFactoryAdapter();
        }

        public static ILog GetLogger(Type type)
        {
            return LogManager.GetLogger(type);
        }
    }
}
