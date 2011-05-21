using DDE=System.Windows.Forms.DragDropEffects;

namespace mwg.Windows.TreeViewClasses{
	/// <summary>
	/// TreeView の項目に対する DragDrop 操作を実装します。
	/// </summary>
	/// <remarks>
	/// 今のところはノードの移動のみに対応します。
	/// 落とす先のノード・その先祖が IDropTarget を実装している場合はその情報を参照します。
	/// </remarks>
	public abstract class NodeDragDrop{
		/// <summary>
		/// このクラスに関連付けられている TreeView を保持します。
		/// </summary>
		protected System.Windows.Forms.TreeView view;
		/// <summary>
		/// ドラッグ中のアイテムを保持します。
		/// </summary>
		protected System.Windows.Forms.TreeNode item=null;
		/// <summary>
		/// TreeNodeDragDrop のコンストラクタです。
		/// </summary>
		/// <param name="treeView">ノードの位置を変更する TreeView を指定します。</param>
		/// <param name="source">この TreeView がノードの Drag 元になる事を示します。</param>
		/// <param name="target">この TreeView がノードの Drop 先になる事を示します。</param>
		public NodeDragDrop(System.Windows.Forms.TreeView treeView,bool source,bool target){
			this.view=treeView;
			if(source){
				this.view.ItemDrag+=new System.Windows.Forms.ItemDragEventHandler(view_ItemDrag);
			}
			if(target){
				this.view.AllowDrop=true;
				this.view.DragOver+=new System.Windows.Forms.DragEventHandler(view_DragOver);
				this.view.DragDrop+=new System.Windows.Forms.DragEventHandler(view_DragDrop);
			}
		}
		private void view_ItemDrag(object sender, System.Windows.Forms.ItemDragEventArgs e){
			if(this.item!=null)return;
			this.item=(System.Windows.Forms.TreeNode)e.Item;
			this.view.DoDragDrop(this,DDE.Move);//e.Data.GetData(typeof(System.Windows.Forms.TreeNode));
			this.item=null;
		}
		private void view_DragOver(object sender, System.Windows.Forms.DragEventArgs e){
			const int NONE=0;
			const int SELF=1;
			const int SIBLB=2;
			const int SIBLA=3;
			const int CHILD=4;
			//----------------
			System.Drawing.Point p=this.view.PointToClient(new System.Drawing.Point(e.X,e.Y));
			System.Windows.Forms.TreeNode target=this.view.GetNodeAt(p);
			if(target==null){
				this.rev_Draw(this.rev_rect,0);
				return;
			}
			this.hov_OnDragOver(target);

			System.Drawing.Rectangle rect=target.Bounds;
			DDE self,chld,sibl;
			int dropAllow=this.GetDropAllowance(target,e,out self,out sibl,out chld);
			switch(this.view_switch(dropAllow,p,rect,target.IsExpanded&&target.Nodes.Count>0)){
				case SELF:
					e.Effect=self;
					this.rev_Draw(rect,2);
					break;
				case SIBLB://sibling before
					e.Effect=sibl;
					this.rev_Draw(new System.Drawing.Rectangle(
						rect.Left,rect.Top,0,0
						),1);
					break;
				case SIBLA://sibling after
					e.Effect=sibl;
					this.rev_Draw(new System.Drawing.Rectangle(
						rect.Left,rect.Bottom,0,0
						),1);
					break;
				case CHILD:
					e.Effect=chld;
					this.rev_Draw(new System.Drawing.Rectangle(
						target.Nodes[0].Bounds.Left,rect.Bottom,0,0
						),1);
					break;
				case NONE://else:
					e.Effect=DDE.None;
					this.rev_Draw(rect,0);
					break;
			}
		}
		private void view_DragDrop(object sender, System.Windows.Forms.DragEventArgs e){
			const int SELF=1;
			const int SIBLB=2;
			const int SIBLA=3;
			const int CHILD=4;
			//----------------
			System.Drawing.Point p=this.view.PointToClient(new System.Drawing.Point(e.X,e.Y));
			System.Windows.Forms.TreeNode target=this.view.GetNodeAt(p);
			this.rev_Draw(target.Bounds,0);

			System.Drawing.Rectangle rect=target.Bounds;
			DDE self,chld,sibl;
			int dropAllow=this.GetDropAllowance(target,e,out self,out sibl,out chld);
			switch(this.view_switch(dropAllow,p,rect,target.IsExpanded&&target.Nodes.Count>0)){
				case SELF:
					e.Effect=self;
					this.DropOn(target,e);
					break;
				case SIBLB://sibling before
					e.Effect=sibl;
					this.Insert(target.Parent,e,target.Index);
					break;
				case SIBLA://sibling after
					e.Effect=sibl;
					this.Insert(target.Parent,e,target.Index+1);
					break;
				case CHILD:
					e.Effect=chld;
					this.Insert(target,e,0);
					break;
			}
		}
		/// <summary>
		/// 位置関係から Drop を使用しようとしている厳密な対象を取得します。
		/// </summary>
		/// <param name="dropAllow">Drop の対象として許される物を示すフラグを指定します。</param>
		/// <param name="rect">カーソルの下にあるノードの矩形を指定します。</param>
		/// <param name="expanded">カーソル直下のノードが子を展開しているかどうかを指定します。</param>
		/// <returns>
		/// 0: 落とす事の出来る対象はありません。
		/// 1: カーソル直下のノード自体に落とします。
		/// 2: 直下のノードの前に挿入します。
		/// 3: 直下のノードの後に挿入します。
		/// 4: 直下のノードの子として挿入します。
		/// </returns>
		private int view_switch(int dropAllow,System.Drawing.Point p,System.Drawing.Rectangle rect,bool expanded){
			const int NONE=0;
			const int SELF=1;
			const int SIBLB=2;
			const int SIBLA=3;
			const int CHILD=4;
			//----------------
			switch(dropAllow){
				case 0:return NONE;
				case 1:return SELF;
				case 2:
					if(p.Y>rect.Top+rect.Height/2){
						if(expanded)return NONE;else return SIBLA;
					}else return SIBLB;
				case 3:
					if(p.Y>rect.Top+rect.Height*3/4){
						if(expanded)return NONE;else return SIBLA;
					}else if(p.Y>rect.Top+rect.Height/4){
						return SELF;
					}else return SIBLB;
				case 4:
					if(expanded&&p.Y>rect.Y+rect.Height/2)return CHILD;
					return NONE;
				case 5:
					if(expanded&&p.Y>rect.Y+rect.Height*3/4)return CHILD;
					return SELF;
				case 6:
					if(p.Y>rect.Top+rect.Height/2){
						if(expanded)return CHILD;else return SIBLA;
					}else return SIBLB;
				case 7:
					if(p.Y>rect.Top+rect.Height*3/4){
						if(expanded)return CHILD;else return SIBLA;
					}else if(p.Y>rect.Top+rect.Height/4){
						return SELF;
					}else return SIBLB;
			}
			return NONE;
		}
		/// <summary>
		/// 指定したノードの指定したインデックスの位置にアイテムを挿入した時の動作を実行します。
		/// </summary>
		/// <param name="target">Drop 先の親のノードを指定します。</param>
		/// <param name="e">Drag の状況を指定します。</param>
		/// <param name="index">Drop する先の子ノードとしての位置を指定します。</param>
		protected abstract void Insert(System.Windows.Forms.TreeNode target,System.Windows.Forms.DragEventArgs e,int index);
		/// <summary>
		/// 指定したノードにアイテムを落とした時の動作を実行します。
		/// </summary>
		/// <param name="target">Drop 先のノードを指定します。</param>
		/// <param name="e">Drag の状況を指定します。</param>
		protected abstract void DropOn(System.Windows.Forms.TreeNode target,System.Windows.Forms.DragEventArgs e);
		//===========================================================
		//		ノードにカーソルを翳した時の処理
		//===========================================================
		private System.Windows.Forms.TreeNode hov_node;
		private System.DateTime hov_time;
		private void hov_OnDragOver(System.Windows.Forms.TreeNode node){
			if(hov_node==node){
				long milisec=(System.DateTime.Now-this.hov_time).Ticks/System.TimeSpan.TicksPerMillisecond;
				if(!this.hov_node.IsExpanded&&milisec>800){
					this.hov_node.Expand();
					this.hov_node=null;
				}else if(milisec>2000){
					if(this.hov_node.IsExpanded)this.hov_node.Collapse();
					this.hov_node=null;
				}
			}else{
				this.hov_node=node;
				this.hov_time=System.DateTime.Now;
				this.hov_node.EnsureVisible();
			}
		}
		//===========================================================
		//		反転表示の管理
		//===========================================================
		/// <summary>
		/// 0: 反転表示されている部分はない
		/// 1: 反転表示されている直線
		/// 2: 反転表示されている矩形領域
		/// </summary>
		private int rev_mode=0;
		private System.Drawing.Rectangle rev_rect;
		/// <summary>
		/// 反転表示している位置を変更します。
		/// </summary>
		/// <param name="rect"></param>
		/// <param name="mode">
		/// 0: 反転表示を取り消す
		/// 1: 直線を反転表示
		/// 2: 矩形領域を反転表示
		/// </param>
		private void rev_Draw(System.Drawing.Rectangle rect,int mode){
			if(rev_mode!=mode||rev_rect!=rect){
				if(rev_mode==1){
					this.rev_DrawLine(rev_rect);
				}else if(rev_mode==2){
					this.rev_DrawRect(rev_rect);
				}
				rev_mode=mode;
				rev_rect=rect;
				if(rev_mode==1){
					this.rev_DrawLine(rev_rect);
				}else if(rev_mode==2){
					this.rev_DrawRect(rev_rect);
				}
			}
		}
		private void rev_DrawLine(System.Drawing.Rectangle rect){
			rect.Y--;
			rect.Height=3;
			rect.Width=this.view.ClientRectangle.Width-rect.X;
			rect=this.view.RectangleToScreen(rect);
			System.Windows.Forms.ControlPaint.FillReversibleRectangle(rect,System.Drawing.Color.Black);
		}
		private void rev_DrawRect(System.Drawing.Rectangle rect){
			rect=this.view.RectangleToScreen(rect);
			System.Windows.Forms.ControlPaint.FillReversibleRectangle(rect,System.Drawing.Color.Black);
		}
		//===========================================================
		//		そこに Drop 出来るか否かの判定
		//===========================================================
		/// <summary>
		/// Drop の仕方を取得します。
		/// </summary>
		/// <param name="target">現在マウスが載っている TreeNode を指定します。</param>
		/// <param name="dde_on">指定した Drop 目標自体に落とす時の効果を返します。</param>
		/// <param name="dde_prt_bet">兄弟位置に挿入する時の効果を返します。</param>
		/// <param name="dde_self_bet">子供位置に挿入する時の効果を返します。</param>
		/// <returns>
		/// 以下の組合せの値が返されます。
		/// 1. 自分自身に Drop 可能か
		/// 2. 兄弟位置に挿入可能か
		/// 4. 子供位置に挿入可能か
		/// </returns>
		protected abstract int GetDropAllowance(
			System.Windows.Forms.TreeNode target,
			System.Windows.Forms.DragEventArgs e,
			out DDE dde_on,out DDE dde_prt_bet,out DDE dde_self_bet);

