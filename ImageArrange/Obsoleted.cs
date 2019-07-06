namespace mwg.Forms{
	//================================================================
	//	ITreeNodeInfo
	//================================================================
	[System.Obsolete]
	public interface ITreeNodeInfo{
		/// <summary>
		/// �w�肵�� TreeNode �Ɋ֘A�t�����鎞�ɌĂяo����܂��B
		/// </summary>
		/// <param name="node">�֘A�t������Ώۂ� TreeNode ���w�肵�܂��B</param>
		void BindTreeNode(TreeNode node);
	}
	public partial class TreeNode:System.Runtime.Serialization.ISerializable{
		// [������]
		// override ������ł͂Ȃ�����Ǔ����ς���
		// �N���X�̌p���ɍS��炸�ݒ�����������p
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
		/// CheckBox �̕\����\�����e�Ɠ����ɂȂ鎖�������܂��B
		/// </summary>
		[System.Obsolete]
		InheritVisible=0x10,
		/// <summary>
		/// CheckBox �� Check ��Ԃ��e�Ɉˑ����鎖�������܂��B
		/// (�e�̏�Ԃ� Intermediate �̏ꍇ�ɂ́A
		/// �e�̏�Ԃɉe������܂���B)
		/// </summary>
		[System.Obsolete]
		InheritChecked=0x20,
		/// <summary>
		/// CheckBox �� Check ��Ԃ��q�m�[�h�B�� check ��Ԃ𔽉f����l�ɂ��܂��B
		/// �q�m�[�h���S�ē��� Check ��Ԃ̏ꍇ�ɂ͂��̏�ԂɂȂ�A
		/// �q�m�[�h�� Check ��Ԃ��l�X�ł���ꍇ�ɂ� Intermediate �ɂȂ�܂��B
		/// </summary>
		[System.Obsolete]
		MirrorChildren=0x40,
		//----------------------------------------
		/// <summary>
		/// ����̐ݒ�ł��B
		/// </summary>
		Default=ToggleOnDblClickText|ToggleOnDblClickIcon//|InheritVisible
	}
	/// <summary>
	/// TreeNode �� Checked ��Ԃ̊Ǘ��Ɏg�p���܂��B
	/// </summary>
	[System.Flags]
	public enum TreeNodeCheckedState{
		/// <summary>
		/// CheckBox ���\������Ȃ����������܂��B
		/// </summary>
		[System.Obsolete]
		None=0,
		/// <summary>
		/// CheckBox ���\������鎖�������܂��B
		/// </summary>
		[System.Obsolete]
		Visible=1,
		/// <summary>
		/// CheckBox �� check �������Ă����Ԃ������܂��B
		/// </summary>
		Checked=2,
		/// <summary>
		/// CheckBox �� Check ��Ԃ����Ԃł��鎖�������܂��B
		/// </summary>
		Intermediate=4,
		/// <summary>
		/// CheckBox ���L���ȏ�ԂɈׂ��Ă��鎖�������܂��B
		/// </summary>
		[System.Obsolete]
		Enabled=8,
		/// <summary>
		/// CheckBox �̕\���Ɋւ������̐ݒ�ł��B
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

			// �f�o�O�p
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

		#region TemplateProcessor �ɂ�鎩�������ɐؑ�
		/// <summary>
		/// �q�m�[�h�̃C���f���g�����擾���͐ݒ肵�܂��B
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
		/// �q�m�[�h�̃C���f���g�ʂ̌�����@���擾���͐ݒ肵�܂��B
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
		/// ���̃m�[�h�̍������擾���͐ݒ肵�܂��B
		/// �m�[�h�̍����́AContentSize �Ɉˑ����܂���B
		/// <remarks>
		/// ContentSize �Ɉˑ����ĕω�����l�ɂ������ꍇ�ɂ́A
		/// NodeHeightInherit �� Custom �ɐݒ肵�� ContentSizeChanged �ɂ� "Height" �ɒl��ݒ肷��K�v������܂��B
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
		/// ���m�[�h�̍����̌�����@���擾���͐ݒ肵�܂��B
		/// </summary>
		public TreeNodeInheritType NodeHeightInherit{
			get{return (TreeNodeInheritType)bits[sHeightInherit];}
			set{this.bits[sHeightInherit]=(uint)value;}
		}
		#endregion

		#region CheckBox �n��
		TreeNodeCheckedManagement chk_manage=TreeNodeCheckedManagement.Default;
		[System.Obsolete]
		TreeNodeCheckedState chk_state=TreeNodeCheckedState.Default;
		/// <summary>
		/// CheckBox ��\�����邩�ۂ����擾���͐ݒ肵�܂��B
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
		/// CheckBox �̏�Ԃ����ݗL���ł��邩�ۂ����擾���͐ݒ肵�܂��B
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
		/// �w�肵���m�[�h�̘_���ʒu���擾���܂��B
		/// </summary>
		/// <param name="node">���̘_���ʒu��m�肽���m�[�h���w�肵�܂��B
		/// ���� TreeView �ɏ������Ă���m�[�h�łȂ���Έׂ�܂���B</param>
		/// <returns>�w�肵���m�[�h�̘_���I�ȏꏊ���w�肵�܂��B</returns>
		[System.Obsolete("TreeNode.LogicalPosition")]
		internal Gdi::Point Node2LogicalPosition(TreeNode node) {
			if(node.view!=this)
				throw new System.InvalidOperationException("�w�肵���m�[�h�͂��� TreeView �ɑ�����m�[�h�ł͂���܂���B");

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
		/// ����̃m�[�h�̍������w�肵�܂��B
		/// </summary>
		public int DefaultNodeHeight{
			get{return this.defaultNodeHeight;}
			set{this.defaultNodeHeight=value;}
		}
		int defaultChildIndent=18;
		/// <summary>
		/// ����̎q�m�[�h�́u�C���f���g�v�̑傫�����擾���͐ݒ肵�܂��B
		/// 䢂Ō����C���f���g�Ƃ́A�q�m�[�h��e�m�[�h�����E���ɂ��炵�ĕ\�����鎞�́A
		/// ���炷�ʂ������Ă��܂��B
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
			// TreeNode �Ɋւ��関����
			throw new System.NotImplementedException();
		}
	}
}