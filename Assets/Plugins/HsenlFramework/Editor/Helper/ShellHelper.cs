using System;
using System.Diagnostics;
using System.Collections.Generic;

namespace Hsenl {
    public static class ShellHelper {
        // 运行一个执行文件
        public static void Run(string exe, string ctx, string workDirectory, List<string> environmentVars = null) {
#if UNITY_EDITOR_OSX
            exe = $"./{exe}";
#else
            exe = $".\\{exe}.exe";
#endif
            var cmd = $"{exe} {ctx}";
            Process process = new();
            try {
#if UNITY_EDITOR_OSX
                const string app = "bash";
                const string splitChar = ":";
                const string arguments = "-c";
#elif UNITY_EDITOR_WIN
                const string app = "cmd.exe";
                const string splitChar = ";";
                const string arguments = "/c";
#endif
                var start = new ProcessStartInfo(app);

                if (environmentVars != null) {
                    foreach (var var in environmentVars) {
                        start.EnvironmentVariables["PATH"] += (splitChar + var);
                    }
                }

                process.StartInfo = start;
                start.Arguments = arguments + " \"" + cmd + "\"";
                start.CreateNoWindow = true;
                start.ErrorDialog = true;
                start.UseShellExecute = false;
                start.WorkingDirectory = workDirectory;

                if (start.UseShellExecute) {
                    start.RedirectStandardOutput = false;
                    start.RedirectStandardError = false;
                    start.RedirectStandardInput = false;
                }
                else {
                    start.RedirectStandardOutput = true;
                    start.RedirectStandardError = true;
                    start.RedirectStandardInput = true;
                    start.StandardOutputEncoding = System.Text.Encoding.UTF8;
                    start.StandardErrorEncoding = System.Text.Encoding.UTF8;
                }

                var endOutput = false;
                var endError = false;

                process.OutputDataReceived += (sender, args) => {
                    if (args.Data != null) {
                        UnityEngine.Debug.Log(args.Data);
                    }
                    else {
                        endOutput = true;
                    }
                };

                process.ErrorDataReceived += (sender, args) => {
                    if (args.Data != null) {
                        UnityEngine.Debug.LogError(args.Data);
                    }
                    else {
                        endError = true;
                    }
                };

                process.Start();
                process.BeginOutputReadLine();
                process.BeginErrorReadLine();

                while (!endOutput || !endError) { }

                process.CancelOutputRead();
                process.CancelErrorRead();
            }
            catch (Exception e) {
                UnityEngine.Debug.LogException(e);
            }
            finally {
                process.Close();
            }
        }
    }
}