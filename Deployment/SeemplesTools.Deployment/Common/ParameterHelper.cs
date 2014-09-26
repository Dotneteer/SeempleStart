using System;
using SeemplesTools.Deployment.Infrastructure;

namespace SeemplesTools.Deployment.Common
{
    /// <summary>
    /// Ez az osztály a paraméterek kezeléséhez kínál hasznos műveleteket
    /// </summary>
    public static class ParameterHelper
    {
        /// <summary>
        /// Ellenőrzi és visszadja egy kötelező paraméter értékét
        /// </summary>
        /// <param name="argument">Az argumentum értéke</param>
        /// <param name="original">A parancs</param>
        /// <returns>Az argumentum értéke, ha a kötelező argumentum ki volt töltve</returns>
        public static string Mandatory(string argument, string original)
        {
            if (argument == null)
            {
                throw new ParameterException(String.Format("Parameter {0} requires an argument.", original));
            }
            return argument;
        }
    }
}
