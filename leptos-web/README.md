# Getting Started

## Running your project, with VsCode/Codium

1. Run the task `Leptos Watch` this will run `Leptos Tailwind Watch` and `LeptosWeb Cargo Watch` which run the shell commands described in the next section for you
2. Now using "Run and Debug" start debugging the `Launch Leptos` configuration, this will launch a headless version of Microsoft Edge. You should now see a bar appear with *Pause*, *Restart*, *Stop* and *Open Browser Devtools*.
3. Press the *Open Browser Devtools* button.
4. You should now have a window containing the dev tools and a window containing the website itself.

### Tips

* Hot loading is now enabled and should work in most cases, however you should disable the cache in  `Dev Tools > Networking > Cache (near the top)` otherwise the files won't reload
* The tailwind watcher will only add to the css and not remove things, starting the watcher again, will trim everything.
* It is possible to configure VS Code to load changes from editing in the dev tools to the code files, however that will be complex and likely not make sense

## Running your project


`cargo leptos watch`

### 

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