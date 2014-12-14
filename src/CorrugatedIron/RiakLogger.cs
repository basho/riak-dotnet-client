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
		};

		private int _deliverables = 0;

		public static RiakLogger Instance {
			get {
				return instance;
			}
		}

		/// <summary>

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
					//TODO 
					//ThreadPool.GetMinThreads
					ThreadPool.SetMinThreads(10, 10);
					Thread externalLogHandler = null;
					AutoResetEvent signal = new AutoResetEvent(false);
					// wrap calls to handlers in try catch for safety
					try 
					{
						// move contention from signal.waitOne off the main thread
						ThreadPool.QueueUserWorkItem((o) => 
						{							
							// call the external handler
							ThreadPool.QueueUserWorkItem((oo) => 
							{
								
								externalLogHandler = Thread.CurrentThread;
								//ensure that a call is never blocking
								value.Invoke(data);
								signal.Set();
							});
							// and never hangs indefinitely 
							new Timer((ooo) => { signal.Set(); }, null, 2000, Timeout.Infinite);
							signal.WaitOne(); // wait for one or the other
						});
					}
					catch(ThreadAbortException)
					{
						// TODO see below although a thread getting aborted here is more likely to mean, 
						// there are too many thread which means the threshhold needs to be lower 
					}
					catch (Exception ex) 
					{
						Console.WriteLine (ex.Message);
						Console.WriteLine ("uncaught exception in external handler");				
					}
					finally
					{
						if (externalLogHandler != null && externalLogHandler.IsAlive)
						{
							//Console.WriteLine("external handler finished");
							try
							{
								externalLogHandler.Abort(); 
							}
							catch(ThreadAbortException)
							{
								// TODO possibility of error counter and disabling the logger if this 
								// happens enough times...diable for a time period... whatever. 
								// if this happens enough times, the person who implemented logging 
								// did something wrong. Default is to ignore, 
							}

						}
						// If all goes well as it should, there should never be any exceptions. Usually when there are,
						// something is not playing nice or something needs to be tuned a little bit. 
					}
				};
			}
		}
			
		private RiakLogger ()
		{

		}

		/// <summary>
		/// what logging information should be sent to output
		/// </summary>
		/// <param name="level">Level.</param>
		public void AddDeliverable (RiakLogLevel level)
		{
			if ((_deliverables & (int)level) == 0)
				_deliverables |= (int)level;
		}

		/// <summary>
		/// disable something from being sent to the logging output
		/// </summary>
		/// <param name="level">Level.</param>
		public void RemoveDeliverable (RiakLogLevel level)
		{
			if ((_deliverables & (int)level) != 0)
				_deliverables = _deliverables & ~(int)level;
		}


		public void Warn (string msg)
		{
			if ((_deliverables & (int)RiakLogLevel.Warn) != 0)
				Output (new RiakLogData () { message = msg, level = RiakLogLevel.Warn });
		}


		public void Info (string msg)
		{
			if ((_deliverables & (int)RiakLogLevel.Info) != 0)
				Output (new RiakLogData () { message = msg, level = RiakLogLevel.Info });
		}


		public void Error (string msg)
		{
			if ((_deliverables & (int)RiakLogLevel.Error) != 0)
				Output (new RiakLogData () { message = msg, level = RiakLogLevel.Error });
		}


		public void Fatal (string msg)
		{
			if ((_deliverables & (int)RiakLogLevel.Fatal) != 0)
				Output (new RiakLogData () { message = msg, level = RiakLogLevel.Fatal });
		}


		public void Debug (object data, string msg)
		{
			if ((_deliverables & (int)RiakLogLevel.Debug) != 0)
				_Debug(new object[] { data }, msg);
		}

		//[DebuggerStepThrough]
		public void Debug (object[] data, string msg)
		{
			if ((_deliverables & (int)RiakLogLevel.Debug) != 0)
				_Debug(data, msg);
		}

		/// <summary>
		/// Instead of writing a disclaimer on how to use this correctly I decided to do it in a way
		/// that technically shouldn't have implications in objects being passed by reference, should be fine
		/// as long as there's no read-once get accessors laying around which I can't imagine why anybody
		/// would do such a thing, its for debugging, ultimately I'd like to be able to generate a curl request
		/// for each call made to riak. 
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