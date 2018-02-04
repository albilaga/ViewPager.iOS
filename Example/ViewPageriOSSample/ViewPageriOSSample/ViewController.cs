using System;
using System.Collections.Generic;
using UIKit;

namespace ViewPageriOSSample
{
	public partial class ViewController : UIViewController
	{
		protected ViewController(IntPtr handle) : base(handle)
		{
			// Note: this .ctor should not contain any initialization logic.
		}

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();
			// Perform any additional setup after loading the view, typically from a nib.
			var viewPager = new ViewPager.iOS.ViewPager(this.View.Frame);
			this.View.Add(viewPager);
			var dataSource = new ImagesDataSource(new List<string>
			{
				"View Pager 1","View Pager 2","View Pager 3"
			});
			viewPager.DataSource = dataSource;
		}

		public override void DidReceiveMemoryWarning()
		{
			base.DidReceiveMemoryWarning();
			// Release any cached data, images, etc that aren't in use.
		}
	}
}
