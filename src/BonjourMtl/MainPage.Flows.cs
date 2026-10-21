using BonjourMtl.Models;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Automation;
using Microsoft.UI.Xaml.Controls;

namespace BonjourMtl;
public sealed partial class MainPage
{
    private void RenderBallot()
    {
        PageTitle(T("YOUR LOCAL LOVE, ALL IN ONE PLACE","VOS COUPS DE CŒUR AU MÊME ENDROIT"),
            Locked?T("Merci, Montréal.","Merci, Montréal."):T("Looking good, neighbour.","De beaux choix, voisin."),
            Locked?T("Your demo ballot is saved on this device. No official votes were sent.","Votre bulletin de démo est enregistré sur cet appareil. Aucun vote officiel n’a été envoyé."):
            T("Review your favourites. Change your mind as often as you like before saving.","Vérifiez vos favoris. Vous pouvez encore changer d’avis avant d’enregistrer."));
        if(_ballot.Choices.Count==0)
        {
            var empty=Stack(16);empty.Children.Add(Txt("✦",56,"#B28A36"));
            empty.Children.Add(Txt(T("Every favourite starts somewhere.","Chaque coup de cœur a son début."),27,serif:true));
            empty.Children.Add(Txt(T("You don’t have to know every category. Start with one place you love.","Pas besoin de connaître toutes les catégories. Commencez par une adresse que vous aimez."),15,"#746F65"));
            empty.Children.Add(Btn(T("Explore categories  →","Explorer les catégories  →"),()=>Go("home"),"PrimaryButton"));
            ContentPanel.Children.Add(Box(empty,"#F2ECDB",32));return;
        }
        if(Locked)
        {
            var receipt=Stack(8); receipt.Children.Add(Kicker(T("✓  DEMO COMPLETE","✓  DÉMO TERMINÉE"),"#8A6722"));
            receipt.Children.Add(Txt(T($"{_ballot.Receipt.Count} {(_ballot.Receipt.Count==1?"choice":"choices")} saved · {_ballot.SubmittedOn}",$"{_ballot.Receipt.Count} choix enregistré{(_ballot.Receipt.Count==1?"":"s")} · {_ballot.SubmittedOn}"),20,serif:true));
            receipt.Children.Add(Txt(T("To make them count in the real contest, open the official ballot and select your favourites there. Choices are not transferred automatically.",
                "Pour participer au vrai concours, ouvrez le bulletin officiel et sélectionnez vos favoris. Vos choix ne sont pas transférés automatiquement."),14,"#696459"));
            receipt.Children.Add(Btn(T("Open the official ballot  ↗","Ouvrir le bulletin officiel  ↗"),()=>OpenUrl("https://www.votemtl.com/"),"PrimaryButton"));
            ContentPanel.Children.Add(Box(receipt,"#F2ECDB"));
        }
        var list=Stack(10);
        foreach(var choice in _ballot.Choices)
        {
            var c=Catalog.Categories.Single(c=>c.Id==choice.Key);var n=c.Nominees.Single(n=>n.Id==choice.Value);
            var row=TwoColumns();var labels=Stack(4);
            labels.Children.Add(Kicker(c.Name(Fr).ToUpperInvariant()));
            labels.Children.Add(Txt(n.Name,19,serif:true));row.Children.Add(labels);
            var actions=new StackPanel {Orientation=Orientation.Horizontal,Spacing=4};
            if(!Locked)
            {
                actions.Children.Add(Btn(T("Edit","Modifier"),()=>OpenCategory(c)));
                var remove=Btn("×",()=>{_ballot.Remove(c.Id,Today);Persist();Render();});
                AutomationProperties.SetName(remove,T("Remove ","Retirer ")+n.Name);actions.Children.Add(remove);
            }
            else actions.Children.Add(Txt("✓",22,"#8A6722"));
            Grid.SetColumn(actions,1);row.Children.Add(actions);list.Children.Add(Box(row,"#FFFFFF",20));
        }
        ContentPanel.Children.Add(list);
        var info=Stack(12);
        info.Children.Add(Txt(T("One ballot. Your own pace.","Un bulletin. À votre rythme."),24,serif:true));
        info.Children.Add(Txt(T("The official contest permits one ballot per person per day. After submitting, you cannot add more choices until tomorrow. You can skip any category.",
            "Le concours officiel autorise un bulletin par personne et par jour. Après l’envoi, vous devez attendre le lendemain pour ajouter des choix. Chaque catégorie est facultative."),14,"#746F65"));
        info.Children.Add(Txt(T("This demo saves locally only. It has no connection to the contest’s vote-counting system.",
            "Cette démo enregistre uniquement sur votre appareil. Elle n’est pas reliée au système de comptage du concours."),12,"#746F65"));
        var buttons=Stack(8);
        if(!Locked)
        {
            var save=Btn(T("Save demo ballot  →","Enregistrer le bulletin de démo  →"),()=>
            {
                if(Locked){Render();return;}
                var before=_ballot.Serialize();
                _ballot.SubmitDemo(Today);
                if(!_store.Save(_ballot))
                {
                    _ballot=Ballot.Deserialize(before);_saved=false;
                    Notify(T("Saving failed. Your ballot is still editable. Please try again.","Échec de l’enregistrement. Votre bulletin reste modifiable. Réessayez."));
                    UpdateBar();return;
                }
                _saved=true;Render();MainScroll.ChangeView(null,0,null,true);
            },"PrimaryButton");Id(save,"SaveDemoBallot");buttons.Children.Add(save);
            buttons.Children.Add(Btn(T("Keep exploring","Continuer à explorer"),()=>Go("home")));
        }
        else buttons.Children.Add(Btn(T("Try the demo again","Réessayer la démo"),()=>{_ballot.RestartDemo();Persist();Render();}));
        info.Children.Add(buttons);ContentPanel.Children.Add(Box(info,"#F1EFE9"));
    }
    private void RenderHow()
    {
        PageTitle(T("LESS FRICTION. MORE LOCAL LOVE.","MOINS D’OBSTACLES. PLUS D’AMOUR LOCAL."),
            T("A small act for your city.","Un petit geste pour votre ville."),
            T("Here’s how a favourite becomes a Best of Montréal contender.","Voici comment une adresse devient candidate au Best of Montréal."));
        var steps=new[]{
            (T("01  ·  Nominate","01  ·  Proposer"),T("Put a favourite forward.","Faites connaître un favori."),
             T("During nominations, anyone can suggest a local business or person with a name and city. The organizer verifies entries before the ballot opens.",
               "Pendant les candidatures, chacun peut proposer une entreprise ou une personne avec son nom et sa ville. L’organisateur vérifie les candidatures avant le vote.")),
            (T("02  ·  Choose","02  ·  Choisir"),T("One favourite per category.","Un favori par catégorie."),
             T("Browse the categories you know. Pick one nominee in each, and leave the rest blank if you like. The official ballot currently has ten topic areas.",
               "Explorez les catégories que vous connaissez. Choisissez un candidat par catégorie et passez les autres si vous le souhaitez. Le bulletin officiel comporte dix thèmes.")),
            (T("03  ·  Review & vote","03  ·  Vérifier et voter"),T("Make your choices count.","Faites compter vos choix."),
             T("Review before submitting. The official site accepts one ballot per person per day. A selection is only a draft until you submit the ballot.",
               "Vérifiez avant d’envoyer. Le site officiel accepte un bulletin par personne et par jour. Une sélection reste un brouillon tant que le bulletin n’est pas envoyé."))
        };
        foreach(var step in steps)
        {
            var p=Stack(8);p.Children.Add(Kicker(step.Item1,"#8A6722"));p.Children.Add(Txt(step.Item2,25,serif:true));p.Children.Add(Txt(step.Item3,15,"#746F65"));
            ContentPanel.Children.Add(Box(p,"#FFFFFF"));
        }
        var dates=Stack(10);dates.Children.Add(Txt(T("The 2026 season","L’édition 2026"),25,serif:true));
        dates.Children.Add(Txt(T("Official ballot checked October 20, 2026: voting through November 22 at 5 pm; results February 19, 2027.",
            "Bulletin officiel vérifié le 20 octobre 2026 : vote jusqu’au 22 novembre à 17 h; résultats le 19 février 2027."),15));
        dates.Children.Add(Txt(T("The rules page contains inconsistent nomination and voting dates. Confirm current deadlines on VoteMTL before participating.",
            "La page du règlement contient des dates contradictoires. Confirmez les échéances actuelles sur VoteMTL avant de participer."),13,"#746F65"));
        dates.Children.Add(Btn(T("Official ballot & current dates  ↗","Bulletin officiel et dates actuelles  ↗"),()=>OpenUrl("https://www.votemtl.com/")));
        dates.Children.Add(Btn(T("Read the organizer’s rules  ↗","Lire le règlement de l’organisateur  ↗"),()=>OpenUrl("https://www.shopbestofmtl.com/official-rules")));
        ContentPanel.Children.Add(Box(dates,"#F2ECDB"));
        var about=Stack(8);about.Children.Add(Txt(T("About this concept","À propos du concept"),25,serif:true));
        about.Children.Add(Txt(T("This is an independent Uno Platform prototype, not an official Best of Montréal product. It contains 13 demo categories. The bagel names were checked against the official ballot; other places and categories are illustrative.",
            "Ce concept est un prototype Uno Platform indépendant, pas un produit officiel Best of Montréal. Il contient 13 catégories de démo. Les noms des bagels ont été vérifiés sur le bulletin officiel; les autres adresses et catégories sont illustratives."),14,"#746F65"));
        about.Children.Add(Txt(T("Your selections stay on this device. We do not collect an email address or send your preferences to the organizer. An official integration would need organizer approval, current ballot data and server-side voting safeguards.",
            "Vos choix restent sur cet appareil. Nous ne recueillons pas votre courriel et ne transmettons pas vos préférences. Une intégration officielle nécessiterait l’accord de l’organisateur, les données actuelles et des contrôles du vote côté serveur."),14,"#746F65"));
        ContentPanel.Children.Add(about);
    }
    private void RenderNomination()
    {
        PageTitle(T("PUT A LOCAL GEM ON THE MAP","FAITES DÉCOUVRIR UNE PERLE LOCALE"),
            T("Know a place we should know?","Une adresse à faire découvrir ?"),
            T("Keep a nomination ready for a future nomination period.","Préparez une candidature pour une prochaine période de nomination."));
        ContentPanel.Children.Add(Box(Txt(T("The official site is in its voting phase. This form saves a draft on your device; it does not add a business to the official ballot.",
            "Le site officiel est en période de vote. Ce formulaire enregistre un brouillon sur votre appareil; il n’ajoute pas d’entreprise au bulletin officiel."),14,"#746546"),"#F2ECDB",20));
        var draft=_ballot.Nomination;var form=Stack(18);
        TextBox Field(string header,string text,string placeholder)
        {
            var field=new TextBox {Header=header,Text=text,PlaceholderText=placeholder,MinHeight=48,CornerRadius=new CornerRadius(8),MaxLength=180};
            AutomationProperties.SetName(field,header);form.Children.Add(field);return field;
        }
        var name=Field(T("Business or place name *","Nom de l’entreprise ou du lieu *"),draft?.Name??"",T("The name on the door","Le nom sur la porte"));
        var city=Field(T("City *","Ville *"),draft?.City??"Montréal","Montréal");
        var category=new ComboBox {Header=T("Suggested category *","Catégorie suggérée *"),HorizontalAlignment=HorizontalAlignment.Stretch,MinHeight=48};
        foreach(var c in Catalog.Categories)category.Items.Add(new ComboBoxItem {Content=c.Name(Fr),Tag=c.Id});
        category.SelectedIndex=Array.FindIndex(Catalog.Categories,c=>c.Id==draft?.Category);
        form.Children.Add(category);
        var website=Field(T("Website (optional)","Site Web (facultatif)"),draft?.Website??"","https://");
        var error=Txt("",13,"#8A6722");AutomationProperties.SetLiveSetting(error,Microsoft.UI.Xaml.Automation.Peers.AutomationLiveSetting.Assertive);form.Children.Add(error);
        // Keep unsaved work across language, navigation and responsive layout changes.
        void Capture()
        {
            _ballot.Nomination=new(name.Text,city.Text,(category.SelectedItem as ComboBoxItem)?.Tag as string??"",website.Text);
            _store.Save(_ballot);
        }
        name.TextChanged+=(_,_)=>Capture();city.TextChanged+=(_,_)=>Capture();website.TextChanged+=(_,_)=>Capture();category.SelectionChanged+=(_,_)=>Capture();
        form.Children.Add(Btn(T("Save nomination draft","Enregistrer le brouillon"),()=>
        {
            if(string.IsNullOrWhiteSpace(name.Text)||string.IsNullOrWhiteSpace(city.Text)||category.SelectedIndex<0)
            {error.Text=T("Please add a name, city and category.","Veuillez ajouter un nom, une ville et une catégorie.");return;}
            if(!string.IsNullOrWhiteSpace(website.Text) && (!Uri.TryCreate(website.Text,UriKind.Absolute,out var uri)||(uri.Scheme!="https"&&uri.Scheme!="http")))
            {error.Text=T("Enter a complete website address starting with https://.","Entrez une adresse Web complète commençant par https://.");return;}
            Capture();_saved=_store.Save(_ballot);UpdateBar();
            error.Foreground=B(_saved?"#8A6722":"#8A6722");
            error.Text=_saved?T("✓ Draft saved on this device. Nothing was submitted.","✓ Brouillon enregistré sur cet appareil. Rien n’a été envoyé."):
                T("Unable to save. Please try again.","Enregistrement impossible. Réessayez.");
        },"PrimaryButton"));
        form.Children.Add(Btn(T("Clear this draft","Effacer ce brouillon"),()=>
        {
            _ballot.Nomination=null;Persist();Render();
            if(_saved)Notify(T("Nomination draft cleared.","Brouillon effacé."));
        }));
        form.Children.Add(Btn(T("Check official nomination details  ↗","Voir les détails officiels des candidatures  ↗"),()=>OpenUrl("https://www.shopbestofmtl.com/official-rules")));
        var card=Box(form,"#FFFFFF",28);card.MaxWidth=740;card.HorizontalAlignment=HorizontalAlignment.Stretch;ContentPanel.Children.Add(card);
    }
}
