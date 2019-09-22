using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Mapi.Views;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace Mapi
{
    public partial class App : Application
    {
        #region Interactors
        public static IGeometryInteractor geometryInteractor = new GeometryInteractor();
        public static ILineInteractor lineInteractor = new LineInteractor();
        public static IProcessingInteractor processingInteractor = new ProcessingInteractor();
        public static IDataInteractor dataInteractor = new DataInteractor();
        public static IAuthenticateInteractor authenticateInteractor = new AuthenticateInteractor();
        public static IUserDataInteractor manager;
        #endregion

        #region Router 
        public static IRouter router = new Router();
        #endregion 

    
        public static string userId = "";
        public static bool finishLoading = false;
        public static UserData mainUser;

        // Static variable for making authentification 
        public static IAuthenticate Authenticator;

        public App()
        {
            InitializeComponent();
            // Initialize all polygons from Coordinates.cs 
            geometryInteractor.InitializePolygonsArray();
            MainPage = new SignUpPage();
        }

        public static void Init(IAuthenticate authenticator)
        {
            Authenticator = authenticator;
        }
    }
}
