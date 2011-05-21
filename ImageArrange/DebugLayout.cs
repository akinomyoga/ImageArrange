using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using Gdi=System.Drawing;
using System.Text;
using Forms=System.Windows.Forms;

namespace afh {
	using afh.Layout;

	public partial class DebugLayout:Forms::Form {
		public DebugLayout() {
			InitializeComponent();
		}

		private void button1_Click(object sender,EventArgs e) {
			afh.Layout.ContainerNode node=new afh.Layout.ContainerNode(){
				DesiredRect=new Gdi::Rectangle(10,10,200,200),
				ChildrenAdder=new ILayoutNode[]{
					new LayoutNode(){DesiredRect=new Gdi::Rectangle(5,5,32,32)},
					new LayoutNode(){DesiredRect=new Gdi::Rectangle(20,20,32,32)},
					new LayoutNode(){DesiredRect=new Gdi::Rectangle(100,5,32,32)},
					new LayoutNode(){DesiredRect=new Gdi::Rectangle(5,100,32,32)},
					new LayoutNode(){DesiredRect=new Gdi::Rectangle(5,5,10,10),ArrangeStyle=ArrangeStyle.DockTop,Margin=new FourWidth(3)},
					new LayoutNode(){DesiredRect=new Gdi::Rectangle(5,5,10,10),ArrangeStyle=ArrangeStyle.DockTop,Margin=new FourWidth(3)},
					new LayoutNode(){DesiredRect=new Gdi::Rectangle(5,5,10,10),ArrangeStyle=ArrangeStyle.DockTop,Margin=new FourWidth(3)},
					new LayoutNode(){DesiredRect=new Gdi::Rectangle(5,5,10,10),ArrangeStyle=ArrangeStyle.DockLeft,Margin=new FourWidth(3,3)},
					new LayoutNode(){DesiredRect=new Gdi::Rectangle(5,5,10,10),ArrangeStyle=ArrangeStyle.DockLeft,Margin=new FourWidth(3,3)},
					new LayoutNode(){DesiredRect=new Gdi::Rectangle(5,5,10,10),ArrangeStyle=ArrangeStyle.DockBottom,Margin=new FourWidth(3)},
					new LayoutNode(){DesiredRect=new Gdi::Rectangle(5,5,10,10),ArrangeStyle=ArrangeStyle.DockRight,Margin=new FourWidth(3)},
					new LayoutNode(){DesiredRect=new Gdi::Rectangle(5,5,10,10),ArrangeStyle=ArrangeStyle.DockRight,Margin=new FourWidth(3)},
					new LayoutNode(){DesiredRect=new Gdi::Rectangle(5,5,10,10),ArrangeStyle=ArrangeStyle.DockFill,Margin=new FourWidth(10)},
				}
			};
			((ILayoutNode)node).InternalSetBoundingRect(node.DesiredRect);

			using(var g=this.CreateGraphics()){
				Gdi::Drawing2D.Matrix mat=g.Transform;
				mat.Translate(10,0);
				mat.Shear(0.5f,0);
				g.Transform=mat;
				node.Draw(g);
				g.DrawString("三五夜",this.Font,Gdi::Brushes.Blue,new Gdi::PointF(50,50));
			}
		}
	}
}
