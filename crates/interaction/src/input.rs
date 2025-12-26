use infrastructure::math::Vec2;

#[derive(Debug, Clone, Copy, PartialEq)]
pub enum MouseButton {
    Left,
    Right,
    Middle,
    Other(u16),
}

#[derive(Debug, Clone, Copy, PartialEq)]
pub enum InputEvent {
    MouseMove(Vec2),
    MouseDown { button: MouseButton, pos: Vec2 },
    MouseUp { button: MouseButton, pos: Vec2 },
    Scroll(Vec2),
    KeyDown(u32), // Scan code or Virtual Key code
    KeyUp(u32),
}

pub struct InputManager {
    pub mouse_pos: Vec2,
    pub pressed_keys: std::collections::HashSet<u32>,
}

impl InputManager {
    pub fn new() -> Self {
        Self {
            mouse_pos: Vec2::ZERO,
            pressed_keys: std::collections::HashSet::new(),
        }
    }

    pub fn process_event(&mut self, event: &InputEvent) {
        match event {
            InputEvent::MouseMove(pos) => self.mouse_pos = *pos,
            InputEvent::KeyDown(key) => { self.pressed_keys.insert(*key); },
            InputEvent::KeyUp(key) => { self.pressed_keys.remove(key); },
            _ => {}
        }
    }
}

pub fn init() {
    println!("Input system initialized.");
}
