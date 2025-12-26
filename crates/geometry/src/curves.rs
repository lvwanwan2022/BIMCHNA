use crate::basic_geometry::Point3;

pub trait Curve {
    fn point_at(&self, t: f32) -> Point3;
    fn tangent_at(&self, t: f32) -> infrastructure::math::Vec3;
}

pub struct LineCurve {
    pub start: Point3,
    pub end: Point3,
}

impl Curve for LineCurve {
    fn point_at(&self, t: f32) -> Point3 {
        let dir = self.end.0 - self.start.0;
        Point3(self.start.0 + dir * t)
    }

    fn tangent_at(&self, _t: f32) -> infrastructure::math::Vec3 {
        (self.end.0 - self.start.0).normalize()
    }
}
