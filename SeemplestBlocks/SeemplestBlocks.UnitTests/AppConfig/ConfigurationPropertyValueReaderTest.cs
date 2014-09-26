using System;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Seemplest.Core.DataAccess.DataServices;
using Seemplest.Core.DependencyInjection;
using SeemplestBlocks.Core.AppConfig;
using SoftwareApproach.TestingExtensions;

namespace SeemplestBlocks.UnitTests.AppConfig
{
    [TestClass]
    public class ConfigurationPropertyValueReaderTest
    {
        [ClassInitialize]
        public static void Initialize(TestContext context)
        {
            ServiceManager.SetRegistry(new DefaultServiceRegistry());
            ServiceManager.Register<IConfigurationReader, FakeConfigurationReader>();
            DataAccessFactory.SetRegistry(ServiceManager.ServiceRegistry);
        }

        [TestMethod]
        public void GetValueWorksWithPropertyName()
        {
            // --- Assert
            TestConfig.IntProperty.ShouldEqual(234);
        }

        [TestMethod]
        public void GetValueWorksWithCategoryKeyAttribute()
        {
            // --- Assert
            TestConfig.Int2Property.ShouldEqual(2345);
        }

        [TestMethod]
        public void GetValueWorksWithExplicitName()
        {
            // --- Assert
            TestConfig.Int3Property.ShouldEqual(-123);
        }

        [TestMethod]
        public void GetValueWorksWithString()
        {
            // --- Assert
            TestConfig.StringProperty.ShouldEqual("AlmaBeka");
        }

        [TestMethod]
        public void GetValueWorksWithCustomType()
        {
            // --- Assert
            var customer = TestConfig.CustomerNameProperty;
            customer.FirstName.ShouldEqual("John");
            customer.LastName.ShouldEqual("Doe");
        }

        [TestMethod]
        public void GetValueWorksWithConfigurationCategoryAttribute()
        {
            // --- Assert
            TestConfig2.IntProperty.ShouldEqual(-234);
            TestConfig2.Int3Property.ShouldEqual(123);
        }
    }

    static class TestConfig
    {
        private static readonly ConfigurationPropertyValueReader s_Reader = 
            new ConfigurationPropertyValueReader();

        public static int IntProperty
        {
            [MethodImpl(MethodImplOptions.NoInlining)]
            get { return s_Reader.GetValue<int>(); }
        }

        [ConfigurationKey("MyIntConfig")]
        public static int Int2Property
        {
            [MethodImpl(MethodImplOptions.NoInlining)]
            get { return s_Reader.GetValue<int>(); }
        }

        public static int Int3Property
        {
            [MethodImpl(MethodImplOptions.NoInlining)]
            get { return s_Reader.GetValue("MyInt3", 123); }
        }

        public static string StringProperty
        {
            [MethodImpl(MethodImplOptions.NoInlining)]
            get { return s_Reader.GetValue<string>(); }
        }

        [ConfigurationKey("MyCustomer")]
        public static CustomerName CustomerNameProperty
        {
            [MethodImpl(MethodImplOptions.NoInlining)]
            get { return s_Reader.GetValue<CustomerName>(); }
        }
    }

    [ConfigurationCategory("MyTestConfig")]
    static class TestConfig2
    {
        private static readonly ConfigurationPropertyValueReader s_Reader =
            new ConfigurationPropertyValueReader();

        public static int IntProperty
        {
            [MethodImpl(MethodImplOptions.NoInlining)]
            get { return s_Reader.GetValue<int>(); }
        }

        public static int Int3Property
        {
            [MethodImpl(MethodImplOptions.NoInlining)]
            get { return s_Reader.GetValue("MyInt3", 123); }
        }

        public static string StringProperty
        {
            [MethodImpl(MethodImplOptions.NoInlining)]
            get { return s_Reader.GetValue<string>(); }
        }
    }

    sealed class FakeConfigurationReader : IConfigurationReader
    {
        public bool GetConfigurationValue(string category, string key, out string value)
        {
            if (category == "TestConfig")
            {
                switch (key)
                {
                    case "IntProperty":
                        value = "234";
                        return true;
                    case "MyIntConfig":
                        value = "2345";
                        return true;
                    case "MyInt3":
                        value = "-123";
                        return true;
                    case "StringProperty":
                        value = "AlmaBeka";
                        return true;
                    case "MyCustomer":
                        value = "Doe, John";
                        return true;
                    default:
                        value = null;
                        return false;
                }
            }
            if (category == "MyTestConfig")
            {
                switch (key)
                {
                    case "IntProperty":
                        value = "-234";
                        return true;
                    default:
                        value = null;
                        return false;
                }
            }
            value = null;
            return false;
        }
    }

    [TypeConverter(typeof(CustomerNameTypeConverter))]
    sealed class CustomerName
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public static CustomerName Parse(string fullName)
        {
            var parts = fullName.Split(',');
            return new CustomerName
            {
                LastName = parts.Length > 0 ? parts[0].Trim() : "",
                FirstName = parts.Length > 1 ? parts[1].Trim() : "",
            };
        }
    }

    sealed class CustomerNameTypeConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof (string);
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return destinationType == typeof (string);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            return value == null ? null : CustomerName.Parse(value.ToString());
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            var custName = value as CustomerName;
            return custName == null ? "" : (custName.LastName ?? "") + ", " + (custName.FirstName ?? "");
        }
    }
}
