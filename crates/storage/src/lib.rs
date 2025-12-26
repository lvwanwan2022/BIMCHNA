pub mod format;
pub mod exchange;

pub fn init() {
    println!("Storage module initialized.");
    format::init();
    exchange::init();
}
