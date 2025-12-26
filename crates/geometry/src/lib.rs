pub mod basic_geometry;
pub mod curves;
pub mod surfaces;
pub mod brep;
pub mod meshes;
pub mod algorithms;

pub use infrastructure::math;

pub fn init() {
    println!("Geometry kernel initialized.");
}
