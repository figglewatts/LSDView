using System;
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
            form.ShowDialog();
        }
    }
}