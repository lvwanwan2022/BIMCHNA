pub mod logging;
pub mod math;
pub mod plugin;
pub mod event_bus;

pub fn init() {
    println!("Infrastructure module initialized.");
    logging::init();
    math::init();
    plugin::init();
    event_bus::init();
}
