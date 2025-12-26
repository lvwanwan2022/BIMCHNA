pub mod math;
pub mod logging;
pub mod plugin;
pub mod event_bus;

pub fn init() {
    logging::init();
    println!("Infrastructure initialized.");
}
