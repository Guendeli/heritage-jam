using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Oryx.Utils
{
    public class TaskUtils
    {
        public static async Task WaitFrame()
        {
            var currnet = Time.frameCount;
 
            while (currnet == Time.frameCount)
            {
                await Task.Yield();
            }
        }
        
        public static async Task WaitForSecondsAsync(float delay)
        {
            await Task.Delay(TimeSpan.FromSeconds(delay));
        }
        
        public static CancellationToken RefreshToken(ref CancellationTokenSource tokenSource) {
            tokenSource?.Cancel();
            tokenSource?.Dispose();
            tokenSource = new CancellationTokenSource();
            return tokenSource.Token;
            
        }
    }
}