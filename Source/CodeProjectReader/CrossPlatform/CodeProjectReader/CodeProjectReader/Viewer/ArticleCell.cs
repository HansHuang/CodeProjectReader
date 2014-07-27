using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CodeProjectReader.Model;
using Xamarin.Forms;

namespace CodeProjectReader.Viewer
{
    internal class ArticleCell : ViewCell
    {
        public ArticleCell()
        {
            var title = new Label {LineBreakMode = LineBreakMode.WordWrap, Font = Font.BoldSystemFontOfSize(20)};
            title.SetBinding<Article>(Label.TextProperty, s => s.FullTitle);

            var desc = new Label { LineBreakMode = LineBreakMode.WordWrap,TextColor=Color.Silver };
            desc.SetBinding<Article>(Label.TextProperty, s => s.Description);

            //var img=

            var layout = new StackLayout
            {
                Padding = 10,
                Children = { title, desc }
            };
            View = layout;
        }
    }
}
