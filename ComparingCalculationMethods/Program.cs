﻿
using System.Data;
using System.Diagnostics;

using Task = System.Threading.Tasks.Task;
using Thread = System.Threading.Thread;
using Barrier = System.Threading.Barrier;
using Monitor = System.Threading.Monitor;
using IDisposable = System.IDisposable;
using TaskEnum = System.Collections.Generic.IEnumerable<System.Threading.Tasks.Task>;
using TaskQueue = System.Collections.Generic.Queue<System.Threading.Tasks.Task>;
using Enumerable = System.Linq.Enumerable;
using ObjectDisposedException = System.ObjectDisposedException;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;


using System.Data;
using System.Globalization;

using static System.Net.Mime.MediaTypeNames;
using System.Diagnostics;
using Npgsql;
{


    //for (int i =0; i < 5; i++)
    {     
    Stopwatch sw = Stopwatch.StartNew();
        int UpperCount = 10000;
        Serial(UpperCount);
        Console.WriteLine();
        Console.WriteLine("Serial: {0:f2} s", sw.Elapsed.TotalSeconds);

        sw = Stopwatch.StartNew();

        ParallelFor(UpperCount);
        Console.WriteLine();
        Console.WriteLine("Parallel.For: {0:f2} s", sw.Elapsed.TotalSeconds);

        sw = Stopwatch.StartNew();

        CustomParallelExtractedMaxHalfParallelism(UpperCount);
        Console.WriteLine();
        Console.WriteLine("Custom parallel (extracted max, half parallelism): {0:f2} s", sw.Elapsed.TotalSeconds);
    }


}






static void Serial(int j)
{
    List<Task> tasks = new List<Task>();
    Object lockObj = new Object();
    int outputItem = 0;
    for (int tCtr = 0; tCtr <= 4; tCtr++)
    {
        int iteration = tCtr;
        Task t = Task.Run(() => {

            for (int i = 0; i < (j / 5); i++)
            {
               
                lock (lockObj)
                {
                    doStuff("Task" + i.ToString());
                    //Console.Write("{0} in task t-{1} on thread {2}   ",
                    //              i, iteration, Thread.CurrentThread.ManagedThreadId);
                    //outputItem++;
                    //if (outputItem % 3 == 0)
                    //    Console.WriteLine();
                }

            }
            

        });
        tasks.Add(t);
    }

    ExecuteJob(tasks.ToArray());



   
}
 static async void ExecuteJob(Task[] test)
{

    await Task.WhenAll(test).ConfigureAwait(false);
}
static void ParallelFor(int j)
{
    Parallel.For(
        0, j, i => { doStuff("test2"); });

    
}

 static NpgsqlConnection GetConnection()
{
    var connection = System.Configuration.ConfigurationManager.ConnectionStrings["ConnStr"].ConnectionString;
    string ConnStr = connection;
    return new NpgsqlConnection(ConnStr);

}

static void doStuff(string strName)
{

    using (NpgsqlConnection conn = GetConnection())
    {
        conn.Open();
        if (conn.State == ConnectionState.Open)
        {/* label1.Text = "Connected";*/ }
        var cmd2 = new NpgsqlCommand("INSERT INTO COMPANY ( NAME,AGE,ADDRESS,SALARY) VALUES ('Jan', 32, 'California', 28000.00 );", conn);
        var da = new NpgsqlDataAdapter(cmd2);
        var ds = new DataSet();
        NpgsqlDataReader dr = cmd2.ExecuteReader();

        dr.Close();
        conn.Close();

    }

    using (NpgsqlConnection conn = GetConnection())
    {
        conn.Open();

        var currentCulture = Thread.CurrentThread.CurrentCulture;
        try
        {
            Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture("en-us");

            var strTime = DateTime.Now.ToLongTimeString();
            var strDateString = DateTime.Now.ToShortDateString().Replace("/ ", "-");

            // Do something with shortDateString...
        }
        finally
        {
            Thread.CurrentThread.CurrentCulture = currentCulture;
        }

        var dtNow = new DateTime();
        string TaskID = "";

        Random rnd = new Random();

        int rndNum = rnd.Next(1000000);

        var shortDateString = dtNow.ToString();
        var strSQL = @"insert into TaskResults(TaskID, TaskName, TaskDate) values('";
        strSQL += rndNum.ToString() + "','" + strName.ToString();
        strSQL += "','" + shortDateString + "'" + ")";

        var cmd2 = new NpgsqlCommand(strSQL, conn);
        int nNoAdded = cmd2.ExecuteNonQuery();

        conn.Close();

    }


    //MessageBox.Show(strName);

    Thread.Yield();

}

static void CustomParallelExtractedMaxHalfParallelism(int j)
{
    var degreeOfParallelism = Environment.ProcessorCount / 2;

    var tasks = new Task[degreeOfParallelism];

    for (int taskNumber = 0; taskNumber < degreeOfParallelism; taskNumber++)
    {
       
        int taskNumberCopy = taskNumber;

        tasks[taskNumber] = Task.Factory.StartNew(
            () =>
            {
                var max = j * (taskNumberCopy + 1) / degreeOfParallelism;
                for (int i = j * taskNumberCopy / degreeOfParallelism;
                    i < max;
                    i++)
                {
                    doStuff("test3");
                }
            });
    }

    Task.WaitAll(tasks);
}

