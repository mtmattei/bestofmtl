# Best of Montréal — Uno app concept

A working Uno Platform app concept for making Best of Montréal participation easier.

The second visual direction follows Best of Montréal’s editorial identity: a large black serif masthead, warm gold accents, a photographic Jacques-Cartier Bridge hero, fine rules and clear horizontal navigation. Ten bundled photographs bring the city, its neighbourhoods and its food culture into the app. The persistent black ballot bar keeps the main action visible.

## Try it

On this Windows computer, open **Launch Best of Montreal.cmd**. The app is also left running after development.

For development, open `BonjourMtl.sln` in an Uno-ready IDE. From the `BonjourMtl` project folder:

```powershell
dotnet run -f net10.0-desktop
```

For a compact phone-width desktop preview:

```powershell
dotnet run -f net10.0-desktop -- --compact
```

The same shared project targets WebAssembly (`net10.0-browserwasm`). With the .NET 10 WebAssembly toolchain installed, publish it with `dotnet publish BonjourMtl/BonjourMtl.csproj -f net10.0-browserwasm -c Release`. Host the contents of the resulting `publish/wwwroot` folder on a static HTTPS host. Android and iOS packaging are not included or tested in this delivery.

## What works

- Search categories and places, including accent-insensitive French searches.
- Filter all ten topic areas across a deliberately small catalogue of 13 demo categories.
- Select, replace or remove one place per category without disruptive popups.
- Review choices, edit them, save a local demo receipt and restart the demo.
- Restore the ballot and language on reopening.
- Switch the interface between English and French.
- Save a nomination draft with required fields and website validation.
- Use a compact layout with horizontal navigation, one-column cards and a persistent ballot button.
- Open the organizer’s official ballot, rules and past winners.

**This is an independent concept, not an official product.** No votes, nominations or personal data are submitted to the organizer. Opening the official ballot does not transfer choices. The bagel names were checked against the live ballot on October 20, 2026. Other places and categories are illustrative, not claims of official nomination or eligibility.

## Website review — October 20, 2026