		#region cls:NddByNode
		/// <summary>
		/// ノード毎に DragDrop の振る舞いを設定する際に使用します。
		/// </summary>
		public sealed class NddByNode:NodeDragDrop{
			public NddByNode(System.Windows.Forms.TreeView treeView,bool source,bool target):base(treeView,source,target){}
			/// <summary>
			/// 指定したノードの指定したインデックスの位置にアイテムを挿入した時の動作を実行します。
			/// </summary>
			/// <param name="target">Drop 先の親のノードを指定します。</param>
			/// <param name="e">Drag の状況を指定します。</param>
			/// <param name="index">Drop する先の子ノードとしての位置を指定します。</param>
			protected override void Insert(System.Windows.Forms.TreeNode target, System.Windows.Forms.DragEventArgs e, int index){
				((IDropTarget)target).Drop(e,index);
			}
			/// <summary>
			/// 指定したノードにアイテムを落とした時の動作を実行します。
			/// </summary>
			/// <param name="target">Drop 先のノードを指定します。</param>
			/// <param name="e">Drag の状況を指定します。</param>
			protected override void DropOn(System.Windows.Forms.TreeNode target, System.Windows.Forms.DragEventArgs e){
				((IDropTarget)target).Drop(e);
			}
			/// <summary>
			/// Drop の仕方を取得します。
			/// </summary>
			/// <param name="target">現在マウスが載っている TreeNode を指定します。</param>
			/// <param name="dde_on">指定した Drop 目標自体に落とす時の効果を返します。</param>
			/// <param name="dde_prt_bet">兄弟位置に挿入する時の効果を返します。</param>
			/// <param name="dde_self_bet">子供位置に挿入する時の効果を返します。</param>
			/// <returns>
			/// 以下の組合せの値が返されます。
			/// 1. 自分自身に Drop 可能か
			/// 2. 兄弟位置に挿入可能か
			/// 4. 子供位置に挿入可能か
			/// </returns>
			protected override int GetDropAllowance(
				System.Windows.Forms.TreeNode target,
				System.Windows.Forms.DragEventArgs e,
				out DDE dde_on,out DDE dde_prt_bet,out DDE dde_self_bet){
				IDropTarget dt;
				bool on,self_bet,prt_bet;
				//-- (IDropTarget)target
				if((dt=target as IDropTarget)!=null){
					on=dt.DragOver(e);
					dde_on=e.Effect;
					self_bet=dt.DragOverBetweenChild(e);
					dde_self_bet=e.Effect;
				}else{
					on=self_bet=false;
					dde_on=dde_self_bet=DDE.None;
				}
				//-- (IDropTarget)target.Parent
				if((dt=target.Parent as IDropTarget)!=null){
					prt_bet=dt.DragOverBetweenChild(e);
					dde_prt_bet=e.Effect;
				}else{
					prt_bet=false;
					dde_prt_bet=DDE.None;
				}
				//--結果
				return (on?1:0)+(prt_bet?2:0)+(self_bet?4:0);
			}
		}
		/// <summary>
		/// NddByNode に於けるドロップターゲットの動作を記述します。
		/// </summary>
		public interface IDropTarget{
			/// <summary>
			/// アイテムの上に Drop された時に呼び出します。
			/// </summary>
			/// <param name="e">ドラッグの状況を指定します。</param>
			void Drop(System.Windows.Forms.DragEventArgs e);
			/// <summary>
			/// 子ノードとして挿入する指定で Drop された時に呼び出します。
			/// </summary>
			/// <param name="e">ドラッグの状況を指定します。</param>
			/// <param name="index">挿入先の Index です。</param>
			void Drop(System.Windows.Forms.DragEventArgs e,int index);
			/// <summary>
			/// 現在の状況で、アイテムを落とす事が出来るか否かを取得します。
			/// </summary>
			/// <param name="e">ドラッグの状況を指定します。</param>
			/// <returns>ドロップする事が出来る場合に true を返します。</returns>
			bool DragOver(System.Windows.Forms.DragEventArgs e);
			/// <summary>
			/// 子要素と子要素の間にアイテムを落とす事が出来るかを取得します。
			/// </summary>
			/// <param name="e">ドラッグの状況を指定します。</param>
			/// <returns>
			/// 子要素の間にアイテムを落とす事が出来る場合に true を返します。
			/// 通常は、要素の順序に意味があり、そこに新しい要素を挿入する際などに true を返します。
			/// </returns>
			/// <remarks>
			/// 親が IDropTarget を実装する
			/// </remarks>
			bool DragOverBetweenChild(System.Windows.Forms.DragEventArgs e);
		}
		#endregion

