using BonjourMtl.Models;
using BonjourMtl.Services;
using Microsoft.UI.Xaml.Automation;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml;
using Windows.UI;
using Microsoft.UI.Text;

namespace BonjourMtl;

public sealed partial class MainPage : Page
{
    private readonly BallotStore _store = new();
    private Ballot _ballot = new();
    private string _page = "home", _group = "all", _query = "", _nomineeQuery = "";
    private Category? _category;
    private double _width = 1280;
    private bool _ready, _saved = true;
    private Grid? _cards, _nominees;
    private TextBlock? _resultCount;
    private readonly DispatcherTimer _toastTimer = new() { Interval = TimeSpan.FromSeconds(3) };
    private bool Fr => _ballot.Language == "fr";
    private DateOnly Today => Ballot.MontrealToday(DateTimeOffset.UtcNow);
    private bool Locked => _ballot.IsSubmitted(Today);
    private int Columns => _width >= 1200 ? 3 : _width >= 760 ? 2 : 1;
    private string T(string en, string fr) => Fr ? fr : en;
#if __WASM__
    private const string SerifFont = "ms-appx:///Assets/Fonts/Gelasio-Regular.ttf";
    private const string BodyFont = "ms-appx:///Uno.Fonts.OpenSans/Fonts/OpenSans.ttf";
#else
    private const string SerifFont = "Georgia";
    private const string BodyFont = "Segoe UI";
#endif
    private static SolidColorBrush B(string hex) => new(Color.FromArgb(255, Convert.ToByte(hex.Substring(1,2),16), Convert.ToByte(hex.Substring(3,2),16), Convert.ToByte(hex.Substring(5,2),16)));
    private static TextBlock Txt(string text, double size=14, string color="#191919", bool bold=false, bool serif=false) => new()
    {
        Text=text, FontSize=size, Foreground=B(color), TextWrapping=TextWrapping.Wrap,
        FontWeight=bold ? FontWeights.SemiBold : FontWeights.Normal,
        FontFamily=new FontFamily(serif ? SerifFont : BodyFont), LineHeight=Math.Ceiling(size*1.35/4)*4
    };
    private static StackPanel Stack(double spacing=12) => new() { Spacing=spacing };
    private static Border Box(UIElement child, string color="#FFFFFF", double pad=24, double radius=8) => new()
    { Child=child, Background=B(color), Padding=new Thickness(pad), CornerRadius=new CornerRadius(radius) };
    private Button Btn(string text, Action action, string style="QuietButton")
    {
        var b=new Button { Content=text, Style=(Style)Resources[style] };
        AutomationProperties.SetName(b,text); b.Click+=(_,_)=>action(); return b;
    }
    private void Id(DependencyObject element,string id) => AutomationProperties.SetAutomationId(element,id);
    private TextBlock Kicker(string text, string color="#8A723E")
    { var t=Txt(text,10,color,true); t.CharacterSpacing=120; return t; }
    private static Grid TwoColumns(double gap=18)
    {
        var g=new Grid { ColumnSpacing=gap };
        g.ColumnDefinitions.Add(new() {Width=new GridLength(1,GridUnitType.Star)});
        g.ColumnDefinitions.Add(new() {Width=GridLength.Auto}); return g;
    }
    public MainPage()
    {
        InitializeComponent();
        HeaderTitle.FontFamily = new FontFamily(SerifFont);
        EditionLabel.FontFamily = new FontFamily(SerifFont);
        try { _ballot=_store.Load(); } catch { _saved=false; }
        _toastTimer.Tick+=(_,_)=> { Toast.Visibility=Visibility.Collapsed; _toastTimer.Stop(); };
        _ready=true; Render();
        Loaded+=(_,_)=>MainScroll.ChangeView(null,0,null,true);
    }
    private void OnSizeChanged(object sender, SizeChangedEventArgs e)
    {
        var oldColumns=Columns; var oldMobile=_width<760; var oldHero=_width>=1080;
        _width=e.NewSize.Width;
        if(!_ready) return;
        ApplySizing();
        if(oldColumns!=Columns || oldMobile!=(_width<760) || oldHero!=(_width>=1080)) Render();
    }
    private void ApplySizing()
    {
        var mobile=_width<760;
        Navigation.Visibility=_width<1080?Visibility.Collapsed:Visibility.Visible;
        MobileNavigation.Visibility=_width<1080?Visibility.Visible:Visibility.Collapsed;
        EditionLabel.Visibility=_width<1100?Visibility.Collapsed:Visibility.Visible;
        var gutter=mobile?18:48;
        ContentPanel.Margin=new Thickness(gutter,mobile?18:24,gutter,30);
        ContentPanel.Spacing=20;
        HeaderGrid.Margin=new Thickness(gutter,0,gutter,0);
        NavGrid.Margin=new Thickness(mobile?6:gutter,0,mobile?6:gutter,0);
        BallotBar.Margin=new Thickness(gutter,12,gutter,12);
        HeaderTitle.FontSize=mobile?24:36;
        HeaderSubtitle.Visibility=Visibility.Visible;
        DemoBadge.Visibility=_width<760?Visibility.Collapsed:Visibility.Visible;
        BallotHint.Visibility=_width<1000?Visibility.Collapsed:Visibility.Visible;
        BallotCount.FontSize=mobile?12:15;
        ReviewButton.Padding=new Thickness(mobile?14:22,12,mobile?14:22,12);
    }
    private void Go(string page)
    { _page=page; _nomineeQuery=""; Render(); MainScroll.ChangeView(null,0,null,true); }
    private void OpenCategory(Category c) { _category=c; Go("category"); }
    private void ToggleLanguage(object sender,RoutedEventArgs e)
    { _ballot.Language=Fr?"en":"fr"; Persist(); Render(); }
    private void ReviewBallot(object sender,RoutedEventArgs e)=>Go("ballot");
    private void Render()
    {
#if __WASM__
        Uno.Foundation.WebAssemblyRuntime.InvokeJS(Fr ? "document.documentElement.lang='fr'" : "document.documentElement.lang='en'");
#endif
        ApplySizing(); RenderNavigation(); UpdateBar();
        LanguageButton.Content=Fr?"EN":"FR";
        AutomationProperties.SetName(LanguageButton,T("Switch to French","Passer en anglais"));
        HeaderSubtitle.Text=_width<760?T("INDEPENDENT APP CONCEPT · 2026","CONCEPT INDÉPENDANT · 2026"):
            T("THE CITY. THE PEOPLE. THE BEST.","LA VILLE. SES GENS. LE MEILLEUR.");
        DemoLabel.Text=T("APP CONCEPT · 2026","CONCEPT · 2026");
        ContentPanel.Children.Clear(); _cards=null; _nominees=null;
        switch(_page)
        {
            case "category": RenderCategory(); break;
            case "ballot": RenderBallot(); break;
            case "how": RenderHow(); break;
            case "nominate": RenderNomination(); break;
            default: RenderHome(); break;
        }
    }
    private void RenderNavigation()
    {
        Navigation.Children.Clear(); MobileNavItems.Children.Clear();
        foreach(var item in new[] {
            ("home","\uE80A",T("Explore & vote","Explorer et voter")),
            ("ballot","\uE73E",T("My ballot","Mon bulletin")+(_ballot.Choices.Count>0?$"   {_ballot.Choices.Count}":"")),
            ("how","\uE946",T("How it works","Comment participer")),
            ("nominate","\uE710",T("Nominate a place","Proposer un lieu")) })
        {
            foreach(var host in new[]{_width<1080?MobileNavItems:Navigation})
            {
                var b=Btn(item.Item3,()=>Go(item.Item1));
                b.HorizontalAlignment=HorizontalAlignment.Left;
                var p=new StackPanel {Orientation=Orientation.Horizontal,Spacing=12};
                p.Children.Add(new FontIcon {Glyph=item.Item2,FontSize=14,Foreground=B("#8A723E")}); p.Children.Add(Txt(item.Item3,13)); b.Content=p;
                if(_page==item.Item1 || (_page=="category" && item.Item1=="home")) {b.Background=B("#F4EDDB"); b.BorderThickness=new Thickness(0,0,0,2); b.BorderBrush=B("#B38D39"); b.CornerRadius=new CornerRadius(0); b.FontWeight=FontWeights.SemiBold;}
                host.Children.Add(b);
            }
        }
        if(_width>=1080) Navigation.Children.Add(Btn(T("2025 winners  ↗","Lauréats 2025  ↗"),()=>OpenUrl("https://www.bestofmtl.com/categories/2025")));
    }
    private void UpdateBar()
    {
        int count=_ballot.Choices.Count;
        BallotCount.Text=count==0?T("Your next favourite is waiting.","Votre bulletin vous attend."):
            T($"{count} {(count==1?"favourite":"favourites")} in your ballot",$"{count} choix dans votre bulletin");
        BallotHint.Text=Locked?T("Demo saved · No official votes sent.","Démo enregistrée · Aucun vote officiel envoyé."):
            count==0?T("Choose the places you love. Every category is optional.","Choisissez vos favoris. Chaque catégorie est facultative."):
            _saved?T("Saved on this device · Ready whenever you are.","Enregistré sur cet appareil · À votre rythme."):
            T("Unable to save on this device. Keep this window open.","Enregistrement impossible. Gardez cette fenêtre ouverte.");
        ReviewButton.Content=_width<760?T("My ballot →","Mon bulletin →"):T("Review my ballot  →","Voir mon bulletin  →");
    }
    private void Persist()
    {
        _saved=_store.Save(_ballot); UpdateBar();
        if(!_saved) Notify(T("Your choices could not be saved. Keep this window open.","Vos choix n’ont pas pu être enregistrés. Gardez cette fenêtre ouverte."));
    }
    private void Notify(string message)
    { _toastTimer.Stop(); ToastText.Text=message; Toast.Visibility=Visibility.Visible; _toastTimer.Start(); }
    private async void OpenUrl(string url)
    {
        try { if(!await Windows.System.Launcher.LaunchUriAsync(new Uri(url))) Notify(T("Could not open your browser.","Impossible d’ouvrir le navigateur.")); }
        catch { Notify(T("Could not open your browser.","Impossible d’ouvrir le navigateur.")); }
    }
    private void PageTitle(string overline,string title,string description)
    {
        var p=Stack(8); p.Children.Add(Kicker(overline)); p.Children.Add(Txt(title,_width<760?34:42,serif:true));
        p.Children.Add(Txt(description,15,"#746F65")); ContentPanel.Children.Add(p);
    }
    private void RenderHome()
    {
        var mobile=_width<760;
        var hero=new Grid {Height=mobile?330:286,Background=B("#151515"),CornerRadius=new CornerRadius(8)};
        hero.ColumnDefinitions.Add(new(){Width=new GridLength(mobile?0: .42,GridUnitType.Star)});
        hero.ColumnDefinitions.Add(new(){Width=new GridLength(1,GridUnitType.Star)});
        var photograph=new Border {Background=Photography.Brush("montreal-bridge"),CornerRadius=new CornerRadius(8)};
        Grid.SetColumn(photograph,1);hero.Children.Add(photograph);
        var shade=new Border {Background=Photography.Shade(mobile)};
        Grid.SetColumnSpan(shade,2);hero.Children.Add(shade);
        var copy=Stack(mobile?11:8);copy.Margin=new Thickness(mobile?24:28,mobile?24:20,mobile?24:28,mobile?24:20);copy.VerticalAlignment=VerticalAlignment.Center;
        copy.MaxWidth=600;copy.HorizontalAlignment=HorizontalAlignment.Left;Grid.SetColumnSpan(copy,2);
        copy.Children.Add(Kicker(T("2026 VOTING  /  THROUGH NOVEMBER 22","VOTE 2026  /  JUSQU’AU 22 NOVEMBRE"),"#E6C875"));
        var headline=Txt(T("THE BEST?\nYOU TELL US.","LE MEILLEUR ?\nÀ VOUS DE JUGER."),mobile?(Fr?32:38):(Fr?42:46),"#FFFFFF",serif:true);
        headline.LineHeight=mobile?46:50;headline.FontWeight=FontWeights.Bold;copy.Children.Add(headline);
        var sub=Txt(T("For the places that make Montréal, Montréal.\nPick your favourites. We’ll keep your place.","Pour les adresses qui font vibrer Montréal.\nVos coups de cœur. À votre rythme."),14,"#DEDAD0");
        sub.MaxWidth=480;copy.Children.Add(sub);
        var actions=new StackPanel {Orientation=Orientation.Horizontal,Spacing=10,Margin=new Thickness(0,5,0,0)};
        var start=Btn(T("Start my ballot  →","Créer mon bulletin  →"),()=>MainScroll.ChangeView(null,mobile?410:340,null,false),"PrimaryButton");Id(start,"StartBallot");actions.Children.Add(start);
        if(_width>=1080){var help=Btn(T("How it works","Comment participer"),()=>Go("how"));help.Foreground=B("#FFFFFF");actions.Children.Add(help);}
        copy.Children.Add(actions);hero.Children.Add(copy);
        if(!mobile)
        {
            var caption=Kicker("PONT JACQUES-CARTIER · MONTRÉAL","#E7D6A7");caption.HorizontalAlignment=HorizontalAlignment.Right;caption.VerticalAlignment=VerticalAlignment.Bottom;caption.Margin=new Thickness(20,0,24,20);Grid.SetColumnSpan(caption,2);hero.Children.Add(caption);
            var edition=Box(Kicker(T("✦  YOUR CITY. YOUR SAY.","✦  VOTRE VILLE. VOTRE VOIX."),"#E6C875"),"#191919",10,3);edition.HorizontalAlignment=HorizontalAlignment.Right;edition.VerticalAlignment=VerticalAlignment.Top;edition.Margin=new Thickness(0,20,20,0);Grid.SetColumnSpan(edition,2);hero.Children.Add(edition);
        }
        ContentPanel.Children.Add(hero);
        var section=Stack(14);var heading=TwoColumns(24);var title=Stack(3);
        title.Children.Add(Kicker(T("FIND YOUR LOCAL FAVOURITES","TROUVEZ VOS COUPS DE CŒUR")));
        title.Children.Add(Txt(T(mobile?"Made for local love.":"A little local love goes a long way.",mobile?"Vos adresses préférées.":"Les adresses qui font Montréal."),mobile?25:28,serif:true));heading.Children.Add(title);
        var search=new TextBox {Text=_query,PlaceholderText=T("Search a category or a place…","Chercher une catégorie ou une adresse…"),MinHeight=48,CornerRadius=new CornerRadius(6),FontSize=14,Padding=new Thickness(16,12,16,12),Background=B("#FFFFFF"),BorderBrush=B("#D9D4C8"),BorderThickness=new Thickness(1)};
        Id(search,"CategorySearch");AutomationProperties.SetName(search,T("Search categories and places","Chercher des catégories et des adresses"));
        search.TextChanged+=(_,_)=>{_query=search.Text;RefreshCards();};
        if(_width>=1080){search.Width=360;search.VerticalAlignment=VerticalAlignment.Center;Grid.SetColumn(search,1);heading.Children.Add(search);}
        section.Children.Add(heading);if(_width<1080)section.Children.Add(search);
        var chips=new StackPanel {Orientation=Orientation.Horizontal,Spacing=6};
        foreach(var group in Catalog.Groups)
        {
            var b=Btn(Fr?group.Fr:group.En,()=>{_group=group.Id;Render();});b.FontSize=12;b.Padding=new Thickness(14,9,14,9);b.CornerRadius=new CornerRadius(5);
            b.Background=B(_group==group.Id?"#191919":"#F0EDE6");b.Foreground=B(_group==group.Id?"#FFFFFF":"#696459");chips.Children.Add(b);
        }
        section.Children.Add(new ScrollViewer {Content=chips,HorizontalScrollBarVisibility=ScrollBarVisibility.Hidden,VerticalScrollBarVisibility=ScrollBarVisibility.Disabled,HorizontalScrollMode=ScrollMode.Enabled});
        ContentPanel.Children.Add(section);
        var meta=TwoColumns();_resultCount=Kicker("");meta.Children.Add(_resultCount);
        var optional=Txt(T("Every category is optional. Start with what you love.","Chaque catégorie est facultative."),12,"#746F65");Grid.SetColumn(optional,1);if(!mobile)meta.Children.Add(optional);
        ContentPanel.Children.Add(meta);
        _cards=new Grid {ColumnSpacing=18,RowSpacing=20};ContentPanel.Children.Add(_cards);RefreshCards();
        var note=TwoColumns();var text=Stack(6);
        text.Children.Add(Kicker(T("THE NEXT GREAT DISCOVERY","LA PROCHAINE BELLE DÉCOUVERTE")));
        text.Children.Add(Txt(T("Every neighbourhood has a hidden gem.","Chaque quartier cache une perle."),26,serif:true));
        text.Children.Add(Txt(T("Put a favourite on the map. Prepare a nomination for the next round.","Faites découvrir un favori. Préparez une candidature pour la prochaine édition."),13,"#746F65"));
        note.Children.Add(text);var nominate=Btn(T("Nominate a place  ↗","Proposer un lieu  ↗"),()=>Go("nominate"),"PrimaryButton");Grid.SetColumn(nominate,1);nominate.VerticalAlignment=VerticalAlignment.Center;
        if(_width>=1000)note.Children.Add(nominate);ContentPanel.Children.Add(Box(note,"#F0EBDF",28));
        ContentPanel.Children.Add(Txt(T("Independent app concept · 13 demo categories · Photography is illustrative · No official votes are submitted.","Concept indépendant · 13 catégories de démo · Photos illustratives · Aucun vote officiel n’est envoyé."),11,"#746F65"));
    }
    private void RefreshCards()
    {
        if(_cards==null)return; _cards.Children.Clear(); _cards.ColumnDefinitions.Clear();_cards.RowDefinitions.Clear();
        for(int i=0;i<Columns;i++)_cards.ColumnDefinitions.Add(new(){Width=new GridLength(1,GridUnitType.Star)});
        var q=Ballot.SearchKey(_query.Trim());
        var matches=Catalog.Categories.Where(c=>(_group=="all"||c.Group==_group) && (q.Length==0||Ballot.SearchKey(c.English+" "+c.French+" "+string.Join(" ",c.Nominees.Select(n=>n.Name))).Contains(q))).ToArray();
        if(_resultCount!=null)_resultCount.Text=T($"{matches.Length} categories",$"{matches.Length} catégories");
        if(matches.Length==0)
        {
            var empty=Stack(); empty.Children.Add(Txt(T("No matches just yet.","Aucun résultat pour le moment."),22,serif:true));
            empty.Children.Add(Txt(T("Try “bagel”, “café”, or reset your filters.","Essayez « bagel », « café » ou réinitialisez les filtres."),14,"#746F65"));
            empty.Children.Add(Btn(T("Reset filters","Réinitialiser"),()=>{_query="";_group="all";Render();}));
            var box=Box(empty,"#F1EFE9");Grid.SetColumnSpan(box,Columns);_cards.Children.Add(box);return;
        }
        for(int i=0;i<matches.Length;i++)
        {
            if(i%Columns==0)_cards.RowDefinitions.Add(new(){Height=GridLength.Auto});
            var c=matches[i];var body=Stack(0);
            var art=new Grid {Height=_width<760?180:128,Background=Photography.Brush(Photography.ForCategory(c.Id)),CornerRadius=new CornerRadius(7,7,0,0)};
            var group=Catalog.Groups.First(g=>g.Id==c.Group);
            var label=Box(Kicker(Fr?group.Fr.ToUpperInvariant():group.En.ToUpperInvariant(),"#FFFFFF"),"#191919",8,3);
            label.Margin=new Thickness(14);label.HorizontalAlignment=HorizontalAlignment.Left;label.VerticalAlignment=VerticalAlignment.Top;art.Children.Add(label);
            bool selected=_ballot.Choices.ContainsKey(c.Id);
            if(selected){var check=Box(Txt("✓",18,"#191919",true),"#E6C875",8,24);check.HorizontalAlignment=HorizontalAlignment.Right;check.VerticalAlignment=VerticalAlignment.Bottom;check.Margin=new Thickness(14);art.Children.Add(check);}
            body.Children.Add(art);
            var words=Stack(6);words.Margin=new Thickness(18,14,18,16);
            var title=TwoColumns(8);title.Children.Add(Txt(c.Name(Fr),24,serif:true));var arrow=Txt("↗",21,"#9A762B");Grid.SetColumn(arrow,1);title.Children.Add(arrow);words.Children.Add(title);
            words.Children.Add(Txt(Fr?c.DescriptionFr:c.Description,12,"#746F65"));
            words.Children.Add(Txt(selected?T("✓  In your ballot","✓  Dans votre bulletin"):T($"{c.Nominees.Length} places · Choose your favourite",$"{c.Nominees.Length} adresses · Faites votre choix"),11,selected?"#8A6722":"#746F65",true));
            body.Children.Add(words);
            var card=Btn(c.Name(Fr),()=>OpenCategory(c),"CardButton");card.Content=body;card.HorizontalAlignment=HorizontalAlignment.Stretch;
            Id(card,"Category_"+c.Id);if(selected)card.BorderBrush=B("#B28A36");
            Grid.SetRow(card,i/Columns);Grid.SetColumn(card,i%Columns);_cards.Children.Add(card);
        }
    }
    private void RenderCategory()
    {
        if(_category==null){Go("home");return;} var c=_category;
        ContentPanel.Children.Add(Btn(T("←  All categories","←  Toutes les catégories"),()=>Go("home")));
        var banner=new Grid {MinHeight=180,Background=Photography.Brush(Photography.ForCategory(c.Id)),CornerRadius=new CornerRadius(8)};
        banner.Children.Add(new Border {Background=Photography.Shade(false)});
        var intro=Stack(6);intro.Margin=new Thickness(26);intro.VerticalAlignment=VerticalAlignment.Center;
        intro.Children.Add(Kicker(T("CHOOSE ONE FAVOURITE","CHOISISSEZ UN FAVORI"),"#E6C875"));
        intro.Children.Add(Txt(c.Name(Fr),_width<760?34:44,"#FFFFFF",serif:true));
        intro.Children.Add(Txt(Fr?c.DescriptionFr:c.Description,14,"#EEE8DD"));banner.Children.Add(intro);ContentPanel.Children.Add(banner);
        var note=c.Id=="bagels"?T("Names from the official ballot, checked Oct 20. This is a local demo selection.","Noms du bulletin officiel, vérifiés le 20 octobre. Votre choix reste dans la démo."):
            T("Illustrative places for this concept, not a verified list of official nominees.","Adresses illustratives pour ce concept, sans confirmation de candidature officielle.");
        ContentPanel.Children.Add(Box(Txt(note,13,"#746546"),"#F2ECDB",18,8));
        if(Locked)ContentPanel.Children.Add(Box(Txt(T("Today’s demo ballot is saved. Open My ballot to try the demo again.","Votre bulletin de démo est enregistré. Ouvrez Mon bulletin pour réessayer."),14),"#F2ECDB",18));
        var search=new TextBox {Text=_nomineeQuery,PlaceholderText=T("Find a place in this category…","Chercher une adresse dans cette catégorie…"),MinHeight=48,CornerRadius=new CornerRadius(8),Padding=new Thickness(16,12,16,12)};
        Id(search,"NomineeSearch"); AutomationProperties.SetName(search,T("Search places","Chercher des adresses"));
        search.TextChanged+=(_,_)=>{_nomineeQuery=search.Text;RefreshNominees();};ContentPanel.Children.Add(search);
        _nominees=new Grid {ColumnSpacing=14,RowSpacing=12};ContentPanel.Children.Add(_nominees);RefreshNominees();
        ContentPanel.Children.Add(Btn(T("Can’t find your favourite? See nomination details  →","Votre favori manque ? Voir les candidatures  →"),()=>Go("nominate")));
    }
    private void RefreshNominees()
    {
        if(_nominees==null||_category==null)return;var c=_category;
        _nominees.Children.Clear();_nominees.ColumnDefinitions.Clear();_nominees.RowDefinitions.Clear();
        int cols=_width>=1080?2:1;for(int j=0;j<cols;j++)_nominees.ColumnDefinitions.Add(new(){Width=new GridLength(1,GridUnitType.Star)});
        var q=Ballot.SearchKey(_nomineeQuery);var matches=c.Nominees.Where(n=>Ballot.SearchKey(n.Name).Contains(q)).OrderBy(n=>n.Name,StringComparer.CurrentCultureIgnoreCase).ToArray();
        if(matches.Length==0){_nominees.Children.Add(Txt(T("No places match your search.","Aucune adresse ne correspond à votre recherche."),16));return;}
        for(int i=0;i<matches.Length;i++)
        {
            if(i%cols==0)_nominees.RowDefinitions.Add(new(){Height=GridLength.Auto});var n=matches[i];
            bool chosen=_ballot.Choices.TryGetValue(c.Id,out var id)&&id==n.Id;
            var row=TwoColumns(14);row.Margin=new Thickness(18);row.MinHeight=48;
            var labels=Stack(4);labels.VerticalAlignment=VerticalAlignment.Center;labels.Children.Add(Txt(n.Name,16,bold:true));
            labels.Children.Add(Txt(chosen?T("Your choice · tap to remove","Votre choix · toucher pour retirer"):T("Add to my ballot","Ajouter à mon bulletin"),12,chosen?"#8A6722":"#746F65"));row.Children.Add(labels);
            var circle=Box(Txt(chosen?"✓":"+",19,chosen?"#FFFFFF":"#746546"),chosen?"#8A6722":"#F1EDE3",0,18);circle.Width=34;circle.Height=34;circle.VerticalAlignment=VerticalAlignment.Center;
            ((TextBlock)circle.Child).HorizontalAlignment=HorizontalAlignment.Center;((TextBlock)circle.Child).VerticalAlignment=VerticalAlignment.Center;Grid.SetColumn(circle,1);row.Children.Add(circle);
            var b=Btn((chosen?T("Remove ","Retirer "):T("Choose ","Choisir "))+n.Name,()=>Choose(c,n),"CardButton");b.Content=row;b.IsEnabled=!Locked;b.HorizontalAlignment=HorizontalAlignment.Stretch;
            b.Background=B(chosen?"#FCF5E2":"#FFFFFF");b.BorderBrush=B(chosen?"#B28A36":"#E5E2DB");Id(b,$"Nominee_{c.Id}_{n.Id}");
            Grid.SetRow(b,i/cols);Grid.SetColumn(b,i%cols);_nominees.Children.Add(b);
        }
    }
    private void Choose(Category c,Nominee n)
    {
        if(Locked)return;
        var focusState=XamlRoot is { } root ? (Microsoft.UI.Xaml.Input.FocusManager.GetFocusedElement(root) as Control)?.FocusState ?? FocusState.Unfocused : FocusState.Unfocused;
        bool remove=_ballot.Choices.TryGetValue(c.Id,out var existing)&&existing==n.Id;
        if(remove)_ballot.Remove(c.Id,Today);else _ballot.Choose(c.Id,n.Id,Today);
        Persist();RefreshNominees();RenderNavigation();
        // Rebuilt nominee controls must retain the user's keyboard position.
        if(focusState!=FocusState.Unfocused)
        {
            var target=_nominees?.Children.OfType<Button>().FirstOrDefault(button=>AutomationProperties.GetAutomationId(button)==$"Nominee_{c.Id}_{n.Id}");
            if(target!=null) DispatcherQueue.TryEnqueue(()=>target.Focus(focusState));
        }
        if(_saved)Notify(remove?T("Removed from your ballot.","Retiré de votre bulletin."):T($"{n.Name} — added. Keep exploring!",$"{n.Name} — ajouté. Continuez à explorer !"));
    }
}


