using Microsoft.AspNetCore.Http;

namespace CC98.Share.ViewModels.FileController
{
	public class UploadViewModel
	{
		public IFormFile[] Files { get; set; }
		public bool Value { get; set; }
	}
}