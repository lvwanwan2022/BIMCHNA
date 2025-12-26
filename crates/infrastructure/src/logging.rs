use tracing::{info, Level};
use tracing_subscriber::FmtSubscriber;

pub fn init() {
    let subscriber = FmtSubscriber::builder()
        .with_max_level(Level::INFO)
        .finish();

    tracing::subscriber::set_global_default(subscriber)
        .expect("setting default subscriber failed");
        
    info!("Logging system initialized.");
}
