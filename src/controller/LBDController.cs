using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using libLSD.Formats;
using LSDView.util;
using LSDView.view;

namespace LSDView.controller
{
	public class LBDController
	{
		public ILSDView View { get; set; }
		public string LBDPath { get; private set; }

		private LBD _lbdFile;

		public LBDController(ILSDView view)
		{
			View = view;
		}

		public void LoadLBD(string path)
		{
			LBDPath = path;

			using (BinaryReader br = new BinaryReader(File.Open(path, FileMode.Open)))
			{
				_lbdFile = new LBD(br);
			}

			Logger.Log()(LogLevel.INFO, "Loaded LBD: {0}", path);


		}
	}
}
