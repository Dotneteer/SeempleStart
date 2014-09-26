using System;

namespace SeemplesTools.Deployment.Infrastructure
{
    /// <summary>
    /// A parancs tulajdonságasit leíró attribútum
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class CommandAttribute : Attribute
    {
        /// <summary>
        /// A végrehajtható prancsok listája
        /// </summary>
        public string[] Commands { get; private set; }
        
        /// <summary>
        /// A parancs használatának leírása
        /// </summary>
        public string UsageCommand { get; set; }
        
        /// <summary>
        /// Az argumentumok használatának leírása
        /// </summary>
        public string UsageArguments { get; set; }
        
        /// <summary>
        /// Rejtett parancsról van szó?
        /// </summary>
        public bool Hidden { get; set; }

        /// <summary>
        /// Inicializálja az attribútumot
        /// </summary>
        /// <param name="commands">A parancsok listája</param>
        public CommandAttribute(params string[] commands)
        {
            Commands = commands ?? new string[0];
        }
    }
}
