using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace SeemplesTools.Deployment.Common
{
    /// <summary>
    /// Ez az osztály egy adatbázis-változatot ír le.
    /// </summary>
    public class DatabaseVersion : IComparable<DatabaseVersion>
    {
        #region Properties

        private List<int> _components;
        private string _originalVersion;

        #endregion

        #region Initialization

        /// <summary>
        /// Egy üres változatot hoz létre.
        /// </summary>
        private DatabaseVersion()
        {
        }

        /// <summary>
        /// Egy komponensből álló változatot hoz létre
        /// </summary>
        /// <param name="first">Az első komponens értéke</param>
        public DatabaseVersion(int first)
        {
            if (first < 0)
            {
                throw new ArgumentOutOfRangeException("first");
            }

            _components = new List<int> {first};
            _originalVersion = String.Format(CultureInfo.InvariantCulture, "{0}", first);
        }

        /// <summary>
        /// Két komponensből álló változatot hoz létre
        /// </summary>
        /// <param name="first">Az első komponens értéke</param>
        /// <param name="second">A második komponens értéke</param>
        public DatabaseVersion(int first, int second)
        {
            if (first < 0)
            {
                throw new ArgumentOutOfRangeException("first");
            }

            if (second < 0)
            {
                throw new ArgumentOutOfRangeException("second");
            }

            _components = new List<int> {first};
            if (second > 0)
            {
                _components.Add(second);
            }
            _originalVersion = String.Format(CultureInfo.InvariantCulture, "{0}.{1}", first, second);
        }

        /// <summary>
        /// Három komponensből álló változatot hoz létre
        /// </summary>
        /// <param name="first">Az első komponens értéke</param>
        /// <param name="second">A második komponens értéke</param>
        /// <param name="third">A harmadik komponens értéke</param>
        public DatabaseVersion(int first, int second, int third)
        {
            if (first < 0)
            {
                throw new ArgumentOutOfRangeException("first");
            }

            if (second < 0)
            {
                throw new ArgumentOutOfRangeException("second");
            }

            if (third < 0)
            {
                throw new ArgumentOutOfRangeException("third");
            }

            _components = new List<int> {first};

            if (second > 0 || third > 0)
            {
                _components.Add(second);
            }
            if (third > 0)
            {
                _components.Add(third);
            }
            _originalVersion = String.Format(CultureInfo.InvariantCulture, "{0}.{1}.{2}", first, second, third);
        }

        /// <summary>
        /// Egy szövegként megadott változatot kísérel meg értelmezni.
        /// </summary>
        /// <param name="value">A változat szöveges reprezentációja</param>
        /// <param name="result">A változat értéke</param>
        /// <returns>Sikerült az értelmezés?</returns>
        public static bool TryParse(string value, out DatabaseVersion result)
        {
            if (value == null)
            {
                result = null;
                return false;
            }

            var components = value.Split('.');

            if (components.Length > 3)
            {
                result = null;
                return false;
            }

            var componentsInt = new List<int>();

            foreach (var component in components)
            {
                int componentInt;
                if (!Int32.TryParse(component, NumberStyles.None, CultureInfo.InvariantCulture, out componentInt))
                {
                    result = null;
                    return false;
                }

                componentsInt.Add(componentInt);
            }

            while (componentsInt.Count > 1 && componentsInt[componentsInt.Count - 1] == 0)
            {
                componentsInt.RemoveAt(componentsInt.Count - 1);
            }

            result = new DatabaseVersion
            {
                _components = componentsInt, 
                _originalVersion = value
            };

            return true;
        }

        #endregion

        #region Comparison

        /// <summary>
        /// Összehasonlítja ezt a változatot egy másikkal
        /// </summary>
        /// <param name="other">A másik változat</param>
        /// <returns></returns>
        public int CompareTo(DatabaseVersion other)
        {
            if (ReferenceEquals(other, null)) return 1;

            var commonCount = Math.Min(_components.Count, other._components.Count);

            for (var i = 0; i < commonCount; i++)
            {
                if (_components[i] < other._components[i]) return -1;
                if (_components[i] > other._components[i]) return 1;
            }

            if (_components.Count > commonCount) return 1;
            if (other._components.Count > commonCount) return -1;
            return 0;
        }

        /// <summary>
        /// Egyezik ez a változat az átadott objektummal?
        /// </summary>
        /// <param name="obj">A másik objektum</param>
        /// <returns>Van egyezés?</returns>
        public override bool Equals(object obj)
        {
            if (!(obj is DatabaseVersion)) return false;
            return CompareTo((DatabaseVersion)obj) == 0;
        }

        /// <summary>
        /// "Egyenlő" operátor
        /// </summary>
        /// <param name="left">Bal operandus</param>
        /// <param name="right">Jobb operandus</param>
        /// <returns>A művelet eredménye</returns>
        public static bool operator ==(DatabaseVersion left, DatabaseVersion right)
        {
            if (ReferenceEquals(left, null))
                return ReferenceEquals(right, null);

            return left.CompareTo(right) == 0;
        }

        /// <summary>
        /// "Nem egyenlő" operátor
        /// </summary>
        /// <param name="left">Bal operandus</param>
        /// <param name="right">Jobb operandus</param>
        /// <returns>A művelet eredménye</returns>
        public static bool operator !=(DatabaseVersion left, DatabaseVersion right)
        {
            if (ReferenceEquals(left, null))
                return !ReferenceEquals(right, null);

            return left.CompareTo(right) != 0;
        }

        /// <summary>
        /// "Kisebb" operátor
        /// </summary>
        /// <param name="left">Bal operandus</param>
        /// <param name="right">Jobb operandus</param>
        /// <returns>A művelet eredménye</returns>
        public static bool operator <(DatabaseVersion left, DatabaseVersion right)
        {
            if (ReferenceEquals(left, null))
                return !ReferenceEquals(right, null);

            return left.CompareTo(right) < 0;
        }

        /// <summary>
        /// "Nagyobb" operátor
        /// </summary>
        /// <param name="left">Bal operandus</param>
        /// <param name="right">Jobb operandus</param>
        /// <returns>A művelet eredménye</returns>
        public static bool operator >(DatabaseVersion left, DatabaseVersion right)
        {
            if (ReferenceEquals(left, null))
                return false;

            return left.CompareTo(right) > 0;
        }

        /// <summary>
        /// "Kisebb vagy egyenlő" operátor
        /// </summary>
        /// <param name="left">Bal operandus</param>
        /// <param name="right">Jobb operandus</param>
        /// <returns>A művelet eredménye</returns>
        public static bool operator <=(DatabaseVersion left, DatabaseVersion right)
        {
            if (ReferenceEquals(left, null))
                return true;

            return left.CompareTo(right) <= 0;
        }

        /// <summary>
        /// "Nagyobb vagy egyenlő" operátor
        /// </summary>
        /// <param name="left">Bal operandus</param>
        /// <param name="right">Jobb operandus</param>
        /// <returns>A művelet eredménye</returns>
        public static bool operator >=(DatabaseVersion left, DatabaseVersion right)
        {
            if (ReferenceEquals(left, null))
                return ReferenceEquals(right, null);

            return left.CompareTo(right) >= 0;
        }

        /// <summary>
        /// Összehasonlítja a két átadott változatot
        /// </summary>
        /// <param name="left">Bal operandus</param>
        /// <param name="right">Jobb operandus</param>
        /// <returns>A művelet eredménye</returns>
        public static int Compare(DatabaseVersion left, DatabaseVersion right)
        {
            if (ReferenceEquals(left, null))
                return ReferenceEquals(right, null) ? 0 : -1;

            return left.CompareTo(right);
        }

        /// <summary>
        /// A változat hash kódját adja vissza
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            // ReSharper disable once NonReadonlyFieldInGetHashCode
            return String.Join(".", _components.Select(c => c.ToString(CultureInfo.InvariantCulture))).GetHashCode();
        }

        #endregion

        #region ToString

        /// <summary>
        /// A változat szöveges reprezentációját adja vissza.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return _originalVersion;
        }

        #endregion

        #region Versions

        /// <summary>
        /// A következő adatbázis-változatot adja vissza
        /// </summary>
        public DatabaseVersion Next
        {
            get
            {
                return new DatabaseVersion(
                    _components[0],
                    _components.Count >= 2 ? _components[1] : 0,
                    _components.Count >= 3 ? _components[2] + 1 : 1);
            }
        }

        /// <summary>
        /// A minimális adatbázis-változatot adja vissza
        /// </summary>
        public static DatabaseVersion MinValue
        {
            get { return new DatabaseVersion(0); }
        }

        /// <summary>
        /// A maximális adatbázis-változatot adja vissza
        /// </summary>
        public static DatabaseVersion MaxValue
        {
            get { return new DatabaseVersion(Int32.MaxValue, Int32.MaxValue, Int32.MaxValue); }
        }

        #endregion
    }
}
