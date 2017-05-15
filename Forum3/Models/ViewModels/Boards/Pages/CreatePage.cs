﻿using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace Forum3.Models.ViewModels.Boards.Pages {
	public class CreatePage {
		[Required]	
		[StringLength(64)]
		public string Name { get; set; }
		
		[StringLength(64)]
		public string Category { get; set; }
		
		[StringLength(64)]
		public string NewCategory { get; set; }
		
		public bool VettedOnly { get; set; }
		
		public List<SelectListItem> Categories { get; set; }
	}
}