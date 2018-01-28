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
            form.TimController = new TIMController(form);
            form.TixController = new TIXController(form, form.TimController);
            form.VramController = new VRAMController(form, form.TimController);
            form.TmdController = new TMDController(form, form.VramController);
            form.ShowDialog();
        }
    }
}