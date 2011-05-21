using Gen=System.Collections.Generic;
using Gdi=System.Drawing;

namespace afh.Layout{
	using afh.Layout.Extension;

#if false
	public class SimpleNode:ILayoutNode{

		#region ILayoutNode メンバ
		protected IContainerNode parent=null;
		public virtual IContainerNode Parent{get{return this.parent;}}
		public virtual void InternalSetParent(IContainerNode node){
			this.parent=node;
			this.rect=new Gdi::Rectangle(10,10,100,100);
		}

		public virtual bool HasChildren{get{return false;}}

		public virtual ArrangeStyle ArrangeStyle{
			get{return ArrangeStyle.None;}
		}
		public virtual int MarginLeft{get{return 0;}}
		public virtual int MarginRight{get{return 0;}}
		public virtual int MarginTop{get{return 0;}}
		public virtual int MarginBottom{get{return 0;}}

		protected Gdi::Rectangle rect;
		public virtual void InternalSetBoundingRect(System.Drawing.Rectangle rect){return;}
		public virtual HitValue HitTest(System.Drawing.Point posInParent){
			HitValue ret=0;
			if(posInParent.X<rect.Left){
				ret=HitValue.HHitLeft;
			}else if(posInParent.X>rect.Right){
				ret=HitValue.HHitRight;
			}

			if(posInParent.Y<rect.Top){
				ret|=HitValue.VHitAbove;
			}else if(posInParent.Y>rect.Bottom){
				ret|=HitValue.VHitBelow;
			}

			if(ret!=0)ret|=HitValue.NotHit;
			return ret;
		}

		protected virtual void OnSizeChanged(Gdi::Size oldSize){
			//TODO:
		}
		protected virtual void OnPositionChanged(Gdi::Point oldLocation){
			//TODO:
		}
		#endregion

		#region property:DesiredRect
		/// <summary>
		/// 設定として求める配置を取得又は設定します。
		/// </summary>
		public virtual System.Drawing.Rectangle DesiredRect{
			get{return rect;}
			set{
				if(this.rect==value)return;
				Gdi::Rectangle oldrect=this.rect;
				this.rect=value;
				if(oldrect.Location!=value.Location)
					this.OnPositionChanged(oldrect.Location);
				if(oldrect.Size!=value.Size)
					this.OnSizeChanged(oldrect.Size);
			}
		}
		/// <summary>
		/// 設定横位置を取得又は設定します。
		/// </summary>
		public virtual int DesiredLeft{
			get{return this.rect.X;}
			set{
				if(this.rect.X==value)return;
				Gdi::Point oldPos=rect.Location;
				this.rect.X=value;
				this.OnPositionChanged(oldPos);
			}
		}
		/// <summary>
		/// 設定縦位置を取得又は設定します。
		/// </summary>
		public virtual int DesiredTop{
			get{return this.rect.Y;}
			set{
				if(this.rect.Y==value)return;
				Gdi::Point oldPos=rect.Location;
				this.rect.Y=value;
				this.OnPositionChanged(oldPos);
			}
		}
		/// <summary>
		/// 設定横幅を取得又は設定します。
		/// </summary>
		public virtual int DesiredWidth{
			get{return this.rect.Width;}
			set{
				if(this.rect.Width==value)return;
				Gdi::Size oldSize=rect.Size;
				this.rect.Width=value;
				this.OnSizeChanged(oldSize);
			}
		}
		/// <summary>
		/// 設定縦幅を取得又は設定します。
		/// </summary>
		public virtual int DesiredHeight{
			get{return this.rect.Height;}
			set{
				if(this.rect.Height==value)return;
				Gdi::Size oldSize=rect.Size;
				this.rect.Height=value;
				this.OnSizeChanged(oldSize);
			}
		}
		//--------------------------------------------------------------------------
		public int BoundingLeft{
			get{return this.rect.Left;}
		}
		public int BoundingTop{
			get{return this.rect.Top;}
		}
		public int BoundingWidth{
			get{return this.rect.Width;}
		}
		public int BoundingHeight{
			get{return this.rect.Height;}
		}
		#endregion

		//public virtual int GetHeight(RectCoord coord){
		//  switch(coord){
		//    case RectCoord.Desired:
		//    case RectCoord.Bounding:
		//      return this.rect.Height;
		//    default:
		//      throw new System.NotSupportedException();
		//  }
		//}
		public virtual void Draw(Gdi::Graphics g){}
	}
#endif

	/// <summary>
	/// 基本的なノード機能を提供します。
	/// </summary>
	public class LayoutNode:ILayoutNode{
		IContainerNode parent;
		protected Gdi::Rectangle rect;
		protected Gdi::Rectangle rectA;
		FourWidth margin;

		public LayoutNode(){
			this.parent=null;
			this.rect=new Gdi::Rectangle(10,10,100,100);
			this.rectA=this.rect;
			this.margin=new FourWidth();
			this.arrangeStyle=ArrangeStyle.None;
		}


		#region ILayoutNode メンバ
		public IContainerNode Parent{
			get{return this.parent;}
		}
		void ILayoutNode.InternalSetParent(IContainerNode node){
			this.parent=node;
		}

		public virtual bool HasChildren{
			get{return false;}
		}

		#endregion

		ArrangeStyle arrangeStyle;
		/// <summary>
		/// 自動配置をする際のスタイルを取得又は設定します。
		/// </summary>
		public ArrangeStyle ArrangeStyle{
			get{return this.arrangeStyle;}
			set{this.arrangeStyle=value;}
		}

		public virtual void Draw(Gdi::Graphics g){
			g.Clear(Gdi::Color.LightCoral);
			Gdi::Rectangle rect=this.GetRect(RectCoord.Bounding,RectCoord.Bounding);
			rect.Width--;
			rect.Height--;
			g.DrawRectangle(Gdi::Pens.Red,rect);
		}

		//**************************************************************************
		//  幾何計算

		public virtual int GetLeft(RectCoord coord,RectCoord axis){
			return ILayoutNodeExtensionMethods.GetLeft(this,coord,axis);
		}
		public virtual int GetTop(RectCoord coord,RectCoord axis){
			return ILayoutNodeExtensionMethods.GetTop(this,coord,axis);
		}
		public virtual Gdi::Point GetLocation(RectCoord coord,RectCoord axis){
			return ILayoutNodeExtensionMethods.GetLocation(this,coord,axis);
		}
		public virtual int GetWidth(RectCoord coord){
			return ILayoutNodeExtensionMethods.GetWidth(this,coord);
		}
		public virtual int GetHeight(RectCoord coord){
			return ILayoutNodeExtensionMethods.GetHeight(this,coord);
		}
		public virtual Gdi::Size GetSize(RectCoord coord){
			return ILayoutNodeExtensionMethods.GetSize(this,coord);
		}

