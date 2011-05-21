using System;
using System.Windows.Forms;
using Gen=System.Collections.Generic;
using Interop=System.Runtime.InteropServices;

namespace afh.ImageArrange {
	static class Program{
		internal static afh.Application.Log log;

		/// <summary>
		/// アプリケーションのメイン エントリ ポイントです。
		/// </summary>
		[STAThread]
		static void Main() {
			System.Windows.Forms.Application.EnableVisualStyles();
			System.Windows.Forms.Application.SetCompatibleTextRenderingDefault(false);

			log=afh.Application.LogView.Instance.CreateLog("<afh.ImageArrange>");

			//afh.File.Riff.RiffFile riff=afh.File.Riff.RiffFile.FromFile(@"C:\Documents and Settings\koichi\デスクトップ\ANI\IE5.ani");
			
			/*
			ThumbsFile file=ThumbsFile.OpenFile(@"C:\IMAGE\Images\test.thm");
			file.AddFileDirectory(@"C:\IMAGE\Images\sm1278957");
			file.Save();
			//*/

			//ThumbsFile file=ThumbsFile.OpenFile(@"C:\IMAGE\Images\test.thm");
			// Main
#if true
			{
				Form1 form1=new Form1();
				form1.ReadFile(@"F:\IMAGE\東方画像\imgarn.thm");
				//form1.ReadFile(@"C:\IMAGE\東方画像\imgarn.thm");
				//form1.ReadFile(@"C:\IMAGE\Images\test.thm");
				System.Windows.Forms.Application.Run(form1);
			}
#else
			//*/

			// Debug
			{
				DebugLayout form1=new DebugLayout();
				System.Windows.Forms.Application.Run(form1);
			}
#endif
		}


		private static void dbg_GenricCovariance(){
			// Gen::List<object> x=(Gen::List<object>)new Gen::List<int>(new int[] { 1,2,3 });
			// コンパイル自体通りません。
		}
		private static void dbg_GetThumbnailImage(){
			// GetThumbnailImage はちゃんと全体の平均を出すのか、それとも適当に点を拾って出すのか?
			System.Drawing.Bitmap bmp=new System.Drawing.Bitmap(@"C:\Documents and Settings\koichi\デスクトップ\test.gif");
			System.Drawing.Image bmp2=bmp.GetThumbnailImage(8,8,delegate(){return true;},System.IntPtr.Zero);
			bmp2.Save(@"C:\Documents and Settings\koichi\デスクトップ\test2.gif");
			// 結果: 多分全体の平均を出している。
		}
		private static void dbg_ZipCompress(){
			System.IO.Stream str=System.IO.File.OpenRead(@"C:\IMAGE\Images\test.thm");
			afh.File.StreamUtils.GZipCompress(ref str);
			afh.File.StreamUtils.GZipDecompress(ref str);
			str.Close();
		}
	}
}