use anyhow::Result;

pub trait Plugin {
    fn name(&self) -> &str;
    fn load(&mut self) -> Result<()>;
    fn unload(&mut self) -> Result<()>;
}

pub struct PluginManager {
    plugins: Vec<Box<dyn Plugin>>,
}

impl PluginManager {
    pub fn new() -> Self {
        Self {
            plugins: Vec::new(),
        }
    }

    pub fn register(&mut self, plugin: Box<dyn Plugin>) {
        self.plugins.push(plugin);
    }
}
