# Product

<!-- impeccable:product-schema 1 -->

## Platform

web

## Users

Customers browse Mudi's grocery catalogue, save products, manage a cart, and place orders. Administrators manage categories, products, stock, orders, website details and administrator accounts.

## Product Purpose

Mudi is an online grocery shop for Dhaka, Bangladesh. This redesign makes existing customer and administrator workflows cleaner and usable on phones, tablets and desktops.

## Capabilities and Constraints

The user confirmed: keep existing features and content, redesign the interface. Preserve ASP.NET Core MVC and Identity routes, form bindings, authorization, database content, product images, Bangladeshi taka prices and account tools. Local development uses SQLite, a demo grocery catalogue and file email delivery. Do not invent delivery guarantees, discounts, testimonials, sales metrics or payment capabilities.

## Brand Commitments

Preserve the Mudi name and existing factual content. The user selected the Catalogue shelves visual direction and requested both light and dark modes. The implemented visual rules live in DESIGN.md and the surface brief.

## Evidence on Hand

The running application, Razor views, real database counts, existing product image uploads, basket logo and About Us content. A fresh development database contains 16 demo products in 7 categories, using matching existing product images. The current local database has 17 products because the original demo pack and its order links were preserved. Demo prices and stock are sample values, not evidence of a production catalogue.

## Product Principles

- Keep existing workflows available across screen sizes.
- Make task priorities and next actions easy to find.
- Show real product and order data.
- Use consistent controls, readable text and clear feedback.

## Open Decisions

The user chose to build directly in code. No deployment has been requested.

User-authorized additions: local demo grocery products with matching existing images and proper categories; persistent light and dark themes across customer and admin panels. Demo prices and stock are explicitly sample data.
