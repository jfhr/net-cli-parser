using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace CliParser
{
    /// <summary>
    /// Parses command line arguments into a container type.
    /// </summary>
    public static class CliParser
    {
        private static readonly Type[] allowedTypes = new[]
        {
            typeof(string),
            typeof(bool),
        };

        private static readonly string allowedTypesString =
            string.Join(", ", allowedTypes.Select(t => t.Name));

        /// <summary>
        /// Parses the given command line arguments and 
        /// </summary>
        /// <param name="args">
        /// Command line arguments to parse.
        /// </param>
        /// <param name="handleWrongArguments">
        /// If <see langword="true"/>, when invalid or incomplete arguments 
        /// are encountered, a message will be written to stderr and the program
        /// will exit. If <see langword="false"/>, an exception will be thrown.
        /// Default is <see langword="false"/>.
        /// </param>
        public static T Parse<T>(string[] args, bool handleWrongArguments = false) where T : new()
        {
            try
            {
                var container = new T();
                ParseInternal(container, args);
                VerifyAllRequiredArgumentsAreSet(container);
                return container;
            }
            catch (CliOptionsException e) when (handleWrongArguments)
            {
                Debug.Fail("Process exited because an invalid command line was parsed with handleWrongArguments = true.");
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Error.WriteLine(e.Message);
                Console.ResetColor();
                Environment.Exit(e.HResult);

                throw;  // this line will not be called, but is needed to satisfy the compiler
            }
        }

        private static void VerifyAllRequiredArgumentsAreSet(object container)
        {
            var requiredButNotSet = DiscoverOptions(container)
                .Where(o => o.Required && o.PropertyType == typeof(string))
                .Where(o => o.Getter.Invoke(container, new object[0]) == null);

            if (requiredButNotSet.Any())
            {
                throw new CliOptionsException("The following required options were not specified: " +
                    string.Join(", ", requiredButNotSet.Select(o => o.Flags.First())));
            }
        }

        private static void ParseInternal(object container, string[] args)
        {
            var cliOptions = DiscoverOptions(container);
            var argsEnumerator = args.Where(x => true).GetEnumerator();

            while (argsEnumerator.MoveNext())
            {
                string arg = argsEnumerator.Current;
                var option = cliOptions.FirstOrDefault(x =>
                    x.Flags.Any(f =>
                        f.Equals(arg, StringComparison.OrdinalIgnoreCase)));

                if (option == null)
                {
                    continue;
                }

                SetPropertyFromArgs(container, argsEnumerator, option);
            }
        }

        /// <summary>
        /// Sets the value of a property from the given command line arguments.
        /// </summary>
        /// <param name="argsEnumerator">
        /// An enumerator over the command line arguments.
        /// The value of argsEnumerator.Current should be the name of the
        /// current option.
        /// </param>
        private static void SetPropertyFromArgs(object container, 
            IEnumerator<string> argsEnumerator,
            CliOptionInternal option)
        {
            if (option.PropertyType == typeof(string))
            {
                SetValue(container, option, argsEnumerator, x => x);
            }
            else if (option.PropertyType == typeof(bool))
            {
                option.Setter.Invoke(container, new object[] { true });
            }
        }

        /// <summary>
        /// Sets the value of a property from the next value of the given
        /// enumerator. Optionally applies a transformation function
        /// to the argument before assigning.
        /// </summary>
        private static void SetValue<T>(object container,
            CliOptionInternal option,
            IEnumerator<string> argsEnumerator,
            Func<string, T> transformation)
        {
            if (!option.PropertyType.IsAssignableFrom(typeof(T)))
            {
                throw new CliInternalException($"Internal Error when parsing cli " +
                    $"options: Transformation function for option {option.Flags.First()} " +
                    $"returns value of type {typeof(T).Name}, but the associated " +
                    $"property is of type {option.PropertyType}.");
            }

            if (argsEnumerator.MoveNext())
            {
                string rawValue = argsEnumerator.Current;
                T value = transformation(rawValue);
                option.Setter.Invoke(container, new object[] { value });
            }
            else
            {
                throw new CliOptionsException($"Value expected for option " +
                    $"{option.Flags.First()}.");
            }
        }

        private static IEnumerable<CliOptionInternal> DiscoverOptions(object container)
        {
            return container.GetType().GetProperties()
                .Select(p => new CliOptionInternal(p))
                .Where(a => a.Attribute != null && a.Setter != null)
                .Select(a => allowedTypes.Contains(a.PropertyType) ? a
                    : throw new CliOptionsException(
                        $"CliOption property type must be one of " +
                        $"{allowedTypesString}, not {a.PropertyType}"));
        }

        public static string GetHelpMessage(object container)
        {
            var allRequired = string.Join("\n",
                DiscoverOptions(container)
                .Where(x => x.Required)
                .Select(GetHelpForOption));

            var allOptional = string.Join("\n",
                DiscoverOptions(container)
                .Where(x => x.Required == false)
                .Select(GetHelpForOption));

            return $"" +
                   $"Required:" +
                   $"{allRequired}" +
                   $"" +
                   $"Optional:" +
                   $"{allOptional}";
        }

        private static string GetHelpForOption(CliOptionInternal option)
        {
            var flags = string.Join(", ", option.Flags);
            return $"{flags}: {option.Description}";
        }
    }
}