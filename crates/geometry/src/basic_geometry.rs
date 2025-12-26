use infrastructure::math::{Vec3, Quat};

#[derive(Debug, Clone, Copy, Default)]
pub struct Point3(pub Vec3);

impl Point3 {
    pub fn new(x: f32, y: f32, z: f32) -> Self {
        Self(Vec3::new(x, y, z))
    }
    
    pub fn distance(&self, other: &Point3) -> f32 {
        self.0.distance(other.0)
    }
}

#[derive(Debug, Clone, Copy)]
pub struct Line3 {
    pub start: Point3,
    pub end: Point3,
}

#[derive(Debug, Clone, Copy)]
pub struct Plane {
    pub origin: Point3,
    pub normal: Vec3,
}
