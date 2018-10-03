using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameOuput
{
    static IOutputabble output = new ConsoleOutput();

    public static void Output(string message)
    {
        output.Output(message);
    }

    public static void UseUnityDebug()
    {
        output = new UnityDebugOutput();
    }

    public static void SetOutput(IOutputabble output)
    {
        GameOuput.output = output;
    }
}

public interface IOutputabble
{
    void Output(string output);
}

public class ConsoleOutput : IOutputabble
{
    public void Output(string output)
    {
        Console.WriteLine(output);
    }
}

public class UnityDebugOutput : IOutputabble
{
    public void Output(string output)
    {
        Debug.Log(output);
    }
}
