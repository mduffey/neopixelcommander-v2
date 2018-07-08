///*
//  In App.xaml:
//  <Application.Resources>
//      <vm:ViewModelLocator xmlns:vm="clr-namespace:NeoPixelCommander"
//                           x:Key="Locator" />
//  </Application.Resources>

//  In the View:
//  DataContext="{Binding Source={StaticResource Locator}, Path=ViewModelName}"

//  You can also use Blend to do all this with the tool's support.
//  See http://www.galasoft.ch/mvvm
//*/

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using CommonServiceLocator;
using System.Collections.Generic;
using NeoPixelCommander.ViewModel.LightManagers;
using NeoPixelCommander.Library;
using NeoPixelCommander.Library.ColorManagers;

namespace NeoPixelCommander.ViewModel
{
    /// <summary>
    /// This class contains static references to all the view models in the
    /// application and provides an entry point for the bindings.
    /// </summary>
    public class ViewModelLocator
    {
        /// <summary>
        /// Initializes a new instance of the ViewModelLocator class.
        /// </summary>
        public ViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

            ////if (ViewModelBase.IsInDesignModeStatic)
            ////{
            ////    // Create design time view services and models
            ////    SimpleIoc.Default.Register<IDataService, DesignDataService>();
            ////}
            ////else
            ////{
            ////    // Create run time view services and models
            ////    SimpleIoc.Default.Register<IDataService, DataService>();
            ////}
            SimpleIoc.Default.Register<Communicator>();
            SimpleIoc.Default.Register<PackageHandler>();
            SimpleIoc.Default.Register<GradientManager>();
            SimpleIoc.Default.Register<MoodlightManager>();
            SimpleIoc.Default.Register<ManualManager>();
            SimpleIoc.Default.Register<MainViewModel>();

            WireUpManagerViewModels();
        }

        public MainViewModel Main
        {
            get
            {
                return ServiceLocator.Current.GetInstance<MainViewModel>();
            }
        }

        public static void Cleanup()
        {
            // TODO Clear the ViewModels
        }

        private void WireUpManagerViewModels()
        {
            SimpleIoc.Default.Register<IEnumerable<ILightManager>>(() => 
                new List<ILightManager>
                {
                    new OffViewModel(SimpleIoc.Default.GetInstance<PackageHandler>()),
                    new MoodlightViewModel(SimpleIoc.Default.GetInstance<MoodlightManager>()),
                    new SingleColorViewModel(SimpleIoc.Default.GetInstance<PackageHandler>()),
                    new GradientViewModel(SimpleIoc.Default.GetInstance<GradientManager>()),
                    new ManualViewModel(SimpleIoc.Default.GetInstance<ManualManager>())
                });
        }
    }
}