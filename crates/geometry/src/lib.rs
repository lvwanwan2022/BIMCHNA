pub mod algorithms;
pub mod basic_geometry;
pub mod brep;
pub mod curves;
pub mod meshes;
pub mod surfaces;

pub fn init() {
    println!("Geometry module initialized.");
    basic_geometry::init();
}
