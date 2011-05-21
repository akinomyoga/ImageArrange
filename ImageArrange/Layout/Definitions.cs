using Gen=System.Collections.Generic;
using Gdi=System.Drawing;

namespace afh.Layout{
	using afh.Layout.Extension;

	/// <summary>
	/// 矩形の座標系の種類を表します。
	/// </summary>
	public enum RectCoord{
		Parent    =0x01,
		Desired   =0x02,
		Bounding  =0x03,
		View      =0x04,
		Content   =0x05,
		Inner     =0x06,
	}

	[System.Flags]
	public enum ArrangeStyle{
		None         =0x0000, // Dock 既定値
		DockRight    =0x0001,
		DockLeft     =0x0002,
		DockTop      =0x0003,
		DockBottom   =0x0004,
		DockFill     =0x0005,
		MASK_DOCK    =0x000F,
		AlignStretch =0x0000, // Align 既定値
		AlignCenter  =0x0100,
		AlignRight   =0x0200,
		AlignLeft    =0x0300,
		AlignMiddle  =0x1000,
		AlignTop     =0x2000,
		AlignBottom  =0x3000,
		MASK_HALIGN  =0x0F00,
		MASK_VALIGN  =0xF000,
	}

	[System.Flags]
	public enum HitValue:int{
		NotHit       =unchecked((int)0x80000000),

		MASK_VHIT    =0x03000000,
		VHitIn       =0x00000000, // VHit default
		VHitAbove    =0x01000000,
		VHitBelow    =0x02000000,

		MASK_HHIT    =0x0C000000,
		HHitIn       =0x00000000, // HHit default
		HHitLeft     =0x04000000,
		HHitRight    =0x08000000,

		RegionBorder =0x00010000,
	}

	#region struct FourWidth
	/// <summary>
	/// 上下左右の間隙・幅に関する情報を保持します。
	/// </summary>
	[System.Serializable]
	public struct FourWidth{
		internal int left;
		internal int right;
		internal int top;
		internal int bottom;
		//==========================================================================
		//  Constructers
		//--------------------------------------------------------------------------
		/// <summary>
		/// 指定した値を使用して FourWidth を初期化します。
		/// </summary>
		/// <param name="top">上側の値を指定します。</param>
		public FourWidth(int top){
			this.top=top;
			this.right=int.MinValue;
			this.bottom=int.MinValue;
			this.left=int.MinValue;
		}
		/// <summary>
		/// 指定した値を使用して FourWidth を初期化します。
		/// </summary>
		/// <param name="top">上側の値を指定します。</param>
		/// <param name="right">右側の値を指定します。</param>
		public FourWidth(int top,int right){
			this.top=top;
			this.right=right;
			this.bottom=int.MinValue;
			this.left=int.MinValue;
		}
		/// <summary>
		/// 指定した値を使用して FourWidth を初期化します。
		/// </summary>
		/// <param name="top">上側の値を指定します。</param>
		/// <param name="right">右側の値を指定します。</param>
		/// <param name="bottom">下側の値を指定します。</param>
		public FourWidth(int top,int right,int bottom){
			this.top=top;
			this.right=right;
			this.bottom=bottom;
			this.left=int.MinValue;
		}
		/// <summary>
		/// 指定した値を使用して FourWidth を初期化します。
		/// </summary>
		/// <param name="top">上側の値を指定します。</param>
		/// <param name="right">右側の値を指定します。</param>
		/// <param name="bottom">下側の値を指定します。</param>
		/// <param name="left">左側の値を指定します。</param>
		public FourWidth(int top,int right,int bottom,int left){
			this.top=top;
			this.right=right;
			this.bottom=bottom;
			this.left=left;
		}
		//==========================================================================
		//  Properties
		//--------------------------------------------------------------------------
		/// <summary>
		/// 左側の大きさを取得又は設定します。
		/// </summary>
		public int Left{
			get{
				if(this.left==int.MinValue)return this.Top;
				return this.left;
			}
			set{this.left=value;}
		}
		/// <summary>
		/// 右側の大きさを取得又は設定します。
		/// </summary>
		public int Right{
			get{
				if(this.right==int.MinValue)return this.Left;
				return this.right;
			}
			set{this.right=value;}
		}
		/// <summary>
		/// 上側の大きさを取得又は設定します。
		/// </summary>
		public int Top{
			get{
				if(this.top==int.MinValue)return 0;
				return this.top;
			}
			set{this.top=value;}
		}
		/// <summary>
		/// 下側の大きさを取得又は設定します。
		/// </summary>
		public int Bottom{
			get{
				if(this.bottom==int.MinValue)return this.Top;
				return this.bottom;
			}
			set{this.bottom=value;}
		}
		//==========================================================================
		//  Operators
		//--------------------------------------------------------------------------
		/// <summary>
		/// 指定した FourWidth のインスタンスが等しいか否かを判定します。
		/// </summary>
		/// <param name="left">比較する FourWidth のインスタンスを指定します。</param>
		/// <param name="right">比較する FourWidth のインスタンスを指定します。</param>
		/// <returns>指定した FourWidth のインスタンスが等しかった場合に true を返します。</returns>
		public static bool operator==(FourWidth left,FourWidth right){
			return left.left==right.left
				&&left.right==right.right
				&&left.top==right.top
				&&left.bottom==right.bottom;
		}
		/// <summary>
		/// 指定した FourWidth のインスタンスが相異なるか否かを判定します。
		/// </summary>
		/// <param name="left">比較する FourWidth のインスタンスを指定します。</param>
		/// <param name="right">比較する FourWidth のインスタンスを指定します。</param>
		/// <returns>指定した FourWidth のインスタンスが相異なる場合に true を返します。</returns>
		public static bool operator!=(FourWidth left,FourWidth right){
			return !(left==right);
		}
		/// <summary>
		/// このインスタンスが指定したオブジェクトに等しいか否かを判定します。
		/// </summary>
		/// <param name="obj">比較対象のインスタンスを指定します。</param>
		/// <returns>指定したインスタンスと等しい場合に true を返します。</returns>
		public override bool Equals(object obj){
			return obj!=null&&obj is FourWidth&&this==(FourWidth)obj;
		}
		/// <summary>
		/// このインスタンスのハッシュコードを計算します。
		/// </summary>
		/// <returns>計算したハッシュコードを返します。</returns>
		public override int GetHashCode(){
			return this.left.GetHashCode()
				^this.right.GetHashCode()
				^this.top.GetHashCode()
				^this.bottom.GetHashCode();
		}
	}
	#endregion

