namespace hnk.memopad{
	using mwg.Windows.TreeViewClasses;
	/// <summary>
	/// メモファイルの階層を表す TreeView です。
	/// </summary>
	public sealed class MemoTreeView:System.Windows.Forms.TreeView{
		private NodeDragDrop.NddMove ddctrl;
		private MemoDocument doc;
		private ContextMenuByNode menuctrl;
		//-----------------------------------------------------------
		private System.Windows.Forms.ContextMenu contextMenu1;
		private System.Windows.Forms.MenuItem menuItem1;
		private System.Windows.Forms.MenuItem menuItem2;
		private System.Windows.Forms.MenuItem menuItem3;
		private System.Windows.Forms.TextBox textBox;
		//-----------------------------------------------------------
		/// <summary>
		/// この TreeView の内容の表示先である TextBox を関連付けます。
		/// </summary>
		public System.Windows.Forms.TextBox TextBox{
			get{return this.textBox;}
			set{this.textBox=value;}
		}
		public MemoTreeView(MemoDocument doc):this(){
			this.doc=doc;
			this.ddctrl=new NodeDragDrop.NddMove(this);
			this.menuctrl=new ContextMenuByNode(this);

			this.RefreshFiles();
			this.menuctrl.ContextMenu+=new mwg.Windows.TreeViewClasses.ContextMenuByNode.ContextMenuEventHandler(menuctrl_ContextMenu);
		}

		#region デザイン
		private MemoTreeView():base(){
			this.InitializeComponent();
		}
		private void InitializeComponent(){
			this.contextMenu1 = new System.Windows.Forms.ContextMenu();
			this.menuItem3 = new System.Windows.Forms.MenuItem();
			this.menuItem1 = new System.Windows.Forms.MenuItem();
			this.menuItem2 = new System.Windows.Forms.MenuItem();
			//
			// contextMenu1
			//
			this.contextMenu1.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																						 this.menuItem3,
																						 this.menuItem1,
																						 this.menuItem2});
			//
			// menuItem3
			//
			this.menuItem3.Index = 0;
			this.menuItem3.Text = "新規作成 - 前";
			this.menuItem3.Click+=new System.EventHandler(menuItem3_Click);
			//
			// menuItem1
			//
			this.menuItem1.Index = 1;
			this.menuItem1.Text = "新規作成 - 子";
			this.menuItem1.Click+=new System.EventHandler(menuItem1_Click);
			//
			// menuItem2
			//
			this.menuItem2.Index = 2;
			this.menuItem2.Text = "新規作成 - 後";
			this.menuItem2.Click+=new System.EventHandler(menuItem2_Click);
			//
			// MemoTreeView
			//
			this.LabelEdit = true;

		}
		#endregion

		protected override void Dispose(bool disposing){
			if(disposing){
				this.UpdateToDocument();
				if(this.doc!=null)this.doc.Dispose();
			}
			base.Dispose(disposing);
		}
		//===========================================================
		//		公開メソッド
		//===========================================================
		/// <summary>
		/// 現在の MemoDocument に登録されている File を TreeView に表示します。
		/// </summary>
		public void RefreshFiles(){
			this.Nodes.Clear();
			foreach(File f in this.doc.files)this.Nodes.Add(f);
		}
		/// <summary>
		/// 現在 TextBox に表示されている内容を MemoDocument に適用します。
		/// </summary>
		private void UpdateToDocument(){
			File file=this.SelectedNode as File;
			if(file!=null&&this.textBox!=null){
				file.Content=this.textBox.Text;
			}
		}
		/// <summary>
		/// 現在の MemoDocument の File の内容を TextBox に表示します。
		/// </summary>
		private void UpdateToTextBox(){
			File file=this.SelectedNode as File;
			if(file!=null&&this.textBox!=null){
				this.textBox.Text=file.Content;
			}
		}
		//===========================================================
		//		EventHandlers
		//===========================================================
		protected override void OnAfterSelect(System.Windows.Forms.TreeViewEventArgs e){
			this.UpdateToTextBox();
			base.OnAfterSelect (e);
		}
		protected override void OnBeforeSelect(System.Windows.Forms.TreeViewCancelEventArgs e){
			this.UpdateToDocument();
			base.OnBeforeSelect (e);
		}
		protected override void OnAfterLabelEdit(System.Windows.Forms.NodeLabelEditEventArgs e){
			File file=e.Node as File;
			if(file!=null){
				if(e.Label!=null)file.Title=e.Label;
				file.UpdateNodeText();
			}
			base.OnAfterLabelEdit(e);
		}
		private File contextNode;
		private void menuctrl_ContextMenu(object sender,ContextMenuByNode.ContextMenuEventArgs e){
			this.contextNode=e.Node as File;
			if(this.contextNode==null||!e.OnNodeLabel)return;
			this.contextMenu1.Show(this,e.ClientPoint);
		}
		private void menuItem3_Click(object sender, System.EventArgs e){
			if(this.contextNode==null)return;
			this.contextNode.CreateBefore(System.DateTime.Now.Ticks.ToString("X16"));
		}
		private void menuItem1_Click(object sender, System.EventArgs e){
			if(this.contextNode==null)return;
			this.contextNode.CreateChild(System.DateTime.Now.Ticks.ToString("X16"),-1);
		}
		private void menuItem2_Click(object sender, System.EventArgs e){
			if(this.contextNode==null)return;
			this.contextNode.CreateAfter(System.DateTime.Now.Ticks.ToString("X16"));
		}
	}
}