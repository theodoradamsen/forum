﻿namespace Forum3.Models.ViewModels.Boards {
	public class EditPage {
		public int Id { get; set; }
		public string Name { get; set; }
		public string Parent { get; set; }
		public bool VettedOnly { get; set; }
	}
}