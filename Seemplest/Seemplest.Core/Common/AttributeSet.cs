using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Linq;

namespace Seemplest.Core.Common
{
    /// <summary>
    /// This class describes an attribute set belongign to a <see cref="MemberInfo"/> instance
    /// </summary>
    public sealed class AttributeSet
    {
        private readonly Dictionary<MemberInfo, List<Attribute>> _attributes =
            new Dictionary<MemberInfo, List<Attribute>>();

        /// <summary>
        /// Gets the member info owning this attribute set.
        /// </summary>
        public MemberInfo Owner { get; private set; }

        /// <summary>
        /// Creates a new attribute set from the specified <see cref="MemberInfo"/> instance
        /// </summary>
        /// <param name="memberInfo">Member info to scan for attributes</param>
        /// <param name="attrType">Attributes deriving from this type are collected only</param>
        /// <param name="scanBaseTypes">Scan the inheritance chain?</param>
        public AttributeSet(MemberInfo memberInfo, Type attrType = null, bool scanBaseTypes = false)
        {
            if (memberInfo == null) throw new ArgumentNullException("memberInfo");
            Owner = memberInfo;
            var attrs = (attrType == null
                             ? memberInfo.GetCustomAttributes(scanBaseTypes)
                             : memberInfo.GetCustomAttributes(attrType, scanBaseTypes));
            Debug.Assert(attrs != null, "attrs != null");
            foreach (var attr in attrs)
            {
                List<Attribute> attrList;
                if (!_attributes.TryGetValue(attr.GetType(), out attrList))
                {
                    attrList = new List<Attribute>();
                    _attributes.Add(attr.GetType(), attrList);
                }
                attrList.Add(attr as Attribute);
            }
        }

        /// <summary>
        /// Gets the value of the specified attribute
        /// </summary>
        /// <typeparam name="TAttr">Attribute type</typeparam>
        /// <returns>
        /// The specified attribute instance
        /// </returns>
        public TAttr Single<TAttr>()
            where TAttr : class
        {
            List<Attribute> attrs;
            if (!_attributes.TryGetValue(typeof (TAttr), out attrs))
            {
                throw new KeyNotFoundException(
                    String.Format("The specified {0} attribute memberInfo cannot be found in {1}",
                        typeof (TAttr), Owner));
            }
            if (attrs.Count != 1)
            {
                throw new InvalidOperationException(
                    String.Format("More than one {0} attribute instance found in {1}",
                        typeof (TAttr), Owner));
            }
            return attrs[0] as TAttr;
        }

        /// <summary>
        /// Gets the value of the specified optional attribute
        /// </summary>
        /// <typeparam name="TAttr">Attribute type</typeparam>
        /// <returns>
        /// The specified attribute instance
        /// </returns>
        public TAttr Optional<TAttr>(TAttr defaultValue = null)
            where TAttr : class
        {
            List<Attribute> attrs;
            if (!_attributes.TryGetValue(typeof (TAttr), out attrs))
            {
                return defaultValue;
            }
            if (attrs.Count != 1)
            {
                throw new InvalidOperationException(
                    String.Format("More than one {0} attribute instance found in {1}",
                        typeof (TAttr), Owner));
            }
            return attrs[0] as TAttr;
        }

        /// <summary>
        /// Gets all attributes with the specified type.
        /// </summary>
        /// <typeparam name="TAttr">Type of attributes to retrieve.</typeparam>
        /// <returns>List of attributes with the specified type.</returns>
        public List<TAttr> All<TAttr>()
        {
            List<Attribute> attrs;
            return !_attributes.TryGetValue(typeof(TAttr), out attrs) 
                ? new List<TAttr>() 
                : attrs.Cast<TAttr>().ToList();
        }
    }
}