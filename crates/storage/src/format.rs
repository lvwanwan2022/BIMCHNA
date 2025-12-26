use serde::{Serialize, Deserialize};

#[derive(Debug, Serialize, Deserialize)]
pub struct ProjectFile {
    pub version: String,
    pub name: String,
    pub objects: Vec<ObjectData>,
}

#[derive(Debug, Serialize, Deserialize)]
pub struct ObjectData {
    pub id: String,
    pub type_name: String,
    pub transform: [f32; 16],
    // More data...
}
