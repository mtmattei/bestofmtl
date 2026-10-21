using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Shapes;
using Windows.UI;

namespace BonjourMtl;
/// <summary>Original vector artwork drawn with native Uno XAML primitives.</summary>
public static class Artwork
{
    private static SolidColorBrush B(string h)=>new(Color.FromArgb(255,Convert.ToByte(h.Substring(1,2),16),Convert.ToByte(h.Substring(3,2),16),Convert.ToByte(h.Substring(5,2),16)));
    private static void Add(Canvas c,FrameworkElement e,double x,double y){Canvas.SetLeft(e,x);Canvas.SetTop(e,y);c.Children.Add(e);}
    private static void Rect(Canvas c,double x,double y,double w,double h,string fill,double r=0)=>Add(c,new Rectangle{Width=w,Height=h,Fill=B(fill),RadiusX=r,RadiusY=r},x,y);
    private static void Oval(Canvas c,double x,double y,double w,double h,string fill)=>Add(c,new Ellipse{Width=w,Height=h,Fill=B(fill)},x,y);
    private static void Line(Canvas c,double x,double y,double xx,double yy,string color,double thickness=2)=>c.Children.Add(new Line{X1=x,Y1=y,X2=xx,Y2=yy,Stroke=B(color),StrokeThickness=thickness});
    private static void Path(Canvas c,string data,string fill,string? stroke=null,double thick=2)
    {
        var p=(Microsoft.UI.Xaml.Shapes.Path)Microsoft.UI.Xaml.Markup.XamlReader.Load(
            $"<Path xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\" Data=\"{data}\" Fill=\"{fill}\" Stroke=\"{stroke??fill}\" StrokeThickness=\"{thick}\"/>");
        c.Children.Add(p);
    }
    public static FrameworkElement Motif(string kind)
    {
        var c=new Canvas{Width=220,Height=100};
        switch(kind)
        {
            case "bagel":
                Oval(c,66,28,92,59,"#9C512C");Oval(c,65,21,92,59,"#C98A4B");Oval(c,94,39,32,22,"#F4DFC5");
                foreach(var (x,y) in new[]{(78,40),(89,29),(115,29),(139,40),(136,61),(114,68),(85,62),(73,54),(126,34)})
                    Line(c,x,y,x+4,y-2,"#F9E7BC",3);
                break;
            case "coffee":
                Oval(c,67,76,99,10,"#BCCEBB");Oval(c,134,35,29,28,"#32644F");Oval(c,141,41,15,15,"#DCE7DD");
                Path(c,"M78,32 L140,32 L135,67 Q109,88 83,67 Z","#F9F5E9","#32644F");
                Oval(c,78,27,62,15,"#32644F");Oval(c,84,30,50,9,"#77533A");
                Path(c,"M98,20 C85,10 107,9 97,1 M120,20 C108,10 132,9 121,0","Transparent","#789380",2);
                break;
            case "fries":
                foreach(var (x,y,angle) in new[]{(76,25,0),(91,18,0),(107,27,0),(123,19,0),(139,30,0)})
                    Rect(c,x,y,9,43,"#DBAD56",2);
                Path(c,"M65,47 L158,47 L146,82 L79,82 Z","#B94434");
                Path(c,"M78,48 Q98,38 110,50 Q132,37 146,50 L138,59 Q115,50 84,61 Z","#8A5837");
                Rect(c,87,44,13,9,"#FFF6D8",2);Rect(c,110,49,12,9,"#FFF6D8",2);Rect(c,130,41,12,8,"#FFF6D8",2);
                break;
            case "bread":
                Path(c,"M59,65 Q62,36 88,40 Q106,12 129,36 Q157,28 167,57 L153,76 Q129,68 115,78 Q96,70 80,80 Z","#C78B43","#9A6632");
                Line(c,91,43,96,65,"#F3CD8C",4);Line(c,114,32,120,64,"#F3CD8C",5);Line(c,138,41,144,65,"#F3CD8C",4);
                break;
            case "music":
                Oval(c,75,18,75,75,"#3E5368");Oval(c,90,33,45,45,"#6F859A");Oval(c,103,46,19,19,"#E8B866");Oval(c,110,53,5,5,"#3E5368");
                Line(c,157,25,157,68,"#3E5368",3);Line(c,157,25,181,20,"#3E5368",4);Oval(c,143,65,16,11,"#3E5368");
                break;
            case "tree":
                Rect(c,102,45,7,45,"#876D48");Oval(c,68,17,61,49,"#839A5C");Oval(c,99,30,48,40,"#617F4D");Oval(c,86,5,50,51,"#9DAF72");
                Line(c,58,91,158,91,"#9BAA7D",2);Rect(c,143,73,34,4,"#876D48");Rect(c,147,77,3,12,"#876D48");Rect(c,171,77,3,12,"#876D48");
                break;
            case "book":
                Path(c,"M66,28 Q89,20 109,32 Q131,20 154,28 L154,80 Q130,72 109,85 Q89,72 66,80 Z","#FFF9E9","#7C6278");
                Line(c,109,32,109,85,"#7C6278",2);for(int j=0;j<4;j++){Line(c,75,40+j*8,99,43+j*8,"#B9A4B3",2);Line(c,119,43+j*8,145,40+j*8,"#B9A4B3",2);}break;
            case "house":
                Path(c,"M73,49 L112,20 L151,49 L151,88 L73,88 Z","#B6866C");Rect(c,101,61,19,27,"#F7EBDD");Rect(c,80,50,13,18,"#F7EBDD");Rect(c,130,50,13,18,"#F7EBDD");break;
            case "car":
                Path(c,"M59,65 L72,47 L93,47 L105,29 L140,29 L151,49 L165,57 L165,77 L59,77 Z","#608394");
                Rect(c,107,34,30,15,"#E8F0EA",3);Oval(c,75,67,22,22,"#314D5B");Oval(c,136,67,22,22,"#314D5B");break;
            case "sun":
                Oval(c,88,30,46,46,"#CFAC60");for(int i=0;i<8;i++){double a=i*Math.PI/4;Line(c,111+31*Math.Cos(a),53+31*Math.Sin(a),111+42*Math.Cos(a),53+42*Math.Sin(a),"#B69755",3);}break;
            default:Path(c,"M111,82 C62,56 69,22 91,25 Q104,25 111,39 Q117,25 131,25 C158,22 161,56 111,82 Z","#B94434");break;
        }
        return new Viewbox{Child=c,Height=100,Stretch=Stretch.Uniform,VerticalAlignment=VerticalAlignment.Center};
    }
    public static FrameworkElement City()
    {
        var c=new Canvas{Width=430,Height=290};
        // Mount Royal and its cross behind Plateau duplexes.
        Oval(c,304,14,65,65,"#E6BC69");
        Path(c,"M0,156 Q79,111 132,127 Q197,61 263,113 Q327,98 430,150 L430,264 L0,264 Z","#D2DCC3");
        Line(c,218,74,218,107,"#839575",3);Line(c,208,85,228,85,"#839575",3);
        Line(c,16,264,420,264,"#849780",2);
        // Three colourful local houses.
        Rect(c,42,130,96,132,"#B7735C");Rect(c,36,123,109,9,"#476457");
        Rect(c,151,99,111,163,"#D3BD88");Rect(c,143,90,127,10,"#526C5A");
        Rect(c,272,141,101,121,"#93AB9C");Rect(c,266,132,114,10,"#466355");
        foreach(var x in new[]{56,106}){Rect(c,x,145,20,33,"#EAE8D4");Rect(c,x+3,149,14,25,"#496459");Rect(c,x,197,20,33,"#EAE8D4");Rect(c,x+3,201,14,25,"#496459");}
        foreach(var x in new[]{166,224}){Rect(c,x,114,23,35,"#FBF0CF");Rect(c,x+4,118,15,27,"#668077");Rect(c,x,172,23,35,"#FBF0CF");Rect(c,x+4,176,15,27,"#668077");}
        foreach(var x in new[]{286,337}){Rect(c,x,153,21,32,"#EDF0D9");Rect(c,x+3,157,15,24,"#4A6559");}
        Rect(c,92,212,20,50,"#314E42");Rect(c,202,213,24,49,"#526C5A");Rect(c,314,213,23,49,"#365747");
        Rect(c,281,200,84,6,"#365747");
        for(int x=284;x<365;x+=10)Line(c,x,188,x,204,"#365747",1.5);
        Line(c,281,188,365,188,"#365747",2);
        // Exterior spiral staircase, a familiar Plateau silhouette.
        Path(c,"M235,210 C306,212 303,235 244,240 C216,243 226,252 256,262","Transparent","#365747",3);
        Path(c,"M235,192 C306,194 303,217 244,222 C216,225 226,234 256,244","Transparent","#365747",2);
        for(int j=0;j<7;j++){var x=247+j*6;Line(c,x,196+j*1.9,x,215+j*1.9,"#365747",1.5);}
        for(int j=0;j<6;j++)Line(c,240+j*6,226-j*.8,240+j*6,242-j*.8,"#365747",1.5);
        // Café awning and shopfront.
        Rect(c,47,232,37,30,"#F5EACA");Rect(c,45,225,42,8,"#F5EACA");
        for(int j=0;j<4;j++)Rect(c,47+j*11,225,5,8,"#A64C39");
        // Tree, lamp and bicycle.
        Rect(c,389,186,6,78,"#6F7755");Oval(c,364,164,53,46,"#6D8A60");Oval(c,375,147,43,50,"#88A078");
        Line(c,19,207,19,263,"#496459",3);Oval(c,12,195,14,17,"#F7E5B5");
        Oval(c,167,249,24,24,"#496459");Oval(c,169,251,20,20,"#E8ECDE");Oval(c,197,249,24,24,"#496459");Oval(c,199,251,20,20,"#E8ECDE");
        Line(c,179,261,189,244,"#A64C39",2);Line(c,189,244,208,261,"#A64C39",2);Line(c,179,261,199,261,"#A64C39",2);Line(c,199,261,192,240,"#A64C39",2);
        Line(c,184,242,193,242,"#496459",2);Line(c,190,239,201,237,"#496459",2);
        var label=new TextBlock{Text="45°30′ N   73°34′ W",FontFamily=new FontFamily("Consolas"),FontSize=9,Foreground=B("#6A8067")};Add(c,label,153,280);
        return new Viewbox{Child=c,Stretch=Stretch.Uniform,MaxHeight=248,VerticalAlignment=VerticalAlignment.Center};
    }
}
