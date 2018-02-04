using System;
using System.Collections.Generic;
using CoreFoundation;
using CoreGraphics;
using Foundation;
using UIKit;

namespace ViewPager.iOS
{
	public abstract class ViewPagerDataSource
	{
		public abstract int NumberOfItems(ViewPager viewPager);
		public abstract UIView ViewAtIndex(ViewPager viewPager, int index, UIView view = null);
		public abstract void ItemSelected(int index);
	}

	public class ViewPager : UIView, IUIScrollViewDelegate
	{
		UIPageControl _PageControl = new UIPageControl();
		UIScrollView _ScrollView = new UIScrollView();
		int _CurrentPosition, _NumberOfItems;
		Dictionary<int, UIView> _ItemViews = new Dictionary<int, UIView>();

		ViewPagerDataSource _DataSource;
		public ViewPagerDataSource DataSource
		{
			get => _DataSource;
			set
			{
				_DataSource = value;
				this.ReloadData();
			}
		}

		public ViewPager(IntPtr handle) : base(handle)
		{
			this.SetupView();
		}

		public ViewPager(NSCoder coder) : base(coder)
		{
			this.SetupView();
		}

		public ViewPager(CGRect frame) : base(frame)
		{
			this.SetupView();
		}

		void SetupView()
		{
			this.Add(this._ScrollView);
			this.Add(this._PageControl);
			this.SetupScrollView();
			this.SetupPageControl();
			this.ReloadData();
		}

		void SetupScrollView()
		{
			this._ScrollView.PagingEnabled = true;
			this._ScrollView.AlwaysBounceHorizontal = false;
			this._ScrollView.Bounces = false;
			this._ScrollView.ShowsVerticalScrollIndicator = false;
			this._ScrollView.ShowsHorizontalScrollIndicator = false;
			this._ScrollView.Delegate = this;
			var topConstraints = NSLayoutConstraint.Create(this._ScrollView, NSLayoutAttribute.Top, NSLayoutRelation.Equal, this, NSLayoutAttribute.Top, 1.0f, 0);
			var bottomConstraints = NSLayoutConstraint.Create(this._ScrollView, NSLayoutAttribute.Bottom, NSLayoutRelation.Equal, this, NSLayoutAttribute.Bottom, 1.0f, 0);
			var leftConstraints = NSLayoutConstraint.Create(this._ScrollView, NSLayoutAttribute.LeadingMargin, NSLayoutRelation.Equal, this, NSLayoutAttribute.LeadingMargin, 1.0f, 0);
			var rightConstraints = NSLayoutConstraint.Create(this._ScrollView, NSLayoutAttribute.TrailingMargin, NSLayoutRelation.Equal, this, NSLayoutAttribute.TrailingMargin, 1.0f, 0);
			this._ScrollView.TranslatesAutoresizingMaskIntoConstraints = false;
			NSLayoutConstraint.ActivateConstraints(new NSLayoutConstraint[] { topConstraints, bottomConstraints, leftConstraints, rightConstraints });
		}

		void SetupPageControl()
		{
			this._PageControl.Pages = this._NumberOfItems;
			this._PageControl.CurrentPage = 0;
			this._PageControl.PageIndicatorTintColor = UIColor.LightGray;
			this._PageControl.CurrentPageIndicatorTintColor = UIColor.Green;

			var heightConstraints = NSLayoutConstraint.Create(this._PageControl, NSLayoutAttribute.Height, NSLayoutRelation.Equal, null, NSLayoutAttribute.NoAttribute, 1.0f, 25);
			var bottomConstraints = NSLayoutConstraint.Create(this._PageControl, NSLayoutAttribute.Bottom, NSLayoutRelation.Equal, this, NSLayoutAttribute.Bottom, 1.0f, 4);
			var leftConstraints = NSLayoutConstraint.Create(this._PageControl, NSLayoutAttribute.LeadingMargin, NSLayoutRelation.Equal, this, NSLayoutAttribute.LeadingMargin, 1.0f, 0);
			var rightConstraints = NSLayoutConstraint.Create(this._PageControl, NSLayoutAttribute.TrailingMargin, NSLayoutRelation.Equal, this, NSLayoutAttribute.TrailingMargin, 1.0f, 0);
			this._PageControl.TranslatesAutoresizingMaskIntoConstraints = false;
			NSLayoutConstraint.ActivateConstraints(new NSLayoutConstraint[] { heightConstraints, bottomConstraints, leftConstraints, rightConstraints });
		}

