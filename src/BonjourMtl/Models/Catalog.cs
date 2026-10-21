namespace BonjourMtl.Models;
/// <summary>Bagel names read from VoteMTL on 2026-10-20. All other entries are illustrative.</summary>
public static class Catalog
{
    public static readonly (string Id, string En, string Fr, string Icon)[] Groups =
    [
        ("all","All categories","Toutes","\uE80A"), ("eat","Eat & drink","À boire et à manger","\uED56"),
        ("entertainment","Entertainment","Culture","\uE8D6"), ("things","Things to do","Sorties","\uE707"),
        ("shopping","Shopping","Boutiques","\uE719"), ("health","Health & beauty","Santé et beauté","\uE95E"),
        ("education","Education","Éducation","\uE7BE"), ("home","Home & garden","Maison et jardin","\uE80F"),
        ("motors","Motors","Auto","\uE804"), ("estate","Real estate","Immobilier","\uE81D"),
        ("services","Services","Services","\uE90F")
    ];
    private static Nominee[] N(params string[] names) => names.Select((n,i)=>new Nominee($"n{i}", n)).ToArray();
    public static readonly Category[] Categories =
    [
        new("bagels","eat","Bagels","Bagels","The Montréal classic. Always a good debate.","Un classique montréalais. Le débat est ouvert.","bagel","#F4DFC5",
            N("Fairmount Bagel","St-Viateur Bagel","Bagels Le Trou","Bagelmania","Bagel de l'Ouest","Beaubien Bagel",
              "Dizz's Bagel & Deli","Hansel & Bagel","Café CoCo & Bean","Café Vito","Bagels on Greene","Mo'Bagels & Beignerie",
              "La Fabrique de Bagel de Montréal","District Bagel","M.T.L. Bagel","Bagel Fréna","Brossard Bagel",
              "Kettleman's Bagel","Hinnawi Bros Bagel & Café","Côte St-Luc Bagel","Bagel Henri-Bourassa","Yagel Bagel",
              "R.E.A.L. Bagel","Schnitzel & Cie par Bagel St-Henri")),
        new("coffee","eat","Coffee shops","Cafés","For your daily ritual and the long catch-ups.","Pour le rituel du matin et les retrouvailles.","coffee","#DCE7DD",
            N("Café Olimpico","Café Myriade","Crew Collective & Café","Café Saint-Henri","Café Vito","Dispatch Coffee")),
        new("poutine","eat","Poutine","Poutine","Golden fries. Squeaky cheese. Your favourite.","Frites dorées. Fromage frais. Votre préférée.","fries","#F0DCD6",
            N("La Banquise","Chez Claudette","Ma Poule Mouillée","Patati Patata","Poutineville","Chez Tousignant")),
        new("bakery","eat","Bakeries","Boulangeries","A very good reason to take the long way home.","Une bonne raison de faire un petit détour.","bread","#ECE5CB",
            N("Boulangerie Guillaume","Hof Kelsten","Automne Boulangerie","Mamie Clafoutis","Boulangerie Jarry","Pâtisserie Rhubarbe")),
        new("music","entertainment","Live music venues","Salles de concert","The places that give our city its rhythm.","Les lieux qui donnent son rythme à la ville.","music","#DCE2EB",
            N("MTELUS","Théâtre Fairmount","Casa del Popolo","Le National","Club Soda","L'Escogriffe")),
        new("parks","things","Parks & green spaces","Parcs et espaces verts","A little room to breathe, right here in the city.","Une pause au grand air, au cœur de la ville.","tree","#DFE6CB",
            N("Parc du Mont-Royal","Parc La Fontaine","Parc Jarry","Parc Maisonneuve","Parc Angrignon","Parc Jean-Drapeau")),
        new("books","shopping","Bookshops","Librairies","Your next favourite story starts around the corner.","Votre prochaine histoire commence au coin de la rue.","book","#E8DDE5",
            N("Librairie Drawn & Quarterly","Librairie Le Port de tête","Librairie l'Échange","Librairie Saint-Viateur")),
        new("wellness","health","Wellness studios","Studios de bien-être","Good places to feel like yourself again.","Les bonnes adresses pour prendre soin de soi.","sun","#E8E4D4",
            N("Studio du quartier · Demo","Espace bien-être · Demo","Atelier mouvement · Demo")),
        new("learning","education","Learning & workshops","Cours et ateliers","A new skill. A new favourite place.","Un nouveau talent. Une nouvelle adresse.","book","#E1E7EB",
            N("Atelier créatif · Demo","École du quartier · Demo","Maison des savoirs · Demo")),
        new("garden","home","Garden centres","Centres de jardinage","A little more green, from balcony to backyard.","Plus de verdure, du balcon au jardin.","tree","#DEE7D3",
            N("Jardin du quartier · Demo","Les serres locales · Demo","Atelier botanique · Demo")),
        new("auto","motors","Auto services","Services automobiles","The people who keep your days moving.","Les gens qui vous aident à aller plus loin.","car","#DCE4E8",
            N("Garage du quartier · Demo","Atelier automobile · Demo","Service local · Demo")),
        new("housing","estate","Real estate services","Services immobiliers","Helping you find your own corner of the city.","Pour trouver votre petit coin de la ville.","house","#EADFD5",
            N("Courtier du quartier · Demo","Équipe immobilière · Demo","Maison locale · Demo")),
        new("services","services","Neighbourhood services","Services de proximité","Everyday help from the people nearby.","Les gens d’ici qui simplifient le quotidien.","heart","#EBDDD8",
            N("Atelier de réparation · Demo","Service du quartier · Demo","Équipe de proximité · Demo"))
    ];
}
