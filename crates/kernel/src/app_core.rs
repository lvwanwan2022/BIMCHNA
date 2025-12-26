use crate::scene_graph::SceneGraph;

pub struct ApplicationCore {
    pub scene: SceneGraph,
}

impl ApplicationCore {
    pub fn new() -> Self {
        Self {
            scene: SceneGraph::new(),
        }
    }
    
    pub fn update(&mut self) {
        // Update logic here
    }
}


