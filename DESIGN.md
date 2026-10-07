---
name: Mudi
description: Grocery browsing and administration in ink, warm paper and amber.
colors:
  dark-ground: "#0b0e13"
  dark-surface: "#151921"
  dark-raised: "#1f2530"
  dark-line: "#363d48"
  dark-ink: "#eeeef0"
  dark-muted: "#aab2be"
  dark-accent: "#ffca64"
  dark-danger: "#ffa2a8"
  dark-success: "#a8d5bb"
  dark-accent-hover: "#ffdb91"
  light-ground: "#f6f4ef"
  light-surface: "#fffefa"
  light-raised: "#edeae3"
  light-line: "#d6d2ca"
  light-ink: "#23272e"
  light-muted: "#616873"
  light-accent: "#925600"
  light-danger: "#ad2538"
  light-success: "#28724b"
  light-accent-hover: "#754500"
typography:
  display:
    fontFamily: "Bebas, sans-serif"
    fontSize: "clamp(38px, 4.5vw, 64px)"
    fontWeight: 400
    lineHeight: 1.13
    letterSpacing: "0.012em"
  headline:
    fontFamily: "Bebas, sans-serif"
    fontSize: "32px"
    fontWeight: 400
    lineHeight: 1.13
    letterSpacing: "0.012em"
  title:
    fontFamily: "Inter, sans-serif"
    fontSize: "21px"
    fontWeight: 600
    lineHeight: 1.13
  product-title:
    fontFamily: "Inter, sans-serif"
    fontSize: "16px"
    fontWeight: 600
    lineHeight: 1.4
  body:
    fontFamily: "Inter, sans-serif"
    fontSize: "15px"
    fontWeight: 400
    lineHeight: 1.6
  label:
    fontFamily: "Inter, sans-serif"
    fontSize: "13px"
    fontWeight: 500
    lineHeight: 1.6
  action:
    fontFamily: "Inter, sans-serif"
    fontSize: "13px"
    fontWeight: 600
    lineHeight: 1.5
rounded:
  thumbnail: "3px"
  badge: "4px"
  control: "5px"
  artwork: "6px"
  surface: "8px"
spacing:
  compact: "8px"
  control-gap: "12px"
  text: "16px"
  item: "20px"
  section-inset: "24px"
  panel-inset: "28px"
  section: "32px"
  shelf: "40px"
  page: "48px"
components:
  button-primary:
    backgroundColor: "{colors.dark-accent}"
    textColor: "{colors.dark-ground}"
    typography: "{typography.action}"
    rounded: "{rounded.control}"
    padding: "10px 18px"
  button-primary-hover:
    backgroundColor: "{colors.dark-accent-hover}"
    textColor: "{colors.dark-ground}"
  button-primary-light:
    backgroundColor: "{colors.light-accent}"
    textColor: "{colors.light-surface}"
    typography: "{typography.action}"
    rounded: "{rounded.control}"
    padding: "10px 18px"
  button-primary-light-hover:
    backgroundColor: "{colors.light-accent-hover}"
    textColor: "{colors.light-surface}"
  button-secondary:
    backgroundColor: "transparent"
    textColor: "{colors.dark-ink}"
    typography: "{typography.action}"
    rounded: "{rounded.control}"
    padding: "10px 18px"
  button-secondary-light:
    backgroundColor: "transparent"
    textColor: "{colors.light-ink}"
    typography: "{typography.action}"
    rounded: "{rounded.control}"
    padding: "10px 18px"
  button-danger:
    backgroundColor: "#ffa2a810"
    textColor: "{colors.dark-danger}"
    typography: "{typography.action}"
    rounded: "{rounded.control}"
    padding: "10px 18px"
  button-danger-light:
    backgroundColor: "#ad253807"
    textColor: "{colors.light-danger}"
    typography: "{typography.action}"
    rounded: "{rounded.control}"
    padding: "10px 18px"
  field:
    backgroundColor: "{colors.dark-ground}"
    textColor: "{colors.dark-ink}"
    typography: "{typography.body}"
    rounded: "{rounded.control}"
    padding: "10px 12px"
  field-light:
    backgroundColor: "{colors.light-ground}"
    textColor: "{colors.light-ink}"
    typography: "{typography.body}"
    rounded: "{rounded.control}"
    padding: "10px 12px"
  panel:
    backgroundColor: "{colors.dark-surface}"
    textColor: "{colors.dark-ink}"
    rounded: "{rounded.surface}"
    padding: "28px"
  panel-light:
    backgroundColor: "{colors.light-surface}"
    textColor: "{colors.light-ink}"
    rounded: "{rounded.surface}"
    padding: "28px"
  badge:
    backgroundColor: "{colors.dark-raised}"
    textColor: "{colors.dark-ink}"
    rounded: "{rounded.badge}"
    padding: "5px 10px"
  badge-light:
    backgroundColor: "{colors.light-raised}"
    textColor: "{colors.light-ink}"
    rounded: "{rounded.badge}"
    padding: "5px 10px"
---

# Design System: Mudi

## Overview

**Creative North Star: "Catalogue shelves"**

