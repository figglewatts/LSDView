using System;
using LSDView.controller;
using LSDView.view;

public class Program
{
    /// <summary>
    /// Entry point of this example.
    /// </summary>
    [STAThread]
    public static void Main()
    {
        OpenTK.Toolkit.Init();
        using (LSDViewForm form = new LSDViewForm())
        {
            form.TmdController = new TMDController(form);
            form.ShowDialog();
        }
    }
}