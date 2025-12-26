use serde::{Serialize, Deserialize};

#[derive(Debug, Serialize, Deserialize)]
pub enum BridgeCommand {
    CreateObject { id: String, data: String },
    DeleteObject { id: String },
    UpdateView { camera: String },
}

#[derive(Debug, Serialize, Deserialize)]
pub enum BridgeEvent {
    ObjectSelected { id: String },
    LogMessage { level: String, msg: String },
}

pub fn init() {
    println!("Bridge module initialized.");
}


