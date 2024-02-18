namespace XmasAppCodePro.Views;

public partial class AggiungeProdottoPage : ContentPage
{
	public AggiungeProdottoPage()
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
    private void cameraview_CamerasLoaded(object sender, EventArgs e)
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

    private void cameraview_BarcodeDetected(object sender, Camera.MAUI.ZXingHelper.BarcodeEventArgs args)
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            barcodeResult.Text = $"{args.Result[0].Text}";

        });
    }

    void OnPickerSelectedIndexChanged(object sender, EventArgs e)
    {
        var picker = (Picker)sender;
        int selectedIndex = picker.SelectedIndex;

        if (selectedIndex != -1)
        {
            lblCategoria.Text = $"{picker.Items[selectedIndex]}\t\t";
            entryCategoria.Text = (selectedIndex+1).ToString();

        }
    }
}