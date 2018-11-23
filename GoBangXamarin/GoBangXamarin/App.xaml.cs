using GoBangCL.Standard;
using GoBangCL.Standard.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace GoBangXamarin
{
    public partial class App : Application
    {

        public App()
        {
            InitializeComponent();

            MainPage = new MainPage();
            MainPage.Title = "GoBang";
            //Current.Properties["CurrentStep"] = 0;
        }

        protected override void OnStart()
        {
            // Handle when your app starts
            Debug.WriteLine("OnStart");

            //if (current.properties.containskey("boardlist"))
            //{
            //    string str = utils.currentboardlogstr(properties["boardlist"] as list<board>, convert.toint32(properties["currentstep"]));
            //    debug.writeline(str);
            //}
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
            Debug.WriteLine("OnSleep");
            //if (Current.Properties.ContainsKey("BoardList"))
            //{
            //    string str = Utils.CurrentBoardLogStr(Properties["BoardList"] as List<Board>, Convert.ToInt32(Properties["CurrentStep"]));
            //    Debug.WriteLine(str);
            //}

        }

        protected override void OnResume()
        {
            // Handle when your app resumes
            Debug.WriteLine("OnResume");

            //if (Current.Properties.ContainsKey("BoardList"))
            //{
            //    string str = Utils.CurrentBoardLogStr(Properties["BoardList"] as List<Board>, Convert.ToInt32(Properties["CurrentStep"]));
            //    Debug.WriteLine(str);
            //}
        }
    }
}
