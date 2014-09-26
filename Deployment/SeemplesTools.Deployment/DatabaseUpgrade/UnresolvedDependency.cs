using System;
using SeemplesTools.Deployment.Common;

namespace SeemplesTools.Deployment.DatabaseUpgrade
{
    /// <summary>
    /// Class used to mark dependencies specified in upgrade scripts.
    /// </summary>
    /// <remarks>
    /// <see cref="DependencyGraph.ResolveDependencies"/> creates edges in the <see cref="DependencyGraph"/> based on
    /// instances of this class in the <see cref="StepGroup.UnresolvedDependencies"/> property of step groups.
    /// </remarks>
    public class UnresolvedDependency
    {
        #region Properties

        /// <summary>
        /// Type of the dependency.
        /// </summary>
        public DependencyType Type { get; private set; }

        /// <summary>
        /// Name of the database this step group depends on.
        /// </summary>
        public string TargetDatabaseName { get; private set; }

        /// <summary>
        /// Minimal version of the database this step group depends on.
        /// </summary>
        public DatabaseVersion MinVersion { get; private set; }

        /// <summary>
        /// Maximal version of the database this step group depends on.
        /// </summary>
        public DatabaseVersion MaxVersion { get; private set; }

        #endregion

        #region Initialization

        /// <summary>
        /// Create a new instance of the <see cref="UnresolvedDependency"/> class.
        /// </summary>
        /// <param name="type">Type of the dependency.</param>
        /// <param name="targetDatabaseName">Name of the database the current step group depends on.</param>
        /// <param name="minVersion">Minimal version of the database the current step group depends on.</param>
        /// <param name="maxVersion">Maximal version of the database the current step group depends on.</param>
        public UnresolvedDependency(DependencyType type, string targetDatabaseName, DatabaseVersion minVersion,
            DatabaseVersion maxVersion)
        {
            if (targetDatabaseName == null)
            {
                throw new ArgumentNullException("targetDatabaseName");
            }

            if (minVersion == null)
            {
                throw new ArgumentNullException("minVersion");
            }

            if (maxVersion == null)
            {
                throw new ArgumentNullException("maxVersion");
            }

            Type = type;
            TargetDatabaseName = targetDatabaseName;
            MinVersion = minVersion;
            MaxVersion = maxVersion;
        }

        /// <summary>
        /// Create a new instance of the <see cref="UnresolvedDependency"/> class from a descriptor in an upgrade script.
        /// </summary>
        /// <param name="type">Type of the dependency.</param>
        /// <param name="descriptor">Descriptor from which the <see cref="UnresolvedDependency"/> should be created.
        /// </param>
        /// <param name="sourceVersion">Source version of the upgrade script the descriptor is found in.</param>
        /// <param name="scriptName">Name of the upgrade script.</param>
        /// <returns></returns>
        public static UnresolvedDependency FromDescriptor(
            DependencyType type,
            string descriptor,
            DatabaseVersion sourceVersion,
            string scriptName)
        {
            if (descriptor == null)
            {
                throw new ArgumentNullException("descriptor");
            }

            if (scriptName == null)
            {
                throw new ArgumentNullException("scriptName");
            }

            string name;
            DatabaseVersion minVersion;
            DatabaseVersion maxVersion;

            var index = descriptor.IndexOf('-');
            if (index == -1)
            {
                if (sourceVersion == null)
                {
                    throw new ApplicationException(String.Format(
                        "Invalid dependency descriptor in {0}: {1}.", scriptName, descriptor));
                }

                name = descriptor;
                minVersion = sourceVersion;
                maxVersion = sourceVersion;
            }
            else
            {
                name = descriptor.Substring(0, index);
                var versions = descriptor.Substring(index + 1);

                index = versions.IndexOf('-');
                if (index == -1)
                {
                    if (versions == "*")
                    {
                        minVersion = DatabaseVersion.MinValue;
                        maxVersion = DatabaseVersion.MaxValue;
                    }
                    else
                    {
                        if (!DatabaseVersion.TryParse(versions, out minVersion))
                        {
                            throw new ApplicationException(String.Format(
                                "Invalid dependency descriptor in {0}: {1}.", scriptName, descriptor));
                        }

                        maxVersion = minVersion;
                    }
                }
                else
                {
                    var minVersionString = versions.Substring(0, index);
                    var maxVersionString = versions.Substring(index + 1);

                    if (minVersionString == "*")
                    {
                        minVersion = DatabaseVersion.MinValue;
                    }
                    else if (!DatabaseVersion.TryParse(minVersionString, out minVersion))
                    {
                        throw new ApplicationException(String.Format(
                            "Invalid dependency descriptor in {0}: {1}.", scriptName, descriptor));
                    }

                    if (maxVersionString == "*")
                    {
                        maxVersion = DatabaseVersion.MaxValue;
                    }
                    else if (!DatabaseVersion.TryParse(maxVersionString, out maxVersion))
                    {
                        throw new ApplicationException(String.Format(
                            "Invalid dependency descriptor in {0}: {1}.", scriptName, descriptor));
                    }
                }
            }

            return new UnresolvedDependency(type, name, minVersion, maxVersion);
        }

        #endregion
    }
}
