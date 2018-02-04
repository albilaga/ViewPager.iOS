using System.Collections.Generic;
using UIKit;
using ViewPager.iOS;

namespace ViewPageriOSSample
{
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
}
