using System;
using System.Collections.Generic;
using McMaster.Extensions.CommandLineUtils;

namespace Beefweb.CommandLineTool.Services;

public interface ITabularWriter
{
    void WriteTable(IReadOnlyCollection<string[]> rows, bool[]? rightAlign = null);

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
                console.Write(' ');
            }

            i++;
        }
    }

    public void WriteTable(IReadOnlyCollection<string[]> rows, bool[]? rightAlign = null)
    {
        var widths = new List<int>();
        rightAlign ??= [];

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
                var padding = widths[i] - value.Length;
                var isRightAlign = i < rightAlign.Length && rightAlign[i];

                if (isRightAlign && padding > 0)
                {
                    console.Out.Write(PaddingData.AsSpan(0, padding));
                }

                console.Out.Write(value);

                if (!isRightAlign && padding > 0)
                {
                    console.Out.Write(PaddingData.AsSpan(0, padding));
                }

                console.Out.Write(' ');
                i++;
            }

            console.Out.WriteLine();
        }
    }
}
