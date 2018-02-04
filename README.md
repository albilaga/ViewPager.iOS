# ViewPager.iOS
An easy to use view pager library for Xamarin.iOS (ported from https://github.com/SeptiyanAndika/ViewPager---Swift)

# Installation
Just add ViewPager.csproj to your project,  project are present inside src directory.

# Preview
![Preview ](https://raw.githubusercontent.com/albilaga/ViewPager.iOS/master/Screenshot/viewpager.gif)

# Setup
You need to implement methods from `ViewPagerDataSource`:

	public override int NumberOfItems(ViewPager.iOS.ViewPager viewPager)

Return the number of items (views) in the Viewpager.

	public override UIView ViewAtIndex(ViewPager.iOS.ViewPager viewPager, int index, UIView view = null)


Return a view to be displayed at the specified index in the ViewPager. The `view` argument, where views that have previously been displayed in the ViewPager are passed back to the method to be recycled. If this argument is not null, you can set its properties and return it instead of creating a new view instance, which will slightly improve performance.

    public override void ItemSelected(int index)

Callback when ViewPager clicked, return the number of items (views) in the Viewpager.


# Example
Example are included in example directory

ViewController:
    
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

ViewPagerDataSource:
    
	public class ImagesDataSource : ViewPagerDataSource
	{
		IReadOnlyList<string> _Datas;
		public ImagesDataSource(IReadOnlyList<string> datas)
		{
			this._Datas = datas;
		}

		public override void ItemSelected(int index)
		{
			//throw new NotImplementedException();
		}

		public override int NumberOfItems(ViewPager.iOS.ViewPager viewPager)
		{
			return _Datas.Count;
		}

		public override UIView ViewAtIndex(ViewPager.iOS.ViewPager viewPager, int index, UIView view = null)
		{
			return new UILabel
			{
				Text = _Datas[index],
				ContentMode=UIViewContentMode.Center,
				TextAlignment=UITextAlignment.Center
			};
		}
	}

  
## License

ViewPager is available under the MIT license. See the LICENSE file for more info.


