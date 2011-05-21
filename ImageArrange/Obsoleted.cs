namespace mwg.Forms{
	//================================================================
	//	ITreeNodeInfo
	//================================================================
	[System.Obsolete]
	public interface ITreeNodeInfo{
		/// <summary>
		/// 指定した TreeNode に関連付けられる時に呼び出されます。
		/// </summary>
		/// <param name="node">関連付けられる対象の TreeNode を指定します。</param>
		void BindTreeNode(TreeNode node);
	}
	public partial class TreeNode:System.Runtime.Serialization.ISerializable{
		// [未実装]
		// override する程ではないけれど動作を変える
		// クラスの継承に拘わらず設定をしたい時用
		[System.Obsolete]
		ITreeNodeInfo info;
		[System.Obsolete]
		public ITreeNodeInfo Info{
			get{return this.info;}
		}
	}
	//****************************************************************
	[System.Flags]
	public enum TreeNodeCheckedManagement{
		ToggleOnClickIcon	=0x1,
		ToggleOnDblClickIcon=0x2,
		ToggleOnClickText	=0x4,
		ToggleOnDblClickText=0x8,
		/// <summary>
		/// CheckBox の表示非表示が親と同じになる事を示します。
		/// </summary>
		[System.Obsolete]
		InheritVisible=0x10,
		/// <summary>
		/// CheckBox の Check 状態が親に依存する事を示します。
		/// (親の状態が Intermediate の場合には、
		/// 親の状態に影響されません。)
		/// </summary>
		[System.Obsolete]
		InheritChecked=0x20,
		/// <summary>
		/// CheckBox の Check 状態を子ノード達の check 状態を反映する様にします。
		/// 子ノードが全て同じ Check 状態の場合にはその状態になり、
		/// 子ノードの Check 状態が様々である場合には Intermediate になります。
		/// </summary>
		[System.Obsolete]
		MirrorChildren=0x40,
		//----------------------------------------
		/// <summary>
		/// 既定の設定です。
		/// </summary>
		Default=ToggleOnDblClickText|ToggleOnDblClickIcon//|InheritVisible
	}
	/// <summary>
	/// TreeNode の Checked 状態の管理に使用します。
	/// </summary>
	[System.Flags]
	public enum TreeNodeCheckedState{
		/// <summary>
		/// CheckBox が表示されない事を示します。
		/// </summary>
		[System.Obsolete]
		None=0,
		/// <summary>
		/// CheckBox が表示される事を示します。
		/// </summary>
		[System.Obsolete]
		Visible=1,
		/// <summary>
		/// CheckBox に check が入っている状態を示します。
		/// </summary>
		Checked=2,
		/// <summary>
		/// CheckBox の Check 状態が中間である事を示します。
		/// </summary>
		Intermediate=4,
		/// <summary>
		/// CheckBox が有効な状態に為っている事を示します。
		/// </summary>
		[System.Obsolete]
		Enabled=8,
		/// <summary>
		/// CheckBox の表示に関する既定の設定です。
		/// </summary>
		[System.Obsolete]
		Default=Enabled,
	}
	public class TreeNode{

		[System.Obsolete]
		protected internal void ReDraw(){
			this.ReDraw(true);
		}
		protected virtual void DrawContent(Gdi::Graphics g){
			Gdi::Rectangle rect=new Gdi::Rectangle(Gdi::Point.Empty,this.ContentSize);

			// デバグ用
			if(this.IsEnabled){
				if(this.IsActivated){
					if(this.IsFocused){
						afh.Drawing.GraphicsUtils.FillRectangleReverseDotFramed(g,Gdi::SystemColors.Highlight,rect);
					}else{
						g.FillRectangle(Gdi::SystemBrushes.Highlight,rect);
					}
					g.DrawString("<TreeNode>",this.Font,Gdi::Brushes.White,new Gdi::PointF(0,0));
				}else{
					if(this.IsFocused){
						afh.Drawing.GraphicsUtils.DrawRectangleReverseDotFramed(g,this.BackColor,rect);					
					}
					g.DrawString("<TreeNode>",this.Font,Gdi::Brushes.Black,new Gdi::PointF(0,0));
				}
			}else{
				if(this.IsFocused){
					afh.Drawing.GraphicsUtils.FillRectangleReverseDotFramed(g,Gdi::Color.LightGray,rect);
					g.DrawString("<TreeNode>",this.Font,Gdi::Brushes.Gray,new Gdi::PointF(0,0));
				}else{
					g.DrawString("<TreeNode>",this.Font,Gdi::Brushes.Silver,new Gdi::PointF(0,0));
				}
				/*
				Forms::ControlPaint.DrawStringDisabled(
					g,"<TreeNode>",this.Font,Gdi::Color.Transparent,
					rect,Gdi::StringFormat.GenericDefault);
				//*/
			}
		}
		[System.Obsolete]
		private readonly Gen::Dictionary<string,object> xmembers=new Gen::Dictionary<string,object>();
		[System.Obsolete]
		private bool GetMember<T>(string name,out T val){
			val=default(T);
			object o;
			bool ret=xmembers.TryGetValue(name,out o);
			if(ret)val=(T)o;
			return ret;
		}

		private void OnIsExpandedChange(bool value){
			afh.EventHandler<TreeNode,PropertyChangingEventArgs<bool>> m;
			if(this.xmem.GetMember("BeforeIsExpandedChange",out m))
				m(this,new PropertyChangingEventArgs<bool>(!value,value));

			_todo.TreeNodeDisplayHeight("");
			this.RefreshDisplaySize();

			if(this.xmem.GetMember("AfterIsExpandedChange",out m))
				m(this,new PropertyChangingEventArgs<bool>(value,!value));
		}

		[System.Obsolete]
		private const BitSection sChildIndentWidthInherit	=(BitSection)(2<<16|30);

		#region TemplateProcessor による自動生成に切替
		/// <summary>
		/// 子ノードのインデント幅を取得又は設定します。
		/// </summary>
		public int ChildIndent{
			get{
				switch(this.ChildIndentInheritance){
					case TreeNodeInheritType.Default:
						if(this.view!=null)
							return this.view.DefaultNodeParams.ChildIndent;
						break;
					case TreeNodeInheritType.Inherit:
						if(this.parent!=null)
							return this.parent.ChildIndent;
						break;
					case TreeNodeInheritType.Custom:
						int ret;
						if(this.xmem.GetMember("ChildPadding",out ret))
							return ret;
						break;
				}
				return 18;
			}
			set{
				this.ChildIndentInheritance=TreeNodeInheritType.Custom;
				this.xmem["ChildPadding"]=value;
			}
		}
		/// <summary>
		/// 子ノードのインデント量の決定方法を取得又は設定します。
		/// </summary>
		public TreeNodeInheritType ChildIndentInheritance{
			get{return (TreeNodeInheritType)bits[sChildIndentInherit];}
			set{this.bits[sChildIndentInherit]=(uint)value;}
		}
		public TreeNodeInheritType IsIconVisibleInherit{
			get{return (TreeNodeInheritType)this.bits[sShowIconInheritance];}
			set{this.bits[sShowIconInheritance]=(uint)value;}
		}
		public bool IsIconVisible{
			get{
				switch(this.HeightInheritance){
					case TreeNodeInheritType.Default:
						if(this.view!=null)
							return this.view.DefaultNodeParams.IsIconVisible;
						break;
					case TreeNodeInheritType.Inherit:
						if(this.parent!=null)
							return this.parent.IsIconVisible;
						break;
					case TreeNodeInheritType.Custom:
						bool ret;
						if(this.xmem.GetMember("IsIconVisible",out ret))
							return ret;
						break;
				}
				return false;
			}
			set{
				this.HeightInheritance=TreeNodeInheritType.Custom;
				this.xmem["Height"]=value;
			}
		}
		/// <summary>
		/// このノードの高さを取得又は設定します。
		/// ノードの高さは、ContentSize に依存しません。
		/// <remarks>
		/// ContentSize に依存して変化する様にしたい場合には、
		/// NodeHeightInherit を Custom に設定して ContentSizeChanged にて "Height" に値を設定する必要があります。
		/// </remarks>
		/// </summary>
		public int NodeHeight{
			get{
				switch(this.NodeHeightInherit){
					case TreeNodeInheritType.Default:
						if(this.view!=null)
							return this.view.DefaultNodeParams.Height;
						break;
					case TreeNodeInheritType.Inherit:
						if(this.parent!=null)
							return this.parent.NodeHeight;
						break;
					case TreeNodeInheritType.Custom:
						int ret;
						if(this.xmem.GetMember("Height",out ret))
							return ret;
						break;
				}
				return 14;
			}
			set{
				this.NodeHeightInherit=TreeNodeInheritType.Custom;
				this.xmem["Height"]=value;
			}
		}
		/// <summary>
		/// 自ノードの高さの決定方法を取得又は設定します。
		/// </summary>
		public TreeNodeInheritType NodeHeightInherit{
			get{return (TreeNodeInheritType)bits[sHeightInherit];}
			set{this.bits[sHeightInherit]=(uint)value;}
		}
		#endregion

		#region CheckBox 系統
		TreeNodeCheckedManagement chk_manage=TreeNodeCheckedManagement.Default;
		[System.Obsolete]
		TreeNodeCheckedState chk_state=TreeNodeCheckedState.Default;
		/// <summary>
		/// CheckBox を表示するか否かを取得又は設定します。
		/// </summary>
		public bool CheckBoxVisible{
			get{
				if(this.parent!=null&&(this.chk_manage&TreeNodeCheckedManagement.InheritVisible)!=0){
					return this.parent.CheckBoxVisible;
				}else{
					return (this.chk_state&TreeNodeCheckedState.Visible)!=0;
				}
			}
			set{
				this.chk_manage&=~TreeNodeCheckedManagement.InheritVisible;
				if(value){
					this.chk_state|=TreeNodeCheckedState.Visible;				
				}else{
					this.chk_state&=~TreeNodeCheckedState.Visible;
				}
			}
		}
		/// <summary>
		/// CheckBox の状態が現在有効であるか否かを取得又は設定します。
		/// </summary>
		public bool CheckBoxEnabled{
			get{
				return 0!=(this.chk_state&TreeNodeCheckedState.Enabled);
			}
			set{
				if(value){
					this.chk_state|=TreeNodeCheckedState.Enabled;
				}else{
					this.chk_state&=~TreeNodeCheckedState.Enabled;
				}
			}
		}
		private static bool? CheckState2Bool(TreeNodeCheckedState state) {
			if(0!=(state&TreeNodeCheckedState.Intermediate))
				return null;
			return 0!=(state&TreeNodeCheckedState.Checked);
		}
		public bool CheckedReflectChildren{
			get{return 0!=(this.chk_manage&TreeNodeCheckedManagement.MirrorChildren);}
			set{
				if(value){
					this.chk_manage|=TreeNodeCheckedManagement.MirrorChildren;
				}else{
					this.chk_manage&=~TreeNodeCheckedManagement.MirrorChildren;
				}
			}
		}
		#endregion
	}
	public class TreeView{
		/// <summary>
		/// 指定したノードの論理位置を取得します。
		/// </summary>
		/// <param name="node">その論理位置を知りたいノードを指定します。
		/// この TreeView に所属しているノードでなければ為りません。</param>
		/// <returns>指定したノードの論理的な場所を指定します。</returns>
		[System.Obsolete("TreeNode.LogicalPosition")]
		internal Gdi::Point Node2LogicalPosition(TreeNode node) {
			if(node.view!=this)
				throw new System.InvalidOperationException("指定したノードはこの TreeView に属するノードではありません。");

			_todo.ExamineTreeView();
			Gdi::Point p=Gdi::Point.Empty;
			while(node.parent!=null) {
				p=node.LocalPos2ParentLocalPos(p);
				node=node.parent;
			}
			Diag::Debug.Assert(node==this.root);
			return p;
		}
		int defaultNodeHeight=14;
		/// <summary>
		/// 既定のノードの高さを指定します。
		/// </summary>
		public int DefaultNodeHeight{
			get{return this.defaultNodeHeight;}
			set{this.defaultNodeHeight=value;}
		}
		int defaultChildIndent=18;
		/// <summary>
		/// 既定の子ノードの「インデント」の大きさを取得又は設定します。
		/// 茲で言うインデントとは、子ノードを親ノードよりも右側にずらして表示する時の、
		/// ずらす量を示しています。
		/// </summary>
		public int DefaultChildIndent{
			get{return this.defaultChildIndent;}
			set{this.defaultChildIndent=value;}
		}
		//============================================================
		//		Scroll
		//============================================================
		[System.Obsolete("AutoScrollMinSize.Height")]
		protected int VMax{
			get{return this.AutoScrollMinSize.Height;}
			set{
				Gdi::Size size=this.AutoScrollMinSize;
				if(size.Height==value)return;
				size.Height=value;
				this.AutoScrollMinSize=size;
			}
		}
	}
	internal static class _debug{
		[Diag::Conditional("CALL_DBG_SPEC")]
		public static void TreeNodeNotImplemented(){
			// TreeNode に関する未実装
			throw new System.NotImplementedException();
		}
	}
}