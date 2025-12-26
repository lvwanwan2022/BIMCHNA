use crate::input::InputEvent;

pub trait Tool {
    fn name(&self) -> &str;
    fn on_enter(&mut self);
    fn on_exit(&mut self);
    fn process_input(&mut self, event: &InputEvent);
}

pub struct ToolManager {
    active_tool: Option<Box<dyn Tool>>,
}

impl ToolManager {
    pub fn new() -> Self {
        Self {
            active_tool: None,
        }
    }

    pub fn set_tool(&mut self, mut tool: Box<dyn Tool>) {
        if let Some(mut current) = self.active_tool.take() {
            current.on_exit();
        }
        tool.on_enter();
        self.active_tool = Some(tool);
    }

    pub fn process_input(&mut self, event: &InputEvent) {
        if let Some(tool) = &mut self.active_tool {
            tool.process_input(event);
        }
    }
}

pub fn init() {
    println!("Tool system initialized.");
}