		#region cls:NddMove
		/// <summary>
		/// ノードの移動を行います。
		/// </summary>
		public sealed class NddMove:NodeDragDrop{
			public NddMove(System.Windows.Forms.TreeView treeView):base(treeView,true,true){}
			/// <summary>
			/// ノードの上に落とす許可を取得亦は設定します。
			/// true に設定されていると、treeView 内の全域で落とす事が出来ます。
			/// </summary>
			public bool AllowDropOnNode{
				get{return this._on;}
				set{this._on=value;}
			}bool _on=true;
			/// <summary>
			/// ノードとノードの間に落とす許可を取得亦は設定します。
			/// true に設定されていると、treeView 内の全域で落とす事が出来ます。
			/// </summary>
			public bool AllowDropBetween{
				get{return this._on;}
				set{this._on=value;}
			}bool _bet=true;
			/// <summary>
			/// Drop が可能かどうかを取得します。
			/// </summary>
			/// <param name="target">Drop の対象の TreeNode を指定します。</param>
			/// <param name="e">Drag の状況を指定します。</param>
			/// <param name="dde_on">TreeNode に落とす時の動作を返します。</param>
			/// <param name="dde_prt_bet">兄弟位置に落とす時の動作を返します。</param>
			/// <param name="dde_self_bet">子ノード位置に落とす時の動作を返します。</param>
			/// <returns>可能な Drop の方法を数値で返します。継承元の説明を参照して下さい。</returns>
			protected override int GetDropAllowance(System.Windows.Forms.TreeNode target, System.Windows.Forms.DragEventArgs e, out System.Windows.Forms.DragDropEffects dde_on, out System.Windows.Forms.DragDropEffects dde_prt_bet, out System.Windows.Forms.DragDropEffects dde_self_bet){
				dde_on=dde_prt_bet=dde_self_bet=DDE.Move;
				//System.Console.WriteLine("o");
				return (_on?1:0)+(_bet?6:0);
			}
			/// <summary>
			/// 指定したノードの指定したインデックスの位置にアイテムを挿入した時の動作を実行します。
			/// </summary>
			/// <param name="target">Drop 先の親のノードを指定します。</param>
			/// <param name="e">Drag の状況を指定します。</param>
			/// <param name="index">Drop する先の子ノードとしての位置を指定します。</param>
			protected override void Insert(System.Windows.Forms.TreeNode target,System.Windows.Forms.DragEventArgs e,int index){
				//--× 他の TreeView からのアイテム
				NddMove this0=e.Data.GetData(typeof(NddMove)) as NddMove;
				if(this0==null||this0!=this)return;

				//--× 自分・子孫に移動
				System.Windows.Forms.TreeNodeCollection tnc;
				if(target!=null){
					System.Windows.Forms.TreeNode pp=target;
					do{
						if(pp==this.item)return;
						pp=pp.Parent;
					}while(pp!=null);
					tnc=target.Nodes;
				}else{
					tnc=this.view.Nodes;
				}
				
				IOnInsert ioi=this.item as IOnInsert;
				//--移動前の処理
				if(ioi!=null)ioi.BeforeInsert(target,index);

				//--移動
				if(index<0){
					this.item.Remove();
					tnc.Add(this.item);
				}else{
					if(tnc.Contains(this.item)&&this.item.Index<index)index--;
					this.item.Remove();
					tnc.Insert(index,this.item);
				}

				//--移動後の処理
				if(ioi!=null)ioi.AfterInsert(target,index);
			}
			/// <summary>
			/// 指定したノードにアイテムを落とした時の動作を実行します。
			/// </summary>
			/// <param name="target">Drop 先のノードを指定します。</param>
			/// <param name="e">Drag の状況を指定します。</param>
			protected override void DropOn(System.Windows.Forms.TreeNode target, System.Windows.Forms.DragEventArgs e){
				this.Insert(target,e,-1);
			}
		}
		/// <summary>
		/// NddMove を設定した先の TreeView の子ノードに実装します。
		/// ノードの位置を変更した際に通知を受け取りたい場合に TreeNode に実装して下さい。
		/// </summary>
		public interface IOnInsert{
			/// <summary>
			/// 元の位置から削除される前に呼び出されます。
			/// </summary>
			/// <param name="target">追加先の TreeNode を指定します。null の場合はトップレベルに追加されます。</param>
			/// <param name="index">追加する先の位置を指定します。-1 の場合は末尾に追加されます。</param>
			void BeforeInsert(System.Windows.Forms.TreeNode target,int index);
			/// <summary>
			/// 新しい位置に挿入された後に呼び出されます。
			/// </summary>
			/// <param name="target">追加先の TreeNode を指定します。null の場合はトップレベルに追加されます。</param>
			/// <param name="index">追加する先の位置を指定します。-1 の場合は末尾に追加されます。</param>
			void AfterInsert(System.Windows.Forms.TreeNode target,int index);
		}
		#endregion
	}
	public class ContextMenuByNode{
		private System.Windows.Forms.TreeView treeView;
		public ContextMenuByNode(System.Windows.Forms.TreeView treeView){
			this.treeView=treeView;
			this.treeView.MouseUp+=new System.Windows.Forms.MouseEventHandler(treeView_MouseUp);
		}

