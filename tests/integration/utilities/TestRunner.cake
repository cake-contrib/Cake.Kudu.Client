public static byte[] ReadAllBytes(this Stream stream)
{
    stream.Position = 0;
    var ms = new MemoryStream();
    stream.CopyTo(ms);
    return ms.ToArray();
}

public class TestStatus
{
    public static int Total => Passed + Failed + Skipped;
    public static int Passed { get; set; }
    public static int Failed { get; set;}
    public static int Skipped { get; set; }
    public static bool Success => Total > 0 && Failed == 0;
    public static string Report()
    {
        return $"=== TEST EXECUTION SUMMARY ===\r\n    Total tests: {Total}. Passed: {Passed}. Failed: {Failed}. Skipped: {Skipped}.";
    }
}

public void RunTestTarget(string target)
{
    var testTask = Task($"{target} tests - {DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss")}");

    testTask.Does(ctx =>
    {
        if (TestStatus.Success)
        {
            Information(TestStatus.Report());
        }
        else
        {
            Error(TestStatus.Report());
        }
    });

    foreach(var task in Tasks.OrderBy(t => t.Name, StringComparer.OrdinalIgnoreCase))
    {
        if (task.Name == testTask.Task.Name
            || !task.Name.StartsWith(target, StringComparison.OrdinalIgnoreCase))
            continue;

        testTask.IsDependentOn(task.Name);
    }

    RunTarget(testTask.Task.Name);
    System.Environment.Exit(TestStatus.Failed);
}

public static CakeTaskBuilder<ActionTask> Test(this CakeTaskBuilder<ActionTask> builder, Action action)
{
    return builder.Test(ctx => action());
}

public static CakeTaskBuilder<ActionTask> Test(this CakeTaskBuilder<ActionTask> builder, Action<ICakeContext> action)
{
    return builder.Does(ctx =>
    {
        try
        {
            ctx.Information("[PASSED]");
            action(ctx);
            TestStatus.Passed++;
        }
        catch(Exception ex)
        {
            TestStatus.Failed++;
            ctx.Error("{0}\r\n[FAILED]", ex);
        }
    });
}