using Gen=System.Collections.Generic;

namespace afh.ImageArrange{
	using System.Drawing;
	using Img=System.Drawing.Imaging;
	using afh.File;
	using Riff=afh.File.Riff;

	#region cls:ThumbsFile
	[CustomRead("read"),CustomWrite("write")]
	public class ThumbsFile{
		private const int VERSION=1;
		//=================================================
		//		fld: filename
		//=================================================
		private string filename="";
		/// <summary>
		/// このファイルが存在しているディレクトリのパスを取得します。
		/// </summary>
		public string DirectoryName{
			get{return System.IO.Path.GetDirectoryName(this.filename);}
		}
		//=================================================
		//		fld: thumbs
		//=================================================
		public ImageCollection thumbs;
		//=================================================
		//		fld: dirs
		//=================================================
		public DirectoryCollection dirs;
		/// <summary>
		/// ディレクトリ毎ファイルを追加します。
		/// </summary>
		/// <param name="path">ディレクトリのパスを指定します。</param>
		public void AddFileDirectory(string path){
			string imgdirName=afh.Application.Path.GetRelativePath(path,this.DirectoryName);
			if(imgdirName.StartsWith("."))imgdirName=System.IO.Path.GetFileName(path);
			this.dirs[imgdirName].AddFilesFromDirectory(path);
		}
		//=================================================
		//		他
		//=================================================
		/// <summary>
		/// ファイルを保存します。
		/// </summary>
		public void Save(){
			StreamAccessor accessor=new StreamAccessor(System.IO.File.OpenWrite(this.filename));
			accessor.Write(this);
			accessor.Stream.Close();
		}
		/// <summary>
		/// ThumbsFile をファイルから読み込みます。
		/// 指定したファイルが存在しない場合は新しくファイルを作成します。
		/// </summary>
		/// <param name="filename">ファイル名を指定します。</param>
		/// <returns>作成した ThumbsFile のインスタンスを返します。</returns>
		public static ThumbsFile OpenFile(string filename){
			ThumbsFile thumbsfile;
			if(System.IO.File.Exists(filename)){
				StreamAccessor accessor=new StreamAccessor(System.IO.File.OpenRead(filename));
				thumbsfile=accessor.Read<ThumbsFile>();
				accessor.Stream.Close();
			}else if(System.IO.Path.GetExtension(filename).ToLower()!=".thm"&&System.IO.File.Exists(filename+".thm")){
				filename+=".thm";
				StreamAccessor accessor=new StreamAccessor(System.IO.File.OpenRead(filename));
				thumbsfile=accessor.Read<ThumbsFile>();
				accessor.Stream.Close();
			}else{
				thumbsfile=new ThumbsFile();
				thumbsfile.dirs=new DirectoryCollection(thumbsfile);
				thumbsfile.thumbs=new ImageCollection(thumbsfile);
			}
			thumbsfile.filename=filename;
			return thumbsfile;
		}

