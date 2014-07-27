﻿using System;
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
    /// Class: ArticleListPage
    /// Author: Hans Huang @ Jungo Studio
    /// Create On: July 27th, 2014
    /// Description: The data template for article list  
    /// Version: 0.1
    /// Note: DataContext is Model.ArticlePackage
    /// </summary> 
    internal class ArticleListPage:ContentPage
    {
        public ArticleListPage()
        {
            this.SetBinding(TitleProperty, "Name");
            //NavigationPage.SetHasNavigationBar(this, true);
            
            var busyIndicator = GetBusyIndicator();
            var listViewer = GetListViewer();
            Content = new StackLayout {Children = {busyIndicator, listViewer}};
        }

        private StackLayout GetBusyIndicator()
        {
            var busy = new ActivityIndicator{IsRunning=true};
            var label = new Label {Text = "Loading ...", HorizontalOptions = LayoutOptions.CenterAndExpand};
            var stack= new StackLayout
            {
                VerticalOptions=LayoutOptions.CenterAndExpand,
                Children = {busy, label}
            };

            stack.SetBinding<ArticlePackage>(VisualElement.IsVisibleProperty, s => s.IsBuffering, BindingMode.OneWay);
            return stack;
        }

        private ListView GetListViewer()
        {
            var listView = new ListView
            {
                HorizontalOptions = LayoutOptions.FillAndExpand,
                ItemTemplate = new DataTemplate(typeof (ArticleCell))
            };
            listView.SetBinding<ArticlePackage>(ListView.IsVisibleProperty, s => s.IsBuffering,
                BindingMode.OneWay, new InverseBoolConverter());
            listView.SetBinding<ArticlePackage>(ListView.ItemsSourceProperty,s=>s.ArticleList);
            return listView;
        }

    }
}
