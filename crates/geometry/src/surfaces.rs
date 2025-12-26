use crate::basic_geometry::Point3;
use infrastructure::math::Vec3;

pub trait Surface {
    fn point_at(&self, u: f32, v: f32) -> Point3;
    fn normal_at(&self, u: f32, v: f32) -> Vec3;
}

pub struct PlaneSurface {
    pub origin: Point3,
    pub u_axis: Vec3,
    pub v_axis: Vec3,
}

impl Surface for PlaneSurface {
    fn point_at(&self, u: f32, v: f32) -> Point3 {
        Point3(self.origin.0 + self.u_axis * u + self.v_axis * v)
    }

    fn normal_at(&self, _u: f32, _v: f32) -> Vec3 {
        self.u_axis.cross(self.v_axis).normalize()
    }
}
