﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarChess.Objects.Items {
	public abstract class Item {
		public string Name;
		public override string ToString() {
			return Name;
		}
	}
}