		//=================================================
		//		Read Write
		//=================================================
		private ThumbsFile(){}
		public static ThumbsFile read(StreamAccessor accessor){
			ThumbsFile r=new ThumbsFile();
			Riff::RiffFile riff=Riff::RiffFile.FromStream(accessor.Stream,true);
			StreamAccessor ac;

			foreach(Riff::Chunk chunk in riff.EnumerateChunksByName("head")){
				ac=new StreamAccessor(chunk.Stream);
				if(VERSION!=ac.ReadInt32(EncodingType.I4))
					throw new System.ApplicationException("ファイルフォーマットの version が異なります。");
				break;
			}

			foreach(Riff::Chunk chunk in riff.EnumerateChunksByName("thms")){
				r.thumbs=chunk.GetContent<ImageCollection>();
				break;
			}

			foreach(Riff::Chunk chunk in riff.EnumerateChunksByName("dirs")){
				ac=new StreamAccessor(chunk.Stream);
				r.dirs=ac.Read<DirectoryCollection>();
				break;
			}
			riff.Close();

			r.read_initialize();
			return r;
		}
		private void read_initialize(){
			this.dirs.read_initialize(this);
			this.thumbs.read_initialize(this);
		}
		public static void write(ThumbsFile file,StreamAccessor accessor){
			Riff::RiffWriter riffw=new afh.File.Riff.RiffWriter("thum");
			StreamAccessor ac_chunk;
			
			riffw.AddChunk("head",out ac_chunk);
			ac_chunk.Write(VERSION,EncodingType.I4);

			riffw.AddChunk("thms",file.thumbs);
			
			riffw.AddChunk("dirs",out ac_chunk);
			ac_chunk.Write(file.dirs);

			riffw.Write(accessor);
			/*
			Riff::RiffFile riff=new Riff::RiffFile("thum");
			StreamAccessor ac;
			Riff::Chunk chunk;

			chunk=new Riff::Chunk("head");
			System.IO.Stream memstr=new System.IO.MemoryStream();
			ac=new StreamAccessor(memstr);
			ac.Write(VERSION,EncodingType.I4);
			chunk.Stream=memstr;
			riff.Chunks.Add(chunk);

			chunk=new Riff::Chunk("thms");
			chunk.SetContent(file.thumbs);
			riff.Chunks.Add(chunk);

			chunk=new Riff::Chunk("dirs");
			System.IO.MemoryStream memstr3=new System.IO.MemoryStream();
			ac=new StreamAccessor(memstr3);
			ac.Write(file.dirs);
			chunk.Stream=memstr3;
			riff.Chunks.Add(chunk);

			accessor.Write(riff);
			memstr.Close();
			memstr3.Close();
			//*/
		}
	}
	#endregion

	#region cls:Thumb
	/// <summary>
	/// 画像ファイルの Thumbnail に関する情報を保持します。
	/// </summary>
	[afh.File.ReadSchedule("index","filepath","lastwrite","data")]
	[afh.File.WriteSchedule("index","filepath","lastwrite","data")]
	public class Thumb:System.IComparable<Thumb>,System.IEquatable<Thumb>{
		public Thumb(string filename){
			this.filepath=filename;
			this.Update();
		}

		/// <summary>
		/// 読み込み用初期化子
		/// </summary>
		public Thumb(){}
		[afh.File.ReadWriteAs(EncodingType.I4)]
		public int index;
		[afh.File.ReadWriteAs(afh.File.EncodingType.StrBasic|afh.File.EncodingType.EncEmbedded|afh.File.EncodingType.Enc_utf_8)]
		public string filepath;
		[afh.File.ReadWriteAs(EncodingType.NoSpecified)]
		public System.DateTime lastwrite;
		[afh.File.ReadWriteAs(afh.File.EncodingType.NoSpecified)]
		public ThumbData data;

		/// <summary>
		/// サムネイルの内容を更新します。
		/// </summary>
		public void Update(){
			System.TimeSpan span=new System.IO.FileInfo(filepath).LastWriteTimeUtc-this.lastwrite;
			if(span.Ticks/System.TimeSpan.TicksPerSecond<=2)return;

			this.data=ThumbData.ReadFromFile(filepath);
			this.lastwrite=new System.IO.FileInfo(filepath).LastWriteTimeUtc;
		}

		public override string ToString() {
			return System.IO.Path.GetFileName(this.filepath);
		}

		//============================================================
		//		比較
		//============================================================
		public int CompareTo(Thumb other){
			float r=this.data.aspect-other.data.aspect;
			return r>0?1:r==0?0:-1;
		}
		bool System.IEquatable<Thumb>.Equals(Thumb other) {
			return this.data.aspect==other.data.aspect;
		}

