#define USE_AFH_TREEVIEW

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using Gdi=System.Drawing;

namespace afh.ImageArrange{
	public partial class Form1:Form{
		public Form1(){
			InitializeComponent();
			this.toolStripButton3.Image=afh.Drawing.Icons.Save;
			this.toolStripButton2.Image=afh.Drawing.Icons.Open;

			this.treeView2.DefaultNodeParams.DDBehavior
				=afh.Forms.TreeNodeDDBehaviorStatic.AllForDebug;
			//this.treeView2.DefaultNodeParams.CheckBox
			//  =afh.Forms.TreeNodeCheckBox.DoubleBorder;
			//this.treeView2.DefaultNodeParams.CheckBoxVisible=true;
			this.treeView2.DefaultNodeParams.Icon
				=afh.Forms.TreeNodeIcon.Folder;
			this.treeView2.DefaultNodeParams.IconVisible=true;
			this.treeView2.DefaultNodeParams.Font=new Gdi::Font("MeiryoKe_PGothic",8);
		}

		private ThumbsFile file;

		public void ReadFile(string filepath){
			/*
			if(!System.IO.File.Exists(filepath)){
				System.Console.WriteLine("指定したファイルは、存在していません。");
				return;
			}//*/

			this.file=ThumbsFile.OpenFile(filepath);
			this.UpdateView();
		}

		public void UpdateView(){
			this.treeView1.Nodes.Clear();
			this.treeView2.Nodes.Clear();
			foreach(ImageDirectory child in this.file.dirs){
				this.treeView1.Nodes.Add(new ImageTreeNode(child));
#if USE_AFH_TREEVIEW
				this.treeView2.Nodes.Add(new ImageTreeNode2(child));
#endif
			}
		}

		private sealed class ImageTreeNode:System.Windows.Forms.TreeNode{
			ImageDirectory dir;
			public ImageDirectory Directory{get{return this.dir;}}
			public ImageTreeNode(ImageDirectory dir):base(dir.name){
				this.dir=dir;
				this.UpdateChildNodes();
			}

			public void UpdateChildNodes(){
				this.Nodes.Clear();
				foreach(ImageDirectory child in this.dir.dirs){
					this.Nodes.Add(new ImageTreeNode(child));
				}
			}
		}
#if USE_AFH_TREEVIEW
		private sealed class ImageTreeNode2:afh.Forms.TextTreeNode{
			readonly ImageDirectory dir;
			public ImageDirectory Directory{get{return this.dir;}}
			public ImageTreeNode2(ImageDirectory dir):base(dir.name){
				this.dir=dir;
			}
			protected override void InitializeNodes(afh.Forms.TreeNodeCollection c){
				foreach(ImageDirectory child in this.dir.dirs){
					c.Add(new ImageTreeNode2(child));
				}
			}
			public override bool HasChildren{
				get{return this.dir.dirs.Count>0;}
			}
		}
#endif

		private void treeView1_AfterSelect(object sender,TreeViewEventArgs e){
			ImageTreeNode node=e.Node as ImageTreeNode;
			if(node!=null){
				this.listBox1.Items.Clear();
				foreach(int index in node.Directory.images){
					this.listBox1.Items.Add(file.thumbs[index]);
				}
			}
		}
		private void treeView2_FocusedNodeChanged(afh.Forms.TreeView sender){
#if USE_AFH_TREEVIEW
			ImageTreeNode2 node=sender.FocusedNode as ImageTreeNode2;
			if(node!=null){
				this.listBox1.Items.Clear();
				foreach(int index in node.Directory.images){
					this.listBox1.Items.Add(file.thumbs[index]);
				}
			}
#endif
		}

		private void listBox1_SelectedIndexChanged(object sender,EventArgs e){
			Thumb thm=this.listBox1.SelectedItem as Thumb;
			if(thm!=null){
				this.pictureBox2.LoadImageFromFile(thm.filepath);
			}
		}

		private void toolStripButton1_Click(object sender,EventArgs e){
			Utility.SearchNearImage(this.file);
			this.UpdateView();
		}

		private void toolStripButton2_Click(object sender,EventArgs e){
			// フォルダの追加
			this.folderBrowserDialog1.SelectedPath=this.file.DirectoryName;
			if(this.folderBrowserDialog1.ShowDialog(this)!=DialogResult.OK)return;

			this.file.AddFileDirectory(this.folderBrowserDialog1.SelectedPath);
			this.UpdateView();
		}

		private void toolStripButton3_Click(object sender,EventArgs e){
			// 保存
			this.file.Save();
		}

		private void Form1_Load(object sender,EventArgs e){
#if !USE_AFH_TREEVIEW
			afh.Forms.TreeNode node;
			for(int x=0;x<5;x++){
				node=new afh.Forms.TreeNode();
				for(int i=0;i<5;i++)
					//node.Nodes.Add(new mwg.Forms.TreeNode());
					node.Nodes.Add(new afh.Forms.TextTreeNode(i.ToString()+" 番目の要素"));
				this.treeView2.Nodes.Add(node);
			}
			this.treeView2.Nodes[0].IsEnabled=false;
#endif
		}

		private void treeView2_Click(object sender,EventArgs e) {

		}

		private void panel1_Paint(object sender,PaintEventArgs e) {

		}
	}
}