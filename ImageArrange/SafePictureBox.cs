using Ref=System.Reflection;
using Gen=System.Collections.Generic;

namespace afh.ImageArrange{
	public sealed class SafePictureBox:System.Windows.Forms.PictureBox{
#if ERR_GIF_INSPECT
		protected override void WndProc(ref System.Windows.Forms.Message m){
			try{
				if(m.Msg==0xf&&this.Image!=null&&gdi_erred){
					//gdi_erred=false;
					//this.Force0x20_0();
				}
				base.WndProc(ref m);
				if(m.Msg==0xf&&this.Image!=null){
					result=m.Result;
					//this.SaveStatus();
				}
			}catch(System.Runtime.InteropServices.ExternalException xe){
				Program.log.WriteError(xe);
				Ref::BindingFlags BF=Ref::BindingFlags.Instance|Ref::BindingFlags.NonPublic;
				typeof(System.Windows.Forms.Control).GetMethod("SetState",BF).Invoke(this,new object[]{0x400000,false});
				this.gdi_erred=true;
			}
		}

		private static System.IntPtr result;
		private bool gdi_erred=false;
		Gen::Dictionary<Ref::FieldInfo,object> status;
		private string cmp_result;
		private void SaveStatus(){
			Ref::BindingFlags BF=Ref::BindingFlags.Instance|Ref::BindingFlags.NonPublic;
			System.Type ptype=typeof(System.Windows.Forms.PictureBox);
			this.status=new Gen::Dictionary<Ref::FieldInfo,object>();

			foreach(Ref::FieldInfo finfo in ptype.GetFields(BF)){
				this.status.Add(finfo,finfo.GetValue(this));
			}
		}
		private void CompareStatus(){
			string ret="";
			foreach(Gen::KeyValuePair<Ref::FieldInfo,object> pair in this.status){
				object before=pair.Value;
				object current=pair.Key.GetValue(this);
				if(before==null&&current==null) continue;
				if(before==null||!before.Equals(current))
					ret+=string.Format("フィールド {0} の値が変化しています。\r\n    前 {1}\r\n    後 {2}\r\n",
						pair.Key,before,current);
			}
			cmp_result=ret==""?"変化はありませんでした":ret;
		}
		private void RestoreStatus(){
			string ret="";
			foreach(Gen::KeyValuePair<Ref::FieldInfo,object> pair in this.status){
				object before=pair.Value;
				object current=pair.Key.GetValue(this);
				if(before==null&&current==null)continue;
				if(before==null||!before.Equals(current))pair.Key.SetValue(this,before);
			}
			cmp_result=ret==""?"変化はありませんでした":ret;
		}
		private void Force0x20_0(){
			Ref::BindingFlags BF=Ref::BindingFlags.Instance|Ref::BindingFlags.NonPublic;
			System.Type ptype=typeof(System.Windows.Forms.PictureBox);
			
			Ref::FieldInfo fstate=ptype.GetField("pictureBoxState",BF);
			System.Collections.Specialized.BitVector32 vec=(System.Collections.Specialized.BitVector32)fstate.GetValue(this);
			vec[0x20]=false;
			fstate.SetValue(this,vec);
		}

		protected override void OnPaint(System.Windows.Forms.PaintEventArgs pe) {
			base.OnPaint(pe);
		}
#else
		protected override void WndProc(ref System.Windows.Forms.Message m){
			try{
				base.WndProc(ref m);
			}catch(System.Runtime.InteropServices.ExternalException xe){
				if(!this.DesignMode)Program.log.WriteError(xe);
				Ref::BindingFlags BF=Ref::BindingFlags.Instance|Ref::BindingFlags.NonPublic;
				typeof(System.Windows.Forms.Control).GetMethod("SetState",BF).Invoke(this,new object[]{0x400000,false});
			}
		}
#endif
	}
}