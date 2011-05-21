namespace afh.ImageArrange {
	partial class Form1 {
		/// <summary>
		/// 必要なデザイナ変数です。
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// 使用中のリソースをすべてクリーンアップします。
		/// </summary>
		/// <param name="disposing">マネージ リソースが破棄される場合 true、破棄されない場合は false です。</param>
		protected override void Dispose(bool disposing) {
			if(disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows フォーム デザイナで生成されたコード

		/// <summary>
		/// デザイナ サポートに必要なメソッドです。このメソッドの内容を
		/// コード エディタで変更しないでください。
		/// </summary>
		private void InitializeComponent() {
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
      this.treeView1 = new System.Windows.Forms.TreeView();
      this.splitter1 = new System.Windows.Forms.Splitter();
      this.listBox1 = new System.Windows.Forms.ListBox();
      this.splitter2 = new System.Windows.Forms.Splitter();
      this.toolStrip1 = new System.Windows.Forms.ToolStrip();
      this.toolStripButton1 = new System.Windows.Forms.ToolStripButton();
      this.toolStripButton2 = new System.Windows.Forms.ToolStripButton();
      this.toolStripButton3 = new System.Windows.Forms.ToolStripButton();
      this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
      this.panel1 = new System.Windows.Forms.Panel();
      this.treeView2 = new afh.Forms.TreeView();
      this.pictureBox2 = new afh.Forms.PictureBox();
      this.toolStrip1.SuspendLayout();
      this.panel1.SuspendLayout();
      this.SuspendLayout();
      // 
      // treeView1
      // 
      this.treeView1.Dock = System.Windows.Forms.DockStyle.Left;
      this.treeView1.Location = new System.Drawing.Point(209,27);
      this.treeView1.Name = "treeView1";
      this.treeView1.Size = new System.Drawing.Size(152,362);
      this.treeView1.TabIndex = 0;
      this.treeView1.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeView1_AfterSelect);
      // 
      // splitter1
      // 
      this.splitter1.Location = new System.Drawing.Point(361,27);
      this.splitter1.Name = "splitter1";
      this.splitter1.Size = new System.Drawing.Size(3,362);
      this.splitter1.TabIndex = 1;
      this.splitter1.TabStop = false;
      // 
      // listBox1
      // 
      this.listBox1.Dock = System.Windows.Forms.DockStyle.Left;
      this.listBox1.FormattingEnabled = true;
      this.listBox1.ItemHeight = 12;
      this.listBox1.Location = new System.Drawing.Point(364,27);
      this.listBox1.Name = "listBox1";
      this.listBox1.Size = new System.Drawing.Size(166,352);
      this.listBox1.TabIndex = 2;
      this.listBox1.SelectedIndexChanged += new System.EventHandler(this.listBox1_SelectedIndexChanged);
      // 
      // splitter2
      // 
      this.splitter2.Location = new System.Drawing.Point(530,27);
      this.splitter2.Name = "splitter2";
      this.splitter2.Size = new System.Drawing.Size(3,362);
      this.splitter2.TabIndex = 3;
      this.splitter2.TabStop = false;
      // 
      // toolStrip1
      // 
      this.toolStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
      this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButton1,
            this.toolStripButton2,
            this.toolStripButton3});
      this.toolStrip1.Location = new System.Drawing.Point(2,2);
      this.toolStrip1.Name = "toolStrip1";
      this.toolStrip1.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
      this.toolStrip1.Size = new System.Drawing.Size(712,25);
      this.toolStrip1.TabIndex = 5;
      this.toolStrip1.Text = "toolStrip1";
      // 
      // toolStripButton1
      // 
      this.toolStripButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
      this.toolStripButton1.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton1.Image")));
      this.toolStripButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
      this.toolStripButton1.Name = "toolStripButton1";
      this.toolStripButton1.Size = new System.Drawing.Size(23,22);
      this.toolStripButton1.Text = "重複 test";
      this.toolStripButton1.Click += new System.EventHandler(this.toolStripButton1_Click);
      // 
      // toolStripButton2
      // 
      this.toolStripButton2.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
      this.toolStripButton2.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton2.Image")));
      this.toolStripButton2.ImageTransparentColor = System.Drawing.Color.Magenta;
      this.toolStripButton2.Name = "toolStripButton2";
      this.toolStripButton2.Size = new System.Drawing.Size(23,22);
      this.toolStripButton2.Text = "フォルダを追加";
      this.toolStripButton2.Click += new System.EventHandler(this.toolStripButton2_Click);
      // 
      // toolStripButton3
      // 
      this.toolStripButton3.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
      this.toolStripButton3.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton3.Image")));
      this.toolStripButton3.ImageTransparentColor = System.Drawing.Color.Magenta;
      this.toolStripButton3.Name = "toolStripButton3";
      this.toolStripButton3.Size = new System.Drawing.Size(23,22);
      this.toolStripButton3.Text = "保存";
      this.toolStripButton3.Click += new System.EventHandler(this.toolStripButton3_Click);
      // 
      // panel1
      // 
      this.panel1.Controls.Add(this.treeView2);
      this.panel1.Dock = System.Windows.Forms.DockStyle.Left;
      this.panel1.Location = new System.Drawing.Point(2,27);
      this.panel1.Name = "panel1";
      this.panel1.Size = new System.Drawing.Size(207,362);
      this.panel1.TabIndex = 6;
      this.panel1.Paint += new System.Windows.Forms.PaintEventHandler(this.panel1_Paint);
      // 
      // treeView2
      // 
      this.treeView2.AllowDrop = true;
      this.treeView2.AutoScroll = true;
      this.treeView2.AutoScrollMinSize = new System.Drawing.Size(131,0);
      this.treeView2.BackColor = System.Drawing.Color.White;
      this.treeView2.DefaultNodeParams.BackColor = System.Drawing.Color.White;
      this.treeView2.DefaultNodeParams.Icon = null;
      this.treeView2.Dock = System.Windows.Forms.DockStyle.Fill;
      this.treeView2.Location = new System.Drawing.Point(0,0);
      this.treeView2.MultiSelect = true;
      this.treeView2.Name = "treeView2";
      this.treeView2.Size = new System.Drawing.Size(207,362);
      this.treeView2.TabIndex = 0;
      this.treeView2.Text = "treeView2";
      this.treeView2.FocusedNodeChanged += new afh.CallBack<afh.Forms.TreeView>(this.treeView2_FocusedNodeChanged);
      // 
      // pictureBox2
      // 
      this.pictureBox2.Dock = System.Windows.Forms.DockStyle.Fill;
      this.pictureBox2.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox2.Image")));
      this.pictureBox2.LimitMagnificationFullsize = true;
      this.pictureBox2.Location = new System.Drawing.Point(533,27);
      this.pictureBox2.Name = "pictureBox2";
      this.pictureBox2.Size = new System.Drawing.Size(181,362);
      this.pictureBox2.SizeMode = afh.Forms.PictureBox.SizeModeType.Stretch;
      this.pictureBox2.TabIndex = 7;
      this.pictureBox2.Text = "pictureBox2";
      // 
      // Form1
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F,12F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(716,391);
      this.Controls.Add(this.pictureBox2);
      this.Controls.Add(this.splitter2);
      this.Controls.Add(this.listBox1);
      this.Controls.Add(this.splitter1);
      this.Controls.Add(this.treeView1);
      this.Controls.Add(this.panel1);
      this.Controls.Add(this.toolStrip1);
      this.Name = "Form1";
      this.Padding = new System.Windows.Forms.Padding(2);
      this.Text = "ImageArrange";
      this.Load += new System.EventHandler(this.Form1_Load);
      this.toolStrip1.ResumeLayout(false);
      this.toolStrip1.PerformLayout();
      this.panel1.ResumeLayout(false);
      this.ResumeLayout(false);
      this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.TreeView treeView1;
		private System.Windows.Forms.Splitter splitter1;
		private System.Windows.Forms.ListBox listBox1;
		private System.Windows.Forms.Splitter splitter2;
		private System.Windows.Forms.ToolStrip toolStrip1;
		private System.Windows.Forms.ToolStripButton toolStripButton1;
		private System.Windows.Forms.ToolStripButton toolStripButton2;
		private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
		private System.Windows.Forms.ToolStripButton toolStripButton3;
		private System.Windows.Forms.Panel panel1;
		private afh.Forms.TreeView treeView2;
		private afh.Forms.PictureBox pictureBox2;
	}
}

