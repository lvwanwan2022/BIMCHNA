use glam::{Mat4, Vec3};

#[repr(C)]
#[derive(Debug, Copy, Clone, bytemuck::Pod, bytemuck::Zeroable)]
pub struct CameraUniform {
    pub view_proj: [[f32; 4]; 4],
    pub params: [f32; 4], // x: alpha, y: mode, z: unused, w: unused
    pub camera_pos: [f32; 4], // xyz: camera position, w: unused
}

impl CameraUniform {
    pub fn new() -> Self {
        Self {
            view_proj: Mat4::IDENTITY.to_cols_array_2d(),
            params: [1.0, 0.0, 0.0, 0.0],
            camera_pos: [0.0, 0.0, 0.0, 0.0],
        }
    }

    pub fn update_view_proj(&mut self, camera: &Camera) {
        self.view_proj = camera.build_view_projection_matrix().to_cols_array_2d();
        self.camera_pos = [camera.eye.x, camera.eye.y, camera.eye.z, 1.0];
    }
}

#[derive(Debug, Copy, Clone, PartialEq)]
pub enum ProjectionType {
    Perspective,
    Orthographic,
}

pub struct Camera {
    pub eye: Vec3,
    pub target: Vec3,
    pub up: Vec3,
    pub aspect: f32,
    pub fovy: f32,
    pub znear: f32,
    pub zfar: f32,
    pub projection_type: ProjectionType,
    pub ortho_width: f32, // Width in world units for orthographic view
}

impl Camera {
    pub fn build_view_projection_matrix(&self) -> Mat4 {
        let view = Mat4::look_at_rh(self.eye, self.target, self.up);
        
        let proj = match self.projection_type {
            ProjectionType::Perspective => {
                Mat4::perspective_rh(self.fovy, self.aspect, self.znear, self.zfar)
            },
            ProjectionType::Orthographic => {
                let w = self.ortho_width;
                let h = w / self.aspect;
                Mat4::orthographic_rh(-w/2.0, w/2.0, -h/2.0, h/2.0, self.znear, self.zfar)
            }
        };

        proj * view
    }
}

#[derive(Debug, Copy, Clone, PartialEq)]
pub enum CameraMode {
    Orbit,
    Game,
}

#[derive(Clone, Copy, Debug)]
pub struct CameraState {
    pub distance: f32,
    pub yaw: f32,
    pub pitch: f32,
    pub target: Vec3,
    pub eye: Vec3,
    pub mode: CameraMode,
}

pub struct CameraController {
    pub distance: f32,
    pub yaw: f32,   // Rotation around Z axis (azimuth)
    pub pitch: f32, // Elevation angle
    pub target: Vec3,
    pub eye: Vec3,  // Added for Game mode persistence
    pub mode: CameraMode,
    pub speed: f32,
    pub history: Vec<CameraState>,
}

impl CameraController {
    pub fn new(distance: f32) -> Self {
        Self {
            distance,
            yaw: -std::f32::consts::FRAC_PI_4, // 45 deg
            pitch: std::f32::consts::FRAC_PI_6, // 30 deg
            target: Vec3::ZERO,
            eye: Vec3::new(10.0, 10.0, 10.0),
            mode: CameraMode::Orbit,
            speed: 0.5,
            history: Vec::new(),
        }
    }

    pub fn update_camera(&mut self, camera: &mut Camera) {
        match self.mode {
            CameraMode::Orbit => {
                // Z-up coordinate system conversion
                // x = r * cos(pitch) * cos(yaw)
                // y = r * cos(pitch) * sin(yaw)
                // z = r * sin(pitch)
                
                let r = self.distance;
                let x = r * self.pitch.cos() * self.yaw.cos();
                let y = r * self.pitch.cos() * self.yaw.sin();
                let z = r * self.pitch.sin();
        
                camera.eye = self.target + Vec3::new(x, y, z);
                camera.target = self.target;
                self.eye = camera.eye; // Sync eye for mode switch
            }
            CameraMode::Game => {
                // In game mode, eye is the primary source of truth.
                // We calculate target based on yaw/pitch
                let x = self.pitch.cos() * self.yaw.cos();
                let y = self.pitch.cos() * self.yaw.sin();
                let z = self.pitch.sin();
                
                let forward = Vec3::new(x, y, z).normalize();
                camera.eye = self.eye;
                camera.target = self.eye + forward;
                
                // Sync target for mode switch? 
                // Maybe calculate a target at 'distance' for smooth transition
                self.target = self.eye + forward * self.distance;
            }
        }

        camera.up = Vec3::Z;
        
        // Update ortho width based on distance (zoom)
        if camera.projection_type == ProjectionType::Orthographic {
            // A heuristic: align ortho width somewhat with distance
            camera.ortho_width = self.distance; 
        }
    }

    pub fn rotate(&mut self, dx: f32, dy: f32) {
        self.yaw -= dx;
        self.pitch += dy;
        
        // Limit pitch to avoid Gimbal lock and flipping
        let limit = std::f32::consts::FRAC_PI_2 - 0.01;
        self.pitch = self.pitch.clamp(-limit, limit);
    }

