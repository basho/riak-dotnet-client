namespace RiakClient.Models.MapReduce.Fluent
{
    using Models.MapReduce.Languages;
    using Models.MapReduce.Phases;

    /// <summary>
    /// A fluent builder class for defining JavaScript Map and Reduce phases.
    /// </summary>
    public class RiakFluentActionPhaseJavascript
    {
        private readonly RiakActionPhase<RiakPhaseLanguageJavascript> phase;

        internal RiakFluentActionPhaseJavascript(RiakActionPhase<RiakPhaseLanguageJavascript> phase)
        {
            this.phase = phase;
        }

        /// <summary>
        /// The option to keep the results of this phase, or just pass them onto the next phase.
        /// </summary>
        /// <param name="keep"><b>true</b> to keep the phase results for the final result set, <b>false</b> to omit them. </param>
        /// <returns>A reference to this updated instance, for fluent chaining.</returns>
        public RiakFluentActionPhaseJavascript Keep(bool keep)
        {
            phase.Keep(keep);
            return this;
        }

        /// <summary>
        /// The arguments to pass to the Javascript function that's defined in the <see name="Source"/> parameter, 
        /// or the one listed in the <see name="Name"/> parameter.
        /// </summary>
        /// <typeparam name="T">The type of the argument.</typeparam>
        /// <param name="argument">The argument.</param>
        /// <returns>A reference to this updated instance, for fluent chaining.</returns>
        public RiakFluentActionPhaseJavascript Argument<T>(T argument)
        {
            phase.Argument(argument);
            return this;
        }

        /// <summary>
        /// Specify a name of the known JavaScript function to execute for this phase.
        /// </summary>
        /// <param name="name">The name of the function to execute.</param>
        /// <returns>A reference to this updated instance, for fluent chaining.</returns>
        public RiakFluentActionPhaseJavascript Name(string name)
        {
            phase.Language.Name(name);
            return this;
        }

        /// <summary>
        /// Specify the source code of the JavaScript function to dynamically load and execute for this phase.
        /// </summary>
        /// <param name="source">The source code of the function to execute.</param>
        /// <returns>A reference to this updated instance, for fluent chaining.</returns>
        public RiakFluentActionPhaseJavascript Source(string source)
        {
            phase.Language.Source(source);
            return this;
        }

        /// <summary>
        /// Specify a bucket and key where a stored JavaScript function can be dynamically loaded from Riak and executed for this phase.
        /// </summary>
        /// <param name="bucket">The bucket name of the JavaScript function's address.</param>
        /// <param name="key">The key of the JavaScript function's address.</param>
        /// <returns>A reference to this updated instance, for fluent chaining.</returns>
        public RiakFluentActionPhaseJavascript BucketKey(string bucket, string key)
        {
            phase.Language.BucketKey(bucket, key);
            return this;
        }
    }
}