Mudi combines browsable grocery artwork with clear working records. Condensed headings provide character; neutral body text, visible labels and ruled surfaces keep shopping, ordering and administration readable. The Mudi name and existing basket mark remain the identity.

Dark mode uses ink surfaces and pale amber. Light mode uses warm paper and deep amber, with the same information hierarchy and controls. Catalogue composition belongs to the [shared-interface surface brief](.impeccable/surfaces/mudi-views-shared-layout-cshtml.md); other routes use the layout their task needs.

**Key Characteristics:**
- Condensed display lettering paired with neutral, readable forms.
- Semantic light and dark themes shared by customer, admin and Identity pages.
- Restrained borders, small corners and tonal surfaces.
- Product facts and administrative actions remain available on small screens.

## Colors

The palette pairs warm amber with ink at night and warm paper by day. Frontmatter values are normative; the stylesheet exposes the active roles as CSS custom properties.

### Primary

Pale Amber (`dark-accent`) marks actions, links, selected filters and focus in dark mode. Deep Amber (`light-accent`) performs the same jobs in light mode. Primary buttons use dark ground as their text in dark mode and light surface as their text in light mode. The paired hover colors strengthen the interaction without changing its role.

### Secondary

Soft Sage (`dark-success`) and Garden Green (`light-success`) communicate successful feedback. These are semantic confirmation colors, rather than a second brand accent.

### Tertiary

Soft Rose (`dark-danger`) and Deep Cranberry (`light-danger`) identify destructive actions and validation feedback. Subtle transparent tints supply the alert background and border; the readable semantic text carries the meaning.

### Neutral

Ink Ground and Ink Surface distinguish page and panel in dark mode; Ink Raised backs badges, disabled fields and editor tools. Pale Ink provides main text, Slate Muted provides supporting copy, and Slate Line supplies divisions.

Warm Paper Ground and Cream Surface distinguish page and panel in light mode; Warm Stone Raised backs secondary surfaces. Charcoal Ink provides main text, Stone Muted provides supporting copy, and Paper Line supplies divisions. Product packshots retain their existing white image ground.

**The Theme Pair Rule.** Use the active semantic ground, surface, raised, line, ink, muted and accent roles together. A theme change changes the palette, not the hierarchy.

The shared layout sets the saved theme before styles render. The `mudi-theme` local-storage key persists `light` or `dark`; missing or inaccessible storage defaults to dark. The toggle updates its destination label, the document's color scheme and browser theme color. Storage events synchronize the theme across tabs.

## Typography

**Display Font:** Bebas Neue, self-hosted as the CSS family `Bebas`, with sans-serif fallback.

**Body Font:** Inter, self-hosted variable font (100–900), with sans-serif fallback.

The selected pairing places condensed, upright headlines above a neutral reading voice. It uses a practical hierarchy rather than a fixed scale ratio. Labels use normal sentence case; tabular numerals keep money and stock legible.

### Hierarchy

- **Display:** the frontmatter display role serves general page headings. Catalogue and authentication introductions use the larger observed size (`clamp(48px, 5vw, 76px)`).
- **Headline:** the frontmatter headline role serves section headings. General headings share compact leading and balanced wrapping.
- **Title:** the frontmatter title role serves working subheadings; product names use the separate product-title role with more open leading.
- **Body:** the frontmatter body role serves prose and inputs. Supporting paragraphs commonly stop at 65–70 characters; long prose uses 72 characters and more open leading (1.8).
- **Label / Action:** visible labels use the label role; buttons use the slightly heavier action role. Supporting stock, units, counts and table headings are smaller (12px), with prices remaining stronger (18px in catalogue tiles).

**The Reading Voice Rule.** Use Bebas Neue for large headings and Inter for product names, prices, form labels and working records. Keep commercial facts in the readable body voice.

## Layout

The desktop shell uses a fixed left task rail (232px), a main content container capped at 1640px and responsive page gutters. Main content begins with generous vertical inset (48px above, 72px below). Gaps follow the reusable frontmatter steps; compact controls use the smallest steps and sections use the larger ones.

At 1200px and below, the rail contracts to 208px and gutters to 28px. At 900px and below, the rail becomes a hidden drawer (280px wide, capped at viewport minus 40px), with a compact top bar (70px minimum height), 24px gutters and no content offset. At 640px and below, gutters become 20px, paired form/detail/checkout/editor/auth/contact columns stack, and account settings navigation becomes a horizontal scroller. At 360px and below, gutters become 16px. At 1800px and above, gutters grow to 56px.

Desktop forms commonly use two equal columns; filters use four columns, then two below 1200px and one below 640px. Product details use near-equal columns; checkout and product editing allocate more room to the main task. Authentication forms are capped at 460px and settings content at 760px.

Tables become labelled records below 640px. Headers are hidden visually; each cell's `data-label` supplies its visible field name, and row actions occupy their own separated strip. Keep this structure when adding administrative tables.

The catalogue surface uses horizontal category shelves with scroll snapping, a search field and category filters. Tiles are 220–280px wide on standard desktop sizes, 290px on very large screens, 200px below 640px and 180px below 360px. Mobile category filters scroll horizontally. This behavior is specific to browsing, not a global page template.

