using System;
using System.Collections.Generic;
using McMaster.Extensions.CommandLineUtils;

namespace Beefweb.CommandLineTool.Services;

public interface ITabularWriter
{
    void WriteTable(IReadOnlyCollection<string[]> rows);

    void WriteRow(IReadOnlyCollection<string> values);
}

public sealed class TabularWriter(IConsole console) : ITabularWriter
{
    private const int MaxColumnWidth = 99;
    private static readonly string PaddingData = new(' ', MaxColumnWidth + 1);

    public void WriteRow(IReadOnlyCollection<string> values)
    {
        var i = 0;
        foreach (var value in values)
        {
            if (i == values.Count - 1)
            {
                console.WriteLine(value);
            }
            else
            {
                console.Write(value);
            }

            i++;
        }
    }

    public void WriteTable(IReadOnlyCollection<string[]> rows)
    {
        var widths = new List<int>();

        foreach (var row in rows)
        {
            var i = 0;
            foreach (var value in row)
            {
                while (i >= widths.Count)
                {
                    widths.Add(0);
                }

                var currentWidth = Math.Min(value.Length, MaxColumnWidth);
                if (currentWidth > widths[i])
                {
                    widths[i] = currentWidth;
                }

                i++;
            }
        }

        foreach (var row in rows)
        {
            var i = 0;
            foreach (var value in row)
            {
                console.Out.Write(value);
                console.Out.Write(PaddingData.AsSpan(0, widths[i] - value.Length + 1));
                i++;
            }

            console.Out.WriteLine();
        }
    }
}
