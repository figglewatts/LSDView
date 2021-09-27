using System.Collections.Generic;
using libLSD.Formats;
using LSDView.Controllers.Interface;
using LSDView.Models;
using LSDView.Util;

namespace LSDView.Controllers.Headless
{
    public class HeadlessTIXController : IFileFormatController<TIX, TIXDocument>
    {
        protected readonly IFileFormatController<TIM, TIMDocument> _timController;

        public HeadlessTIXController(IFileFormatController<TIM, TIMDocument> timController)
        {
            _timController = timController;
        }

        public TIX Load(string path) { return LibLSDUtil.LoadTIX(path); }

        public TIXDocument CreateDocument(TIX tix)
        {
            List<TIMDocument> tims = new List<TIMDocument>();
            foreach (var tim in tix.AllTIMs)
            {
                tims.Add(_timController.CreateDocument(tim));
            }

            return new TIXDocument(tix, tims);
        }
    }
}
