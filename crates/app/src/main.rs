mod ui;

use ui::MainWindow;
use infrastructure;
use geometry;
use display;
use interaction;
use storage;

use eframe::wgpu;

fn main() -> eframe::Result<()> {
    // Initialize Logger
    env_logger::init();
    
    println!("BIMCHNA Application Starting...");
    
    // Init Subsystems
    infrastructure::init();
    geometry::init();
    display::init();
    interaction::init();
    storage::init();

    let native_options = eframe::NativeOptions {
        viewport: eframe::egui::ViewportBuilder::default()
            .with_title("BIMCHNA (Rhino-like)")
            .with_inner_size([1200.0, 800.0]),
        wgpu_options: eframe::egui_wgpu::WgpuConfiguration {
             device_descriptor: std::sync::Arc::new(|_adapter| {
                 let mut desc = wgpu::DeviceDescriptor::default();
                 // Enable PolygonMode::Line for Wireframe
                 desc.required_features |= wgpu::Features::POLYGON_MODE_LINE;
                 desc
             }),
             ..Default::default()
        },
        ..Default::default()
    };

    eframe::run_native(
        "BIMCHNA",
        native_options,
        Box::new(|cc| Ok(Box::new(MainWindow::new(cc)))),
    )
}
