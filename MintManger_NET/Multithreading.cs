using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LowUtilities
{
    public static class Multithreading
    {
        //static List<Task> tasks;
        static int _taskCount = 0;
        static int _maxTaskCount = 10;
        public static void AddTask()
        {
            _taskCount += 1;
            Debug.WriteLine(Thread.CurrentThread + "|Task added" + Multithreading.GetTaskCount() + " | " + Multithreading.GetMaxTaskCount());
            if (_taskCount < 0) _taskCount = 0;
        }
        public static void DeleteTask()
        {
            _taskCount -= 1;
            Debug.WriteLine(Thread.CurrentThread + "|Task deleted" + Multithreading.GetTaskCount() + " | " + Multithreading.GetMaxTaskCount());
            if (_taskCount < 0) _taskCount = 0;
        }
        public static int GetTaskCount()
        {
            return _taskCount;
        }
        public static void SetMaxTaskCount(int maxTaskCount)
        {
            _maxTaskCount = maxTaskCount;
        }
        public static int GetMaxTaskCount()
        {
            return _maxTaskCount;
        }
    }
}
