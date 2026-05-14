using SlimeMarkUp.Core;
using SlimeWrite.Core.Models;

namespace SlimeWrite;

public partial class EditorPreview : ContentPage
{
    double startHeight, startWidth;
    private readonly MarkupParser _parser;
    private readonly HtmlRenderer _renderer;
    DocumentInfo documentInfo;
    public DocumentInfo DocumentInfo { get { return documentInfo; }
        set { documentInfo = value; }
    }
    public EditorPreview()
	{
		InitializeComponent();
        _parser = MainPage.core.InitializeParser();

        _renderer = new HtmlRenderer();

    }
    private void ContentPage_Appearing(object sender, EventArgs e)
    {
        base.OnAppearing();

        try
        {

            initilizeOriantation();
            string[] args = Environment.GetCommandLineArgs();
            Loadeditor();
            //if (args.Length > 1)
            //{
            //    OpenFile(args[1]);
            //}

            /*  if (editor != null)
                  editor.Text = _markdown;*/
        }
        catch (Exception ex)
        {
            MainPage.core.ErrorLog(ex);


        }

    }
    public RowDefinition TopRow { get { return this.topRow; } }
    public RowDefinition BottomRow { get { return this.bottomRow; } }
    public void SetGridContentSizes()
    {
        try
        {
            double editorandpreviewheight = this.Height / 2;
            double editorandpreviewwidth = this.Width / 2;
            this.MainGrid.HeightRequest = this.Height;
            this.MainGrid.WidthRequest = this.Width;

            if (MainPage.options.WebViewOrientation == 1)
            {
                if (editor.Height <= editorandpreviewheight)
                {
                    this.preview.HeightRequest = this.Height / 2;

                    this.editor.HeightRequest = this.Height / 2;

                }
                else
                {
                    this.preview.HeightRequest = this.TopRow.Height.Value;

                    this.editor.HeightRequest = this.BottomRow.Height.Value; ;

                }
                this.preview.WidthRequest = this.Width;
                this.editor.WidthRequest = this.Width; ; ;
            }
            else
            {
                if (editor.Width <= editorandpreviewwidth)
                {
                    this.preview.WidthRequest = this.MainGrid.Width / 2;

                    this.editor.WidthRequest = this.MainGrid.Width / 2;
                }
                else
                {
                    this.preview.WidthRequest = this.MainGrid.Width / 2;

                    this.editor.WidthRequest = this.MainGrid.Width / 2;
                }
                this.preview.HeightRequest = this.Height / 2;
                this.editor.HeightRequest = this.Height / 2;

            }
        }
        catch (Exception ex)
        {
            MainPage.core.ErrorLog(ex);


        }
    }
   public  async void initilizeOriantation()
    {
        try
        {
            //this.MainGrid.HeightRequest = this.Height;

            switch (MainPage.options.WebViewOrientation)
            {

                case 0:
                    {
                        this.MainGrid.RowDefinitions.Clear();
                        this.MainGrid.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Star));


                        this.MainGrid.ColumnDefinitions.
                            Add(new ColumnDefinition());
                        this.MainGrid.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Star));
                        this.MainGrid.ColumnDefinitions[1].Width = new GridLength(10);
                        this.MainGrid.Children.Clear();

                        MainGrid.SetColumn(scrollview, 0);
                        MainGrid.SetColumn(editor, 0);
                        MainGrid.SetColumn(preview, 2);
                        MainGrid.SetColumn(this.spliter, 1);