		#region Properties
		//==========================================================================
		//  Properties
		//--------------------------------------------------------------------------
		//  DesiredRect
		//--------------------------------------------------------------------------
		/// <summary>
		/// 設定として求める配置を取得又は設定します。
		/// </summary>
		public System.Drawing.Rectangle DesiredRect{
			get{return rect;}
			set{
				if(this.rect==value)return;
				this.rect=value;
				this.OnDesiredRectChanged();
			}
		}
		/// <summary>
		/// 設定横位置を取得又は設定します。
		/// </summary>
		public int DesiredLeft{
			get{return this.rect.X;}
			set{
				if(this.rect.X==value)return;
				this.rect.X=value;
				this.OnDesiredRectChanged();
			}
		}
		/// <summary>
		/// 設定縦位置を取得又は設定します。
		/// </summary>
		public int DesiredTop{
			get{return this.rect.Y;}
			set{
				if(this.rect.Y==value)return;
				this.rect.Y=value;
				this.OnDesiredRectChanged();
			}
		}
		/// <summary>
		/// 設定横幅を取得又は設定します。
		/// </summary>
		public int DesiredWidth{
			get{return this.rect.Width;}
			set{
				if(this.rect.Width==value)return;
				this.rect.Width=value;
				this.OnDesiredRectChanged();
			}
		}
		/// <summary>
		/// 設定縦幅を取得又は設定します。
		/// </summary>
		public int DesiredHeight{
			get{return this.rect.Height;}
			set{
				if(this.rect.Height==value)return;
				this.rect.Height=value;
				this.OnDesiredRectChanged();
			}
		}
		private void OnDesiredRectChanged(){
			if(this.parent!=null){
				//TODO:
				//this.parent.Rearrange();
			}
		}
		//--------------------------------------------------------------------------
		//  BoundingRect
		//--------------------------------------------------------------------------
		protected virtual void OnSizeChanged(Gdi::Size oldSize){
			if(this.parent!=null)
				this.parent.ArrangeNodes(this);
			//TODO:
		}
		protected virtual void OnPositionChanged(Gdi::Point oldLocation){
			//TODO:
		}
		/// <summary>
		/// 実際に表示される時の位置を通知・設定する時に使用します。
		/// </summary>
		/// <param name="rect">実際に表示される時の位置を指定します。</param>
		void ILayoutNode.InternalSetBoundingRect(System.Drawing.Rectangle rect){
			Gdi::Rectangle old_rect=this.rectA;
			this.rectA=rect;
			if(rect.Size!=old_rect.Size)
				this.OnSizeChanged(old_rect.Size);
			if(rect.Location!=old_rect.Location)
				this.OnPositionChanged(old_rect.Location);
		}
		/// <summary>
		/// 実際の配置を取得します。
		/// </summary>
		public System.Drawing.Rectangle BoundingRect{
			get{return rectA;}
		}
		/// <summary>
		/// 実際の横位置を取得します。
		/// </summary>
		public int BoundingLeft{
			get{return this.rectA.X;}
		}
		/// <summary>
		/// 実際の縦位置を取得します。
		/// </summary>
		public int BoundingTop{
			get{return this.rectA.Y;}
		}
		/// <summary>
		/// 実際の横幅を取得します。
		/// </summary>
		public int BoundingWidth{
			get{return this.rectA.Width;}
		}
		/// <summary>
		/// 実際の縦幅を取得します。
		/// </summary>
		public int BoundingHeight{
			get{return this.rectA.Height;}
		}
		/// <summary>
		/// 実際の右端を取得します。
		/// </summary>
		public int BoundingRight{
			get{return this.rectA.Right;}
		}
		/// <summary>
		/// 実際の下端を取得します。
		/// </summary>
		public int BoundingBottom{
			get{return this.rectA.Bottom;}
		}
		//--------------------------------------------------------------------------
		//  Margin
		//--------------------------------------------------------------------------
		/// <summary>
		/// 最小間隙を取得又は設定します。
		/// </summary>
		public FourWidth Margin{
			get{return this.margin;}
			set{
				if(this.margin==value)return;
				this.margin=value;
				this.OnMarginChanged();
			}
		}
		/// <summary>
		/// 上側の最小間隙を取得又は設定します。
		/// </summary>
		public int MarginTop{
			get{return this.margin.Top;}
			set{
				if(this.margin.top==value)return;
				this.margin.Top=value;
				this.OnMarginChanged();
			}
		}
		/// <summary>
		/// 下側の最小間隙を取得又は設定します。
		/// </summary>
		public int MarginBottom{
			get{return this.margin.Bottom;}
			set{
				if(this.margin.bottom==value)return;
				this.margin.Bottom=value;
				this.OnMarginChanged();
			}
		}
		/// <summary>
		/// 左側の最小間隙を取得又は設定します。
		/// </summary>
		public int MarginLeft{
			get{return this.margin.Left;}
			set{
				if(this.margin.left==value)return;
				this.margin.Left=value;
				this.OnMarginChanged();
			}
		}
		/// <summary>
		/// 右側の最小間隙を取得又は設定します。
		/// </summary>
		public int MarginRight{
			get{return this.margin.Right;}
			set{
				if(this.margin.right==value)return;
				this.margin.Right=value;
				this.OnMarginChanged();
			}
		}
		private void OnMarginChanged(){
			if(this.parent!=null){
				//TODO:
				//this.parent.Rearrange();
			}
		}
		#endregion
		//==========================================================================
		//  HitTest
		//--------------------------------------------------------------------------
		/// <summary>
		/// 指定した点がこのノードの内部に存在するかを判定します。
		/// </summary>
		/// <param name="posInParent">判定する点を、親 Content 座標系で指定します。</param>
		/// <returns>指定した点がこの要素の内部に存在する時、零以上の値を返します。
		/// 指定した点がこの要素の外部に存在する時、負の値を返します。
		/// </returns>
		public virtual HitValue HitTest(Gdi::Point posInParent){
			HitValue ret=0;
			if(posInParent.X<rectA.Left){
				ret=HitValue.HHitLeft;
			}else if(posInParent.X>rectA.Right){
				ret=HitValue.HHitRight;
			}

			if(posInParent.Y<rectA.Top){
				ret|=HitValue.VHitAbove;
			}else if(posInParent.Y>rectA.Bottom){
				ret|=HitValue.VHitBelow;
			}

			if(ret!=0)ret|=HitValue.NotHit;
			return ret;
		}
	}
	/// <summary>
	/// 内部に子ノードを保持する事が出来る LayoutNode を提供します。
	/// </summary>
	public class ContainerNode:LayoutNode,IContainerNode,ILayoutNode{
		/// <summary>
		/// ConteinerNode を初期化します。
		/// </summary>
		public ContainerNode():base(){
			this.children=null;
			this.border=new FourWidth();
			this.offset=new FourWidth();
		}
		/// <summary>
		/// 子ノードがあるか否かを取得します。
		/// </summary>
		public override bool HasChildren{
			get{return this.children!=null&&this.children.Count>0;}
		}

