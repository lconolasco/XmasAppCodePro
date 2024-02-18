namespace XmasAppCodePro.Views;

public partial class AggiornaProdottoPage : ContentPage
{
	public AggiornaProdottoPage()
	{
		InitializeComponent();
       

    }
    protected override void OnAppearing()
    {
        if (cameraView.Cameras.Count > 0)
        {
            cameraView.Camera = cameraView.Cameras.First();
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                // await cameraView.StopCameraAsync();
                await cameraView.StartCameraAsync();
            });
        }
        base.OnAppearing();
    }
    protected override void OnDisappearing()
    {
        cameraView.Camera = cameraView.Cameras.First();
        MainThread.BeginInvokeOnMainThread(async () =>
        {
            await cameraView.StopCameraAsync();
            //await cameraView.StartCameraAsync();
        });
        base.OnDisappearing();
    }
    private void Cameraview_CamerasLoaded(object sender, EventArgs e)
    {
        if (cameraView.Cameras.Count > 0)
        {
            cameraView.Camera = cameraView.Cameras.First();
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                await cameraView.StopCameraAsync();
                await cameraView.StartCameraAsync();
            });
        }
    }

    private void Cameraview_BarcodeDetected(object sender, Camera.MAUI.ZXingHelper.BarcodeEventArgs args)
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            barcodeResult.Text = args.Result[0].Text;
            //barcodeFormat.Text = $"Tipo di codice scannerizato: {args.Result[0].BarcodeFormat}";

        });
    }
}