	/// <summary>
	/// LayoutNode として要求されるメンバを実装します。
	/// </summary>
	public interface ILayoutNode{
		//==========================================================================
		//  木構造
		//--------------------------------------------------------------------------
		/// <summary>
		/// 親ノードを取得します。親ノードが存在しない場合は null を返します。
		/// </summary>
		IContainerNode Parent{get;}
		/// <summary>
		/// 子要素を保持しているか否かを取得します。
		/// </summary>
		bool HasChildren{get;}
		//--------------------------------------------------------------------------
		// 子を削除する時の手順 (LABEL_REM)
		//   0. check ... 登録されているか
		//   1. 親に登録しているリスナーを削除         @ InternalSetParent
		//   2. 親プロパティのフィールドを null に設定 @ InternalSetParent
		//   3. 親の保持する子コレクションから削除
		//   4. 元兄弟の再配置
		// 子を追加する時の手順 (LABEL_ADD)
		//   0. check ... 既に登録されていないか・循環を作ろうとしていないか
		//   1. 元の親から削除 (元の親が存在する場合)
		//   2. 親の子コレクションに追加
		//   3. 親プロパティのフィールドに登録         @ InternalSetParent
		//   4. リスナーを親に登録                     @ InternalSetParent
		//   5. 兄弟の再配置
		// ※親側で行う処理の方が多いように思われるので基本的に親の方で処理を行う
		//   子供側の処理は全て InternalSetParent 内で実行する事とする。
		//--------------------------------------------------------------------------
		/// <summary>
		/// 親ノードを設定し、親ノードへのイベントハンドラなどの登録を実行します。
		/// </summary>
		/// <param name="node">新しく親ノードとなる ILayoutNode を指定します。</param>
		void InternalSetParent(IContainerNode node);
		//==========================================================================
		//  幾何
		//--------------------------------------------------------------------------
		// 最低限必要な (親が使う) 幾何
		//   MarginLRTB   隣の物体との最小間隙
		//   DesiredRect  (from Parent.ContentRect) : 設定上の希望 BoudingRect
		//   BoundingRect (from Parent.ContentRect) : 境界線を含んだ領域
		// 場合によって実装される幾何
		//   BorderRTB    BoundingRect と ContentRect の間隙
		//   ViewRect     (from Parent.ContentRect) 内容を表示する領域
		//   OffsetRTB    ContentRect と InnerRect の間隙
		//   ContentRect  (from Parent.ContentRect) : 内容が配置される領域
		//   InnerRect    (from ContentRect)        : 内容が自動配置される領域
		// ※実際の配置は親ノードによって行われる。
		//   その際に Margin や Desired や ArrangeStyle のプロパティが参照される
		//--------------------------------------------------------------------------
		/// <summary>
		/// 左側の最小間隙を取得します。
		/// </summary>
		int MarginLeft{get;}
		/// <summary>
		/// 右側の最小間隙を取得します。
		/// </summary>
		int MarginRight{get;}
		/// <summary>
		/// 上側の最小間隙を取得します。
		/// </summary>
		int MarginTop{get;}
		/// <summary>
		/// 下側の最小間隙を取得します。
		/// </summary>
		int MarginBottom{get;}
		//--------------------------------------------------------------------------
		/// <summary>
		/// 設定横位置を取得します。
		/// </summary>
		int DesiredLeft{get;}
		/// <summary>
		/// 設定縦位置を取得します。
		/// </summary>
		int DesiredTop{get;}
		/// <summary>
		/// 設定横幅を取得します。
		/// </summary>
		int DesiredWidth{get;}
		/// <summary>
		/// 設定縦幅を取得します。
		/// </summary>
		int DesiredHeight{get;}
		/// <summary>
		/// 実際の横位置を取得します。
		/// </summary>
		int BoundingLeft{get;}
		/// <summary>
		/// 実際の縦位置を取得します。
		/// </summary>
		int BoundingTop{get;}
		/// <summary>
		/// 実際の横幅を取得します。
		/// </summary>
		int BoundingWidth{get;}
		/// <summary>
		/// 実際の縦幅を取得します。
		/// </summary>
		int BoundingHeight{get;}
		//--------------------------------------------------------------------------
		//  M:GetHoge(RectCoord)
		//--------------------------------------------------------------------------
		int GetLeft(RectCoord coord,RectCoord axis);
		int GetTop(RectCoord coord,RectCoord axis);
		Gdi::Point GetLocation(RectCoord coord,RectCoord axis);
		int GetWidth(RectCoord coord);
		int GetHeight(RectCoord coord);
		Gdi::Size GetSize(RectCoord coord);
#if false
		//EEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEE
		// 之の簡単な実装は以下の物をコピーすれば良い
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
		//EEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEE
#endif
		//--------------------------------------------------------------------------
		/// <summary>
		/// 自動配置をする際のスタイルを取得します。
		/// </summary>
		ArrangeStyle ArrangeStyle{get;}
		/// <summary>
		/// 実際に表示される時の位置を通知・設定します。
		/// </summary>
		/// <param name="rect">実際に表示される時の位置を指定します。</param>
		void InternalSetBoundingRect(Gdi::Rectangle rect);
		//--------------------------------------------------------------------------
		//  座標処理
		//--------------------------------------------------------------------------
		/// <summary>
		/// 指定した点がこのノードの内部に存在するかを判定します。
		/// </summary>
		/// <param name="positionInParent">判定する点を、親 Content 座標系で指定します。</param>
		/// <returns>指定した点がこの要素の内部に存在する時、零以上の値を返します。
		/// 指定した点がこの要素の外部に存在する時、負の値を返します。
		/// </returns>
		HitValue HitTest(Gdi::Point positionInParent);
		//==========================================================================
		//  描画
		//--------------------------------------------------------------------------
		void Draw(Gdi::Graphics g);
	}
	public interface IContainerNode:ILayoutNode{
		/// <summary>
		/// 指定したノードを子ノードとして追加します。
		/// </summary>
		/// <param name="node">追加する子ノードを指定します。</param>
		void AddChildren(ILayoutNode node);
		/// <summary>
		/// 指定した子ノードを子ノードコレクションから削除します。
		/// </summary>
		/// <param name="node">削除する子ノードを指定します。</param>
		bool RemoveChildren(ILayoutNode node);
		/// <summary>
		/// 子ノード集合を変更する事が可能かどうかを取得又は設定します。
		/// </summary>
		bool IsChildrenMutable{get;}
		//--------------------------------------------------------------------------
		//  座標処理
		//--------------------------------------------------------------------------
		/// <summary>
		/// 子ノードの配置を計算・更新します。
		/// </summary>
		/// <param name="changedChild">
		/// 変更が行われた最初の子ノードを指定します。
		/// この子ノード以降のノードが再計算の対象です。
		/// null を指定した場合には、全ての子ノードの配置が再計算されます。
		/// </param>
		void ArrangeNodes(ILayoutNode changedChild);
		void SuppressArrange();
		void ResumeArrange(ILayoutNode changedChild);
		/// <summary>
		/// 指定した位置にある子孫ノードを取得します。
		/// </summary>
		/// <param name="posContent">位置を Content 座標で指定します。</param>
		/// <param name="hit">指定した位置に子孫ノードが存在した場合に、その HitValue を返します。</param>
		/// <returns>指定した位置に子孫ノードが存在した場合にそれを返します。
		/// 指定した位置に子孫ノードが存在しなかった場合は null を返します。
		/// </returns>
		ILayoutNode GetNodeAt(Gdi::Point posContent,out HitValue hit);
	}
	namespace Extension{
		public static class GdiExtensionMethods{
			public static Gdi::Rectangle ToRectangle(this Gdi::RectangleF self){
				return new Gdi::Rectangle((int)self.X,(int)self.Y,(int)self.Width,(int)self.Height);
			}
			public static Gdi::Rectangle Translate(this Gdi::Rectangle self,int dx,int dy){
				self.X+=dx;
				self.Y+=dy;
				return self;
			}
		}
		/// <summary>
		/// ArrangeStyle 列挙体に拡張メソッドを提供します。
		/// </summary>
		public static class EnumExtensionMethods{
			public static ArrangeStyle MaskDock(this ArrangeStyle self){
				return self&ArrangeStyle.MASK_DOCK;
			}
			public static ArrangeStyle MaskVAlign(this ArrangeStyle self){
				return self&ArrangeStyle.MASK_VALIGN;
			}
			public static ArrangeStyle MaskHAlign(this ArrangeStyle self){
				return self&ArrangeStyle.MASK_HALIGN;
			}
		}
		/// <summary>
		/// ILayoutNode に拡張メソッドを提供します。
		/// </summary>
		public static class ILayoutNodeExtensionMethods{
			/// <summary>
			/// 自分が、指定したノードの子孫であるか否かを判定します。
			/// </summary>
			/// <param name="self">判定対象の自ノードを指定します。</param>
			/// <param name="ancestor">先祖である可能性のあるノードを指定します。</param>
			/// <returns>自分が、指定したノードの子孫である場合に true を返します。
			/// そうでない場合には false を返します。
			/// 指定したノードが自分自身である場合には false を返します。
			/// </returns>
			public static bool IsDescendantOf(this ILayoutNode self,ILayoutNode ancestor){
				if(self==null||ancestor==null)return false;
				ILayoutNode p=self;
				while(p.Parent!=null){
					p=p.Parent;
					if(p==ancestor)return true;
				}
				return false;
			}
			/// <summary>
			/// 自分が、指定したノードの先祖であるか否かを判定します。
			/// </summary>
			/// <param name="self">判定対象の自ノードを指定します。</param>
			/// <param name="descen">子孫である可能性のあるノードを指定します。</param>
			/// <returns>自分が、指定したノードの先祖である場合に true を返します。
			/// そうでない場合には false を返します。
			/// 指定したノードが自分自身である場合には false を返します。
			/// </returns>
			public static bool IsAscendantOf(this ILayoutNode self,ILayoutNode descen){
				return descen.IsDescendantOf(self);
			}
			//------------------------------------------------------------------------
			//  自動配置
			//------------------------------------------------------------------------
			public static Gdi::Rectangle GetDesiredRect(this ILayoutNode self){
				return new Gdi::Rectangle(self.DesiredLeft,self.DesiredTop,self.DesiredWidth,self.DesiredHeight);
			}
			public static Gdi::Rectangle GetBoundingRect(this ILayoutNode self){
				return new Gdi::Rectangle(self.BoundingLeft,self.BoundingTop,self.BoundingWidth,self.BoundingHeight);
			}
			public static Gdi::Size GetBoundingSize(this ILayoutNode self){
				return new Gdi::Size(self.BoundingWidth,self.BoundingHeight);
			}
			public static Gdi::Point GetBoundingLocation(this ILayoutNode self){
				return new Gdi::Point(self.BoundingLeft,self.BoundingTop);
			}
			//------------------------------------------------------------------------
			//  M:GetHoge(RectCoord)
			//------------------------------------------------------------------------
			#region M:GetHoge(RectCoord)
			//--------------------------------------------------------------------------
			//  Desired
			//    ↑     DesiredLeft
			//  Parent
			//    ↓     BoundingLeft
			//  Bounding
			//--------------------------------------------------------------------------
			private static int getCoordLevel(RectCoord coord){
				switch(coord){
					case RectCoord.Desired:  return 0;
					case RectCoord.Parent:   return 1;
					case RectCoord.Bounding: return 2;
					default:throw new System.NotSupportedException();
				}
			}
			public static int GetLeft(ILayoutNode self,RectCoord coord,RectCoord axis){
				return GetLeft(self,getCoordLevel(coord),getCoordLevel(axis));
			}
			private static int GetLeft(ILayoutNode self,int lvEnd,int lvSta){
				if(lvSta>lvEnd)return -GetLeft(self,lvSta,lvEnd);
				int ret=0;
				switch(lvSta){
					case 0:if(lvEnd==0)return ret;
						ret-=self.DesiredLeft;
						goto case 1;
					case 1:if(lvEnd==1)return ret;
						ret+=self.BoundingLeft;
						goto case 2;
					case 2:return ret;
					default:throw new System.InvalidProgramException();
				}
			}
			public static int GetTop(ILayoutNode self,RectCoord coord,RectCoord axis){
				return GetTop(self,getCoordLevel(coord),getCoordLevel(axis));
			}
			private static int GetTop(ILayoutNode self,int lvEnd,int lvSta){
				if(lvSta>lvEnd)return -GetTop(self,lvSta,lvEnd);
				int ret=0;
				switch(lvSta){
					case 0:if(lvEnd==0)return ret;
						ret-=self.DesiredTop;
						goto case 1;
					case 1:if(lvEnd==1)return ret;
						ret+=self.BoundingTop;
						goto case 2;
					case 2:return ret;
					default:throw new System.InvalidProgramException();
				}
			}
			public static Gdi::Point GetLocation(ILayoutNode self,RectCoord coord,RectCoord axis){
				return GetLocation(self,getCoordLevel(coord),getCoordLevel(axis));
			}
			private static Gdi::Point GetLocation(ILayoutNode self,int lvEnd,int lvSta){
				Gdi::Point ret=new Gdi::Point();
				if(lvSta>lvEnd){
					ret=GetLocation(self,lvSta,lvEnd);
					ret.X=-ret.X;
					ret.Y=-ret.Y;
					return ret;
				}
				switch(lvSta){
					case 0:if(lvEnd==0)return ret;
						ret.X-=self.DesiredLeft;
						ret.Y-=self.DesiredTop ;
						goto case 1;
					case 1:if(lvEnd==1)return ret;
						ret.X+=self.BoundingLeft;
						ret.Y+=self.BoundingTop ;
						goto case 2;
					case 2:return ret;
					default:throw new System.InvalidProgramException();
				}
			}
			//--------------------------------------------------------------------------
			public static int GetWidth(ILayoutNode self,RectCoord coord){
				switch(coord){
					case RectCoord.Desired:  return self.DesiredWidth;
					case RectCoord.Bounding: return self.BoundingWidth;
					default:throw new System.NotSupportedException();
				}
			}
			public static int GetHeight(ILayoutNode self,RectCoord coord){
				switch(coord){
					case RectCoord.Desired:  return self.DesiredHeight;
					case RectCoord.Bounding: return self.BoundingHeight;
					default:throw new System.NotSupportedException();
				}
			}
			public static Gdi::Size GetSize(ILayoutNode self,RectCoord coord){
				switch(coord){
					case RectCoord.Desired:  return new Gdi::Size(self.DesiredWidth ,self.DesiredHeight );
					case RectCoord.Bounding: return new Gdi::Size(self.BoundingWidth,self.BoundingHeight);
					default:throw new System.NotSupportedException();
				}
			}
			//--------------------------------------------------------------------------
			public static Gdi::Rectangle GetRect(this ILayoutNode self,RectCoord coord,RectCoord axis){
				return new Gdi::Rectangle(self.GetLocation(coord,axis),self.GetSize(coord));
			}
			public static int GetRight(this ILayoutNode self,RectCoord coord,RectCoord axis){
				return self.GetLeft(coord,axis)+self.GetWidth(coord);
			}
			public static int GetBottom(this ILayoutNode self,RectCoord coord,RectCoord axis){
				return self.GetTop(coord,axis)+self.GetHeight(coord);
			}
			//--------------------------------------------------------------------------
			public static void CoordTransf(this ILayoutNode self,ref Gdi::Point pos,RectCoord before,RectCoord after){
				Gdi::Point delta=self.GetLocation(before,after);
				pos.X+=delta.X;
				pos.Y+=delta.Y;
			}
			public static Gdi::Point CoordTransf(this ILayoutNode self,Gdi::Point pos,RectCoord before,RectCoord after){
				Gdi::Point ret=pos;
				self.CoordTransf(ref ret,before,after);
				return ret;
			}
			#endregion
			//------------------------------------------------------------------------
		}
		/// <summary>
		/// IContainerNode に拡張メソッドを提供します。
		/// </summary>
		public static class IContainerNodeExtensionMethods{
			//------------------------------------------------------------------------
			//  座標処理
			//------------------------------------------------------------------------
			/// <summary>
			/// 指定した位置にある子孫ノードを取得します。
			/// </summary>
			/// <param name="self">コンテナノードを指定します。</param>
			/// <param name="position">位置を Bounding 座標で指定します。</param>
			/// <returns>指定した位置に子孫ノードが存在した場合にそれを返します。
			/// 指定した位置に子孫ノードが存在しなかった場合は null を返します。
			/// </returns>
			public static ILayoutNode GetNodeAt(this IContainerNode self,Gdi::Point position){
				HitValue dummy;
				return self.GetNodeAt(position,out dummy);
			}
		}
	}

