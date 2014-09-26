using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Seemplest.Core.Interception;
using SoftwareApproach.TestingExtensions;

namespace Seemplest.Core.UnitTests.Interception
{
    [TestClass]
    public class InterceptorAspectsTest
    {
        private IAspectedOps _aspectedObject;
        private IAspectedOps _noAspectObject;

        [TestInitialize]
        public void Initialize()
        {
            SimpleAspectAttribute.Reset();
            var aspectedObj = new AspectedClass();
            var noAspectObject = new NoAspectClass();
            _aspectedObject = Interceptor.GetInterceptedObject<IAspectedOps>(aspectedObj, null);
            _noAspectObject = Interceptor.GetInterceptedObject<IAspectedOps>(noAspectObject, null);
        }

        [TestMethod]
        public void ClassAndMethodAspectRunsOnMethods()
        {
            // --- Act
            _aspectedObject.Operation1();

            // --- Assert
            SimpleAspectAttribute.VisitInfo.ShouldEqual("cE1E1QcQ1ScS");
        }

        [TestMethod]
        public void OrderWorksAsExpected()
        {
            // --- Act
            _aspectedObject.Operation2();

            // --- Assert
            SimpleAspectAttribute.VisitInfo.ShouldEqual("cE2AE2E2Q2AQcQ2S2AScS");
        }

        [TestMethod]
        public void AttributesUseCache()
        {
            // --- Act
            _aspectedObject.Operation1();
            _aspectedObject.Operation2();

            // --- Assert
            SimpleAspectAttribute.VisitInfo.ShouldEqual("cE1E1QcQ1ScScE2AE2E2Q2AQcQ2S2AScS");
        }

        [TestMethod]
        public void OverrideWorksAsExpected()
        {
            // --- Act
            _aspectedObject.Operation3();

            // --- Assert
            SimpleAspectAttribute.VisitInfo.ShouldEqual("3E3Q3S");
        }

        [TestMethod]
        public void NoMethodAspectWorksAsExpected()
        {
            // --- Act
            _aspectedObject.Operation4();

            // --- Assert
            SimpleAspectAttribute.VisitInfo.ShouldEqual("cEcQcS");
        }

        [TestMethod]
        public void ExceptionWorksAsExpected()
        {
            // --- Act
            try
            {
                _aspectedObject.Operation5();
            }
            catch (InvalidOperationException) {}

            // --- Assert
            SimpleAspectAttribute.VisitInfo.ShouldEqual("cE5E5QcQ5XcX");
        }

        [TestMethod]
        public void ClassWithNoAspectWorks()
        {
            // --- Act
            _noAspectObject.Operation1();

            // --- Assert
            SimpleAspectAttribute.VisitInfo.ShouldEqual("");
        }

        [TestMethod]
        public void OrderWithNoAspectWorksAsExpected()
        {
            // --- Act
            _noAspectObject.Operation2();

            // --- Assert
            SimpleAspectAttribute.VisitInfo.ShouldEqual("");
        }

        [TestMethod]
        public void OverrideWithNoAspectWorksAsExpected()
        {
            // --- Act
            _noAspectObject.Operation3();

            // --- Assert
            SimpleAspectAttribute.VisitInfo.ShouldEqual("");
        }

        [TestMethod]
        public void NoMethodAspectWithNoAspectClassWorksAsExpected()
        {
            // --- Act
            _noAspectObject.Operation4();

            // --- Assert
            SimpleAspectAttribute.VisitInfo.ShouldEqual("");
        }

        [TestMethod]
        public void ExceptionWithNoAspectWorksAsExpected()
        {
            // --- Act
            try
            {
                _noAspectObject.Operation5();
            }
            catch (InvalidOperationException) { }

            // --- Assert
            SimpleAspectAttribute.VisitInfo.ShouldEqual("");
        }

        [TestMethod]
        public void CompositeAspectWorksAsExcepted()
        {
            // --- Act
            _aspectedObject.Operation6();

            // --- Assert
            SimpleAspectAttribute.VisitInfo.ShouldEqual("cEw1Ew2Ew2Qw1QcQw2Sw1ScS");
        }

    }


    public interface IAspectedOps
    {
        void Operation1();
        void Operation2();
        void Operation3();
        void Operation4();
        void Operation5();
        void Operation6();
    }

    public class NoAspectClass: IAspectedOps
    {
        public void Operation1() {}
        public void Operation2() {}
        public void Operation3() {}
        public void Operation4() {}
        public void Operation5()
        {
            throw new InvalidOperationException();
        }
        public void Operation6() { }
    }

    [SimpleAspect("c")]
    public class AspectedClass: IAspectedOps
    {
        [SimpleAspect("1")]
        public void Operation1()
        {
        }

        [SimpleAspect("2", Order = 2)]
        [SimpleAspect("2A", Order = 1)]
        public void Operation2()
        {
        }

        [SimpleAspect("3", Override = true)]
        public void Operation3()
        {
        }

        public void Operation4()
        {
        }

        [SimpleAspect("5")]
        public void Operation5()
        {
            throw new InvalidOperationException();
        }

        [CompositeAspect]
        public void Operation6()
        {
        }
    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
    public class SimpleAspectAttribute: AspectAttributeBase
    {
        public string Arg { get; private set; }

        public static string VisitInfo { get; private set; }

        public static void Reset()
        {
            VisitInfo = "";
        }

        public SimpleAspectAttribute(string arg)
        {
            Arg = arg;
        }

        public override IMethodResultDescriptor OnEntry(IMethodCallDescriptor args, IMethodResultDescriptor result)
        {
            VisitInfo += Arg + "E";
            return base.OnEntry(args, result);
        }

        public override IMethodResultDescriptor OnSuccess(IMethodCallDescriptor args, IMethodResultDescriptor result)
        {
            VisitInfo += Arg + "S";
            return base.OnSuccess(args, result);
        }

        public override Exception OnException(IMethodCallDescriptor argsMessage, Exception exceptionRaised)
        {
            VisitInfo += Arg + "X";
            return base.OnException(argsMessage, exceptionRaised);
        }

        public override void OnExit(IMethodCallDescriptor args, IMethodResultDescriptor result)
        {
            VisitInfo += Arg + "Q";
            base.OnExit(args, result);
        }
    }


    [SimpleAspect("w1", Order = 1)]
    [SimpleAspect("w2", Order = 2)]
    public class CompositeAspect : CompositeAspectAttributeBase
    {
    }
}