		void ReloadData()
		{
			if (this.DataSource != null)
			{
				this._NumberOfItems = this.DataSource.NumberOfItems(this);
			}
			this._PageControl.Pages = this._NumberOfItems;

			this._ItemViews.Clear();
			foreach (var view in this._ScrollView.Subviews)
			{
				view.RemoveFromSuperview();
			}

			DispatchQueue.MainQueue.DispatchAsync(() =>
			{
				this._ScrollView.ContentSize = new CGSize(this._ScrollView.Frame.Width * this._NumberOfItems, this._ScrollView.Frame.Height);
				this.ReloadViews(0);
			});
		}

		void LoadViewAtIndex(int index)
		{
			UIView view = null;
			if (this.DataSource != null)
			{
				if (this._ItemViews.TryGetValue(index, out UIView itemViewNew))
				{
					view = this.DataSource.ViewAtIndex(this, index, itemViewNew);
				}
				else
				{
					view = this.DataSource.ViewAtIndex(this, index, null);
				}
			}
			if (view == null)
			{
				view = new UIView();
			}

			this.SetFrameForView(view, index);

			if (this._ItemViews.TryGetValue(index, out UIView itemView))
			{

				if (itemView == null)
				{
					var tap = new UITapGestureRecognizer((obj) =>
					{
						this.HandleTapSubview();
					})
					{
						NumberOfTapsRequired = 1
					};
					view.AddGestureRecognizer(tap);
					this._ScrollView.Add(view);
				}
				_ItemViews[index] = view;

			}
			else
			{
				var tap = new UITapGestureRecognizer((obj) =>
				{
					this.HandleTapSubview();
				})
				{
					NumberOfTapsRequired = 1
				};
				view.AddGestureRecognizer(tap);
				this._ScrollView.Add(view);
				this._ItemViews.Add(index, view);
			}
		}

		void HandleTapSubview()
		{
			this.DataSource.ItemSelected(this._CurrentPosition);
		}

		void ReloadViews(int index)
		{
			for (int i = index - 1; i <= index + 1; i++)
			{
				if (i >= 0 && i < this._NumberOfItems)
				{
					this.LoadViewAtIndex(i);
				}
			}
		}

		void SetFrameForView(UIView view, int index)
		{
			view.Frame = new CGRect(this._ScrollView.Frame.Width * index, 0, this._ScrollView.Frame.Width, this._ScrollView.Frame.Height);
		}

		[Export("scrollViewDidEndScrollingAnimation:")]
		public void ScrollAnimationEnded(UIScrollView scrollView)
		{
			NSObject.CancelPreviousPerformRequest(this);
			var pageNumber = (int)Math.Round(this._ScrollView.ContentOffset.X / this._ScrollView.Frame.Size.Width);
			pageNumber++;
			this._PageControl.CurrentPage = pageNumber;
			this._CurrentPosition = pageNumber;
			this.ScrollToPage(this._CurrentPosition);
		}

		[Export("scrollViewDidScroll:")]
		public void Scrolled(UIScrollView scrollView)
		{
			NSObject.CancelPreviousPerformRequest(this._ScrollView);
			this.PerformSelector(new ObjCRuntime.Selector("scrollViewDidEndScrollingAnimation:"), this._ScrollView, 0.3);
		}

		public async void AnimationNext()
		{
			await System.Threading.Tasks.Task.Delay(TimeSpan.FromSeconds(2));
			MoveToNextPage();
		}

		//[Export("moveToNextPage:")]
		public void MoveToNextPage()
		{
			if (this._CurrentPosition <= this._NumberOfItems && this._CurrentPosition > 0)
			{
				this.ScrollToPage(this._CurrentPosition);
				this._CurrentPosition++;
				if (this._CurrentPosition > this._NumberOfItems)
				{
					this._CurrentPosition = 1;
				}
			}
		}

		public void ScrollToPage(int index)
		{
			if (index <= this._NumberOfItems && index > 0)
			{
				var zIndex = index - 1;
				var iFrame = new CGRect(this._ScrollView.Frame.Width * zIndex, 0, this._ScrollView.Frame.Width, this._ScrollView.Frame.Height);
				this._ScrollView.SetContentOffset(iFrame.Location, true);
				this._PageControl.CurrentPage = zIndex;
				this.ReloadViews(zIndex);
				this._CurrentPosition = index;
			}
		}
	}
}