                        //this.ContentPage_SizeChanged(null, null);
                        // this.SetGridContentSizes();
                        break;


                    }
                case 1:
                    {

                        break;
                    }

            }
        }
        catch (Exception ex)
        {
            MainPage.core.ErrorLog(ex);


        }

    }
    void OnPanUpdated(object sender, PanUpdatedEventArgs e)
    {
        try
        {
            if (MainPage.options.WebViewOrientation == 1)
            {

                switch (e.StatusType)
                {
                    case GestureStatus.Started:

                        startHeight = TopRow.Height.Value;
                        break;

                    case GestureStatus.Running:
                        double newHeight = startHeight + e.TotalY;

                        if (newHeight < 50) newHeight = 50;
                        if (newHeight > MainGrid.Height - 50) newHeight = MainGrid.Height - 50;

                       TopRow.Height = new GridLength(newHeight, GridUnitType.Absolute);
                         BottomRow.Height =
                            new GridLength(MainGrid.Height - newHeight - 5, GridUnitType.Absolute);
                        break;
                }
                this.editor.HeightRequest =  TopRow.Height.Value;
                this.preview.HeightRequest =  BottomRow.Height.Value;

            }
            else
            {

                switch (e.StatusType)
                {
                    case GestureStatus.Started:
                        startWidth = MainGrid.ColumnDefinitions[0].Width.Value;
                        break;
                    case GestureStatus.Running:
                        double newWidth = startWidth + e.TotalX;
                        if (newWidth < 50) newWidth = 50;
                        if (newWidth > MainGrid.Width - 50) newWidth = MainGrid.Width - 50;
                        MainGrid.ColumnDefinitions[0].Width =
                            new GridLength(newWidth, GridUnitType.Absolute);
                        MainGrid.ColumnDefinitions[2].Width =
                            new GridLength(MainGrid.Width - newWidth - 5, GridUnitType.Absolute);
                        break;
                }
                this.editor.WidthRequest = this.MainGrid.ColumnDefinitions[2].Width.Value;
                this.preview.WidthRequest = this.MainGrid.ColumnDefinitions[0].Width.Value;
            }
            startHeight = 0;
            startWidth = 0;
        }
        catch (Exception ex)
        {
            MainPage.core.ErrorLog(ex);


        }


    }
    private void Loadeditor()
    {
        try
        {



            // Syntax highlighting for C#
            //editor.ConfigurationManager.Language = "cs";\
            //editor.Font = new System.Drawing.Font("Consolas", 16);

            // Faster for large files
            ///this.editor.HeightRequest = this.Height - preview.Height;
            //this.editor.AutoSize = EditorAutoSizeOption.TextChanges;


            if (MainPage.options.UseTextChangedEvent)
            {
                editor.TextChanged += editor_TextChanged;
            }
            if (MainPage.options.UseEnterPressed)
            {

                //editor.KeyDown += editor_KeyDown;
            }
        }
        catch (Exception ex)
        {
            MainPage.core.ErrorLog(ex);


        }
    }



    private void editor_KeyDown(object? sender, TextChangedEventArgs e)
    {
        try
        {
            //if (e == Key.Enter)
            {
                //_markdown = editor.Text;//?? "";
                Updatepreview(editor.Text);
            }
        }
        catch (Exception ex)
        {
            MainPage.core.ErrorLog(ex);


        }
    }
    private void editor_TextChanged(object? sender, EventArgs e)
    {
        try
        {
            //_markdown = editor.Text;//?? "";
            editor.Text = editor.Text.Replace("\r", "\n");
            Updatepreview(editor.Text);
        }
        catch (Exception ex)
        {
            MainPage.core.ErrorLog(ex);


        }
    }
    private async void Updatepreview(string markdown)
    {
        try
        {
            var md = _parser.Parse(markdown);
            if (md != null && markdown != "")
            {


                var html = "<html>\r\n<head>\r\n <meta charset=\"UTF-8\" /></head><style>\r\n                        " +
                    "body {\r\n    color:black; }  </style>" +
                    " <body>" +
                  _renderer.Render(md) + "</body>\r\n</html>";



                // await preview.EnsureCoreWebView2Async(env);

                //  preview.NavigateToString(html);
                NavigateToStringFile(html);
            }
        }
        catch (Exception ex)
        {
            MainPage.core.ErrorLog(ex);


        }

    }

    async void NavigateToStringFile(string html)
    {
        try
        {
            string appname = MainPage.core.GetAppInfo().AppName;
            string file = "";
            if (documentInfo != null)
            {
                file = Path.Combine(documentInfo.ParentDirectory, "output.html");
            }
            else
            {
                file = Path.Combine(MainPage.core.GetTempfolderPath(),
                  "output.html");
            }

            File.WriteAllText(file, html);
            if (preview != null)
            {

                preview.Source = file;


#if ANDROID
                        preview.Dispatcher.Dispatch(() =>
                            {

                                try
                                {
                                    Android.Webkit.WebView web = preview.Handler.PlatformView as Android.Webkit.WebView;
                                    web.Settings.AllowFileAccess = true;
                                    web.LoadUrl($"file://{file}");
                                }
                                catch (Exception ex)
                                {
                                }
                            });
#endif


            }


        }
        catch (Exception ex)
        {
            MainPage.core.ErrorLog(ex);




        }





    }
}