		public static bool operator!=(Thumb thm1,Thumb thm2){
			return !(thm1==thm2);
		}
		public static bool operator==(Thumb thm1,Thumb thm2){
			if((object)thm1==null)return (object)thm1==null;
			if((object)thm2==null)return false;
			return thm1.index==thm2.index
				&&thm1.lastwrite==thm2.lastwrite
				&&thm1.filepath==thm2.filepath
				&&thm1.data.Equals(thm2.data);
		}
		public override int GetHashCode() {
			return this.filepath.GetHashCode()
				^this.lastwrite.GetHashCode()
				//^this.data.GetHashCode()
				^this.index.GetHashCode();
		}
		public override bool Equals(object obj) {
			Thumb thm=obj as Thumb;
			return (object)thm!=null&&this==thm;
		}
	}

	/// <summary>
	/// 画像の内容を表現する構造体です。
	/// TODO: Animation GIF 等には未だ対応していない。(初めのフレームで比較するのだから別に良いのでは?)
	/// </summary>
	[System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)]
	public unsafe struct ThumbData{
		public float aspect;
		public int height;
		public int width;
		public fixed int px[PIXELS];

		private const int WIDTH=8;
		private const int HEIGHT=8;
		private const int PIXELS=HEIGHT*WIDTH;

		public Bitmap GetThumbnailImage(){
			Bitmap ret=new Bitmap(WIDTH,HEIGHT,System.Drawing.Imaging.PixelFormat.Format32bppArgb);
			Img::BitmapData data=ret.LockBits(rect88,Img::ImageLockMode.WriteOnly,Img::PixelFormat.Format32bppArgb);
			// CHECK: data.Stride は 8*4 等でないと困る
			fixed(int* b=this.px){
				long* d=(long*)data.Scan0;
				long* s=(long*)b;
				long* M=(long*)(b+PIXELS);
				while(s<M)*d++=*s++;
			}
			ret.UnlockBits(data);
			return ret;
		}

		private static System.Drawing.Rectangle rect88=new Rectangle(0,0,WIDTH,HEIGHT);
		public static ThumbData ReadFromFile(string path){
			if(!System.IO.File.Exists(path))
				throw new System.ApplicationException("ファイルが存在していません。");

			ThumbData ret=new ThumbData();

			Bitmap bmp=new Bitmap(path);
			ret.width=bmp.Width;
			ret.height=bmp.Height;
			ret.aspect=ret.width/(float)ret.height;

			Bitmap thumb=new Bitmap(bmp.GetThumbnailImage(WIDTH,HEIGHT,delegate(){return true;},System.IntPtr.Zero));
			Img::BitmapData bmpdat=thumb.LockBits(rect88,System.Drawing.Imaging.ImageLockMode.ReadOnly,System.Drawing.Imaging.PixelFormat.Format32bppArgb);
			long* d=(long*)ret.px;
			long* M=(long*)(ret.px+PIXELS);
			long* s=(long*)bmpdat.Scan0;
			while(d<M)*d++=*s++;
			thumb.UnlockBits(bmpdat);

			thumb.Dispose();
			bmp.Dispose();
			return ret;
		}

		/// <summary>
		/// サムネイルが近いかどうかの判定を行います。
		/// </summary>
		/// <param name="l">比較の対象の ThumbData を指定します。交換可能です。</param>
		/// <param name="r">比較の対象の ThumbData を指定します。交換可能です。</param>
		/// <returns>近いと判断した場合に true を返します。</returns>
		public static bool operator ^(ThumbData l,ThumbData r){
			// アスペクト比で跳ねる
			if(!EqualAspect(l,r))return false;

			// ピクセルデータで判定
			int D=0;
			byte* c1=(byte*)l.px;
			byte* c1M=(byte*)(l.px+PIXELS);
			byte* c2=(byte*)r.px;
			while(c1<c1M)D+=*c1>*c2 ? *c1++-*c2++ : *c2++-*c1++;

			return D<=PIXELS*16;
		}

		public static bool EqualAspect(ThumbData l,ThumbData r){
			return System.Math.Abs(l.aspect/r.aspect-1)<=0.01;
		}
	}
	#endregion

	#region cls:ImageCollection
	[Riff::RiffChunkReadWrite("read_chunk","write_chunk")]
	public class ImageCollection:Gen::Dictionary<int,Thumb>{
		private ThumbsFile root;
		private int count;
		private readonly Gen::Dictionary<string,Thumb> paths;
		private readonly afh.Collections.SortedArrayP<Thumb> sorted;

		public ImageCollection(ThumbsFile root):base(){
			this.count=0;
			this.root=root;
			this.paths=new Gen::Dictionary<string,Thumb>();
			this.sorted=new afh.Collections.SortedArrayP<Thumb>();
		}

		public afh.Collections.SortedArrayP<Thumb> SortedThumbs{
			get{return this.sorted;}
		}
		/// <summary>
		/// 指定した画像ファイルの情報を取得します。
		/// 登録されていなかった場合には新しく登録します。
		/// </summary>
		/// <param name="filepath">画像ファイルの場所を指定します。</param>
		/// <returns>画像ファイルの情報を返します。</returns>
		public Thumb this[string filepath]{
			get{
				if(this.paths.ContainsKey(filepath)){
					return this.paths[filepath];
				}else{
					Thumb thm=new Thumb(filepath);
					thm.index=this.count++;
					this.register(thm);
					return thm;
				}
			}
		}
		/// <summary>
		/// 指定した画像ファイルを含んでいるかどうかを確認します。
		/// </summary>
		/// <param name="filepath">含んでいるかどうかを確認する画像ファイルのパスを指定します。</param>
		/// <returns>指定した画像ファイルを既に含んでいる場合には true を返します。
		/// 含んでいない場合には false を返します。</returns>
		public bool Contains(string filepath){
			return this.paths.ContainsKey(filepath);
		}
		/// <summary>
		/// 指定した画像ファイルの情報を更新します。画像ファイルが登録されていない場合には新しく登録します。
		/// </summary>
		/// <param name="filepath">更新する画像ファイルの場所を指定します。</param>
		public void Update(string filepath){
			this[filepath].Update();
		}
#if OLD
		[System.Obsolete]
		public void dbg_NearImage(){
			ImageDirectory overlap=new ImageDirectory("<重複検索>",root);
			Gen::Dictionary<Image,ImageDirectory> groups=new Gen::Dictionary<Image,ImageDirectory>();

			afh.ImageArrange.ProgressDialog progress=new ProgressDialog();
			progress.Title="重複比較中 ...";
			progress.ProgressMax=this.sorted.Count;

			progress.Show();
			for(int i=0;i<this.sorted.Count;i++) {
				Image img1=new Image(this.root,this.sorted[i]);

				if(i%50==0){
					progress.Description="画像 '"+img1.thm.filepath+"' の類似画像を探索中 ...";
					if(progress.IsCanceled)return;
				}

				for(int j=i+1;j<this.sorted.Count;j++){
					Image img2=new Image(this.root,this.sorted[j]);
					if(!Image.EqualAspect(img1,img2))break;

					if(Image.Resembles(img1,img2)){
						ImageDirectory group1=null;
						ImageDirectory group2=null;
						bool b1=groups.TryGetValue(img1,out group1);
						bool b2=groups.TryGetValue(img2,out group2);
						switch((b1?1:0)|(b2?2:0)){
							case 3: // 両方
								if(group1==group2)break;
								// 両グループの併合
								overlap.dirs.Remove(group2);
								foreach(Image img in group2.EnumImages()){
									group1.Add(img);
									groups[img]=group1;
								}
								break;
							case 1: // group1 だけ
								group1.Add(img2);
								groups.Add(img2,group1);
								break;
							case 2: // group2 だけ
								group2.Add(img1);
								groups.Add(img1,group2);
								break;
							case 0: // 両者未登録
								ImageDirectory group=new ImageDirectory("Group "+overlap.dirs.Count.ToString(),root);
								group.Add(img1);
								group.Add(img2);
								groups.Add(img1,group);
								groups.Add(img2,group);
								overlap.Add(group);
								break;
							default:
								throw new System.InvalidProgramException();
						}
						//System.Console.WriteLine("次の画像は似ています\r\n    {0}\r\n    {1}",thm1.filepath,thm2.filepath);
					}
				}
				if(i%10==0)
					progress.Progress=i;
			}
			progress.Close();
			
			this.root.dirs.Add(overlap);
		}
#endif

		private void register(Thumb thm){
			base.Add(thm.index,thm);
			this.paths[thm.filepath]=thm;
			this.sorted.Add(thm);
		}

		#region read write
		private ImageCollection():base(){
			this.paths=new Gen::Dictionary<string,Thumb>();
			this.sorted=new afh.Collections.SortedArrayP<Thumb>();
		}
		private ImageCollection(int capacity):base(capacity){
			this.paths=new Gen::Dictionary<string,Thumb>(capacity);
			this.sorted=new afh.Collections.SortedArrayP<Thumb>(capacity);
		}
		private static ImageCollection read_chunk(StreamAccessor accessor){
			int c=accessor.ReadInt32(EncodingType.I4);
			ImageCollection ret=new ImageCollection(c);
			ret.count=c;
			try{
				while(accessor.RestLength>0){
					ret.register(accessor.Read<Thumb>());
				}
			}catch(System.Exception e){
				afh.File.__dll__.log.WriteError(e,"ImageCollection 読み取り中のエラー");
			}
			return ret;
		}
		internal void read_initialize(ThumbsFile root){
			this.root=root;
		}
		private static void write_chunk(ImageCollection images,StreamAccessor accessor){
			accessor.Write(images.count,EncodingType.I4);
			foreach(Thumb thm in images.sorted)accessor.Write(thm);
		}
		#endregion
	}
	#endregion

	/// <summary>
	/// 画像のディレクトリを表現するクラスです。
	/// </summary>
	[CustomRead("read"),CustomWrite("write")]
	public class ImageDirectory{
		private ThumbsFile root;
		public string name;
		public readonly Gen::List<int> images=new Gen::List<int>();
		internal DirectoryCollection dirs;

		/// <summary>
		/// ImageDirectory の初期化子です。
		/// </summary>
		/// <param name="name">この Directory に付ける名前を指定します。</param>
		/// <param name="root">この ImageDirectory が所属する ThumbsFile を指定します。</param>
		public ImageDirectory(string name,ThumbsFile root){
			this.root=root;
			this.name=name;
			this.dirs=new DirectoryCollection(root);
		}


		//============================================================
		//		基本操作
		//============================================================
		/// <summary>
		/// 指定した画像がこのディレクトリに含まれているか否かを取得します。
		/// </summary>
		/// <param name="image">このディレクトリに含まれているかどうかを調べたい画像を指定します。</param>
		/// <returns>既に含まれていた場合に true を返します。含まれていなかった場合に false を返します。</returns>
		public bool Contains(Image image){
			return image.root==this.root&&this.images.Contains(image.thm.index);
		}
		/// <summary>
		/// 指定した画像をこのディレクトリに追加します。
		/// </summary>
		/// <param name="image">追加する画像を指定します。</param>
		public void Add(Image image){
			if(image.root!=this.root){
				// TODO:
				// 1. thumb オブジェクトの生成
				// 2. root の ImageCollection に追加
				// 3. ディレクトリに番号を登録
				throw new System.NotImplementedException("異なるファイルに属している画像データを追加するのには未対応です。");
			}else{
				this.images.Add(image.thm.index);
			}
		}
		/// <summary>
		/// 指定したディレクトリをこのディレクトリのサブディレクトリに追加します。
		/// </summary>
		/// <param name="directory">サブディレクトリとして追加するディレクトリを指定します。</param>
		public void Add(ImageDirectory directory){
			if(directory.root!=this.root){
				throw new System.NotImplementedException("異なるファイルのディレクトリを追加する操作は未実装です。");
			}else{
				this.dirs.Add(directory);
			}
		}
		/// <summary>
		/// このディレクトリに含まれている画像を列挙します。
		/// </summary>
		/// <returns>画像の列挙子を取得出来る IEnumerable を返します。</returns>
		public Gen::IEnumerable<Image> EnumImages(){
			foreach(int index in this.images)
				yield return new Image(this.root,this.root.thumbs[index]);
		}
		//============================================================
		//		ファイルの追加
		//============================================================
		/// <summary>
		/// 指定した file をこの ImageDirectory に登録します。
		/// </summary>
		/// <param name="filepath">登録する file への path を指定します。</param>
		public void AddFile(string filepath){
			int index=this.root.thumbs[filepath].index;
			if(!this.images.Contains(index))this.images.Add(index);
		}
		/// <summary>
		/// 指定した file-directory 内の画像をこの ImageDirectory に登録します。
		/// </summary>
		/// <param name="dirpath">登録する画像を含んでいるディレクトリへの path を指定します。</param>
		public void AddFilesFromDirectory(string dirpath){
			if(!System.IO.Directory.Exists(dirpath))return;

			System.IO.DirectoryInfo dir=new System.IO.DirectoryInfo(dirpath);
			Gen::List<string> filepaths=new Gen::List<string>();
			foreach(System.IO.FileInfo file in dir.GetFiles()){
				if(!exts.Contains(file.Extension.ToLower()))continue;
				filepaths.Add(file.FullName);
			}
			if(filepaths.Count==0)return;

			afh.Forms.ProgressDialog progress=new afh.Forms.ProgressDialog();
			progress.Title="画像を読込中 ...";
			progress.ProgressMax=filepaths.Count;
			progress.Show();
			// 読込
			foreach(string path in filepaths){
				try{
					progress.Description="画像 "+path+" を読み込んでいます...";
					this.AddFile(path);
					progress.Progress++;
					if(progress.IsCanceled)break;
				}catch(System.Exception e){
					afh.File.__dll__.log.WriteError(e,path+" の読込に失敗しました。");
				}
			}
			progress.Close();
		}
		private static readonly System.Collections.Generic.List<string> exts=new System.Collections.Generic.List<string>(new string[]{
			".jpg",".jpeg",".jpe",".jfif",".png",".gif",".bmp",".dib",".rle",//".tif",".tiff"
		});

		#region read write
		private ImageDirectory(string name){this.name=name;}
		private static ImageDirectory read(StreamAccessor accessor){
			ImageDirectory dir=new ImageDirectory(accessor.ReadString(EncodingType.NoSpecified));

			uint count=accessor.ReadUInt32(EncodingType.U4);
			for(int i=0;i<count;i++)
				dir.images.Add(accessor.ReadInt32(EncodingType.I4));

			dir.dirs=accessor.Read<DirectoryCollection>();
			return dir;
		}
		/// <summary>
		/// 読込の後に実際に機能する様に、仕上げの初期化を行います。
		/// </summary>
		/// <param name="file">根の ThumbsFile を設定します。</param>
		internal void read_initialize(ThumbsFile file){
			this.root=file;
			this.dirs.read_initialize(file);
		}
		private static void write(ImageDirectory dir,StreamAccessor accessor){
			accessor.Write(dir.name,EncodingType.NoSpecified);

			accessor.Write((uint)dir.images.Count,EncodingType.U4);
			foreach(int index in dir.images)
				accessor.Write(index,EncodingType.I4);

			accessor.Write(dir.dirs);
		}
		#endregion
	}

	/// <summary>
	/// 画像ディレクトリの集合を管理するクラスです。
	/// </summary>
	[CustomRead("read"),CustomWrite("write")]
	public class DirectoryCollection:Gen::List<ImageDirectory>{
		private ThumbsFile root;
		Gen::Dictionary<string,ImageDirectory> paths=new Gen::Dictionary<string,ImageDirectory>();
		/// <summary>
		/// DirectoryCollection のコンストラクタです。
		/// </summary>
		/// <param name="root">根の ThumbsFile を指定します。</param>
		public DirectoryCollection(ThumbsFile root):base(){
			this.root=root;
		}

		public ImageDirectory this[string name]{
			get{
				// 一つ下の階層の場合
				int index=name.IndexOf('\\');
				if(index>=0)
					return this[name.Substring(0,index)].dirs[name.Substring(index+1)];

				// 既にある場合
				if(name=="")name="--untitled--";
				if(this.paths.ContainsKey(name))
					return this.paths[name];
				
				// 未だ無い場合
				ImageDirectory r=new ImageDirectory(name,this.root);
				this.paths[name]=r;
				this.Add(r);
				return r;
			}
		}
		public bool Contains(string name){
			if(name=="")return false;

			// 一つ下の階層の場合
			int index=name.IndexOf('\\');
			if(index>=0){
				string name0=name.Substring(0,index);
				return this.paths.ContainsKey(name0)&&this.paths[name0].dirs.Contains(name.Substring(index));
			}
			return this.paths.ContainsKey(name);
		}

		#region read write
		private DirectoryCollection():base(){}
		private static DirectoryCollection read(StreamAccessor accessor){
			DirectoryCollection ret=new DirectoryCollection();

			uint size=accessor.ReadUInt32(EncodingType.U4);
			StreamAccessor acdirs=new StreamAccessor(accessor.ReadSubStream(size));
			try{
				while(acdirs.RestLength>0){
					ImageDirectory item=acdirs.Read<ImageDirectory>();
					ret.Add(item);
					ret.paths[item.name]=item;
				}
			}catch(afh.File.StreamOverRunException){
			}catch(System.Exception e){
				afh.File.__dll__.log.WriteError(e,".thm 内の画像ディレクトリを読込中にエラーが発生しました。");
			}

			return ret;
		}
		/// <summary>
		/// 読み込んだ後に大元の ThumbsFile を設定します。
		/// </summary>
		/// <param name="file">大元の ThumbsFile を設定します。</param>
		internal void read_initialize(ThumbsFile file){
			this.root=file;
			foreach(ImageDirectory dir in this)dir.read_initialize(file);
		}
		private static void write(DirectoryCollection dirs,StreamAccessor accessor){
			StreamAccessor acSize=new StreamAccessor(accessor.WriteSubStream(4));
			long pos0=accessor.Position;
			foreach(ImageDirectory childdir in dirs){
				accessor.Write(childdir);
			}
			acSize.Write(checked((uint)(accessor.Position-pos0)),EncodingType.U4);
			acSize.Stream.Close();
		}
		#endregion
	}

	// CHECK: struct の方が効率が良いかも?
	/// <summary>
	/// 個々の画像を表現するオブジェクトです。
	/// 画像に対する操作を提供するのみで、保存の対象には為りません。
	/// </summary>
	public struct Image{
		internal ThumbsFile root;
		internal Thumb thm;

		internal Image(ThumbsFile file,Thumb thm){
			this.root=file;
			this.thm=thm;
		}

		/// <summary>
		/// 画像のデータの実体が保存されている場所を特定するファイルパスを取得します。
		/// </summary>
		public string FilePath{
			get{return this.thm.filepath;}
		}
		//============================================================
		//		比較
		//============================================================
		public static bool EqualAspect(Image img1,Image img2){
			return ThumbData.EqualAspect(img1.thm.data,img2.thm.data);
		}
		public static bool Resembles(Image img1,Image img2){
			return img1.thm.data^img2.thm.data;
		}

		public static bool operator!=(Image img1,Image img2){
			return !(img1==img2);
		}
		public static bool operator==(Image img1,Image img2){
			return img1.root==img2.root&&img1.thm==img2.thm;
		}
		public override int GetHashCode() {
			return this.root.GetHashCode()^this.thm.GetHashCode();
		}
		public override bool Equals(object obj) {
			return obj is Image&&this==(Image)obj;
		}
	}
}