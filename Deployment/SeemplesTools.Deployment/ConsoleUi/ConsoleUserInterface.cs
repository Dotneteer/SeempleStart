using System;
using System.Runtime.InteropServices;
using SeemplesTools.Deployment.Common;
using SeemplesTools.Deployment.Infrastructure;

namespace SeemplesTools.Deployment.ConsoleUi
{
    public class ConsoleUserInterface : IDeploymentUserInterface
    {
        #region Interface to native DLLs

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr GetStdHandle(int nStdHandle);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern int GetFileType(IntPtr handle);

        private const int STD_OUTPUT_HANDLE = -11;
        private const int FILE_TYPE_CHAR = 2;

        #endregion

        #region Initialization

        private readonly int _lineLength;
        private readonly string _carriageReturn;
        private readonly string _newLine;

        public ConsoleUserInterface()
        {
            var stdOutHandle = GetStdHandle(STD_OUTPUT_HANDLE);
            if (stdOutHandle == IntPtr.Zero ||
                stdOutHandle == IntPtr.Zero - 1 ||
                GetFileType(stdOutHandle) != FILE_TYPE_CHAR)
            {
                _lineLength = 0;
                _carriageReturn = String.Empty;
                _newLine = Environment.NewLine;
            }
            else
            {
                _lineLength = System.Console.WindowWidth - 1;
                _carriageReturn = "\r";
                _newLine = String.Empty;
            }
        }

        #endregion

        #region Progress

        public void StartProgress()
        {
        }

        public void SetProgress(int progress, string progressText)
        {
            var message = String.Format("[{0}%] {1}", progress * 100 / ProgressHelper.RANGE, progressText);
            if (_lineLength > 0)
            {
                message = message.Length > _lineLength 
                    ? message.Substring(0, _lineLength) 
                    : message.PadRight(_lineLength);
            }
            System.Console.Write(_carriageReturn + message + _newLine);
            System.Console.Out.Flush();
        }

        public void EndProgress()
        {
            if (_lineLength > 0)
            {
                System.Console.WriteLine(_carriageReturn + String.Empty.PadRight(_lineLength) + _carriageReturn);
            }
            else
            {
                System.Console.WriteLine(_newLine);
            }
            System.Console.Out.Flush();
        }

        #endregion
    }
}
