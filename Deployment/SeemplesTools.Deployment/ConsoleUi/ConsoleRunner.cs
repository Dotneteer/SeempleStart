using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using SeemplesTools.Deployment.Infrastructure;

namespace SeemplesTools.Deployment.ConsoleUi
{
    public class ConsoleRunner
    {
        private readonly DeploymentTransaction _transaction;

        private ConsoleRunner(IDictionary<Type, CommandAttribute> availableCommands, IList<string> args)
        {
            if (availableCommands == null)
            {
                throw new ArgumentNullException("availableCommands");
            }

            if (args == null)
            {
                throw new ArgumentNullException("args");
            }

            _transaction = new DeploymentTransaction
            {
                LogFile = Path.Combine(Environment.CurrentDirectory, "deployment.log")
            };

            Command currentCommand = null;

            for (var i = 0; i < args.Count; i++)
            {
                var arg = args[i];

                if (arg.StartsWith("-") || arg.StartsWith("/"))
                {
                    string original;
                    string argument;

                    var index = arg.IndexOfAny(new[] { '=', ':' });

                    if (index == -1)
                    {
                        original = arg;
                        argument = i == args.Count - 1 
                            ? null 
                            : args[i + 1];
                    }
                    else
                    {
                        original = arg.Substring(0, index);
                        argument = arg.Substring(index + 1);
                    }

                    original = original.TrimStart('-', '/');
                    var option = original.Replace("-", "").Replace("_", "").ToLower();

                    if (currentCommand == null)
                    {
                        if (ParseOption(option, argument, original))
                        {
                            if (index == -1) i++;
                        }
                        else
                        {
                            if (index != -1)
                            {
                                throw new ParameterException(String.Format(
                                    "Global option {0} doesn't allow an argument.", original));
                            }
                        }
                    }
                    else
                    {
                        if (currentCommand.ParseOption(option, argument, original))
                        {
                            if (index == -1) i++;
                        }
                        else
                        {
                            if (index != -1)
                            {
                                throw new ParameterException(String.Format(
                                    "Option {0} for command {1} doesn't allow an argument.",
                                    original, currentCommand.Original));
                            }
                        }
                    }
                }
                else
                {
                    FinishCommandInitialization(currentCommand);

                    var commandName = arg.Replace("-", "").Replace("_", "").ToLower();

                    var commandType = availableCommands.FirstOrDefault(c => c.Value.Commands.Contains(commandName)).Key;
                    if (commandType == null)
                    {
                        throw new ParameterException(String.Format("Unknown command {0}.", arg));
                    }

                    currentCommand = Activator.CreateInstance(commandType) as Command;
                    if (currentCommand == null)
                    {
                        {
                            throw new ApplicationException(String.Format(
                            "Cannot instantiate command type {0}.", commandType.Name));
                        }
                    }

                    currentCommand.Original = arg;
                }
            }

            FinishCommandInitialization(currentCommand);

            if (_transaction.IsEmpty)
            {
                throw new ParameterException("No command specified.");
            }
        }

        private bool ParseOption(string option, string argument, string original)
        {
            if (option == "?" || option == "h" || option == "help")
                throw new ParameterException();

            switch (option)
            {
                case "log":
                case "l":
                case "logfile":
                case "lf":
                    _transaction.LogFile = String.IsNullOrEmpty(argument) ? null : argument;
                    return true;
            }

            throw new ParameterException(String.Format("Unknown global option {0}.", original));
        }

        private void FinishCommandInitialization(Command currentCommand)
        {
            if (currentCommand == null) return;

            currentCommand.FinishInitialization();
            _transaction.AddCommand(currentCommand);
        }

        public void Run()
        {
            _transaction.Run();
        }

        public static int Run(string[] args, IEnumerable<Assembly> assemblies)
        {
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
            Thread.CurrentThread.CurrentUICulture = CultureInfo.InvariantCulture;

            IDictionary<Type, CommandAttribute> availableCommands;

            try
            {
                availableCommands = EnumerateCommands(assemblies);
                DeploymentUserInterface.Current = new ConsoleUserInterface();
            }
            catch (Exception e)
            {
                var message = e.Message;
                if (String.IsNullOrWhiteSpace(message))
                {
                    message = "An unknown error has occurred during initialization.";
                }
                Console.WriteLine(message);
                return 1;
            }

            ConsoleRunner runner;

            try
            {
                runner = new ConsoleRunner(availableCommands, args);
            }
            catch (ParameterException e)
            {
                Usage(availableCommands);

                var message = e.Message;
                if (String.IsNullOrWhiteSpace(message)) return 1;

                Console.WriteLine();
                Console.WriteLine(message);
                return 1;
            }
            catch (Exception e)
            {
                var message = e.Message;
                if (String.IsNullOrWhiteSpace(message))
                {
                    message = "An unknown error has occurred parsing the command line.";
                }

                Console.WriteLine(message);
                return 1;
            }

            try
            {
                runner.Run();
            }
            catch (Exception e)
            {
                var message = e.Message;
                if (String.IsNullOrWhiteSpace(message))
                {
                    message = "An unknown error has occurred during setup.";
                }

                Console.WriteLine(message);
                return 1;
            }

            return 0;
        }

        private static IDictionary<Type, CommandAttribute> EnumerateCommands(IEnumerable<Assembly> assemblies)
        {
            var result = new Dictionary<Type, CommandAttribute>();
            foreach (var asm in assemblies)
            {
                foreach (var type in asm.GetTypes()
                    .Where(t => typeof(Command).IsAssignableFrom(t) && !t.IsAbstract))
                {
                    var attribute = type.GetCustomAttributes(typeof(CommandAttribute), false).FirstOrDefault();

                    if (attribute != null)
                    {
                        result.Add(type, (CommandAttribute)attribute);
                    }
                }
            }

            return result;
        }

        private static void Usage(IEnumerable<KeyValuePair<Type, CommandAttribute>> availableCommands)
        {
            Console.WriteLine("Usage:");
            Console.WriteLine("{0} [-common option ...] <<command> [-command option ...] ...>",
                Environment.GetCommandLineArgs()[0]);
            Console.WriteLine();
            Console.WriteLine("Common options:");
            Console.WriteLine("  -log          logfile        Log file name (optional)");
            Console.WriteLine();
            Console.Write("Commands:");

            var commandAttributes = availableCommands
                .Where(command => !command.Value.Hidden)
                .OrderBy(command => command.Key.Name)
                .Select(command => command.Value)
                .ToList();

            foreach (var commandAttribute in commandAttributes)
                Console.Write(commandAttribute.UsageCommand);

            Console.WriteLine();

            foreach (var commandAttribute in commandAttributes)
                Console.WriteLine(commandAttribute.UsageArguments);
        }
    }
}
