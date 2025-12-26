use std::collections::HashMap;
use std::sync::{Arc, Mutex};
use std::any::Any;

pub type EventHandler = Box<dyn Fn(&dyn Any) + Send + Sync>;

pub struct EventBus {
    handlers: HashMap<String, Vec<EventHandler>>,
}

impl EventBus {
    pub fn new() -> Self {
        Self {
            handlers: HashMap::new(),
        }
    }

    pub fn subscribe<T: 'static + Any>(&mut self, event_name: &str, handler: impl Fn(&T) + Send + Sync + 'static) {
        let wrapped_handler = Box::new(move |event: &dyn Any| {
            if let Some(concrete_event) = event.downcast_ref::<T>() {
                handler(concrete_event);
            }
        });
        
        self.handlers.entry(event_name.to_string())
            .or_default()
            .push(wrapped_handler);
    }

    pub fn publish<T: 'static + Any>(&self, event_name: &str, event: T) {
        if let Some(handlers) = self.handlers.get(event_name) {
            for handler in handlers {
                handler(&event);
            }
        }
    }
}

// Thread-safe singleton-ish wrapper could be added here if needed, 
// but for now struct is enough.
pub struct GlobalEventBus {
    pub bus: Arc<Mutex<EventBus>>,
}
