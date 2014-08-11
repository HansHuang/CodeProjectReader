using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CodeProjectReader.Helper;
using CodeProjectReader.Model;
using Xamarin.Forms;

namespace CodeProjectReader.Viewer
{
    /// <summary>
    /// Class: ArticleCell
    /// Author: Hans Huang @ Jungo Studio
    /// Create On: July 26th, 2014
    /// Description: The view cell to display article list item
    /// Version: 0.1
    /// Note: Why not inhert from TextCess? NOT support warp.
    /// </summary> 
    internal class ArticleCell : ViewCell
    {
        public ArticleCell()
        {
            var date = new Label
            {
                XAlign = TextAlignment.End,
                TextColor = Color.FromRgba(49, 25, 10, 160),
                Font = Font.SystemFontOfSize(NamedSize.Small)
            };
            date.SetBinding<Article>(Label.TextProperty, s => s.DateString);

            var isCached = new Label
            {
                XAlign = TextAlignment.End,
                TextColor = Color.FromRgba(49, 25, 10, 160),
                Font = Font.SystemFontOfSize(NamedSize.Small),
                Text = "◆"
            };
            isCached.SetBinding<Article>(VisualElement.IsVisibleProperty, s => s.IsCached);
            var infoBox = new StackLayout
            {
                Orientation = StackOrientation.Horizontal,
                Children = {isCached, date},
                HorizontalOptions = LayoutOptions.End
            };

            var title = new Label
            {
                LineBreakMode = LineBreakMode.WordWrap,
                Font = Font.BoldSystemFontOfSize(20),
                TextColor = Color.FromRgba(49, 25, 10,210)
            };
            title.SetBinding<Article>(Label.TextProperty, s => s.FullTitle);

            var desc = new Label
            {
                LineBreakMode = LineBreakMode.WordWrap,
                TextColor = Color.FromRgba(54, 30, 16,180)
            };
            desc.SetBinding<Article>(Label.TextProperty, s => s.Description);

            var layout = new StackLayout
            {
                Padding = new Thickness(10, 15, 5, 15),
                Children = { title, desc, infoBox },
                Spacing = 0
            };
            View = layout;

            Tapped += CellTapped;
        }

        private void CellTapped(object sender, EventArgs e)
        {
            var page = this.FindAnchestor<Page>();
            if (page == null) return;

            var detail = new ArticleDetail((Article) BindingContext, (ArticleViewModel) page.BindingContext);
            page.Navigation.PushAsync(detail);
        }
        

    }
}