		LayoutNodeCollection children;
		/// <summary>
		/// 子ノードのコレクションを取得します。
		/// </summary>
		public LayoutNodeCollection Children{
			get{
				if(this.children==null){
					this.children=new LayoutNodeCollection(this);
					this.children.isreadonly=!this.isch_mutable;
				}
				return this.children;
			}
		}
		public Gen::IEnumerable<ILayoutNode> ChildrenAdder{
			set{this.Children.AddRange(value);}
		}
		/// <summary>
		/// 指定したノードを子ノードとして追加します。
		/// </summary>
		/// <param name="node">追加する子ノードを指定します。</param>
		public void AddChildren(ILayoutNode node){
			this.Children.Add(node);
		}
		/// <summary>
		/// 指定した子ノードを子ノードコレクションから削除します。
		/// </summary>
		/// <param name="node">削除する子ノードを指定します。</param>
		public bool RemoveChildren(ILayoutNode node){
			return this.Children.Remove(node);
		}
		bool isch_mutable=true;
		/// <summary>
		/// 子ノード集合を変更する事が可能かどうかを取得又は設定します。
		/// </summary>
		public bool IsChildrenMutable{
			get{return this.isch_mutable;}
			set{
				this.isch_mutable=value;
				if(this.children!=null)
					this.children.isreadonly=!value;
			}
		}
		//**************************************************************************
		//  描画
		//==========================================================================
		public override void Draw(Gdi::Graphics g){
			_todo.LayoutNodeEssential("DrawBorder");
			_todo.LayoutNodeEssential("DrawBackground");
			this.DrawChildren(g);
		}
		private void DrawChildren(Gdi::Graphics g){
			// 表示領域×Clip
			Gdi::Rectangle rectV=this.GetRect(RectCoord.View,RectCoord.Bounding);
			rectV.Intersect(g.ClipBounds.ToRectangle());

			// Content 座標に移動
			{
				Gdi::Point delta=this.GetLocation(RectCoord.Content,RectCoord.Bounding);
				g.TranslateTransform(delta.X,delta.Y);
				rectV.X-=delta.X;
				rectV.Y-=delta.Y;
			}
			
			using(GraphicsTransformStore stTransf=new GraphicsTransformStore(g))
			foreach(ILayoutNode node in this.children){
				// 交差判定・Clip設定
				Gdi::Rectangle rect=node.GetBoundingRect();
				rect.Intersect(rectV);
				if(rect.IsEmpty)continue;
				g.SetClip(rect);

				// 座標変換・描画
				g.TranslateTransform(node.BoundingLeft,node.BoundingTop);
				node.Draw(g);
				stTransf.Restore();
			}
		}
		//**************************************************************************
		//  幾何計算
		//--------------------------------------------------------------------------
		#region M:ArrangeNodes
		//==========================================================================
		//  子ノード配置の計算
		//--------------------------------------------------------------------------
		bool suppress_arrange=false;
		public void SuppressArrange(){
			this.suppress_arrange=true;
		}
		public void ResumeArrange(ILayoutNode node){
			this.suppress_arrange=false;
			this.ArrangeNodes(node);
		}
		/// <summary>
		/// 子ノードの配置を計算・更新します。
		/// </summary>
		/// <param name="changed">
		/// 変更が行われた最初の子ノードを指定します。
		/// この子ノード以降のノードが再計算の対象です。
		/// null を指定した場合には、全ての子ノードの配置が再計算されます。
		/// </param>
		protected internal void ArrangeNodes(ILayoutNode changed){
			if(suppress_arrange)return;

			_todo.LayoutNodeOptimize("子ノードが沢山ある場合には helper をキャッシュする様にする。");
			_todo.LayoutNodeOptimize("changed よりも前の要素に関しては再計算の必要はない。");
			_todo.LayoutNodeOptimize("changed の更新で outerRect/innerRect に何も変更がなかった場合には、以降も再計算する必要はない。");

			if(this.children==null)return;

			ArrangeNodesWorker helper=new ArrangeNodesWorker(this);
			int nchild=children.Count;
			for(int i=0;i<nchild;i++){
				helper.ProcessNode(this.children[i]);
			}
		}
		void IContainerNode.ArrangeNodes(ILayoutNode changed){
			this.ArrangeNodes(changed);
		}
		private struct ArrangeNodesWorker{
			ContainerNode self;
			public Gdi::Rectangle oRect; // 直近の物体までの距離
			public Gdi::Rectangle iRect; // 直近の物体が張る margin 領域
			public ILayoutNode node;
			public ArrangeNodesWorker(ContainerNode self){
				this.self=self;
				oRect=self.GetRect(RectCoord.Content,RectCoord.Content);
				iRect=self.GetRect(RectCoord.Inner,RectCoord.Content);
				node=null;
			}
			public void ProcessNode(ILayoutNode node){
				this.node=node;
				switch(node.ArrangeStyle.MaskDock()){
					case ArrangeStyle.None:
						node.InternalSetBoundingRect(node.GetRect(RectCoord.Desired,RectCoord.Parent));
						break;
					case ArrangeStyle.DockFill:{
						int t,h,l,w;
						FillH(out l,out w);
						FillV(out t,out h);
						node.InternalSetBoundingRect(new Gdi::Rectangle(l,t,w,h));
						break;
					}
					case ArrangeStyle.DockTop:{
						int h=node.DesiredHeight;
						int t=TopLimit;
						int l,w;AlignH(out l,out w);
						node.InternalSetBoundingRect(new Gdi::Rectangle(l,t,w,h));
						SetTopFixingOther(ref oRect,t+h);
						SetTopFixingOther(ref iRect,t+h+node.MarginBottom);
						break;
					}
					case ArrangeStyle.DockBottom:{
						int h=node.DesiredHeight;
						int t=BottomLimit-h;
						int l,w;AlignH(out l,out w);
						node.InternalSetBoundingRect(new Gdi::Rectangle(l,t,w,h));
						SetBottomFixingOther(ref oRect,t);
						SetBottomFixingOther(ref iRect,t-node.MarginTop);
						break;
					}
					case ArrangeStyle.DockLeft:{
						int w=node.DesiredHeight;
						int l=LeftLimit;
						int t,h;AlignV(out t,out h);
						node.InternalSetBoundingRect(new Gdi::Rectangle(l,t,w,h));
						SetLeftFixingOther(ref oRect,l+w);
						SetLeftFixingOther(ref iRect,l+w+node.MarginRight);
						break;
					}
					case ArrangeStyle.DockRight:{
						int w=node.DesiredHeight;
						int l=RightLimit-w;
						int t,h;AlignV(out t,out h);
						node.InternalSetBoundingRect(new Gdi::Rectangle(l,t,w,h));
						SetRightFixingOther(ref oRect,l);
						SetRightFixingOther(ref iRect,l-node.MarginLeft);
						break;
					}
					default:
						throw new System.NotSupportedException("指定した ArrangeStyle には対応していません。");
				}
			}
			//------------------------------------------------------------------------
			//	Gdi::Rectangle Utility
			//------------------------------------------------------------------------
			public static void SetTopFixingOther(ref Gdi::Rectangle self,int value){
				int d=value-self.Y;
				self.Y=value;
				self.Height-=d;
			}
			public static void SetBottomFixingOther(ref Gdi::Rectangle self,int value){
				int d=value-self.Bottom;
				self.Height+=d;
			}
			public static void SetLeftFixingOther(ref Gdi::Rectangle self,int value){
				int d=value-self.X;
				self.X=value;
				self.Width-=d;
			}
			public static void SetRightFixingOther(ref Gdi::Rectangle self,int value){
				int d=value-self.Right;
				self.Width+=d;
			}
			//------------------------------------------------------------------------
			//	limits : Margin を考慮した近接限界
			//------------------------------------------------------------------------
			public int LeftLimit  {get{return System.Math.Max(iRect.Left,oRect.Left+node.MarginLeft);}}
			public int RightLimit {get{return System.Math.Min(iRect.Right,oRect.Right-node.MarginRight);}}
			public int TopLimit   {get{return System.Math.Max(iRect.Top,oRect.Top+node.MarginTop);}}
			public int BottomLimit{get{return System.Math.Min(iRect.Bottom,oRect.Bottom-node.MarginBottom);}}
			//------------------------------------------------------------------------
			//	arrange boundary
			//------------------------------------------------------------------------
			public void FillH(out int l,out int w){
				int r;
				l=LeftLimit;
				r=RightLimit;
				w=r-l;if(w<1){l-=w/2;w=1;}
			}
			public void AlignH(out int l,out int w){
				switch(node.ArrangeStyle.MaskHAlign()){
					case ArrangeStyle.AlignStretch:
						this.FillH(out l,out w);
						break;
					case ArrangeStyle.AlignLeft:
						w=node.DesiredWidth;
						l=LeftLimit;
						break;
					case ArrangeStyle.AlignRight:
						w=node.DesiredWidth;
						l=RightLimit-w;
						break;
					case ArrangeStyle.AlignCenter:
						w=node.DesiredWidth;
						l=LeftLimit;
						int wlim=RightLimit-l;
						l+=(wlim-w)/2; // 幅の余裕の分だけ中央に移動
						break;
					default:
						throw new System.NotSupportedException("指定した横位置合わせには対応していません。");
				}
			}
			public void FillV(out int t,out int h){
				int b;
				t=TopLimit;
				b=BottomLimit;
				h=b-t;if(h<1){t-=h/2;h=1;}
			}
			public void AlignV(out int t,out int h){
				switch(node.ArrangeStyle.MaskHAlign()){
					case ArrangeStyle.AlignStretch:
						this.FillV(out t,out h);
						break;
					case ArrangeStyle.AlignTop:
						h=node.DesiredHeight;
						t=TopLimit;
						break;
					case ArrangeStyle.AlignBottom:
						h=node.DesiredHeight;
						t=BottomLimit-h;
						break;
					case ArrangeStyle.AlignMiddle:
						h=node.DesiredHeight;
						t=TopLimit;
						int wlim=BottomLimit-t;
						t+=(wlim-h)/2; // 高さの余裕の分だけ中央に移動
						break;
					default:
						throw new System.NotSupportedException("指定した縦位置合わせには対応していません。");
				}
			}
		}
		#endregion
		protected override void OnSizeChanged(System.Drawing.Size oldSize){
			base.OnSizeChanged(oldSize);
			this.ArrangeNodes(null);
		}
		//==========================================================================
		//  M:GetHoge(RectCoord)
		//--------------------------------------------------------------------------
		#region M:GetHoge(RectCoord)
		//  Desired
		//    ↑     DesiredLeft
		//  Parent
		//    ↓     BoundingLeft
		//  Bounding
		//    ↓     Border
		//  View
		//  Content
		//    ↓     Offset
		//  Inner
		//--------------------------------------------------------------------------
		private static int getCoordLevel(RectCoord coord){
			switch(coord){
				case RectCoord.Desired:  return 0;
				case RectCoord.Parent:   return 1;
				case RectCoord.Bounding: return 2;
				case RectCoord.View:     return 3;
				case RectCoord.Content:  return 3;
				case RectCoord.Inner:    return 4;
				default:throw new System.NotSupportedException();
			}
		}
		public override int GetLeft(RectCoord coord,RectCoord axis){
			return this.GetLeft(getCoordLevel(coord),getCoordLevel(axis));
		}
		private int GetLeft(int lvF,int lvI){
			if(lvI>lvF)return -this.GetLeft(lvI,lvF);
			int ret=0;
			switch(lvI){
				case 0:if(lvF==0)return ret;
					ret-=this.DesiredLeft;
					goto case 1;
				case 1:if(lvF==1)return ret;
					ret+=this.BoundingLeft;
					goto case 2;
				case 2:if(lvF==2)return ret;
					ret+=this.BorderLeft;
					goto case 3;
				case 3:if(lvF==3)return ret;
					ret+=this.OffsetLeft;
					goto case 4;
				case 4:return ret;
				default:throw new System.InvalidProgramException();
			}
		}
		public override int GetTop(RectCoord coord,RectCoord axis){
			return this.GetTop(getCoordLevel(coord),getCoordLevel(axis));
		}
		private int GetTop(int lvF,int lvI){
			if(lvI>lvF)return -this.GetTop(lvI,lvF);
			int ret=0;
			switch(lvI){
				case 0:if(lvF==0)return ret;
					ret-=this.DesiredTop;
					goto case 1;
				case 1:if(lvF==1)return ret;
					ret+=this.BoundingTop;
					goto case 2;
				case 2:if(lvF==2)return ret;
					ret+=this.BorderTop;
					goto case 3;
				case 3:if(lvF==3)return ret;
					ret+=this.OffsetTop;
					goto case 4;
				case 4:return ret;
				default:throw new System.InvalidProgramException();
			}
		}
		public override Gdi::Point GetLocation(RectCoord coord,RectCoord axis){
			return this.GetLocation(getCoordLevel(coord),getCoordLevel(axis));
		}
		private Gdi::Point GetLocation(int lvF,int lvI){
			Gdi::Point ret=new Gdi::Point();
			if(lvI>lvF){
				ret=this.GetLocation(lvI,lvF);
				ret.X=-ret.X;
				ret.Y=-ret.Y;
				return ret;
			}
			switch(lvI){
				case 0:if(lvF==0)return ret;
					ret.X-=this.DesiredLeft;
					ret.Y-=this.DesiredTop ;
					goto case 1;
				case 1:if(lvF==1)return ret;
					ret.X+=this.BoundingLeft;
					ret.Y+=this.BoundingTop ;
					goto case 2;
				case 2:if(lvF==2)return ret;
					ret.X+=this.BorderLeft;
					ret.Y+=this.BorderTop ;
					goto case 3;
				case 3:if(lvF==3)return ret;
					ret.X+=this.OffsetLeft;
					ret.Y+=this.OffsetTop ;
					goto case 4;
				case 4:return ret;
				default:throw new System.InvalidProgramException();
			}
		}
		//--------------------------------------------------------------------------
		public override int GetWidth(RectCoord coord){
			switch(coord){
				case RectCoord.Desired:  return this.DesiredWidth;
				case RectCoord.Bounding: return this.BoundingWidth;
				case RectCoord.View:     return this.ViewWidth;
				case RectCoord.Content:  return this.ContentWidth;
				case RectCoord.Inner:    return this.InnerWidth;
				default:throw new System.NotSupportedException();
			}
		}
		public override int GetHeight(RectCoord coord){
			switch(coord){
				case RectCoord.Desired:  return this.DesiredHeight;
				case RectCoord.Bounding: return this.BoundingHeight;
				case RectCoord.View:     return this.ViewHeight;
				case RectCoord.Content:  return this.ContentHeight;
				case RectCoord.Inner:    return this.InnerHeight;
				default:throw new System.NotSupportedException();
			}
		}
		public override Gdi::Size GetSize(RectCoord coord){
			switch(coord){
				case RectCoord.Desired:  return new Gdi::Size(this.DesiredWidth ,this.DesiredHeight );
				case RectCoord.Bounding: return new Gdi::Size(this.BoundingWidth,this.BoundingHeight);
				case RectCoord.View:     return new Gdi::Size(this.ViewWidth    ,this.ViewHeight    );
				case RectCoord.Content:  return new Gdi::Size(this.ContentWidth ,this.ContentHeight );
				case RectCoord.Inner:    return new Gdi::Size(this.InnerWidth   ,this.InnerHeight   );
				default:throw new System.NotSupportedException();
			}
		}
		#endregion
		//==========================================================================
		//  Properties
		//--------------------------------------------------------------------------
		#region Properties
		//  property Offset
		//--------------------------------------------------------------------------
		FourWidth offset;
		/// <summary>
		/// 境界線の幅を取得又は設定します。
		/// </summary>
		public FourWidth Offset{
			get{return this.offset;}
			set{
				if(this.offset==value)return;
				this.offset=value;
				this.OnOffsetChanged();
			}
		}
		/// <summary>
		/// 上境界線の幅を取得又は設定します。
		/// </summary>
		public int OffsetTop{
			get{return this.offset.Top;}
			set{
				if(this.offset.top==value)return;
				this.offset.Top=value;
				this.OnOffsetChanged();
			}
		}
		/// <summary>
		/// 下境界線の幅を取得又は設定します。
		/// </summary>
		public int OffsetBottom{
			get{return this.offset.Bottom;}
			set{
				if(this.offset.bottom==value)return;
				this.offset.Bottom=value;
				this.OnOffsetChanged();
			}
		}
		/// <summary>
		/// 左境界線の幅を取得又は設定します。
		/// </summary>
		public int OffsetLeft{
			get{return this.offset.Left;}
			set{
				if(this.offset.left==value)return;
				this.offset.Left=value;
				this.OnOffsetChanged();
			}
		}
		/// <summary>
		/// 右境界線の幅を取得又は設定します。
		/// </summary>
		public int OffsetRight{
			get{return this.offset.Right;}
			set{
				if(this.offset.right==value)return;
				this.offset.Right=value;
				this.OnOffsetChanged();
			}
		}
		protected virtual void OnOffsetChanged(){
			this.ArrangeNodes(null);
		}
		//--------------------------------------------------------------------------
		//  property Border
		//--------------------------------------------------------------------------
		FourWidth border;
		/// <summary>
		/// 境界線の幅を取得又は設定します。
		/// </summary>
		public FourWidth Border{
			get{return this.border;}
			set{
				if(this.border==value)return;
				this.border=value;
				this.OnBorderChanged();
			}
		}
		/// <summary>
		/// 上境界線の幅を取得又は設定します。
		/// </summary>
		public int BorderTop{
			get{return this.border.Top;}
			set{
				if(this.border.top==value)return;
				this.border.Top=value;
				this.OnBorderChanged();
			}
		}
		/// <summary>
		/// 下境界線の幅を取得又は設定します。
		/// </summary>
		public int BorderBottom{
			get{return this.border.Bottom;}
			set{
				if(this.border.bottom==value)return;
				this.border.Bottom=value;
				this.OnBorderChanged();
			}
		}
		/// <summary>
		/// 左境界線の幅を取得又は設定します。
		/// </summary>
		public int BorderLeft{
			get{return this.border.Left;}
			set{
				if(this.border.left==value)return;
				this.border.Left=value;
				this.OnBorderChanged();
			}
		}
		/// <summary>
		/// 右境界線の幅を取得又は設定します。
		/// </summary>
		public int BorderRight{
			get{return this.border.Right;}
			set{
				if(this.border.right==value)return;
				this.border.Right=value;
				this.OnBorderChanged();
			}
		}
		protected virtual void OnBorderChanged(){
			this.ArrangeNodes(null);
		}
		//--------------------------------------------------------------------------
		//  property ViewRect
		//--------------------------------------------------------------------------
		/// <summary>
		/// 内容表示の矩形を取得又は設定します。
		/// </summary>
		public System.Drawing.Rectangle ViewRect{
			get{
				return new Gdi::Rectangle(
					this.ViewLeft,
					this.ViewTop,
					this.ViewWidth,
					this.ViewHeight
				);
			}
		}
		/// <summary>
		/// 内容表示の位置を取得します。
		/// </summary>
		public Gdi::Point ViewLocation{
			get{return new Gdi::Point(this.ViewLeft,this.ViewTop);}
		}
		/// <summary>
		/// 内容表示の大きさを取得します。
		/// </summary>
		public Gdi::Size ViewSize{
			get{return new Gdi::Size(this.ViewWidth,this.ViewHeight);}
		}
		/// <summary>
		/// 内容表示の横位置を取得します。
		/// </summary>
		public int ViewLeft{
			get{return this.BoundingLeft+this.border.Left;}
		}
		/// <summary>
		/// 内容表示の縦位置を取得します。
		/// </summary>
		public int ViewTop{
			get{return this.BoundingTop+this.border.Top;}
		}
		/// <summary>
		/// 内容表示の横幅を取得します。
		/// </summary>
		public int ViewWidth{
			get{return this.BoundingWidth-this.border.Left-this.border.Right;}
		}
		/// <summary>
		/// 内容表示の縦幅を取得します。
		/// </summary>
		public int ViewHeight{
			get{return this.BoundingHeight-this.border.Top-this.border.Bottom;}
		}
		//--------------------------------------------------------------------------
		//  property ContentRect
		//--------------------------------------------------------------------------
		/// <summary>
		/// 内容の矩形を取得します。
		/// </summary>
		public System.Drawing.Rectangle ContentRect{
			get{
				return new Gdi::Rectangle(
					this.ContentLeft,
					this.ContentTop,
					this.ContentWidth,
					this.ContentHeight
				);
			}
		}
		/// <summary>
		/// 内容の位置を取得します。
		/// </summary>
		public Gdi::Point ContentLocation{
			get{return new Gdi::Point(this.ContentLeft,this.ContentTop);}
		}
		/// <summary>
		/// 内容の大きさを取得します。
		/// </summary>
		public Gdi::Size ContentSize{
			get{return new Gdi::Size(this.ContentWidth,this.ContentHeight);}
		}
		/// <summary>
		/// 内容の横位置を取得します。
		/// </summary>
		public virtual int ContentLeft{
			get{return this.ViewLeft;}
		}
		/// <summary>
		/// 内容の縦位置を取得します。
		/// </summary>
		public virtual int ContentTop{
			get{return this.ViewTop;}
		}
		/// <summary>
		/// 内容の横幅を取得します。
		/// </summary>
		public virtual int ContentWidth{
			get{return this.ViewWidth;}
		}
		/// <summary>
		/// 内容の縦幅を取得します。
		/// </summary>
		public virtual int ContentHeight{
			get{return this.ViewHeight;}
		}
		//--------------------------------------------------------------------------
		//  property InnerRect
		//--------------------------------------------------------------------------
		/// <summary>
		/// 内容配置の矩形を取得します。
		/// </summary>
		public System.Drawing.Rectangle InnerRect{
			get{
				return new Gdi::Rectangle(
					this.InnerLeft,
					this.InnerTop,
					this.InnerWidth,
					this.InnerHeight
				);
			}
		}
		/// <summary>
		/// 内容配置の横位置を取得します。
		/// </summary>
		public int InnerLeft{
			get{return this.offset.Left;}
		}
		/// <summary>
		/// 内容配置の縦位置を取得します。
		/// </summary>
		public int InnerTop{
			get{return this.offset.Top;}
		}
		/// <summary>
		/// 内容配置の横幅を取得します。
		/// </summary>
		public int InnerWidth{
			get{return this.ContentWidth-this.offset.Left-this.offset.Right;}
		}
		/// <summary>
		/// 内容配置の縦幅を取得します。
		/// </summary>
		public int InnerHeight{
			get{return this.ContentHeight-this.offset.Top-this.offset.Bottom;}
		}
		#endregion
		//==========================================================================
		//  座標処理
		//--------------------------------------------------------------------------
		/// <summary>
		/// 指定した点がこのノードの内部に存在するかを判定します。
		/// </summary>
		/// <param name="posInParent">判定する点を、親 Content 座標系で指定します。</param>
		/// <returns>指定した点がこの要素の内部に存在する時、正の値を返します。
		/// 指定した点がこの要素の外部に存在する時、負の値を返します。
		/// </returns>
		public override HitValue HitTest(Gdi::Point posInParent){
			HitValue hit=base.HitTest(posInParent);
			if(hit<0)return hit;

			if(!this.ViewRect.Contains(posInParent)){
				return HitValue.RegionBorder;
			}

			return 0;
		}
		/// <summary>
		/// 指定した位置にある子孫ノードを取得します。
		/// </summary>
		/// <param name="position">位置を Bounding 座標で指定します。</param>
		/// <param name="hit">指定した位置に子孫ノードが存在した場合に、その HitValue を返します。</param>
		/// <returns>指定した位置に子孫ノードが存在した場合にそれを返します。
		/// 指定した位置に子孫ノードが存在しなかった場合は null を返します。
		/// </returns>
		public ILayoutNode GetNodeAt(Gdi::Point position,out HitValue hit){
			if(this.children==null)goto nothit;
			if(!this.GetRect(RectCoord.View,RectCoord.Bounding).Contains(position))goto nothit;

			// content 座標に変換
			position=this.CoordTransf(position,RectCoord.Bounding,RectCoord.Content);
			for(int i=this.children.Count-1;i>=0;i--){
				ILayoutNode node=this.children[i];
				hit=node.HitTest(position);
				if(hit<0)continue;

				// hit
				IContainerNode nodec=node as IContainerNode;
				if(nodec==null)return node;

				// recursion
				HitValue hit2;
				position=nodec.CoordTransf(position,RectCoord.Parent,RectCoord.Bounding);
				ILayoutNode node2=nodec.GetNodeAt(position,out hit2);
				if(node2==null)return node;
				hit=hit2;
				return node2;
			}
		nothit:
			hit=HitValue.NotHit;
			return null;
		}
		//--------------------------------------------------------------------------
		/// <summary>
		/// Content 座標と View 座標の座標原点のずれを取得します。\vec{O}_C-\vec{O}_V
		/// </summary>
		/// <returns>座標原点のずれを返します。</returns>
		private Gdi::Size GetCoordDeltaContentFromView(){
			return new Gdi::Size(ContentLeft-ViewLeft,ContentTop-ViewTop);
		}
		/// <summary>
		/// View 座標から Content 座標に座標変換を行います。
		/// </summary>
		/// <param name="posView">座標変換をする View 座標値を指定します。</param>
		/// <returns>座標変換をした後の Content 座標値を返します。</returns>
		public Gdi::Point CoordTransfView2Content(Gdi::Point posView){
			return posView-this.GetCoordDeltaContentFromView();
		}
		/// <summary>
		/// Content 座標から View 座標に座標変換を行います。
		/// </summary>
		/// <param name="posContent">座標変換をする Content 座標値を指定します。</param>
		/// <returns>座標変換をした後の View 座標値を返します。</returns>
		public Gdi::Point CoordTransfContent2View(Gdi::Point posContent){
			return posContent+this.GetCoordDeltaContentFromView();
		}
		//--------------------------------------------------------------------------
		/// <summary>
		/// View 座標と親 Content 座標の座標原点のずれを取得します。\vec{O}_V-\vec{O}_P
		/// </summary>
		/// <returns>座標原点のずれを返します。</returns>
		private Gdi::Size GetCoordDeltaViewtFromParent(){
			return new Gdi::Size(ViewLeft,ViewTop);
		}
		/// <summary>
		/// 親 Content 座標から View 座標に座標変換を行います。
		/// </summary>
		/// <param name="posParent">座標変換をする親 Content 座標値を指定します。</param>
		/// <returns>座標変換をした後の View 座標値を返します。</returns>
		public Gdi::Point CoordTransfParent2View(Gdi::Point posParent){
			return posParent-this.GetCoordDeltaViewtFromParent();
		}
		/// <summary>
		/// View 座標から親 Content 座標に座標変換を行います。
		/// </summary>
		/// <param name="posView">座標変換をする View 座標値を指定します。</param>
		/// <returns>座標変換をした後の親 Content 座標値を返します。</returns>
		public Gdi::Point CoordTransfView2Parent(Gdi::Point posView){
			return posView+this.GetCoordDeltaViewtFromParent();
		}
		//--------------------------------------------------------------------------
	}
	/// <summary>
	/// ContainerNode の子ノードの集合を管理します。
	/// </summary>
	public class LayoutNodeCollection:Gen::IList<ILayoutNode>{
		IContainerNode parent;
		private Gen::List<ILayoutNode> data;
		internal bool isreadonly;

