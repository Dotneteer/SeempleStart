using System;

namespace SeemplesTools.Deployment.Common
{
    /// <summary>
    /// Ez az osztály az előrehaladás kezeléséhez kínál hasznos műveleteket
    /// </summary>
    public static class ProgressHelper
    {
        /// <summary>
        /// Az előrehaladás maximális tartománya
        /// </summary>
        public const int RANGE = 32768;

        /// <summary>
        /// Az előrehaladás mértékét adja vissza százalékosan
        /// </summary>
        /// <param name="currentCommand">Az aktuális parancs sorszáma</param>
        /// <param name="totalCommands">Az összes parancs száma</param>
        /// <param name="commandProgress">Az aktuális parancs előrehaladása</param>
        /// <returns></returns>
        public static int GetProgress(int currentCommand, int totalCommands, int commandProgress)
        {
            if (totalCommands <= 0)
            {
                throw new ArgumentOutOfRangeException("totalCommands");
            }

            if (currentCommand < 0 || currentCommand > totalCommands)
            {
                throw new ArgumentOutOfRangeException("currentCommand");
            }

            if (commandProgress < 0 || commandProgress > RANGE)
            {
                throw new ArgumentOutOfRangeException("commandProgress");
            }

            if (currentCommand == totalCommands) return RANGE;

            var total = RANGE;

            var start = 0;
            while (currentCommand > 0)
            {
                var skip = total / totalCommands;
                start += skip;
                total -= skip;
                currentCommand--;
                totalCommands--;
            }

            return start + commandProgress * (total / totalCommands) / (RANGE);
        }
    }
}
