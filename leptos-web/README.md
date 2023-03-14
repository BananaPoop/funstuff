# Getting Started
## Running your project

`cargo leptos watch`

### Running Tailwind

`npx tailwindcss -i .\tailwind_input.css -o .\style\tailwind.css --watch` and `-m` for minifying, but I think leptos can be configured to do this instead  

## Installing Additional Tools

By default, `cargo-leptos` uses `nightly` Rust, `cargo-generate`, and `sass`. If you run into any trouble, you may need to install one or more of these tools.

1. `rustup toolchain install nightly --allow-downgrade` - make sure you have Rust nightly
2. `rustup default nightly` - setup nightly as default, or you can use rust-toolchain file later on
3. `rustup target add wasm32-unknown-unknown` - add the ability to compile Rust to WebAssembly
4. `cargo install cargo-generate` - install `cargo-generate` binary (should be installed automatically in future)
5. `npm install -g sass` - install `dart-sass` (should be optional in future)

## Testing

1. `npm install -D @playwright/test` install playwright
2. `npx playwright install` installs additional browsers
3. `npx playwright test` run all tests