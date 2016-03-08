using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using log4net;

namespace Helpers
{
    public static class Helper
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public static bool LoopAttempt<T1>(int maxAttempts, int timeoutSeconds, T1 state, Func<T1, bool> haltCondition, Func<T1> attemptedAction, Func<Exception, T1> exceptionAction = null, bool forceFirstTry = false)
        {
            int attempts = maxAttempts;
            bool halt = haltCondition(state);
            while ((!halt && attempts > 0) || forceFirstTry)
            {
                forceFirstTry = false;
                attempts--;
                log.InfoFormat("\tAttempt {0}", maxAttempts - attempts);
                var lastattempt = DateTime.UtcNow;

                try
                {
                    state = attemptedAction();
                }
                catch (Exception e)
                {
                    if (exceptionAction != null) state = exceptionAction(e);
                }
                finally
                {
                    var duration = DateTime.UtcNow - lastattempt;
                    var secondsLeft = timeoutSeconds - duration.Seconds;
                    halt = haltCondition(state);
                    if (!halt && secondsLeft > 0)
                    {
                        Thread.Sleep(secondsLeft * 1000);
                    }
                }
            }

            return halt;
        }

        public static bool StartTask(Action action, TimeSpan timeout)
        {
            var actualTask = new Task<bool>(() =>
            {
                var longRunningTask = new Task(() =>
                {
                        action();
                }, TaskCreationOptions.LongRunning);

                longRunningTask.Start();

                if (longRunningTask.Wait(timeout)) return true;

                return false;
            });

            actualTask.Start();

            actualTask.Wait();
            return actualTask.Result;
        }

        public static void RaiseSafely<T>(object sender, EventHandler<T> evt, T eventArgs)
        {
            var tmp = evt;
            if (tmp != null)
            {
                try
                {
                    tmp(sender, eventArgs);
                }
                catch (Exception e)
                {
                    log.Error(string.Format("Error raising {0} for {1}", tmp.Method.Name, eventArgs), e);
                }
            }
        }
        

        public static CancellationTokenSource StartRepetativeTask(Action action, TimeSpan repeatDelay)
        {
            CancellationTokenSource cancellationSource = new CancellationTokenSource();
            var task = new Task(new Action<object> (async (ctobject) => {
                var ct = ((CancellationToken)(ctobject));

                while (!ct.IsCancellationRequested)
                {
                    action();
                    await Task.Delay(repeatDelay, ct);
                }

            }), cancellationSource.Token, TaskCreationOptions.LongRunning);
            task.Start();
            return cancellationSource;
        }

        public static void StartProcess(string name, string args, int timeoutInMS)
        {
            var p = StartProcess(name, args);
            Task.Run(async delegate  { 
                await Task.Delay(timeoutInMS);
                p.Kill();
                log.InfoFormat("Killed {0} {1} {2}", p.Id, name, args);
            });
        }
        public static Process StartProcess(string name, string args = null)
        {
            log.InfoFormat("{0} {1}", name, args ?? string.Empty);
            var p = new Process();
            p.StartInfo = new ProcessStartInfo()
            {
                FileName = name,
                Arguments = args ?? string.Empty,
                //CreateNoWindow = true,
                //WindowStyle = ProcessWindowStyle.Normal,
                CreateNoWindow = false,
                UseShellExecute = true,
                WindowStyle = ProcessWindowStyle.Minimized
            };
            try
            {
                p.Start();
                return p;
            }
            catch (Exception e) { return null; }
        }
    }
}
