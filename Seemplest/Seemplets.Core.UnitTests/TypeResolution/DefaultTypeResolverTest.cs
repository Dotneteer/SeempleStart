using System;
using System.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Seemplest.Core.TypeResolution;
using SoftwareApproach.TestingExtensions;
using System.Xml.Linq;

namespace Seemplest.Core.UnitTests.TypeResolution
{
    [TestClass]
    public class DefaultTypeResolverTest
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void DefaultTypeResolverMustHaveNonNullConfiguration()
        {
            // --- Act
            // ReSharper disable ObjectCreationAsStatement
            new DefaultTypeResolver(null, null);
            // ReSharper restore ObjectCreationAsStatement
        }

        [TestMethod]
        public void IntrinsicTypesAreRecognized()
        {
            // --- Arrange
            var typeResolver = new DefaultTypeResolver(null);

            // --- Act
            var type1 = typeResolver.Resolve("bool");
            var type2 = typeResolver.Resolve("char");
            var type3 = typeResolver.Resolve("byte");
            var type4 = typeResolver.Resolve("sbyte");
            var type5 = typeResolver.Resolve("short");
            var type6 = typeResolver.Resolve("ushort");
            var type7 = typeResolver.Resolve("int");
            var type8 = typeResolver.Resolve("uint");
            var type9 = typeResolver.Resolve("long");
            var type10 = typeResolver.Resolve("ulong");
            var type11 = typeResolver.Resolve("decimal");
            var type12 = typeResolver.Resolve("float");
            var type13 = typeResolver.Resolve("double");
            var type14 = typeResolver.Resolve("string");
            var type15 = typeResolver.Resolve("object");
            var type16 = typeResolver.Resolve("Type");
            var type17 = typeResolver.Resolve("Guid");
            var type18 = typeResolver.Resolve("DateTime");

            // --- Assert
            type1.ShouldEqual(typeof(bool));
            type2.ShouldEqual(typeof(char));
            type3.ShouldEqual(typeof(byte));
            type4.ShouldEqual(typeof(sbyte));
            type5.ShouldEqual(typeof(short));
            type6.ShouldEqual(typeof(ushort));
            type7.ShouldEqual(typeof(int));
            type8.ShouldEqual(typeof(uint));
            type9.ShouldEqual(typeof(long));
            type10.ShouldEqual(typeof(ulong));
            type11.ShouldEqual(typeof(decimal));
            type12.ShouldEqual(typeof(float));
            type13.ShouldEqual(typeof(double));
            type14.ShouldEqual(typeof(string));
            type15.ShouldEqual(typeof(object));
            type16.ShouldEqual(typeof(Type));
            type17.ShouldEqual(typeof(Guid));
            type18.ShouldEqual(typeof(DateTime));
        }

        [TestMethod]
        public void DefaultFallsBackToTrivialTypeResolver()
        {
            // --- Arrange
            var typeName = typeof (MyType).AssemblyQualifiedName;
            var typeResolver = new DefaultTypeResolver(null);

            // --- Act
            var type = typeResolver.Resolve(typeName);

            // --- Assert
            type.ShouldEqual(typeof (MyType));
        }

        [TestMethod]
        public void UnknownTypesAreResolvedToNull()
        {
            // --- Arrange
            var settings = new TypeResolverConfigurationSettings(
                (XElement)ConfigurationManager.GetSection("TypeResolver1"));
            var typeResolver = new DefaultTypeResolver(settings);

            // --- Act
            var type = typeResolver.Resolve("ThisIsADummyType");

            // --- Assert
            type.ShouldBeNull();
        }

        [TestMethod]
        public void UnloadableAssembliesDoNotFail()
        {
            // --- Arrange
            var settings = new TypeResolverConfigurationSettings(
                (XElement)ConfigurationManager.GetSection("TypeResolver1"));
            var typeResolver = new DefaultTypeResolver(settings);

            // --- Act
            var type = typeResolver.Resolve("ThisIsADummyType, ThisIsADummyType.dll");

            // --- Assert
            type.ShouldBeNull();
        }

        [TestMethod]
        public void TypeResolutionWorksFromAppConfig()
        {
            // --- Arrange
            var settings = new TypeResolverConfigurationSettings(
                (XElement) ConfigurationManager.GetSection("TypeResolver1"));
            var typeResolver = new DefaultTypeResolver(settings);

            // --- Act
            var type1 = typeResolver.Resolve("Int32");
            var type2 = typeResolver.Resolve("String");
            var type3 = typeResolver.Resolve("uint");

            // --- Assert
            type1.ShouldEqual(typeof(int));
            type2.ShouldEqual(typeof(string));
            type3.ShouldEqual(typeof(uint));
        }

        [TestMethod]
        [ExpectedException(typeof(ConfigurationErrorsException))]
        public void TypeResolutionDetectsAmbigousTypes()
        {
            // --- Arrange
            var settings = new TypeResolverConfigurationSettings(
                (XElement)ConfigurationManager.GetSection("TypeResolver1"));
            var typeResolver = new DefaultTypeResolver(settings);

            // --- Act
            typeResolver.Resolve("MyType").ShouldBeNull();
        }
    }

    public class MyType
    {
    }

    namespace Sub
    {
        public class MyType
        {
        }
    }
}
