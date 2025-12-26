pub mod rhi;
pub mod render_graph;
pub mod scene;
pub mod material;
pub mod picking;

pub use rhi::BimView;

pub fn init() {
    println!("Display module initialized.");
    // rhi::init(); // Removed placeholder init
    render_graph::init();
    scene::init();
    material::init();
    picking::init();
}