		public LayoutNodeCollection(IContainerNode parent){
			this.parent=parent;
			this.isreadonly=false;
			this.data=new Gen::List<ILayoutNode>();
		}
		/// <summary>
		/// 指定した子ノードを指定した位置に移動します。
		/// </summary>
		/// <param name="idst">移動先のコレクション内ので位置番号を指定します。</param>
		/// <param name="node">移動するノードを指定します。</param>
		public void MoveNode(int idst,ILayoutNode node){
			int isrc=data.IndexOf(node);
			if(isrc<0)throw new LayoutNodeFiliationException("指定したノードは子ノードではありません。");
			this.MoveNode(idst,isrc);
		}
		/// <summary>
		/// 指定した子ノードを指定した位置に移動します。
		/// </summary>
		/// <param name="idst">移動先のコレクション内ので位置番号を指定します。</param>
		/// <param name="isrc">移動元のノードの番号を指定します。</param>
		public void MoveNode(int idst,int isrc){
			lock(this.data){
				if(idst<0||data.Count<=idst)throw new System.IndexOutOfRangeException();
				if(isrc<0||data.Count<=isrc)throw new System.IndexOutOfRangeException();
				if(idst==isrc)return;

				ILayoutNode node=data[isrc];
				this.RemoveAt(isrc);
				this.Insert(idst,node);
				parent.ArrangeNodes(data[idst<isrc?idst:isrc]);
			}
		}

