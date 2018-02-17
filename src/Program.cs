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
            ExportController exportController = new ExportController(form);
            OutlineController outlineController = new OutlineController(form);
            DocumentController documentController = new DocumentController(form, outlineController, exportController);
            TIMController timController = new TIMController(form, documentController);
            TIXController tixController = new TIXController(form, timController, documentController);
            VRAMController vramController = new VRAMController(form, timController);
            TMDController tmdController = new TMDController(form, vramController, documentController);
            LBDController lbdController = new LBDController(form, vramController, documentController);
            MOMController momController = new MOMController(form, vramController, documentController);
            SaveFileDialogController saveFileDialogController = new SaveFileDialogController(form);

            outlineController.TMDController = tmdController;
            outlineController.TIMController = timController;
            outlineController.TIXController = tixController;
            outlineController.MOMController = momController;
            outlineController.LBDController = lbdController;

            exportController.DocumentController = documentController;
            exportController.SaveFileDialogController = saveFileDialogController;
            exportController.TMDController = tmdController;
            exportController.TIMController = timController;
            exportController.TIXController = tixController;
            exportController.MOMController = momController;
            exportController.LBDController = lbdController;

            form.TimController = timController;
            form.TixController = tixController;
            form.VramController = vramController;
            form.OutlineController = outlineController;
            form.DocumentController = documentController;
            form.TmdController = tmdController;
            form.LbdController = lbdController;
            form.MomController = momController;
            form.ExportController = exportController;

            form.ShowDialog();
        }
    }
}