	public struct GraphicsClipRegionStore:System.IDisposable{
		Gdi::Graphics g;
		Gdi::Region clip;
		public GraphicsClipRegionStore(Gdi::Graphics g){
			this.g=g;
			this.clip=g.Clip.Clone();
		}
		public void Restore(){
			if(this.clip==null)return;
			Gdi::Region old=g.Clip;
			g.Clip=this.clip.Clone();
			old.Dispose();
		}
		public void Dispose(){
			if(this.clip==null)return;
			Gdi::Region old=g.Clip;
			g.Clip=this.clip;
			old.Dispose();
		}
	}
	public struct GraphicsClipRectStore:System.IDisposable{
		Gdi::Graphics g;
		Gdi::Rectangle rect;
		public GraphicsClipRectStore(Gdi::Graphics g){
			this.g=g;
			Gdi::RectangleF rectF=g.ClipBounds;
			this.rect=new Gdi::Rectangle(
				(int)rectF.X,(int)rectF.Y,
				(int)rectF.Width,(int)rectF.Height
				);
		}
		public void Restore(){
			this.g.SetClip(this.rect);
		}
		public void Dispose(){
			this.Restore();
		}
	}
	public struct GraphicsTransformStore:System.IDisposable{
		Gdi::Graphics g;
		Gdi::Drawing2D.Matrix mat;
		public GraphicsTransformStore(Gdi::Graphics g){
			this.g=g;
			this.mat=this.g.Transform.Clone();
		}
		public void Restore(){
			this.g.Transform.Dispose();
			this.g.Transform=this.mat.Clone();
		}
		public void Dispose(){
			this.g.Transform.Dispose();
			this.g.Transform=this.mat;
		}
	}
}
