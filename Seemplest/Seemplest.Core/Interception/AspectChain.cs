using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Seemplest.Core.Interception
{
    /// <summary>
    /// This class represents a chain of aspects that can be passed to the interceptor.
    /// </summary>
    public class AspectChain : IEnumerable<IMethodAspect>
    {
        private readonly List<IMethodAspect> _aspects;

        /// <summary>
        /// Instantiates an aspect chain with a simple item.
        /// </summary>
        /// <param name="aspect">The singleton aspect item</param>
        public AspectChain(IMethodAspect aspect)
        {
            _aspects = new List<IMethodAspect> { aspect };
        }

        /// <summary>
        /// Instantiates an aspect chain with the specified aspects.
        /// </summary>
        /// <param name="aspects">Array of aspects</param>
        public AspectChain(params IMethodAspect[] aspects)
        {
            _aspects = new List<IMethodAspect>(aspects);
        }

        /// <summary>
        /// Instantiates an aspect chain with the specified aspects.
        /// </summary>
        /// <param name="aspects">Collection of aspects</param>
        public AspectChain(IEnumerable<IMethodAspect> aspects)
        {
            _aspects = new List<IMethodAspect>(aspects);
        }

        /// <summary>
        /// Checks whether two aspect chains are equal.
        /// </summary>
        /// <param name="other">The other aspect chain to check for equality</param>
        protected bool Equals(AspectChain other)
        {
            if (_aspects.Count != other._aspects.Count) return false;
            return !_aspects.Where((t, i) => !t.Equals(other._aspects[i]) 
                && !ReferenceEquals(t, other._aspects[i])).Any();
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator"/> object that can be used to
        /// iterate through the collection.
        /// </returns>
        IEnumerator<IMethodAspect> IEnumerable<IMethodAspect>.GetEnumerator()
        {
            return _aspects.GetEnumerator();
        }

        /// <summary>
        /// Determines whether the specified <see cref="T:System.Object"/> is equal to the current 
        /// <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// true if the specified object  is equal to the current object; otherwise, false.
        /// </returns>
        /// <param name="obj">The object to compare with the current object. </param>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.GetType() == GetType() && Equals((AspectChain) obj);
        }

        /// <summary>
        /// Serves as a hash function for a particular type. 
        /// </summary>
        /// <returns>
        /// A hash code for the current <see cref="T:System.Object"/>.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override int GetHashCode()
        {
            return _aspects.GetHashCode();
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator"/> object that can be used to
        /// iterate through the collection.
        /// </returns>
        [ExcludeFromCodeCoverage]
        IEnumerator IEnumerable.GetEnumerator()
        {
            return _aspects.GetEnumerator();
        }

        /// <summary>
        /// Retreives the items of this aspect chain in reverse order.
        /// </summary>
        public IEnumerable<IMethodAspect> Reverse
        {
            get
            {
                for (var i = _aspects.Count - 1; i >= 0; i--)
                {
                    yield return _aspects[i];
                }
            }
        }
    }
}