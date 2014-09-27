using System.Collections.Generic;
using System.Reflection;
using SeemplestBlocks.Deployment.Commands;
using SeemplesTools.Deployment.ConsoleUi;
using SeemplesTools.Deployment.Infrastructure;
using Setup.Commands;

namespace Setup
{
    class Program
    {
        static void Main(string[] args)
        {
            ConsoleRunner.Run(args,
                new List<Assembly>
                {
                    typeof (Command).Assembly,
                    typeof (DeploySbDatabaseCommand).Assembly,
                    typeof (DeployYwDatabaseCommand).Assembly
                });
        }
    }
}
