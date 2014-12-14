using System;
using System.Diagnostics;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace CorrugatedIron
{
	public class RiakLogger
	{
		private static RiakLogger instance = new RiakLogger ();


		private Action<RiakLogData> output = (data) => 
		{
			// Default handler (does nothing) but doesn't even get called 
			// unless deilverables are set.
		};

		/// <summary>
		/// type of logging data to display (bitwise, see values in RiakLogLevel and methods Warn() Info() ect 
		/// to understand how it is used. This approach should have better performance than a List<LogLevel>.Contains
		/// or something ridiculous like that. This implementation is exactly how PHP's error_reporting works.
		/// 
		/// The  only difference is, I use two methods AddErrorLevel and RemoveErrorLevel because I don't exactly trust 
		/// people to understand how to use it and we're working with enums.
		/// </summary>
		private int _reporting = 0;

		/// <summary>
		/// This will ensure that calls to external handlers that are attached have an explicit timeout period,
		/// incase the user implements one that hangs forever, it will terminated and an auxillary log message 
		/// (console.out) will be generated because this should not be happening (it could though, which is why
		/// I decided to implement it. The calls to the logger should be asynchronous because they could block 
		/// operations in CI causing unexpected behavior. Really I think the right way to do this if the user
		/// needs more time is that they use a ConcurrentQueue in the handler to enqueue their data and get out
		/// as quickly as possible, then spin their own thread to process that concurrent queue.
		/// </summary>
		private int _AsynchronousHandlerTimeoutPeriod = 500;

		public static RiakLogger Instance 
		{
			get 
			{
				return instance;
			}
		}

		/// <summary>
		/// Get / Set for Logger Handler, handlers are called asynchronously, and an explicit timeout period 
		/// will ensure that the thread never hangs forever (unless explicitly disabled.)
		/// </summary>
		/// <value>The output.</value>
		public Action<RiakLogData> Output 
		{
			get 
			{
				return output;
			}
			set 
			{
				output += (data) => 
				{				
					Thread externalLogHandler = null;
					AutoResetEvent signal = new AutoResetEvent(false);

					// wrap calls to handlers in try catch for safety
					try 
					{
						// move contention from signal.waitOne off the main thread
						ThreadPool.QueueUserWorkItem((o) => 
						{							
						    //ensure that a call is never blocking us from reaching signal.waitOne
							ThreadPool.QueueUserWorkItem((oo) => 
							{
								
								externalLogHandler = Thread.CurrentThread;								
								value.Invoke(data);
								signal.Set();
							});
							
							// if our call to the external handler doesn't signal.Set before this timer
						    // the time will allow execution to continue to our finally block below
							// where if the thread is still executing, it is aborted.
							new Timer((ooo) => { signal.Set(); }, null, _AsynchronousHandlerTimeoutPeriod, Timeout.Infinite);
							signal.WaitOne(); // wait for one or the other
						});
					}
					catch(ThreadAbortException ex)
					{
						// TODO I'm having a hard time imagining this ever happening, 
						// or if it even could happen, but a safe assumption would be 
						// that this will happen if there are too many threads already running
						// I haven't seen it happen but I'm going to put an auxillary log message
						// here just in case:
						Console.WriteLine("BUG: too many threads? " + ex.Message);
						// I just thought of a reason why this might happen, namely the user were to set the Timeout to something
						// impossibly high, because their handler sat forever and never exited (like its supposed to.)
					}
					catch (Exception ex) 
					{
						// Every exception that we could throw should be accounted for in the previous blocks
						// An exception caught here should be an exception thrown from the user's external handler
						// which they should be catching
						Console.WriteLine (ex.Message);
						Console.WriteLine ("uncaught exception in external handler ");				
					}
					finally
					{
						if (externalLogHandler != null && externalLogHandler.IsAlive)
						{
							try
							{
								externalLogHandler.Abort(); 
							}
							catch(ThreadAbortException)
							{
								// Auxillary log message 
								Console.WriteLine ("The logger handler timed out, this should not be happening");
							}
						}
						// The end of the road:
						// If all goes well as it should, there should never be any exceptions. Usually when there are,
						// something is not playing nice or something needs to be tuned a little bit. 
					}
				};
			}
		}
			

		/// <summary>
		/// singleton instance
		/// </summary>
		private RiakLogger ()
		{

		}

		/// <summary>
		/// This allows a user to adjust the timeout period of asynchronous calls to their attached 
		/// handler if necesarry.
		/// </summary>
		/// <param name="usecs">Usecs.</param>
		public void SetAsyncCallbackTimeoutPeriod(int milliseconds)
		{
			_AsynchronousHandlerTimeoutPeriod = milliseconds;
		}

		/// <summary>
		/// what logging information should be sent to output
		/// </summary>
		/// <param name="level">Level.</param>
		public void AddErrorLevel (RiakLogLevel level)
		{
			if ((_reporting & (int)level) == 0)
				_reporting |= (int)level;
		}

		/// <summary>
		/// disable something from being sent to the logging output
		/// </summary>
		/// <param name="level">Level.</param>
		public void RemoveErrorLevel (RiakLogLevel level)
		{
			if ((_reporting & (int)level) != 0)
				_reporting = _reporting & ~(int)level;
		}


		public void Warn (string msg)
		{
			if ((_reporting & (int)RiakLogLevel.Warn) != 0)
				Output (new RiakLogData () { message = msg, level = RiakLogLevel.Warn });
		}


		public void Info (string msg)
		{
			if ((_reporting & (int)RiakLogLevel.Info) != 0)
				Output (new RiakLogData () { message = msg, level = RiakLogLevel.Info });
		}


		public void Error (string msg)
		{
			if ((_reporting & (int)RiakLogLevel.Error) != 0)
				Output (new RiakLogData () { message = msg, level = RiakLogLevel.Error });
		}


		public void Fatal (string msg)
		{
			if ((_reporting & (int)RiakLogLevel.Fatal) != 0)
				Output (new RiakLogData () { message = msg, level = RiakLogLevel.Fatal });
		}


		public void Debug (object data, string msg)
		{
			if ((_reporting & (int)RiakLogLevel.Debug) != 0)
				_Debug(new object[] { data }, msg);
		}

		//[DebuggerStepThrough]
		public void Debug (object[] data, string msg)
		{
			if ((_reporting & (int)RiakLogLevel.Debug) != 0)
				_Debug(data, msg);
		}

		/// <summary>
		/// This method can export an array of anonymous types, which the user will have to be able to do 
		/// some reflection (they will have to really know what they're doing) to parse. 
		/// </summary>
		/// <param name="data">Data.</param>
		/// <param name="msg">Message.</param>
		private void _Debug(object[] data, string msg)
		{
			Output (new RiakLogData () 
				{ 
					debugging = data,
					message = msg, 
					level = RiakLogLevel.Debug 
				});
		}
	}
}