    pub fn pan(&mut self, dx: f32, dy: f32) {
        // Pan moves the target
        // We need to map screen dx/dy to world space movement
        
        // Get view basis
        let view_dir = (Vec3::ZERO - Vec3::new(
             self.pitch.cos() * self.yaw.cos(),
             self.pitch.cos() * self.yaw.sin(),
             self.pitch.sin()
        )).normalize();
        
        // In Z-up, "right" is usually on XY plane if we want consistent behavior,
        // but for a general orbit camera:
        // Right = View x Up (if Up is Z, but wait, if view is straight down/up?)
        // Let's use camera basis constructed from view direction and global Z.
        // If view is strictly Z, we have a singularity. 
        
        let mut right_dir = view_dir.cross(Vec3::Z);
        if right_dir.length_squared() < 0.001 {
             // View is parallel to Z (top or bottom view)
             // Use X axis as fallback right?
             // If looking down (-Z), Y is UP on screen? X is Right?
             right_dir = Vec3::X; 
        } else {
             right_dir = right_dir.normalize();
        }
        
        // Up is Right x View
        let up_dir = right_dir.cross(view_dir).normalize();
        
        let factor = self.distance * 0.002;
        match self.mode {
            CameraMode::Orbit => {
                self.target -= right_dir * dx * factor;
                self.target += up_dir * dy * factor;
            }
            CameraMode::Game => {
                 self.eye -= right_dir * dx * factor;
                 self.eye += up_dir * dy * factor;
            }
        }
    }

    pub fn move_forward(&mut self, amount: f32) {
        if let CameraMode::Game = self.mode {
            let x = self.pitch.cos() * self.yaw.cos();
            let y = self.pitch.cos() * self.yaw.sin();
            let z = self.pitch.sin();
            let forward = Vec3::new(x, y, z).normalize();
            self.eye += forward * amount * self.speed;
        }
    }

    pub fn move_right(&mut self, amount: f32) {
        if let CameraMode::Game = self.mode {
            let x = self.pitch.cos() * self.yaw.cos();
            let y = self.pitch.cos() * self.yaw.sin();
            let z = self.pitch.sin();
            let forward = Vec3::new(x, y, z).normalize();
            let right = forward.cross(Vec3::Z).normalize();
            self.eye += right * amount * self.speed;
        }
    }
    
    pub fn move_up(&mut self, amount: f32) {
        if let CameraMode::Game = self.mode {
            self.eye += Vec3::Z * amount * self.speed;
        }
    }

    pub fn zoom(&mut self, delta: f32) {
        match self.mode {
            CameraMode::Orbit => {
                self.distance *= 1.0 - delta * 0.1;
                if self.distance < 0.1 { self.distance = 0.1; }
                if self.distance > 1000.0 { self.distance = 1000.0; }
            }
            CameraMode::Game => {
                // In game mode, zoom could mean moving forward? Or FOV?
                // Usually scroll moves forward/backward
                self.move_forward(delta * 5.0);
            }
        }
    }
    
    // Helper to set standard views
    pub fn set_view(&mut self, yaw: f32, pitch: f32) {
        self.yaw = yaw;
        self.pitch = pitch;
    }

    pub fn save_view(&mut self) {
        self.history.push(CameraState {
            distance: self.distance,
            yaw: self.yaw,
            pitch: self.pitch,
            target: self.target,
            eye: self.eye,
            mode: self.mode,
        });
        // Limit history size
        if self.history.len() > 50 {
            self.history.remove(0);
        }
    }

    pub fn undo_view(&mut self) {
        if let Some(state) = self.history.pop() {
            self.distance = state.distance;
            self.yaw = state.yaw;
            self.pitch = state.pitch;
            self.target = state.target;
            self.eye = state.eye;
            self.mode = state.mode;
        }
    }

    pub fn zoom_to_fit(&mut self, min: Vec3, max: Vec3) {
        self.save_view();
        let center = (min + max) * 0.5;
        let size = (max - min).max_element();
        
        self.target = center;
        self.distance = size * 2.0;
        if self.distance < 0.1 { self.distance = 1.0; }
        
        if self.mode == CameraMode::Game {
            // Update eye to look at new target from current direction
            let x = self.pitch.cos() * self.yaw.cos();
            let y = self.pitch.cos() * self.yaw.sin();
            let z = self.pitch.sin();
            let forward = Vec3::new(x, y, z).normalize();
            self.eye = self.target - forward * self.distance;
        }
    }

    pub fn zoom_extents(&mut self) {
         // Placeholder for backward compatibility if needed, but we should use zoom_to_fit
         self.save_view();
         self.target = Vec3::ZERO;
         self.distance = 15.0; 
         if self.mode == CameraMode::Game {
             self.eye = Vec3::new(10.0, 10.0, 10.0);
             let forward = (Vec3::ZERO - self.eye).normalize();
             self.pitch = forward.z.asin();
             self.yaw = forward.y.atan2(forward.x);
         }
    }

    pub fn zoom_selected(&mut self) {
         self.save_view();
         // Placeholder: In a real app, this would use selected object's bounding box center
         // For now, zoom to a specific point (e.g. 5,5,5) as a demo
         self.target = Vec3::new(5.0, 5.0, 0.0);
         self.distance = 5.0;
         if self.mode == CameraMode::Game {
             self.eye = Vec3::new(10.0, 10.0, 5.0);
             let forward = (self.target - self.eye).normalize();
             self.pitch = forward.z.asin();
             self.yaw = forward.y.atan2(forward.x);
         }
    }

    pub fn set_mode(&mut self, mode: CameraMode) {
        if self.mode != mode {
            // Invert view direction representation when switching
            // Orbit: (yaw, pitch) -> vector from Target to Eye
            // Game:  (yaw, pitch) -> vector from Eye to Target (View Dir)
            // These are opposite vectors, so we flip the spherical coords.
            self.yaw += std::f32::consts::PI;
            self.pitch = -self.pitch;
            self.mode = mode;
        }
    }
}

pub fn init() {
    println!("Scene module initialized");
}
