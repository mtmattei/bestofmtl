# Best of Montréal — Voting app concept

The browser version of the same Uno Platform application in `src/`. This is an independent concept with 13 demo categories, English/French screens, local ballot storage and nomination drafts. It does not submit official votes or collect contact information.

## Build

Install .NET 10 and its WebAssembly tools, then run `npm run build` (or `node scripts/build.mjs`). No JavaScript packages are required. Serve `dist/` over HTTPS with the `application/wasm` MIME type for `.wasm` files.

Source, feature notes, photography credits and font licenses are in `src/README.md` and `src/BonjourMtl/Assets/`.

The tracked `.openai/hosting.json` binds this checkout to its existing Sites project. Keep that identity when publishing updates.

## Delivery status — October 20, 2026

The optimized Uno browser build is complete. The deployed-file package was run locally and inspected at desktop, tablet (768 px) and phone (390 px) widths. The walkthrough covered English/French, category and nominee search, no-results recovery, selection, review, saving a demo receipt, persistence after reopening, and keyboard selection/removal with focus retained. All 18 ballot-domain checks passed. This is not a full screen-reader or cross-browser certification.

The approved design was preserved. Changes address browser font fallback, branded loading/recovery, duplicate hidden navigation in the accessibility tree, keyboard focus and browser language metadata. The packaging script also corrects Uno's stale runtime fingerprint and cached manifest metadata in custom publish directories.

Publication is pending. Sites registration exists, but automatic approval review blocked the publishing helper's required stdin step, including after network and repository-folder permissions were granted. No source upload, saved Site version, deployment, or public audience change completed. Resume using the existing project ID; do not create another Site. The built files are in `dist/`.

The release compiler completed successfully. NuGet vulnerability-audit retrieval could not complete in this environment and emitted NU1900; no audit result is claimed.