Sources: [BestofMTL homepage](https://www.bestofmtl.com/), [live ballot](https://www.votemtl.com/), [official rules](https://www.shopbestofmtl.com/official-rules), [FAQ](https://www.bestofmtl.com/faq).

The homepage has a voting link within a large promotional image. The text-only page emphasizes listings and past winners, making that entry point easier to miss for users of search or assistive technology. Voting takes place on a different domain, while rules are on the advertising/shop site.

The live flow is: topic → subcategory → nominee → a popup offering next destinations → review → submit. The review warns that submission prevents further voting until tomorrow. I inspected the review screen and did not press Submit Ballot.

The official rules permit one choice per subcategory and one ballot per person per day. Nomination uses a name and city, followed by organizer verification. Categories need at least five qualifying businesses; the catalogue in this prototype is not an official ballot. The live ballot lists a November 22, 2026, 5 pm deadline and February 19, 2027 results. The rules contain conflicting dates; production should use organizer-supplied dates.

| Before | After in this concept |
|---|---|
| Vote entry embedded in a promotional banner | Visible participation navigation and persistent ballot action |
| Separate domains with little orientation | Clear steps and labelled links to the official service |
| Long category and nominee lists | Search across categories and names; search within a category |
| A navigation popup after each choice | Inline selected state, ballot count and brief confirmation |
| Finality of submission explained late | Review explains the daily limit before saving |
| Account/business FAQ mixed with participation guidance | Dedicated participation and nomination screens |
| No French control observed in the reviewed flow | English/French controls throughout primary screens |
| Generic web styling | Montréal photography, editorial typography, consistent spacing and large controls |

## Implementation

- Uno SDK 6.7.30 / .NET 10; native XAML controls and C#.
- `MainPage.xaml`: shell, horizontal navigation, compact navigation and persistent ballot bar.
- `MainPage.xaml.cs`: discovery, category search and selection.
- `MainPage.Flows.cs`: review, receipt, help and nomination.
- `Photography.cs`: bundled Montréal and category photographs, with sources credited below.
- `Models/Ballot.cs`: validated choices, daily demo limit using Montréal time, serialization.
- `Services/BallotStore.cs`: local application settings; no remote vote API.
- `BonjourMtl.Checks`: dependency-free checks of ballot invariants and storage recovery.

Validation: native desktop build succeeded with zero compiler warnings; 18 domain checks passed. Native UI walkthrough covered search, selection, review, local receipt, reopening persisted state, English/French and compact layout. The small developer toolbar is omitted when using the provided launcher.

To run domain checks, from `BonjourMtl.Checks`:

```powershell
dotnet run
```

## Before an official launch

An organizer-approved integration is required: live category/nominee IDs, definitive dates, eligibility and identity checks, server-side daily limits and idempotent submission, receipts, nomination moderation, French content review, privacy/retention requirements, and accessibility testing with assistive technology. Local demo limits are not anti-fraud controls. The prototype deliberately never automates official votes.

Branding is an original proposal. The organizer’s registered winner logo is not included. The system Georgia/Segoe UI fonts are not redistributed. The local data store is not a secure store for sensitive information.

## Browser design hardening

The browser build preserves the approved black, white and gold design. It bundles Gelasio, a Georgia-compatible serif, under the SIL Open Font License; the license is included in `Assets/Fonts`. Body text uses Uno’s bundled Open Sans. The native Windows version retains its system fonts. Font files are served with the app rather than fetched from third-party font services.

The loading screen carries the same masthead and palette, offers a retry path, and explains that this is an independent concept. The browser release excludes the development toolbar. Nominee choices, language and nomination drafts remain local to each browser; no official votes are submitted.

Font source: [Sorkin Type / Gelasio](https://github.com/SorkinType/Gelasio).

The optimized browser package is available alongside this project in `BestOfMtlWeb/dist`. Browser walkthroughs covered 390 px phone, 768 px tablet and desktop layouts; English/French, search, no-results recovery, selection, review, local receipt, persistence and keyboard selection/removal. Keyboard focus is retained on the nominee after rebuilding the selection list. Hidden duplicate navigation is no longer added to the accessibility tree, and the HTML language follows the interface language.

The Sites project is registered but remains unpublished because the session's automatic approval policy blocked the publishing helper's required input step. No public deployment is claimed. The browser package is ready for a static HTTPS host. This delivery does not claim a full assistive-technology or cross-browser audit.

## Visual redesign — version 2

| Before | After |
| --- | --- |
| A separate Bonjour identity and muted green palette | Best of Montréal-inspired serif masthead and black, white and gold palette |
| Flat illustrated hero and category panels | A genuine Jacques-Cartier Bridge photograph and richly cropped photographic cards |
| Wide permanent sidebar | Compact horizontal navigation with scrollable navigation on small screens |
| The same quiet styling everywhere | Bold photographic category headers, restrained selection cards and a prominent ballot bar |

The app remains an independent local prototype. The publisher’s registered badge is not reproduced. Category imagery illustrates the category or Montréal context; it is not a photograph of each listed nominee. The French and English layouts were inspected at 414px and 1424px widths. Selecting a place, reviewing the ballot and removing that choice were checked again after the redesign.

## Photography credits

All photos are bundled with the app; the app makes no image requests at runtime. Sources are linked below and recorded in `BonjourMtl/Assets/Photography/photo-sources.json`.

- Jacques-Cartier Bridge: [Phil Desforges / Unsplash](https://unsplash.com/photos/the-bridge-is-silhouetted-against-a-golden-sunset-54tb2mNXdiU).
- Bagels: [Robert Gareth / Unsplash](https://unsplash.com/photos/four-bagels-with-sesame-seeds-on-a-cooling-rack-yKoOxhQ4ook).
- Coffee: [Lex Sirikiat / Unsplash](https://unsplash.com/photos/coffee-latte-on-white-ceramic-coffee-cup-q7hlaoiKsQo).
- Poutine neon: [charlesdeluvio / Unsplash](https://unsplash.com/photos/red-and-green-poutine-neon-signage-4hq4xIolcO4).
- Montréal neighbourhood: [Unsplash photograph](https://unsplash.com/photos/brown-concrete-houses-near-trees-x_CEJ7kn4w4).
- Park and skyline: [Unsplash photograph](https://unsplash.com/photos/trees-near-city-JJYzJXbwB20).
- Leonard Cohen mural: [Unsplash photograph](https://unsplash.com/photos/montreal-skyline-with-leonard-cohen-mural-bqhPSkgJ2vw).
- City skyline: [Unsplash photograph](https://unsplash.com/photos/landscape-photography-of-skyscrapers-BG9oZ15a4Xk).
- Montréal street: [Unsplash photograph](https://unsplash.com/photos/cars-on-street-near-high-rise-buildings-p4OsE2nawf0).
- Biosphere: [Unsplash photograph](https://unsplash.com/photos/gray-stainless-steel-globe-landmark-under-green-skies-ZhWOMuL8ndY).

Photos are offered under the [Unsplash License](https://unsplash.com/license).
