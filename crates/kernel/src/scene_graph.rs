use infrastructure::math::{Vec3, Quat, Mat4};
use std::collections::HashMap;

pub type EntityId = u64;

pub struct SceneNode {
    pub id: EntityId,
    pub parent: Option<EntityId>,
    pub children: Vec<EntityId>,
    pub local_transform: Mat4,
    pub world_transform: Mat4,
    pub name: String,
}

pub struct SceneGraph {
    pub nodes: HashMap<EntityId, SceneNode>,
    pub root: EntityId,
    next_id: EntityId,
}

impl SceneGraph {
    pub fn new() -> Self {
        let root = SceneNode {
            id: 0,
            parent: None,
            children: Vec::new(),
            local_transform: Mat4::IDENTITY,
            world_transform: Mat4::IDENTITY,
            name: "Root".to_string(),
        };
        
        let mut nodes = HashMap::new();
        nodes.insert(0, root);

        Self {
            nodes,
            root: 0,
            next_id: 1,
        }
    }

    pub fn add_node(&mut self, parent: EntityId, name: &str) -> EntityId {
        let id = self.next_id;
        self.next_id += 1;

        let node = SceneNode {
            id,
            parent: Some(parent),
            children: Vec::new(),
            local_transform: Mat4::IDENTITY,
            world_transform: Mat4::IDENTITY,
            name: name.to_string(),
        };

        self.nodes.insert(id, node);

        if let Some(p) = self.nodes.get_mut(&parent) {
            p.children.push(id);
        }

        id
    }
}


