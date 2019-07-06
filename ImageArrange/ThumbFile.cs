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
		/// ���̃t�@�C�������݂��Ă���f�B���N�g���̃p�X���擾���܂��B
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
		/// �f�B���N�g�����t�@�C����ǉ����܂��B
		/// </summary>
		/// <param name="path">�f�B���N�g���̃p�X���w�肵�܂��B</param>
		public void AddFileDirectory(string path){
			string imgdirName=afh.Application.Path.GetRelativePath(path,this.DirectoryName);
			if(imgdirName.StartsWith("."))imgdirName=System.IO.Path.GetFileName(path);
			this.dirs[imgdirName].AddFilesFromDirectory(path);
		}
		//=================================================
		//		��
		//=================================================
		/// <summary>
		/// �t�@�C����ۑ����܂��B
		/// </summary>
		public void Save(){
			StreamAccessor accessor=new StreamAccessor(System.IO.File.OpenWrite(this.filename));
			accessor.Write(this);
			accessor.Stream.Close();
		}
		/// <summary>
		/// ThumbsFile ���t�@�C������ǂݍ��݂܂��B
		/// �w�肵���t�@�C�������݂��Ȃ��ꍇ�͐V�����t�@�C�����쐬���܂��B
		/// </summary>
		/// <param name="filename">�t�@�C�������w�肵�܂��B</param>
		/// <returns>�쐬���� ThumbsFile �̃C���X�^���X��Ԃ��܂��B</returns>
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
					throw new System.ApplicationException("�t�@�C���t�H�[�}�b�g�� version ���قȂ�܂��B");
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
	/// �摜�t�@�C���� Thumbnail �Ɋւ������ێ����܂��B
	/// </summary>
	[afh.File.ReadSchedule("index","filepath","lastwrite","data")]
	[afh.File.WriteSchedule("index","filepath","lastwrite","data")]
	public class Thumb:System.IComparable<Thumb>,System.IEquatable<Thumb>{
		public Thumb(string filename){
			this.filepath=filename;
			this.Update();
		}

		/// <summary>
		/// �ǂݍ��ݗp�������q
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
		/// �T���l�C���̓��e���X�V���܂��B
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
		//		��r
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
	/// �摜�̓��e��\������\���̂ł��B
	/// TODO: Animation GIF ���ɂ͖����Ή����Ă��Ȃ��B(���߂̃t���[���Ŕ�r����̂�����ʂɗǂ��̂ł�?)
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
			// CHECK: data.Stride �� 8*4 ���łȂ��ƍ���
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
				throw new System.ApplicationException("�t�@�C�������݂��Ă��܂���B");

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
		/// �T���l�C�����߂����ǂ����̔�����s���܂��B
		/// </summary>
		/// <param name="l">��r�̑Ώۂ� ThumbData ���w�肵�܂��B�����\�ł��B</param>
		/// <param name="r">��r�̑Ώۂ� ThumbData ���w�肵�܂��B�����\�ł��B</param>
		/// <returns>�߂��Ɣ��f�����ꍇ�� true ��Ԃ��܂��B</returns>
		public static bool operator ^(ThumbData l,ThumbData r){
			// �A�X�y�N�g��Œ��˂�
			if(!EqualAspect(l,r))return false;

			// �s�N�Z���f�[�^�Ŕ���
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
		/// �w�肵���摜�t�@�C���̏����擾���܂��B
		/// �o�^����Ă��Ȃ������ꍇ�ɂ͐V�����o�^���܂��B
		/// </summary>
		/// <param name="filepath">�摜�t�@�C���̏ꏊ���w�肵�܂��B</param>
		/// <returns>�摜�t�@�C���̏���Ԃ��܂��B</returns>
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
		/// �w�肵���摜�t�@�C�����܂�ł��邩�ǂ������m�F���܂��B
		/// </summary>
		/// <param name="filepath">�܂�ł��邩�ǂ������m�F����摜�t�@�C���̃p�X���w�肵�܂��B</param>
		/// <returns>�w�肵���摜�t�@�C�������Ɋ܂�ł���ꍇ�ɂ� true ��Ԃ��܂��B
		/// �܂�ł��Ȃ��ꍇ�ɂ� false ��Ԃ��܂��B</returns>
		public bool Contains(string filepath){
			return this.paths.ContainsKey(filepath);
		}
		/// <summary>
		/// �w�肵���摜�t�@�C���̏����X�V���܂��B�摜�t�@�C�����o�^����Ă��Ȃ��ꍇ�ɂ͐V�����o�^���܂��B
		/// </summary>
		/// <param name="filepath">�X�V����摜�t�@�C���̏ꏊ���w�肵�܂��B</param>
		public void Update(string filepath){
			this[filepath].Update();
		}
#if OLD
		[System.Obsolete]
		public void dbg_NearImage(){
			ImageDirectory overlap=new ImageDirectory("<�d������>",root);
			Gen::Dictionary<Image,ImageDirectory> groups=new Gen::Dictionary<Image,ImageDirectory>();

			afh.ImageArrange.ProgressDialog progress=new ProgressDialog();
			progress.Title="�d����r�� ...";
			progress.ProgressMax=this.sorted.Count;

			progress.Show();
			for(int i=0;i<this.sorted.Count;i++) {
				Image img1=new Image(this.root,this.sorted[i]);

				if(i%50==0){
					progress.Description="�摜 '"+img1.thm.filepath+"' �̗ގ��摜��T���� ...";
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
							case 3: // ����
								if(group1==group2)break;
								// ���O���[�v�̕���
								overlap.dirs.Remove(group2);
								foreach(Image img in group2.EnumImages()){
									group1.Add(img);
									groups[img]=group1;
								}
								break;
							case 1: // group1 ����
								group1.Add(img2);
								groups.Add(img2,group1);
								break;
							case 2: // group2 ����
								group2.Add(img1);
								groups.Add(img1,group2);
								break;
							case 0: // ���Җ��o�^
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
						//System.Console.WriteLine("���̉摜�͎��Ă��܂�\r\n    {0}\r\n    {1}",thm1.filepath,thm2.filepath);
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
				afh.File.__dll__.log.WriteError(e,"ImageCollection �ǂݎ�蒆�̃G���[");
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
	/// �摜�̃f�B���N�g����\������N���X�ł��B
	/// </summary>
	[CustomRead("read"),CustomWrite("write")]
	public class ImageDirectory{
		private ThumbsFile root;
		public string name;
		public readonly Gen::List<int> images=new Gen::List<int>();
		internal DirectoryCollection dirs;

		/// <summary>
		/// ImageDirectory �̏������q�ł��B
		/// </summary>
		/// <param name="name">���� Directory �ɕt���閼�O���w�肵�܂��B</param>
		/// <param name="root">���� ImageDirectory ���������� ThumbsFile ���w�肵�܂��B</param>
		public ImageDirectory(string name,ThumbsFile root){
			this.root=root;
			this.name=name;
			this.dirs=new DirectoryCollection(root);
		}


		//============================================================
		//		��{����
		//============================================================
		/// <summary>
		/// �w�肵���摜�����̃f�B���N�g���Ɋ܂܂�Ă��邩�ۂ����擾���܂��B
		/// </summary>
		/// <param name="image">���̃f�B���N�g���Ɋ܂܂�Ă��邩�ǂ����𒲂ׂ����摜���w�肵�܂��B</param>
		/// <returns>���Ɋ܂܂�Ă����ꍇ�� true ��Ԃ��܂��B�܂܂�Ă��Ȃ������ꍇ�� false ��Ԃ��܂��B</returns>
		public bool Contains(Image image){
			return image.root==this.root&&this.images.Contains(image.thm.index);
		}
		/// <summary>
		/// �w�肵���摜�����̃f�B���N�g���ɒǉ����܂��B
		/// </summary>
		/// <param name="image">�ǉ�����摜���w�肵�܂��B</param>
		public void Add(Image image){
			if(image.root!=this.root){
				// TODO:
				// 1. thumb �I�u�W�F�N�g�̐���
				// 2. root �� ImageCollection �ɒǉ�
				// 3. �f�B���N�g���ɔԍ���o�^
				throw new System.NotImplementedException("�قȂ�t�@�C���ɑ����Ă���摜�f�[�^��ǉ�����̂ɂ͖��Ή��ł��B");
			}else{
				this.images.Add(image.thm.index);
			}
		}
		/// <summary>
		/// �w�肵���f�B���N�g�������̃f�B���N�g���̃T�u�f�B���N�g���ɒǉ����܂��B
		/// </summary>
		/// <param name="directory">�T�u�f�B���N�g���Ƃ��Ēǉ�����f�B���N�g�����w�肵�܂��B</param>
		public void Add(ImageDirectory directory){
			if(directory.root!=this.root){
				throw new System.NotImplementedException("�قȂ�t�@�C���̃f�B���N�g����ǉ����鑀��͖������ł��B");
			}else{
				this.dirs.Add(directory);
			}
		}
		/// <summary>
		/// ���̃f�B���N�g���Ɋ܂܂�Ă���摜��񋓂��܂��B
		/// </summary>
		/// <returns>�摜�̗񋓎q���擾�o���� IEnumerable ��Ԃ��܂��B</returns>
		public Gen::IEnumerable<Image> EnumImages(){
			foreach(int index in this.images)
				yield return new Image(this.root,this.root.thumbs[index]);
		}
		//============================================================
		//		�t�@�C���̒ǉ�
		//============================================================
		/// <summary>
		/// �w�肵�� file ������ ImageDirectory �ɓo�^���܂��B
		/// </summary>
		/// <param name="filepath">�o�^���� file �ւ� path ���w�肵�܂��B</param>
		public void AddFile(string filepath){
			int index=this.root.thumbs[filepath].index;
			if(!this.images.Contains(index))this.images.Add(index);
		}
		/// <summary>
		/// �w�肵�� file-directory ���̉摜������ ImageDirectory �ɓo�^���܂��B
		/// </summary>
		/// <param name="dirpath">�o�^����摜���܂�ł���f�B���N�g���ւ� path ���w�肵�܂��B</param>
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
			progress.Title="�摜��Ǎ��� ...";
			progress.ProgressMax=filepaths.Count;
			progress.Show();
			// �Ǎ�
			foreach(string path in filepaths){
				try{
					progress.Description="�摜 "+path+" ��ǂݍ���ł��܂�...";
					this.AddFile(path);
					progress.Progress++;
					if(progress.IsCanceled)break;
				}catch(System.Exception e){
					afh.File.__dll__.log.WriteError(e,path+" �̓Ǎ��Ɏ��s���܂����B");
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
		/// �Ǎ��̌�Ɏ��ۂɋ@�\����l�ɁA�d�グ�̏��������s���܂��B
		/// </summary>
		/// <param name="file">���� ThumbsFile ��ݒ肵�܂��B</param>
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
	/// �摜�f�B���N�g���̏W�����Ǘ�����N���X�ł��B
	/// </summary>
	[CustomRead("read"),CustomWrite("write")]
	public class DirectoryCollection:Gen::List<ImageDirectory>{
		private ThumbsFile root;
		Gen::Dictionary<string,ImageDirectory> paths=new Gen::Dictionary<string,ImageDirectory>();
		/// <summary>
		/// DirectoryCollection �̃R���X�g���N�^�ł��B
		/// </summary>
		/// <param name="root">���� ThumbsFile ���w�肵�܂��B</param>
		public DirectoryCollection(ThumbsFile root):base(){
			this.root=root;
		}

		public ImageDirectory this[string name]{
			get{
				// ����̊K�w�̏ꍇ
				int index=name.IndexOf('\\');
				if(index>=0)
					return this[name.Substring(0,index)].dirs[name.Substring(index+1)];

				// ���ɂ���ꍇ
				if(name=="")name="--untitled--";
				if(this.paths.ContainsKey(name))
					return this.paths[name];
				
				// ���������ꍇ
				ImageDirectory r=new ImageDirectory(name,this.root);
				this.paths[name]=r;
				this.Add(r);
				return r;
			}
		}
		public bool Contains(string name){
			if(name=="")return false;

			// ����̊K�w�̏ꍇ
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
				afh.File.__dll__.log.WriteError(e,".thm ���̉摜�f�B���N�g����Ǎ����ɃG���[���������܂����B");
			}

			return ret;
		}
		/// <summary>
		/// �ǂݍ��񂾌�ɑ匳�� ThumbsFile ��ݒ肵�܂��B
		/// </summary>
		/// <param name="file">�匳�� ThumbsFile ��ݒ肵�܂��B</param>
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

	// CHECK: struct �̕����������ǂ�����?
	/// <summary>
	/// �X�̉摜��\������I�u�W�F�N�g�ł��B
	/// �摜�ɑ΂��鑀���񋟂���݂̂ŁA�ۑ��̑Ώۂɂׂ͈�܂���B
	/// </summary>
	public struct Image{
		internal ThumbsFile root;
		internal Thumb thm;

		internal Image(ThumbsFile file,Thumb thm){
			this.root=file;
			this.thm=thm;
		}

		/// <summary>
		/// �摜�̃f�[�^�̎��̂��ۑ�����Ă���ꏊ����肷��t�@�C���p�X���擾���܂��B
		/// </summary>
		public string FilePath{
			get{return this.thm.filepath;}
		}
		//============================================================
		//		��r
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