		public void AddRange(Gen::IEnumerable<ILayoutNode> items){
			ILayoutNode first=null;
			parent.SuppressArrange();
			foreach(ILayoutNode node in items){
				if(first==null)first=node;
				this.Add(node);
			}
			parent.ResumeArrange(first);
		}

		#region IList<ILayoutNode> メンバ
		/// <summary>
		/// 指定したノードがコレクションの中で何処に位置しているかを取得します。
		/// </summary>
		/// <param name="item">番号を知りたい子ノードを指定します。</param>
		/// <returns>指定したノードの番号を返します。
		/// 指定したノードがコレクションに含まれていない場合には -1 を返します。</returns>
		public int IndexOf(ILayoutNode item){
			return this.data.IndexOf(item);
		}
		/// <summary>
		/// 指定したノードをコレクションの指定した位置に挿入します。
		/// </summary>
		/// <param name="index">新しく挿入する子ノードを指定します。</param>
		/// <param name="node">子ノードの挿入先を指定します。</param>
		public void Insert(int index,ILayoutNode node){
			if(isreadonly)
				throw new LayoutNodeReadOnlyException("読込専用のノードに子ノードを挿入する事は出来ません。");

			//----- LABEL_ADD --------------------------------------------------------
			lock(this.data){
				//0.chk:loop/already
				if(node.IsAscendantOf(this.parent))
					throw new LayoutNodeFiliationException("先祖を子に追加する事は出来ません。");
				if(node.Parent==this.parent){
					this.MoveNode(index,node);
					return;
				}
				if(node.Parent!=null)node.Parent.RemoveChildren(node); // 1
				data.Insert(index,node); // 2
				node.InternalSetParent(this.parent); // 3-4
				parent.ArrangeNodes(node); //5
			}
			//------------------------------------------------------------------------
		}
		/// <summary>
		/// 指定した位置にあるノードをコレクションから削除します。
		/// </summary>
		/// <param name="index">削除するノードの位置番号を指定します。</param>
		public void RemoveAt(int index){
			if(this.isreadonly)
				throw new LayoutNodeReadOnlyException("読込専用のノードから子ノードを削除する事は出来ません。");

			//----- LABEL_REM --------------------------------------------------------
			lock(this.data){
				ILayoutNode node=this.data[index];//0
				node.InternalSetParent(null);//1,2
				this.data.RemoveAt(index);//3
				if(index<data.Count)
					parent.ArrangeNodes(data[index]);//4
			}
			//------------------------------------------------------------------------
		}
		/// <summary>
		/// 指定した位置にあるノードを取得又は設定します。
		/// </summary>
		/// <param name="index">取得又は設定するノードの位置を指定します。</param>
		/// <returns>指定した位置にあるノードを返します。</returns>
		public ILayoutNode this[int index]{
			get{return this.data[index];}
			set{
				if(this.isreadonly)
					throw new LayoutNodeReadOnlyException("読込専用のノードの子ノードを設定する事は出来ません。");

				lock(this.data){
					if(this.data[index]==value)return;

					//----- LABEL_REM --------------------------------------------------------
					//----- LABEL_ADD --------------------------------------------------------
					ILayoutNode node=this.data[index];//REM0
					//ADD0
					if(value.IsAscendantOf(this.parent))
						throw new LayoutNodeFiliationException("先祖を子に追加する事は出来ません。");
					if(value.Parent==this.parent){
						//*
						throw new LayoutNodeFiliationException("設定しようとしているノードは既に子ノードです。");
						/*/ // 移動上書き
						int isrc=data.IndexOf(value);
						this.RemoveAt(index);
						if(isrc<index)index--;else isrc--;
						this.MoveNode(index,isrc);
						return;
						//*/
					}
					if(value.Parent!=null)value.Parent.RemoveChildren(value);//ADD1
					node.InternalSetParent(null);//REM1,REM2
					this.data[index]=value;//REM3,ADD2
					value.InternalSetParent(this.parent);//ADD3,ADD4
					this.parent.ArrangeNodes(value);//REM4,ADD5
					//------------------------------------------------------------------------
				}
			}
		}
		#endregion

