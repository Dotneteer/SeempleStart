using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Seemplest.Core.Interception;
using SoftwareApproach.TestingExtensions;

namespace Seemplest.Core.UnitTests.Interception
{
    [TestClass]
    public class AspectChainTest
    {
        [TestMethod]
        public void ConstructionWorksWithSingleAspect()
        {
            // --- Arrange
            var chain = new AspectChain(new Aspect1());

            // --- Assert
            chain.ShouldHaveCountOf(1);
        }

        [TestMethod]
        public void ConstructionWorksWithMultipleAspect1()
        {
            // --- Arrange
            var chain = new AspectChain(new Aspect1(), new Aspect2());

            // --- Assert
            chain.ShouldHaveCountOf(2);
        }

        [TestMethod]
        public void ConstructionWorksWithMultipleAspect2()
        {
            // --- Arrange
            var chain = new AspectChain(
                new List<IMethodAspect>
                    {
                        new Aspect1(), 
                        new Aspect2(),
                        new Aspect3()
                    });

            // --- Assert
            chain.ShouldHaveCountOf(3);
        }

        [TestMethod]
        public void EqualsWorksAsExpected()
        {
            // --- Arrange
            var aspect1 = new Aspect1();
            var aspect2 = new Aspect2();
            var aspect3 = new Aspect3();

            var chain = new AspectChain(
                new List<IMethodAspect>
                    {
                        aspect1,
                        aspect2,
                        aspect3
                    });
            var otherChain = new AspectChain(new DummyAspect(1), new DummyAspect(2));

            // --- Assert
            chain.Equals(null).ShouldBeFalse();
            // ReSharper disable EqualExpressionComparison
            chain.Equals(chain).ShouldBeTrue();
            // ReSharper restore EqualExpressionComparison
            chain.Equals(new AspectChain(
                new List<IMethodAspect>
                    {
                        aspect1,
                        aspect2,
                        aspect3
                    })).ShouldBeTrue();
            chain.Equals(new AspectChain(
                new List<IMethodAspect>
                    {
                        aspect1,
                        aspect2
                    })).ShouldBeFalse();
            // ReSharper disable SuspiciousTypeConversion.Global
            otherChain.Equals("hey").ShouldBeFalse();
            // ReSharper restore SuspiciousTypeConversion.Global
            // ReSharper disable EqualExpressionComparison
            otherChain.Equals(otherChain).ShouldBeTrue();
            // ReSharper restore EqualExpressionComparison
            otherChain.Equals(new AspectChain(new DummyAspect(1), new DummyAspect(2)))
                      .ShouldBeTrue();
            otherChain.Equals(new AspectChain(new DummyAspect(2), new DummyAspect(3)))
                      .ShouldBeFalse();
        }

        [TestMethod]
        public void GetHashCodeWorksAsExpected()
        {
            // --- Arrange
            var chain = new AspectChain(new DummyAspect(1), new DummyAspect(2));

            // --- Act
            var hashCode = chain.GetHashCode();

            // --- Assert
            hashCode.ShouldNotEqual(0);
        }
    }

    internal class DummyAspect: IMethodAspect 
    {
        public int Value { get; private set; }

        public DummyAspect(int value)
        {
            Value = value;
        }

        protected bool Equals(DummyAspect other)
        {
            return Value == other.Value;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.GetType() == GetType() && Equals((DummyAspect) obj);
        }

        public override int GetHashCode()
        {
            return Value;
        }

        public IMethodResultDescriptor OnEntry(IMethodCallDescriptor args, IMethodResultDescriptor result)
        {
            return result;
        }

        public void OnExit(IMethodCallDescriptor args, IMethodResultDescriptor result)
        {
        }

        public IMethodResultDescriptor OnSuccess(IMethodCallDescriptor args, IMethodResultDescriptor result)
        {
            return result;
        }

        public Exception OnException(IMethodCallDescriptor argsMessage, Exception exceptionRaised)
        {
            return exceptionRaised;
        }
    }
}
