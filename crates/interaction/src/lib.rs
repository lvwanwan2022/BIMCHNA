pub mod command;
pub mod tools;
pub mod input;

pub fn init() {
    println!("Interaction module initialized.");
    command::init();
    tools::init();
    input::init();
}
