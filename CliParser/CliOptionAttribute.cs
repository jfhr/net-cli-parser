using System;

namespace CliParser
{
    [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    public sealed class CliOptionAttribute : Attribute
    {
        /// <summary>
        /// Indicates that a property should be set from command line arguments.
        /// The type of the property must be <see cref="string"/> or <see cref="bool"/>
        /// </summary>
        /// <param name="flags">
        /// The full command-line name(s) of the option. Usually start with '-' or '--'.
        /// </param>
        public CliOptionAttribute(params string[] flags)
        {
            Flags = flags;
        }

        internal string[] Flags { get; }

        /// <summary>
        /// Indicates whether this command line argument is required.
        /// Only has an effect if the type of the property is <see cref="string"/>.
        /// </summary>
        public bool Required { get; set; }

        /// <summary>
        /// This description will be shown when the GetHelpMessage() method
        /// is called on a class derived from CliOptionsBase.
        /// </summary>
        public string Description { get; set; }
    }
}
