using Gen=System.Collections.Generic;

namespace afh.ImageArrange{
	public static class Utility{
		public static void SearchNearImage(ThumbsFile file) {
			ImageDirectory overlap=new ImageDirectory("<重複検索>",file);

			afh.Collections.SortedArrayP<Thumb> images=file.thumbs.SortedThumbs;
			Gen::Dictionary<Image,ImageDirectory> groups=new Gen::Dictionary<Image,ImageDirectory>();

			afh.Forms.ProgressDialog progress=new afh.Forms.ProgressDialog();
			progress.Title="重複比較中 ...";
			progress.ProgressMax=images.Count;

			progress.Show();
			for(int i=0;i<images.Count;i++) {
				Image img1=new Image(file,images[i]);

				if(i%50==0){
					progress.Description="画像 '"+img1.thm.filepath+"' の類似画像を探索中 ...";
					if(progress.IsCanceled)return;
				}

				for(int j=i+1;j<images.Count;j++) {
					Image img2=new Image(file,images[j]);
					if(!Image.EqualAspect(img1,img2)) break;

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
								ImageDirectory group=new ImageDirectory("Group "+overlap.dirs.Count.ToString(),file);
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

			file.dirs.Add(overlap);
		}
	}
}