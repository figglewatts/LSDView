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
            TIMController timController = new TIMController(form);
            TIXController tixController = new TIXController(form, timController);
            VRAMController vramController = new VRAMController(form, timController);
            OutlineController outlineController = new OutlineController(form);
            DocumentController documentController = new DocumentController(form, outlineController);
            TMDController tmdController = new TMDController(form, vramController, documentController);
            LBDController lbdController = new LBDController(form, vramController);
            MOMController momController = new MOMController(form, vramController);
            outlineController.TMDController = tmdController;

            form.TimController = timController;
            form.TixController = tixController;
            form.VramController = vramController;
            form.OutlineController = outlineController;
            form.DocumentController = documentController;
            form.TmdController = tmdController;
            form.LbdController = lbdController;
            form.MomController = momController;

            form.ShowDialog();
        }
    }
}