		#region ICollection<ILayoutNode> メンバ
		/// <summary>
		/// コレクションにノードを追加します。
		/// </summary>
		/// <param name="node">追加したいノードを指定します。</param>
		public void Add(ILayoutNode node){
			lock(this.data){
				if(node.Parent==this.parent)
					this.MoveNode(data.Count-1,node);
				else
					this.Insert(data.Count,node);
			}
		}
		/// <summary>
		/// コレクションにノードを追加します。
		/// </summary>
		public void  Clear(){
			if(isreadonly)
				throw new LayoutNodeReadOnlyException("読込専用のノードの子ノードをクリアする事は出来ません。");
			//----- LABEL_REM --------------------------------------------------------
			lock(this.data){
				//0
				foreach(ILayoutNode node in data)
					node.InternalSetParent(null);//1,2
				data.Clear();//3
				parent.ArrangeNodes(null);//4
			}
			//------------------------------------------------------------------------
		}
		/// <summary>
		/// 指定したノードがコレクションに含まれているか否かを判定します。
		/// </summary>
		/// <param name="item">含まれているか否か判定したいノードを指定します。</param>
		/// <returns>指定したノードが含まれていた場合に true を返します。
		/// それ以外の場合に false を返します。</returns>
		public bool Contains(ILayoutNode item){
			return this.data.Contains(item);
		}
		/// <summary>
		/// コレクションの内容を配列にコピーします。
		/// </summary>
		/// <param name="array">コピー先の配列を指定します。</param>
		/// <param name="arrayIndex">コピー先配列の中のコピー開始位置を指定します。</param>
		public void CopyTo(ILayoutNode[] array,int arrayIndex){
			this.data.CopyTo(array,arrayIndex);
		}
		/// <summary>
		/// コレクションに含まれているノードの数を取得します。
		/// </summary>
		public int Count{
			get{return this.data.Count;}
		}
		/// <summary>
		/// コレクションが読み取り専用であるか否かを取得します。
		/// </summary>
		public bool IsReadOnly{
			get{return this.isreadonly;}
		}
		/// <summary>
		/// 指定したノードをコレクションから削除します。
		/// </summary>
		/// <param name="item">コレクション中から削除するノードを指定します。</param>
		/// <returns>指定したノードがコレクションに含まれていた場合、
		/// これを削除して true を返します。それ以外の場合には false を返します。</returns>
		public bool Remove(ILayoutNode item){
			if(this.isreadonly)
				throw new LayoutNodeReadOnlyException("読込専用のノードから子ノードを削除する事は出来ません。");
			lock(this.data){
				int index=this.data.IndexOf(item);
				if(index<0)return false;
				this.RemoveAt(index);
				return true;
			}
		}
		#endregion

		#region IEnumerable<ILayoutNode> メンバ
		/// <summary>
		/// 子ノードの列挙子を取得します。
		/// </summary>
		/// <returns>作成した列挙子を返します。</returns>
		public System.Collections.Generic.IEnumerator<ILayoutNode> GetEnumerator(){
			return this.data.GetEnumerator();
		}
		/// <summary>
		/// 子ノードの列挙子を取得します。
		/// </summary>
		/// <returns>作成した列挙子を返します。</returns>
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator(){
			return this.data.GetEnumerator();
		}
		#endregion
	}
	public class ScrollableNode:ContainerNode{
		public ScrollableNode():base(){
			_todo.LayoutNodeEssential("ScrollMount 実装");
			_todo.LayoutNodeEssential("ContentTop, ContentLeft 再実装");
		}
	}
}
