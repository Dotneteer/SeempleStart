using System.Configuration;
using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Seemplest.Core.Configuration;
using Seemplest.Core.TypeResolution;
using SoftwareApproach.TestingExtensions;

namespace Seemplest.Core.UnitTests.TypeResolution
{
    [TestClass]
    public class TypeResolverTest
    {
        private const string SECTION_NAME = "TypeResolver1";

        private bool _configurationChangedVisited;
        private ITypeResolver _before;
        private ITypeResolver _after;

        [TestInitialize]
        public void Initialize()
        {
            _configurationChangedVisited = false;
            _before = null;
            _after = null;
            TypeResolver.Configure((ITypeResolver)null);
            TypeResolver.ConfigurationChanged += TypeResolverConfigurationChanged;
        }

        [TestCleanup]
        public void Cleanup() 
        { 
            TypeResolver.ConfigurationChanged -= TypeResolverConfigurationChanged;
        }

        [TestMethod]
        public void ConfigurationWithSettingsResolverToDefaultTypeResolver()
        {
            // --- Arrange
            var settings = new TypeResolverConfigurationSettings(
                (XElement)ConfigurationManager.GetSection(SECTION_NAME));
            
            // --- Act
            var before = TypeResolver.Current;
            TypeResolver.Configure(settings);
            var after = TypeResolver.Current;

            // --- Assert
            after.ShouldBeOfType(typeof (DefaultTypeResolver));
            _configurationChangedVisited.ShouldBeTrue();
            before.ShouldBeNull();
            _before.ShouldBeNull();
            _after.ShouldEqual(after);
        }

        [TestMethod]
        public void ConfigurationWorksWithConcreteTypeResolver()
        {
            // --- Arrange
            var typeResolver = new TrivialTypeResolver();

            // --- Act
            var before = TypeResolver.Current;
            TypeResolver.Configure(typeResolver);
            var after = TypeResolver.Current;

            // --- Assert
            after.ShouldEqual(typeResolver);
            _configurationChangedVisited.ShouldBeTrue();
            before.ShouldBeNull();
            _before.ShouldBeNull();
            _after.ShouldEqual(after);
        }

        [TestMethod]
        public void ConfigurationChangeIsCaught()
        {
            // --- Arrange
            var settings = new TypeResolverConfigurationSettings(
                (XElement)ConfigurationManager.GetSection(SECTION_NAME));
            var typeResolver = new TrivialTypeResolver();

            // --- Act
            TypeResolver.Configure(settings);
            var before = TypeResolver.Current;
            TypeResolver.Configure(typeResolver);
            var after = TypeResolver.Current;

            // --- Assert
            before.ShouldBeOfType(typeof (DefaultTypeResolver));
            _before.ShouldEqual(before);
            _configurationChangedVisited.ShouldBeTrue();
            after.ShouldEqual(typeResolver);
            _after.ShouldEqual(after);
        }

        [TestMethod]
        public void ConfigurationFromAppConfigWorks()
        {
            // --- Arrange
            var before = TypeResolver.Current;
            TypeResolver.Configure(SECTION_NAME);
            var after = TypeResolver.Current;

            // --- Assert
            after.ShouldBeOfType(typeof(DefaultTypeResolver));
            _configurationChangedVisited.ShouldBeTrue();
            before.ShouldBeNull();
            _before.ShouldBeNull();
            _after.ShouldEqual(after);
        }

        [TestMethod]
        public void ConfigurationFromAppConfigWorksWithNoSectionName()
        {
            // --- Arrange
            var before = TypeResolver.Current;
            TypeResolver.Configure();
            var after = TypeResolver.Current;

            // --- Assert
            after.ShouldBeOfType(typeof(DefaultTypeResolver));
            _configurationChangedVisited.ShouldBeTrue();
            before.ShouldBeNull();
            _before.ShouldBeNull();
            _after.ShouldEqual(after);
        }

        void TypeResolverConfigurationChanged(object sender, ConfigurationChangedEventArgs<ITypeResolver> e)
        {
            _configurationChangedVisited = true;
            _before = e.OldValue;
            _after = e.NewValue;
        }
    }
}