		private System.Windows.Forms.TreeNode contextNode=null;
		private void treeView_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e){
			if(e.Button==System.Windows.Forms.MouseButtons.Right){
				System.Drawing.Point p=new System.Drawing.Point(e.X,e.Y);
				this.OnContextMenu(sender,this.treeView.GetNodeAt(p),p);
			}
		}
		public event ContextMenuEventHandler ContextMenu;
		private void OnContextMenu(object sender,System.Windows.Forms.TreeNode node,System.Drawing.Point p){
			if(this.ContextMenu==null)return;
			ContextMenuEventArgs e=new ContextMenuEventArgs(node,p);
			this.ContextMenu(sender,e);
		}
		public delegate void ContextMenuEventHandler(object sender,ContextMenuEventArgs e);
		/// <summary>
		/// ContextMenu イベントに関する情報を公開します。
		/// </summary>
		public class ContextMenuEventArgs{
			private readonly System.Windows.Forms.TreeNode treenode;
			private readonly System.Drawing.Point p;
			public ContextMenuEventArgs(System.Windows.Forms.TreeNode node,System.Drawing.Point p){
				this.treenode=node;
				this.p=p;
			}
			//=======================================================
			//		Properties
			//=======================================================
			/// <summary>
			/// 右クリックされたノードを取得します。
			/// </summary>
			public System.Windows.Forms.TreeNode Node{get{return this.treenode;}}
			/// <summary>
			/// クライアント座標を取得します。
			/// </summary>
			public System.Drawing.Point ClientPoint{get{return this.p;}}
			/// <summary>
			/// クライアント座標の X 値を取得します。
			/// </summary>
			public int ClientX{get{return this.p.X;}}
			/// <summary>
			/// クライアント座標の Y 値を取得します。
			/// </summary>
			public int ClientY{get{return this.p.Y;}}
			/// <summary>
			/// クリックした位置がノードのラベル上かどうかを取得します。
			/// </summary>
			public bool OnNodeLabel{get{return this.treenode.Bounds.Left<=this.p.X&&this.p.X<=this.treenode.Bounds.Right;}}
			/// <summary>
			/// クリックした位置がノードの左余白かどうかを取得します。
			/// </summary>
			public bool OnLeftMargin{get{return this.p.X<this.treenode.Bounds.Left;}}
			/// <summary>
			/// クリックした位置がノードの右余白かどうかを取得します。
			/// </summary>
			public bool OnRightMargin{get{return this.treenode.Bounds.Right<this.p.X;}}
		}
	}
}