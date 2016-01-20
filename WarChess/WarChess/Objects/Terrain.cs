﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace WarChess.Objects {
	public class Terrain{		
		public string Name;
		public BitmapImage Image;
		public bool IsStandable;
		public bool SeeThrough;
		public bool IsJumpable;
		public int Speed;
		public Terrain(string Name,string PicturePath,bool IsStandable,bool SeeThrough,bool Jumpable,int Speed) {
			this.Name = Name;
			//BitmapImage image = new BitmapImage();
			//image.BeginInit();
			//image.UriSource = new Uri("Terrain Pics\\" + PicturePath, UriKind.Relative);
			//image.DecodePixelWidth = 75;
			//image.DecodePixelHeight = 75;
			//image.EndInit();
			//this.Image = image;
			this.Image = new BitmapImage(new Uri("Terrain Pics\\" + PicturePath, UriKind.Relative));
			this.IsStandable = IsStandable;
			this.SeeThrough = SeeThrough;
			this.IsJumpable = Jumpable;
			this.Speed = Speed;
		}
	}
}
