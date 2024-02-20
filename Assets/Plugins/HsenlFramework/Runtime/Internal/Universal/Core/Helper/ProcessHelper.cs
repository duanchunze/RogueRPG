using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Path = System.IO.Path;

namespace Hsenl {
    public static class ProcessHelper {
        public static Process Run(string exe, string arguments, string workingDirectory = ".", bool waitExit = false) {
            //Log.Debug($"Process Run exe:{exe} ,arguments:{arguments} ,workingDirectory:{workingDirectory}");
            try {
                var redirectStandardOutput = true;
                var redirectStandardError = true;
                var useShellExecute = false;
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) {
                    redirectStandardOutput = false;
                    redirectStandardError = false;
                    useShellExecute = true;
                }

                if (waitExit) {
                    redirectStandardOutput = true;
                    redirectStandardError = true;
                    useShellExecute = false;
                }

                var info = new ProcessStartInfo {
                    FileName = exe,
                    Arguments = arguments,
                    CreateNoWindow = true,
                    UseShellExecute = useShellExecute,
                    WorkingDirectory = workingDirectory,
                    RedirectStandardOutput = redirectStandardOutput,
                    RedirectStandardError = redirectStandardError,
                };

                var process = Process.Start(info);

                if (waitExit) {
                    WaitExitAsync(process).Tail();
                }

                return process;
            }
            catch (Exception e) {
                throw new Exception($"dir: {Path.GetFullPath(workingDirectory)}, command: {exe} {arguments}", e);
            }
        }

        private static async HTask WaitExitAsync(Process process) {
#if UNITY
            await process.WaitForExitAsync();
            UnityEngine.Debug.Log($"process exit, exitcode: {process.ExitCode} {process.StandardOutput.ReadToEnd()} {process.StandardError.ReadToEnd()}");
#else
            await HTask.Completed;
            throw new Exception("only use in unity");
#endif
        }

#if UNITY
        private static async Task WaitForExitAsync(this Process self)
        {
            if (!self.HasExited)
            {
                return;
            }

            try
            {
                self.EnableRaisingEvents = true;
            }
            catch (InvalidOperationException)
            {
                if (self.HasExited)
                {
                    return;
                }
                throw;
            }

            var tcs = new TaskCompletionSource<bool>();

            void Handler(object s, EventArgs e) => tcs.TrySetResult(true);
            
            self.Exited += Handler;

            try
            {
                if (self.HasExited)
                {
                    return;
                }
                await tcs.Task;
            }
            finally
            {
                self.Exited -= Handler;
            }
        }
#endif
    }
}