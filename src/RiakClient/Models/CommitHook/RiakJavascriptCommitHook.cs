namespace RiakClient.Models.CommitHook
{
    using Extensions;
    using Messages;
    using Newtonsoft.Json;

    /// <summary>
    /// Represents a JavaScript-based commit hook.
    /// </summary>
    public class RiakJavascriptCommitHook : RiakCommitHook, IRiakPreCommitHook
    {
        private readonly string name;

        /// <summary>
        /// Initializes a new instance of the <see cref="RiakJavascriptCommitHook" /> class.
        /// </summary>
        /// <param name="name">The name of the JavaScript function to use for this hook.s</param>
        public RiakJavascriptCommitHook(string name)
        {
            this.name = name;
        }

        /// <summary>
        /// The name of the JavaScript function to execute for the hook.
        /// </summary>
        public string Name
        {
            get { return name; }
        }

        /// <summary>
        /// Determines whether the one object is equal to another object.
        /// </summary>
        /// <param name="left">The first <see cref="RiakIndexId"/> to compare.</param>
        /// <param name="right">The other <see cref="RiakIndexId"/> to compare.</param>
        /// <returns><b>true</b> if the specified object is equal to the current object, otherwise, <b>false</b>.</returns>
        public static bool operator ==(RiakJavascriptCommitHook left, RiakJavascriptCommitHook right)
        {
            return RiakJavascriptCommitHook.Equals(left, right);
        }

        /// <summary>
        /// Determines whether the one object is <b>not</b> equal to another object.
        /// </summary>
        /// <param name="left">The first <see cref="RiakObjectId"/> to compare.</param>
        /// <param name="right">The other <see cref="RiakObjectId"/> to compare.</param>
        /// <returns><b>true</b> if the specified object is <b>not</b> equal to the current object, otherwise, <b>false</b>.</returns>
        public static bool operator !=(RiakJavascriptCommitHook left, RiakJavascriptCommitHook right)
        {
            return !RiakJavascriptCommitHook.Equals(left, right);
        }

        /// <summary>
        /// Determines whether the specified object is equal to the current object.
        /// </summary>
        /// <param name="other">The object to compare with the current object.</param>
        /// <returns><b>true</b> if the specified object is equal to the current object, otherwise, <b>false</b>.</returns>
        public override bool Equals(RiakCommitHook other)
        {
            return Equals(other as RiakJavascriptCommitHook);
        }

        /// <summary>
        /// Determines whether the specified object is equal to the current object.
        /// </summary>
        /// <param name="obj">The object to compare with the current object.</param>
        /// <returns><b>true</b> if the specified object is equal to the current object, otherwise, <b>false</b>.</returns>
        public override bool Equals(object obj)
        {
            if (object.ReferenceEquals(null, obj))
            {
                return false;
            }

            if (object.ReferenceEquals(this, obj))
            {
                return true;
            }

            return Equals(obj as RiakJavascriptCommitHook);
        }

        /// <summary>
        /// Returns a hash code for the current object.
        /// Uses a combination of the public properties to generate a unique hash code.
        /// </summary>
        /// <returns>A hash code for the current object.</returns>
        public override int GetHashCode()
        {
            return name.GetHashCode();
        }

        /// <inheritdoc/>
        public override void WriteJson(JsonWriter writer)
        {
            writer.WriteStartObject();
            writer.WriteProperty("name", name);
            writer.WriteEndObject();
        }

        /// <inheritdoc/>
        public override RpbCommitHook ToRpbCommitHook()
        {
            return new RpbCommitHook { name = name.ToRiakString() };
        }

        /// <summary>
        /// Determines whether the specified object is equal to the current object.
        /// </summary>
        /// <param name="other">The object to compare with the current object.</param>
        /// <returns><b>true</b> if the specified object is equal to the current object, otherwise, <b>false</b>.</returns>
        private bool Equals(RiakJavascriptCommitHook other)
        {
            if (object.ReferenceEquals(null, other))
            {
                return false;
            }

            return string.Equals(name, other.Name);
        }
    }
}
