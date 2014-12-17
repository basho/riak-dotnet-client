using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using CorrugatedIron.Models;
using CorrugatedIron.Models.MapReduce.KeyFilters;
using NUnit.Framework;

namespace CorrugatedIron.Tests.Live.Extensions
{
    public static class IntegrationTestExtensions
    {
        // TESTING PURPOSES ONLY
        public static void DeleteBucket(this IRiakBatchClient client, string bucket)
        {
            DeleteBucket(client, null, bucket);
        }

        // TESTING PURPOSES ONLY
        public static void DeleteBucket(this IRiakBatchClient client, string bucketType, string bucket)
        {
            var keylistResult = client.ListKeys(bucketType, bucket);
            var keys = keylistResult.Value != null ? keylistResult.Value.ToList() : new List<string>();
            if (!keys.Any()) return;
            var objectIds = keys.Select(key => new RiakObjectId(bucketType, bucket, key));
            client.Delete(objectIds);
        }

        private static readonly Func<RiakResult, bool> DefaultSuccessFunc = result => result.IsSuccess;

        public static T WaitUntil<T>(this Func<T> action, int attempts = 10) where T : RiakResult
        {
            return WaitUntil<T>(action, DefaultSuccessFunc, attempts);
        }

        public static T WaitUntil<T>(this Func<T> action, Func<T, bool> successCriteriaFunc, int attempts = 10) where T : RiakResult
        {
            var invalidResults = new T[attempts];
            var exceptions = new Exception[attempts];

            T result = null;
            for (var i = 0; i < attempts; i++)
            {
                result = null;
                try
                {
                    result = action.Invoke();
                }
                catch (Exception ex)
                {
                    exceptions[i] = ex;
                    // Do nothing, try again
                }

                if (result != null && successCriteriaFunc.Invoke(result))
                    return result;

                invalidResults[i] = result;

                Thread.Sleep(i * 1000);
            }
            // print retry "trace" and
            // return last result if all failed the success check

            PrintFailedRetries(invalidResults, exceptions, attempts);

            return result;
        }

        /// <summary>
        /// Helper method to retry bulk riak actions, such as deleting a large # of objects for testing purposes. 
        /// </summary>
        /// <typeparam name="T">RiakResult, or RiakResult&lt;T&gt;></typeparam>
        /// <param name="action">The action to perform</param>
        /// <param name="successCriteriaFunc">Func that defines the success of the operation</param>
        /// <param name="attempts">Max # of attempts to make</param>
        /// <param name="timeout">Min time for devolving timeout between attempts</param>
        /// <returns></returns>
        public static IEnumerable<T> WaitUntil<T>(this Func<IEnumerable<T>> action,
                                                  Func<IEnumerable<T>, bool> successCriteriaFunc,
                                                  int attempts = 10,
                                                  int timeout = 1000) where T : RiakResult
        {
            List<T> result = null;
            for (var i = 0; i < attempts; i++)
            {
                result = null;
                try
                {
                    result = action.Invoke().ToList();
                }
                catch
                {
                    // Do nothing, try again
                }

                if (result != null && successCriteriaFunc.Invoke(result))
                    return result;

                Thread.Sleep(i * timeout);
            }
            // return last result if all failed the success check

            return result;
        }

        private static void PrintFailedRetries<T>(T[] invalidResults, Exception[] exceptions, int attempts) where T : RiakResult
        {
            var testName = TestContext.CurrentContext.Test.FullName;

            Console.WriteLine("{0}: Could not reach success criteria\r\n", testName);

            for (var i = 0; i < attempts; i++)
            {
                var result = invalidResults[i];
                var exception = exceptions[i];

                Console.WriteLine("Iteration {0}:", i);
                Console.WriteLine("----------------------------------------");

                if (result != null)
                {
                    Console.WriteLine(
                        "RiakResult:\r\n\tSuccess:       {0}\r\n\tNodeOffline:   {1}\r\n\tResultCode:    {2}\r\n\tError Message: {3}\r\n",
                        result.IsSuccess,
                        result.NodeOffline,
                        result.ResultCode,
                        result.ErrorMessage);
                }
                else
                {
                    Console.WriteLine("RiakResult: No RiakResult Recorded\r\n");
                }

                if (exception != null)
                {
                    Console.WriteLine("Exception: {0}\r\n", exception);
                }
                else
                {
                    Console.WriteLine("Exception: No Exception Recorded\r\n");
                }
            }
            Console.WriteLine("----------------------------------------");
        }
    }
}
