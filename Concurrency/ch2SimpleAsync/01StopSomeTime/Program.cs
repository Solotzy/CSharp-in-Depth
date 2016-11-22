﻿/*
 * Task类有一个返回Task对象的静态函数Delay，这个Task对象会在指定的时间后完成
 */
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace _01StopSomeTime
{
    class Program
    {
        static void Main(string[] args)
        {
        }

        //用于 异步成功 测试
        static async Task<T> DelayResult<T>(T result, TimeSpan delay)
        {
            await Task.Delay(delay);
            return result;
        }

        //指数退避 重试策略，重试的延迟时间会逐次增加
        static async Task<string> DownLoadStringWithRetries(string uri)
        {
            using (var client = new HttpClient())
            {
                //第1次重试前等1秒，第2次等2秒，第3次等4秒
                var nextDelay = TimeSpan.FromSeconds(1);
                for (int i = 0; i != 3; ++i)
                {
                    try
                    {
                        return await client.GetStringAsync(uri);
                    }
                    catch
                    {
                    }

                    await Task.Delay(nextDelay);
                    nextDelay = nextDelay + nextDelay;
                }

                //最后重试一次，以便让调用者知道出错信息
                return await client.GetStringAsync(uri);
            }
        }

        //超时功能：如果服务在3秒内没有响应，就返回null
        static async Task<string> DownLoadStringWithTimeout(string uri)
        {
            using (var client = new HttpClient())
            {
                var downloadTask = client.GetStringAsync(uri);
                var timeoutTask = Task.Delay(3000);

                var completedTask = await Task.WhenAny(downloadTask, timeoutTask);
                if (completedTask == timeoutTask)
                    return null;
                return await downloadTask;
            }
        }
    }
}