## Elevation & Depth

Resting surfaces use tonal layers and fine borders. Buttons and inputs have no decorative shadow. Engaged product artwork receives a diffuse shadow; its border changes to accent. Drawer depth comes from an overlay and stacking, not a card shadow.

### Shadow Vocabulary

- **Dark artwork engagement:** soft black lift (`0 8px 24px #0006`) for hover, artwork focus and focus within its tile.
- **Light artwork engagement:** lighter charcoal lift (`0 8px 24px #23272e15`) for the same states.

**The Local Lift Rule.** Keep resting forms and ledgers flat. Reserve the soft shadow for the engaged product artwork.

State colors transition quickly (0.16s, ease-out). Artwork borders use 0.23s and supporting-copy disclosure/drawer movement use 0.25s, with `cubic-bezier(.16,1,.3,1)`. Reduced motion removes transitions and animations, disables smooth scrolling and retains the revealed state.

## Shapes

The system uses small rounded corners, with a gentle surface radius and finer control corners. Frontmatter defines the established radii: thumbnail, badge, control, artwork and surface. Borders are typically a single pixel in the active line color; ledgers use straight horizontal rules. Product art is framed at 4:5 with contained packshots. The mobile product detail switches to a square frame, capped at 440px tall. Circular geometry is reserved for the existing order-confirmation mark.

## Components

### Buttons

Confident, compact controls use the action role, control radius, horizontal padding and a minimum height (44px). Primary buttons fill with the active accent; secondary buttons use transparent backgrounds, active ink and line borders. Secondary hover uses raised surface and muted border. Destructive buttons use danger text, a translucent danger background and danger border. Disabled buttons use raised background, muted text and reduced opacity.

Interactive controls retain a visible accent outline (2px, offset 4px). Icon buttons provide a labelled square control (44px minimum width and height); task labels remain in text wherever they are visible in the source.

### Chips / Badges

Category filters use bordered control shapes, with muted default text and accent text/border when pressed. Selected state is expressed through `aria-pressed`. Badges use raised background, ink text, line borders, badge corners and compact padding. Category badges sit below product headings on the detail page.

### Cards / Containers

Panels and authentication forms use surface backgrounds, line borders, surface corners and the panel inset. Below 640px their inset contracts to 20px; compact panels and authentication forms contract again to 16px below 360px. Checkout summaries use a 24px inset, reduced to 20px on phones. Keep containers flat and reserve cards for an actual grouping.

### Inputs / Fields

Inputs use ground backgrounds, ink text, line borders and control corners, with a minimum height (46px). Visible labels sit above fields; hints and validation feedback follow them. Focus changes the border to accent and preserves the visible focus outline. Read-only and disabled fields use raised backgrounds and muted text. Validation borders use danger; checkboxes use the active accent.

### Navigation

The task rail separates primary destinations, factual information links and account actions. Primary links have a minimum height (48px), muted default text and a subtle surface hover. Active links use accent text/border and a faint accent tint. Account settings links use a related 44px control with a line border in the active state.

The mobile drawer sets expanded state, traps keyboard focus while open, closes on Escape or backdrop action, and returns focus to the menu toggle. Keep the skip link available before navigation. Theme controls remain available in both the rail and compact header.

### Catalogue Shelves

Product artwork is the local discovery surface: a bordered portrait image with contained packshot. Supporting description and the View details action reveal from below on artwork hover or keyboard focus. Product name, price, unit and stock remain outside that disclosure and visible throughout. When one shelf contains focus, neighbouring shelves attenuate only their images (opacity 0.65); text and all text ancestors stay fully opaque.

Shelf arrows disable at the scroll bounds and move by 80% of the visible shelf width. Focusing an artwork link brings its tile into view. Search and category selection update visible products, category counts and a status message; the empty state offers an explicit reset.

### Administrative Ledgers / Records

Overview rows pair a labelled task with its actual count and management action, separated by rules. On phones the action moves below its row. Counts use condensed display type and tabular numerals. Tables use muted, smaller headings and readable body cells, with row actions aligned to the right on desktop and the labelled record treatment on phones. Counts, prices and stock are data, not decorative testimonials.

## Do's and Don'ts

### Do:
- **Do** use the active theme roles for every customer, admin and account surface.
- **Do** keep product name, price, unit and stock visible while supporting artwork copy is disclosed.
- **Do** retain visible form labels, keyboard focus, table cell labels and accessible names on icon controls.
- **Do** use ruled records and task-specific grids where they help administration, checkout or account work.
- **Do** preserve the selected Bebas Neue and Inter pairing and the existing factual product content.
- **Do** preserve state and information when reduced motion removes animation.

### Don't:
- **Don't** reduce the opacity of neighbouring shelf text or its ancestor containers; only artwork is attenuated.
- **Don't** hide essential product facts or primary actions behind hover-only disclosure.
- **Don't** turn the catalogue's title-card arrangement into a mandatory composition for every route.
- **Don't** use shadows as the default treatment for buttons, forms or administrative records.
- **Don't** present demo stock, prices or catalogue counts as production claims.
